using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using SadrScales.Integration.Internal;

namespace SadrScales.Integration.Items
{
    /// <summary>
    /// Basic Contract v1 operations for PLU/items.
    /// </summary>
    public sealed class SadrItemClient
    {
        /// <summary>
        /// Maximum number of PLUs accepted by one atomic batch call.
        /// </summary>
        public const int MaxBatchSize = 200;

        private const string UpsertSql = @"
IF NOT EXISTS (
    SELECT 1
    FROM dbo.SADR_Item WITH (UPDLOCK, HOLDLOCK)
    WHERE PluNo = @PluNo)
BEGIN
    INSERT INTO dbo.SADR_Item
    (
        ItemClassCode, PluNo, PluUnit, UnitPrice, PrintFormat, PluCost,
        BarFormat, BarFlags, ItemCode, IndexBarcode, Tare,
        ShelfDate, ShelfDatePrint, SaleDatePrint, SaleTimePrint,
        OnlyTare, TaxRate, PluName,
        Text1, Text2, Text3, Text4, Text5, Text6, Text7,
        DeleteFlag
    )
    VALUES
    (
        @ItemClassCode, @PluNo, @PluUnit, @UnitPrice, @PrintFormat, @PluCost,
        @BarFormat, @BarFlags, @ItemCode, @IndexBarcode, @Tare,
        @ShelfDate, @ShelfDatePrint, @SaleDatePrint, @SaleTimePrint,
        @OnlyTare, @TaxRate, @PluName,
        @Text1, @Text2, @Text3, @Text4, @Text5, @Text6, @Text7,
        @DeleteFlag
    );
    SELECT CAST(1 AS int);
END
ELSE
BEGIN
    UPDATE dbo.SADR_Item
    SET ItemClassCode = @ItemClassCode,
        PluUnit = @PluUnit,
        UnitPrice = @UnitPrice,
        PrintFormat = @PrintFormat,
        PluCost = @PluCost,
        BarFormat = @BarFormat,
        BarFlags = @BarFlags,
        ItemCode = @ItemCode,
        IndexBarcode = @IndexBarcode,
        Tare = @Tare,
        ShelfDate = @ShelfDate,
        ShelfDatePrint = @ShelfDatePrint,
        SaleDatePrint = @SaleDatePrint,
        SaleTimePrint = @SaleTimePrint,
        OnlyTare = @OnlyTare,
        TaxRate = @TaxRate,
        PluName = @PluName,
        Text1 = @Text1,
        Text2 = @Text2,
        Text3 = @Text3,
        Text4 = @Text4,
        Text5 = @Text5,
        Text6 = @Text6,
        Text7 = @Text7,
        DeleteFlag = @DeleteFlag
    WHERE PluNo = @PluNo
      AND EXISTS (
          SELECT
              ItemClassCode, PluUnit, UnitPrice, PrintFormat, PluCost,
              BarFormat, BarFlags, ItemCode, IndexBarcode, Tare,
              ShelfDate, ShelfDatePrint, SaleDatePrint, SaleTimePrint,
              OnlyTare, TaxRate, PluName,
              Text1, Text2, Text3, Text4, Text5, Text6, Text7,
              DeleteFlag
          EXCEPT
          SELECT
              @ItemClassCode, @PluUnit, @UnitPrice, @PrintFormat, @PluCost,
              @BarFormat, @BarFlags, @ItemCode, @IndexBarcode, @Tare,
              @ShelfDate, @ShelfDatePrint, @SaleDatePrint, @SaleTimePrint,
              @OnlyTare, @TaxRate, @PluName,
              @Text1, @Text2, @Text3, @Text4, @Text5, @Text6, @Text7,
              @DeleteFlag);

    DECLARE @Rows int = @@ROWCOUNT;
    SELECT CASE WHEN @Rows = 0 THEN 0 ELSE 2 END;
END;";

        private readonly SqlConnectionFactory _connectionFactory;
        private readonly SadrScalesClientOptions _options;

        internal SadrItemClient(SqlConnectionFactory connectionFactory, SadrScalesClientOptions options)
        {
            _connectionFactory = connectionFactory;
            _options = options;
        }

        /// <summary>
        /// Inserts or semantically updates a PLU by its public Contract v1 identity, <c>PluNo</c>.
        /// </summary>
        public async Task<SadrWriteResult> UpsertAsync(
            SadrItem item,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Validate(item);

            using (var connection = await _connectionFactory.OpenAsync(cancellationToken).ConfigureAwait(false))
            using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    var operation = await ExecuteUpsertAsync(connection, transaction, item, cancellationToken).ConfigureAwait(false);
                    transaction.Commit();
                    return new SadrWriteResult(operation);
                }
                catch
                {
                    TryRollback(transaction);
                    throw;
                }
            }
        }

        /// <summary>
        /// Upserts up to <see cref="MaxBatchSize"/> PLUs in one all-or-nothing transaction.
        /// </summary>
        /// <remarks>
        /// The complete batch is validated before SQL access. Duplicate <c>PluNo</c> values in the same
        /// call are rejected. If any SQL write fails, the transaction is rolled back and no item in that
        /// call is committed. Larger workloads should be explicitly paged by the caller.
        /// </remarks>
        public async Task<SadrItemBatchWriteResult> UpsertBatchAsync(
            IEnumerable<SadrItem> items,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var batch = new List<SadrItem>();
            var pluNumbers = new HashSet<int>();

            foreach (var item in items)
            {
                Validate(item);

                if (!pluNumbers.Add(item.PluNo))
                {
                    throw new ArgumentException("A batch cannot contain duplicate PluNo values.", nameof(items));
                }

                batch.Add(item);
                if (batch.Count > MaxBatchSize)
                {
                    throw new ArgumentOutOfRangeException(nameof(items), "A single atomic batch cannot exceed " + MaxBatchSize + " PLUs.");
                }
            }

            if (batch.Count == 0)
            {
                return new SadrItemBatchWriteResult(0, 0, 0);
            }

            using (var connection = await _connectionFactory.OpenAsync(cancellationToken).ConfigureAwait(false))
            using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                var inserted = 0;
                var updated = 0;
                var unchanged = 0;

                try
                {
                    foreach (var item in batch)
                    {
                        var operation = await ExecuteUpsertAsync(connection, transaction, item, cancellationToken).ConfigureAwait(false);
                        switch (operation)
                        {
                            case SadrWriteOperation.Inserted:
                                inserted++;
                                break;
                            case SadrWriteOperation.Updated:
                                updated++;
                                break;
                            default:
                                unchanged++;
                                break;
                        }
                    }

                    transaction.Commit();
                    return new SadrItemBatchWriteResult(inserted, updated, unchanged);
                }
                catch
                {
                    TryRollback(transaction);
                    throw;
                }
            }
        }

        private async Task<SadrWriteOperation> ExecuteUpsertAsync(
            SqlConnection connection,
            SqlTransaction transaction,
            SadrItem item,
            CancellationToken cancellationToken)
        {
            using (var command = new SqlCommand(UpsertSql, connection, transaction))
            {
                command.CommandTimeout = _options.CommandTimeoutSeconds;
                AddParameters(command, item);

                var scalar = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
                return (SadrWriteOperation)Convert.ToInt32(scalar);
            }
        }

        private static void AddParameters(SqlCommand command, SadrItem item)
        {
            command.Parameters.Add("@ItemClassCode", SqlDbType.VarChar, 50).Value = item.ItemClassCode;
            command.Parameters.Add("@PluNo", SqlDbType.Int).Value = item.PluNo;
            command.Parameters.Add("@PluUnit", SqlDbType.Int).Value = item.PluUnit;
            command.Parameters.Add("@UnitPrice", SqlDbType.Int).Value = item.UnitPrice;
            command.Parameters.Add("@PrintFormat", SqlDbType.Int).Value = item.PrintFormat;
            command.Parameters.Add("@PluCost", SqlDbType.Int).Value = item.PluCost;
            command.Parameters.Add("@BarFormat", SqlDbType.Int).Value = item.BarFormat;
            command.Parameters.Add("@BarFlags", SqlDbType.Int).Value = item.BarFlags;
            command.Parameters.Add("@ItemCode", SqlDbType.VarChar, 10).Value = (object?)item.ItemCode ?? DBNull.Value;
            command.Parameters.Add("@IndexBarcode", SqlDbType.VarChar, 50).Value = (object?)item.IndexBarcode ?? DBNull.Value;
            command.Parameters.Add("@Tare", SqlDbType.Float).Value = item.Tare;
            command.Parameters.Add("@ShelfDate", SqlDbType.Int).Value = item.ShelfDate;
            command.Parameters.Add("@ShelfDatePrint", SqlDbType.Int).Value = item.ShelfDatePrint;
            command.Parameters.Add("@SaleDatePrint", SqlDbType.Int).Value = item.SaleDatePrint;
            command.Parameters.Add("@SaleTimePrint", SqlDbType.Int).Value = item.SaleTimePrint;
            command.Parameters.Add("@OnlyTare", SqlDbType.Int).Value = item.OnlyTare;
            command.Parameters.Add("@TaxRate", SqlDbType.Float).Value = item.TaxRate;
            command.Parameters.Add("@PluName", SqlDbType.NVarChar, 100).Value = (object?)item.PluName ?? DBNull.Value;
            AddText(command, "@Text1", item.Text1);
            AddText(command, "@Text2", item.Text2);
            AddText(command, "@Text3", item.Text3);
            AddText(command, "@Text4", item.Text4);
            AddText(command, "@Text5", item.Text5);
            AddText(command, "@Text6", item.Text6);
            AddText(command, "@Text7", item.Text7);
            command.Parameters.Add("@DeleteFlag", SqlDbType.Int).Value = item.DeleteFlag;
        }

        private static void AddText(SqlCommand command, string name, string? value)
        {
            command.Parameters.Add(name, SqlDbType.NVarChar, 100).Value = (object?)value ?? DBNull.Value;
        }

        private static void Validate(SadrItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (item.PluNo == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(item.PluNo), "PluNo must be non-zero.");
            }

            if (string.IsNullOrWhiteSpace(item.ItemClassCode))
            {
                throw new ArgumentException("ItemClassCode is required.", nameof(item));
            }

            ValidateLength(item.ItemClassCode, 50, nameof(item.ItemClassCode));
            ValidateLength(item.ItemCode, 10, nameof(item.ItemCode));
            ValidateLength(item.IndexBarcode, 50, nameof(item.IndexBarcode));
            ValidateLength(item.PluName, 100, nameof(item.PluName));
            ValidateLength(item.Text1, 100, nameof(item.Text1));
            ValidateLength(item.Text2, 100, nameof(item.Text2));
            ValidateLength(item.Text3, 100, nameof(item.Text3));
            ValidateLength(item.Text4, 100, nameof(item.Text4));
            ValidateLength(item.Text5, 100, nameof(item.Text5));
            ValidateLength(item.Text6, 100, nameof(item.Text6));
            ValidateLength(item.Text7, 100, nameof(item.Text7));
        }

        private static void ValidateLength(string? value, int maximumLength, string name)
        {
            if (value != null && value.Length > maximumLength)
            {
                throw new ArgumentException(name + " exceeds the SQL Contract v1 maximum length of " + maximumLength + ".", name);
            }
        }

        private static void TryRollback(SqlTransaction transaction)
        {
            try
            {
                transaction.Rollback();
            }
            catch
            {
                // Preserve the original operation exception.
            }
        }
    }
}
