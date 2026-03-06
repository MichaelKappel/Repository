using System.Text.Json;
using MichaelKappel.Repositories.Common.JsonConverters;

namespace MichaelKappel.Repositories.Common.Tests.JsonConverters;

[TestClass]
public class LenientConverterInt32Tests
{
    private static readonly JsonSerializerOptions Options = new()
    {
        Converters = { new LenientConverterInt32() }
    };

    [DataTestMethod]
    [DataRow("5", 5)]
    [DataRow("\"5\"", 5)]
    [DataRow("\"\"", 0)]
    [DataRow("\"abc\"", int.MaxValue)]
    [DataRow("true", int.MaxValue)]
    public void Read_HandlesSupportedTokens(string json, int expected)
    {
        var result = JsonSerializer.Deserialize<int>(json, Options);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void Write_WritesNumberValue()
    {
        var json = JsonSerializer.Serialize(42, Options);

        Assert.AreEqual("42", json);
    }
}
