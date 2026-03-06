using System.Text.Json;
using MichaelKappel.Repositories.Common.JsonConverters;

namespace MichaelKappel.Repositories.Common.Tests.JsonConverters;

[TestClass]
public class LenientConverterInt32NullableTests
{
    private static readonly JsonSerializerOptions Options = new()
    {
        Converters = { new LenientConverterInt32Nullable() }
    };

    [DataTestMethod]
    [DataRow("5", 5)]
    [DataRow("\"5\"", 5)]
    [DataRow("\"\"", null)]
    [DataRow("\"abc\"", null)]
    [DataRow("true", int.MaxValue)]
    public void Read_HandlesSupportedTokens(string json, int? expected)
    {
        var result = JsonSerializer.Deserialize<int?>(json, Options);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void Read_NullToken_ReturnsNull()
    {
        var result = JsonSerializer.Deserialize<int?>("null", Options);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void Write_Null_WritesNull()
    {
        var json = JsonSerializer.Serialize<int?>(null, Options);

        Assert.AreEqual("null", json);
    }
}
