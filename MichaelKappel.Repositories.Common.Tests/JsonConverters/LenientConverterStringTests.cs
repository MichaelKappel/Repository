using System.Text.Json;
using MichaelKappel.Repositories.Common.JsonConverters;

namespace MichaelKappel.Repositories.Common.Tests.JsonConverters;

[TestClass]
public class LenientConverterStringTests
{
    private static readonly JsonSerializerOptions Options = new()
    {
        Converters = { new LenientConverterString() }
    };

    [DataTestMethod]
    [DataRow("\"text\"", "text")]
    [DataRow("7", "7")]
    [DataRow("true", "true")]
    [DataRow("false", "false")]
    public void Read_HandlesSupportedTokens(string json, string expected)
    {
        var result = JsonSerializer.Deserialize<string>(json, Options);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void Read_NullToken_ReturnsNull()
    {
        var result = JsonSerializer.Deserialize<string>("null", Options);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void Write_WritesStringValue()
    {
        var json = JsonSerializer.Serialize("abc", Options);

        Assert.AreEqual("\"abc\"", json);
    }
}
