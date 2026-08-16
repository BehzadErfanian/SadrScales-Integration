using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using SadrScales.Integration.Exceptions;
using SadrScales.Integration.Internal;

namespace SadrScales.Integration.Contract
{
    internal sealed class SadrContractValidator
    {
        private const string ValidationSql = @"
IF OBJECT_ID(N'dbo.SADR_ItemClass', N'U') IS NULL
    THROW 51001, 'Contract v1: dbo.SADR_ItemClass is missing.', 1;
IF OBJECT_ID(N'dbo.SADR_Item', N'U') IS NULL
    THROW 51002, 'Contract v1: dbo.SADR_Item is missing.', 1;
IF OBJECT_ID(N'dbo.SADR_Logs', N'U') IS NULL
    THROW 51003, 'Contract v1: dbo.SADR_Logs is missing.', 1;

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.SADR_ItemClass')
      AND name = N'ItemClassCode'
      AND TYPE_NAME(user_type_id) = N'varchar'
      AND max_length = 50
      AND is_nullable = 0)
    THROW 51004, 'Contract v1: SADR_ItemClass.ItemClassCode schema mismatch.', 1;

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.SADR_Item')
      AND name = N'PluNo'
      AND TYPE_NAME(user_type_id) = N'int'
      AND is_nullable = 0)
    THROW 51005, 'Contract v1: SADR_Item.PluNo schema mismatch.', 1;

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.SADR_Item')
      AND name = N'UX_SADR_Item_PluNo'
      AND is_unique = 1)
    THROW 51006, 'Contract v1: unique PLU index is missing.', 1;

IF NOT EXISTS (
    SELECT 1 FROM sys.check_constraints
    WHERE parent_object_id = OBJECT_ID(N'dbo.SADR_Item')
      AND name = N'CK_SADR_Item_PluNo_NotZero')
    THROW 51007, 'Contract v1: non-zero PLU constraint is missing.', 1;

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.SADR_Item')
      AND name = N'TimeStamp'
      AND TYPE_NAME(user_type_id) IN (N'timestamp', N'rowversion'))
    THROW 51008, 'Contract v1: SADR_Item.TimeStamp/rowversion is missing.', 1;

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.SADR_Logs')
      AND name = N'DeviceNo'
      AND TYPE_NAME(user_type_id) = N'int'
      AND is_nullable = 0)
    THROW 51009, 'Contract v1: SADR_Logs.DeviceNo must be int after migration.', 1;

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.SADR_Logs')
      AND name = N'ID'
      AND TYPE_NAME(user_type_id) = N'int'
      AND is_identity = 1
      AND is_nullable = 0)
    THROW 51010, 'Contract v1: SADR_Logs.ID identity schema mismatch.', 1;

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.SADR_Logs')
      AND name = N'UX_SADR_Logs_DeviceNo_FID_SubID'
      AND is_unique = 1)
    THROW 51011, 'Contract v1: unique sales row key is missing.', 1;

IF NOT EXISTS (
    SELECT 1 FROM sys.check_constraints
    WHERE parent_object_id = OBJECT_ID(N'dbo.SADR_Logs')
      AND name = N'CK_SADR_Logs_FID_SubID_PLU_NotZero')
    THROW 51012, 'Contract v1: sales non-zero constraint is missing.', 1;

SELECT CAST(1 AS int);";

        private readonly SqlConnectionFactory _connectionFactory;
        private readonly SadrScalesClientOptions _options;

        public SadrContractValidator(SqlConnectionFactory connectionFactory, SadrScalesClientOptions options)
        {
            _connectionFactory = connectionFactory;
            _options = options;
        }

        public async Task ValidateAsync(CancellationToken cancellationToken)
        {
            using (var connection = await _connectionFactory.OpenAsync(cancellationToken).ConfigureAwait(false))
            using (var command = new SqlCommand(ValidationSql, connection))
            {
                command.CommandTimeout = _options.CommandTimeoutSeconds;
                try
                {
                    await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (SqlException exception) when (exception.Number >= 51001 && exception.Number <= 51099)
                {
                    throw new SadrContractMismatchException(exception.Message, exception.Number, exception);
                }
            }
        }
    }
}
