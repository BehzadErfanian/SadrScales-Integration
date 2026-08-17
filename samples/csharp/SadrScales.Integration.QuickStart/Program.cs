using SadrScales.Integration;

var connectionString = Environment.GetEnvironmentVariable("SADR_SCALES_CONNECTION_STRING");
if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.Error.WriteLine("Set SADR_SCALES_CONNECTION_STRING before running this sample.");
    Console.Error.WriteLine("The sample never contains or invents database credentials.");
    return 2;
}

var client = new SadrScalesClient(connectionString);
await client.ValidateAsync();

const long cursor = 0;
var batch = await client.Sales.ReadAfterAsync(cursor, 25);

Console.WriteLine("SQL Contract v1 validation: PASS");
Console.WriteLine($"Read-only sales rows available after cursor {cursor}: {batch.Rows.Count}");
Console.WriteLine($"Candidate LastReadId: {batch.LastReadId}");
Console.WriteLine("No source row was updated or deleted by this sample.");
return 0;
