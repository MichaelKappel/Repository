using System.Text.Json;
using MichaelKappel.Repositories.Common.JsonConverters;

namespace MichaelKappel.Repositories.Common.Tests.JsonConverters;

[TestClass]
public class LenientConverterDateOnlyTests
{
    private static readonly JsonSerializerOptions Options = new()
    {
        Converters = { new LenientConverterDateOnly() }
    };

    [DataTestMethod]
    [DataRow("\"2026-03-06\"", 2026, 3, 6)]
    [DataRow("\"03/06/2026\"", 2026, 3, 6)]
    [DataRow("\"06/03/2026\"", 2026, 6, 3)]
    [DataRow("\"20260306\"", 2026, 3, 6)]
    [DataRow("\"06-03-2026\"", 2026, 3, 6)]
    public void Read_ParsesSupportedFormats(string json, int year, int month, int day)
    {
        var result = JsonSerializer.Deserialize<DateOnly>(json, Options);

        Assert.AreEqual(new DateOnly(year, month, day), result);
    }

    [TestMethod]
    public void Read_InvalidFormat_ThrowsJsonException()
    {
        Assert.ThrowsException<JsonException>(() => JsonSerializer.Deserialize<DateOnly>("\"2026/03/06\"", Options));
    }

    [TestMethod]
    public void Read_NonStringToken_ThrowsJsonException()
    {
        Assert.ThrowsException<JsonException>(() => JsonSerializer.Deserialize<DateOnly>("123", Options));
    }

    [TestMethod]
    public void Write_UsesIsoFormat()
    {
        var json = JsonSerializer.Serialize(new DateOnly(2026, 3, 6), Options);

        Assert.AreEqual("\"2026-03-06\"", json);
    }
}
