using System;
using System.Data;
using System.Threading;
using Microsoft.Data.SqlClient;

namespace SadrScales.Integration.SqlTests
{
    internal sealed class SqlTestDatabase : IDisposable
    {
        private const string EnvironmentVariable = "SADR_INTEGRATION_TEST_SQL";
        private readonly string _masterConnectionString;
        private bool _disposed;

        private SqlTestDatabase(string masterConnectionString, string databaseName)
        {
            _masterConnectionString = masterConnectionString;
            DatabaseName = databaseName;

            var builder = new SqlConnectionStringBuilder(masterConnectionString)
            {
                InitialCatalog = databaseName
            };
            ConnectionString = builder.ConnectionString;
        }

        public string DatabaseName { get; }

        public string ConnectionString { get; }

        public static SqlTestDatabase Create()
        {
            var masterConnectionString = Environment.GetEnvironmentVariable(EnvironmentVariable);
            if (string.IsNullOrWhiteSpace(masterConnectionString))
            {
                throw new InvalidOperationException(
                    EnvironmentVariable + " is required for SQL integration tests. " +
                    "Use only a disposable/local SQL Server instance.");
            }

            WaitUntilAvailable(masterConnectionString, TimeSpan.FromSeconds(90));

            var databaseName = "SadrIntegrationCi_" + Guid.NewGuid().ToString("N");
            var database = new SqlTestDatabase(masterConnectionString, databaseName);
            database.CreateDatabaseAndSchema();
            return database;
        }

        public SqlConnection OpenConnection()
        {
            var connection = new SqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }

        public int ExecuteNonQuery(string sql)
        {
            using (var connection = OpenConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.CommandTimeout = 30;
                return command.ExecuteNonQuery();
            }
        }

        public T ExecuteScalar<T>(string sql)
        {
            using (var connection = OpenConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.CommandTimeout = 30;
                var value = command.ExecuteScalar();
                return (T)value!;
            }
        }

        public byte[] ReadItemRowVersion(int pluNo)
        {
            using (var connection = OpenConnection())
            using (var command = new SqlCommand(
                "SELECT [TimeStamp] FROM dbo.SADR_Item WHERE PluNo = @PluNo;",
                connection))
            {
                command.Parameters.Add("@PluNo", SqlDbType.Int).Value = pluNo;
                return (byte[])command.ExecuteScalar()!;
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            try
            {
                SqlConnection.ClearAllPools();

                using (var connection = new SqlConnection(_masterConnectionString))
                using (var command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText =
                        "ALTER DATABASE [" + DatabaseName + "] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; " +
                        "DROP DATABASE [" + DatabaseName + "];";
                    command.CommandTimeout = 30;
                    command.ExecuteNonQuery();
                }
            }
            catch
            {
                // The CI container is disposable. Cleanup failure must not hide the test result.
            }
        }

        private void CreateDatabaseAndSchema()
        {
            using (var connection = new SqlConnection(_masterConnectionString))
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = "CREATE DATABASE [" + DatabaseName + "];";
                command.CommandTimeout = 30;
                command.ExecuteNonQuery();
            }

            ExecuteNonQuery(SchemaSql);
        }

        private static void WaitUntilAvailable(string connectionString, TimeSpan timeout)
        {
            var started = DateTime.UtcNow;
            Exception? lastException = null;

            while (DateTime.UtcNow - started < timeout)
            {
                try
                {
                    using (var connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        return;
                    }
                }
                catch (SqlException exception)
                {
                    lastException = exception;
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                }
            }

            throw new InvalidOperationException(
                "Disposable SQL Server did not become ready within " + timeout + ".",
                lastException);
        }

        private const string SchemaSql = @"
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;

CREATE TABLE dbo.SADR_ItemClass
(
    ItemClassCode varchar(50) NOT NULL,
    ItemClassName nvarchar(100) NULL,
    Descriptions nvarchar(150) NULL,
    CONSTRAINT PK_SADR_ItemClass PRIMARY KEY CLUSTERED (ItemClassCode ASC)
);
INSERT INTO dbo.SADR_ItemClass(ItemClassCode, ItemClassName, Descriptions)
VALUES('0', N'پیشفرض', N'گروه پیشفرض');

CREATE TABLE dbo.SADR_Item
(
    ID int NULL,
    IDitem int NULL,
    ItemClassCode varchar(50) NOT NULL CONSTRAINT DF_SADR_Item_ItemClassCode DEFAULT('0'),
    PluNo int NOT NULL,
    PluUnit int NULL CONSTRAINT DF_SADR_Item_WeightUnit DEFAULT((3)),
    UnitPrice int NULL CONSTRAINT DF_SADR_Item_UPrice DEFAULT((0)),
    PrintFormat int NULL CONSTRAINT DF_SADR_Item_LabelFormat DEFAULT((0)),
    PluCost int NULL CONSTRAINT DF_SADR_Item_PluCost DEFAULT((0)),
    BarFormat int NULL CONSTRAINT DF_SADR_Item_BarFormat DEFAULT((0)),
    BarFlags int NULL CONSTRAINT DF_SADR_Item_BarFlags DEFAULT((0)),
    ItemCode varchar(10) NULL CONSTRAINT DF_SADR_Item_BarItemCode DEFAULT('0'),
    IndexBarcode varchar(50) NULL CONSTRAINT DF_SADR_Item_IndexBarcode DEFAULT('0'),
    Tare float NULL CONSTRAINT DF_SADR_Item_Tare DEFAULT((0)),
    ShelfDate int NULL CONSTRAINT DF_SADR_Item_ShelfDate DEFAULT((0)),
    ShelfDatePrint int NULL CONSTRAINT DF_SADR_Item_ShelfDatePrint DEFAULT((0)),
    SaleDatePrint int NULL CONSTRAINT DF_SADR_Item_SaleDatePrint DEFAULT((0)),
    SaleTimePrint int NULL CONSTRAINT DF_SADR_Item_SaleTimePrint DEFAULT((0)),
    OnlyTare int NULL CONSTRAINT DF_SADR_Item_OnlyTare DEFAULT((0)),
    TaxNo int NULL CONSTRAINT DF_SADR_Item_TaxNo DEFAULT((0)),
    TaxRate float NULL CONSTRAINT DF_SADR_Item_TaxRate DEFAULT((0)),
    PluName nvarchar(100) NULL,
    Text1 nvarchar(100) NULL,
    Text2 nvarchar(100) NULL,
    Text3 nvarchar(100) NULL,
    Text4 nvarchar(100) NULL,
    Text5 nvarchar(100) NULL,
    Text6 nvarchar(100) NULL,
    Text7 nvarchar(100) NULL,
    DeleteFlag int NULL CONSTRAINT DF_SADR_Item_DeleteFlag DEFAULT((0)),
    SendFlag int NULL CONSTRAINT DF_SADR_Item_SendFlag DEFAULT((1)),
    [TimeStamp] timestamp NOT NULL,
    CONSTRAINT UX_SADR_Item_PluNo UNIQUE NONCLUSTERED (PluNo),
    CONSTRAINT CK_SADR_Item_PluNo_NotZero CHECK (PluNo <> 0),
    CONSTRAINT FK_SADR_Item_SADR_ItemClass FOREIGN KEY(ItemClassCode)
        REFERENCES dbo.SADR_ItemClass(ItemClassCode)
);
CREATE NONCLUSTERED INDEX IX_SADR_Item_ItemClassCode ON dbo.SADR_Item(ItemClassCode);

CREATE TABLE dbo.SADR_Logs
(
    ID int IDENTITY(1,1) NOT NULL,
    DeviceNo int NOT NULL,
    Identify nvarchar(50) NULL,
    [DateTime] datetime NOT NULL,
    FID int NOT NULL,
    SID int NOT NULL,
    Salesman int NOT NULL,
    SubID int NOT NULL,
    TotalPrice float NOT NULL,
    PLU int NOT NULL,
    Class int NOT NULL,
    Dept int NOT NULL,
    Amount float NOT NULL,
    Unit int NOT NULL,
    LogType int NOT NULL,
    Tax float NOT NULL,
    Text1 nvarchar(50) NULL,
    Text2 nvarchar(50) NULL,
    Text3 nvarchar(50) NULL,
    Text4 nvarchar(50) NULL,
    UnitPrice float NOT NULL,
    CoFID int NOT NULL,
    PLUName nvarchar(50) NOT NULL,
    CONSTRAINT PK_SADR_Logs PRIMARY KEY CLUSTERED (ID ASC),
    CONSTRAINT UX_SADR_Logs_DeviceNo_FID_SubID UNIQUE NONCLUSTERED (DeviceNo, FID, SubID),
    CONSTRAINT CK_SADR_Logs_FID_SubID_PLU_NotZero CHECK (FID <> 0 AND SubID <> 0 AND PLU <> 0)
);
";
    }
}
