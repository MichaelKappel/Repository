using System.Text.Json;
using MichaelKappel.Repositories.Common.JsonConverters;

namespace MichaelKappel.Repositories.Common.Tests.JsonConverters;

[TestClass]
public class LenientConverterBooleanTests
{
    private static readonly JsonSerializerOptions Options = new()
    {
        Converters = { new LenientConverterBoolean() }
    };

    [DataTestMethod]
    [DataRow("true", true)]
    [DataRow("false", false)]
    [DataRow("1", true)]
    [DataRow("0", false)]
    [DataRow("\"yes\"", true)]
    [DataRow("\"no\"", false)]
    [DataRow("\"Y\"", true)]
    [DataRow("\"n\"", false)]
    [DataRow("null", false)]
    public void Read_HandlesSupportedTokens(string json, bool expected)
    {
        var result = JsonSerializer.Deserialize<bool>(json, Options);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void Write_WritesBooleanValue()
    {
        var json = JsonSerializer.Serialize(true, Options);

        Assert.AreEqual("true", json);
    }
}
