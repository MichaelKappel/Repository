using System.Text.Json;
using MichaelKappel.Repositories.Common.JsonConverters;

namespace MichaelKappel.Repositories.Common.Tests.JsonConverters;

[TestClass]
public class LenientConverterBooleanNullableTests
{
    private static readonly JsonSerializerOptions Options = new()
    {
        Converters = { new LenientConverterBooleanNullable() }
    };

    [DataTestMethod]
    [DataRow("true", true)]
    [DataRow("false", false)]
    [DataRow("1", true)]
    [DataRow("0", false)]
    [DataRow("\"yes\"", true)]
    [DataRow("\"no\"", false)]
    public void Read_HandlesSupportedTokens(string json, bool expected)
    {
        var result = JsonSerializer.Deserialize<bool?>(json, Options);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void Read_NullToken_ReturnsNull()
    {
        var result = JsonSerializer.Deserialize<bool?>("null", Options);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void Read_InvalidString_ReturnsNull()
    {
        var result = JsonSerializer.Deserialize<bool?>("\"unknown\"", Options);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void Write_Null_WritesNull()
    {
        var json = JsonSerializer.Serialize<bool?>(null, Options);

        Assert.AreEqual("null", json);
    }
}
