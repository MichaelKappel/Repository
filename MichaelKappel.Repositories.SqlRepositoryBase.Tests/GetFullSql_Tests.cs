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
    }
}

