using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Data.SqlClient;
using System.Data;
using MichaelKappel.Repository.Bases;

namespace MichaelKappel.Repositories.SqlRepositoryBase.Tests
{
    public class TestRepo : SqlRepositoryBase<object>
    {
        public TestRepo() : base("server=(local);database=Test;trusted_connection=true;")
        {
        }

        protected override object CreateInfoFromReader(SqlDataReader reader)
        {
            return new object();
        }

        public string CallGetFullSql(string sql, CommandType commandType, params SqlParameter[] parameters)
        {
            return this.GetFullSql(sql, commandType, parameters);
        }
    }

    [TestClass]
    public class GetFullSql_Tests
    {
        [TestMethod]
        public void StoredProcedure_AddsExecAndParameters()
        {
            var repo = new TestRepo();
            var p = new SqlParameter("@Id", SqlDbType.Int) { Value = 5 };

            string actual = repo.CallGetFullSql("dbo.MyProc", CommandType.StoredProcedure, p).Trim();

            string expected = "DECLARE @Id AS Int = 5;\r\nEXEC dbo.MyProc @Id = @Id";

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void FeeSchedTopAda_ProducesValidSql()
        {
            var repo = new TestRepo();

            var parameters = new List<SqlParameter>
            {
                new("@Period_Begin_Dt", SqlDbType.DateTime) { Value = new DateTime(2024, 7, 25) },
                new("@Period_End_Dt", SqlDbType.DateTime) { Value = new DateTime(2025, 7, 25) },
                new("@IncludesLatLong", SqlDbType.Bit) { Value = false },
                new("@Start_Lat", SqlDbType.Decimal) { Value = 0m },
                new("@Start_Long", SqlDbType.Decimal) { Value = 0m },
                new("@Needed_Specialty", SqlDbType.NVarChar) { Value = "GP" },
                new("@strCity", SqlDbType.NVarChar) { Value = string.Empty },
                new("@strCounty", SqlDbType.NVarChar) { Value = string.Empty },
                new("@strState", SqlDbType.NVarChar) { Value = "NJ" },
                new("@strZip", SqlDbType.NVarChar) { Value = string.Empty },
                new("@str3Zip", SqlDbType.NVarChar) { Value = string.Empty },
                new("@Radius_In_Miles", SqlDbType.Decimal) { Value = 0m },
                new("@Has_Manual_Entry", SqlDbType.Bit) { Value = false },
                new("@Percentile_Used_In_Local_Regional", SqlDbType.Int) { Value = 50 },
                new("@Entered_Sched1", SqlDbType.NVarChar) { Value = string.Empty },
                new("@Entered_Sched2", SqlDbType.NVarChar) { Value = string.Empty },
                new("@Entered_Sched3", SqlDbType.NVarChar) { Value = string.Empty },
                new("@Entered_Sched4", SqlDbType.NVarChar) { Value = string.Empty },
                new("@Entered_Sched5", SqlDbType.NVarChar) { Value = string.Empty },
                new("@Entered_Sched6", SqlDbType.NVarChar) { Value = string.Empty },
                new("@Entered_Sched7", SqlDbType.NVarChar) { Value = string.Empty },
                new("@Entered_Sched8", SqlDbType.NVarChar) { Value = string.Empty },
                new("@Entered_Sched9", SqlDbType.NVarChar) { Value = string.Empty },
                new("@Entered_Sched10", SqlDbType.NVarChar) { Value = string.Empty }
            };

            var codes = new[]
            {
                "0120","0140","0145","0150","0160","0170","0180","0210","0220","0230","0240","0270","0272",
                "0273","0274","0277","0330","0364","O367","0431","0460","1110","1120","1206","1208","1351","1354",
                "1510","2140","2150","2160","2161","2330","2331","2332","2335","2390","2391","2392","2393","2394",
                "2740","2750","2751","2752","2790","2920","2929","2930","2933","2940","2950","2951","2952","2954",
                "2980","3220","3221","3230","3240","3310","3320","3330","3332","3346","3347","3348","3410","3430",
                "4211","4212","4249","4260","4261","4263","4265","4266","4341","4342","4346","4355","4381","4910",
                "5110","5120","5130","5140","5211","5212","5213","5214","5225","5226","5410","5421","5422","5511",
                "5512","5520","5611","5630","5640","5650","5660","5730","5750","5751","6010","6056","6057","6058",
                "6059","6060","6061","6065","6066","6092","6104","6240","6241","6242","6245","6740","6750","6751",
                "6752","6930","7111","7140","7210","7220","7230","7240","7250","7280","7283","7310","7311","7321",
                "7510","7953","7960","8660","9110","9120","9222","9223","9230","9239","9243","9248","9310","9410",
                "9920","9944","9945"
            };

            var table = new DataTable();
            table.Columns.Add("Value", typeof(string));
            foreach (var code in codes)
            {
                table.Rows.Add(code);
            }

            parameters.Add(new SqlParameter("@Entered_ADA_Codes", SqlDbType.Structured)
            {
                TypeName = "dbo.ListOfString",
                Value = table
            });

            string actual = repo.CallGetFullSql("dbo.FeeSched_Top_ADA", CommandType.StoredProcedure, parameters.ToArray()).Trim();

            Assert.IsTrue(actual.StartsWith("DECLARE @Period_Begin_Dt AS DateTime = '2024-07-25 00:00:00';"));
            Assert.IsTrue(actual.Contains("DECLARE @strCity AS NVarChar(1) = '';", StringComparison.Ordinal));
            Assert.IsFalse(actual.Contains("NVarChar(0)", StringComparison.Ordinal));
            Assert.IsTrue(actual.EndsWith("@Entered_ADA_Codes = @Entered_ADA_Codes"));
        }
    }
}

