using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;
using MichaelKappel.Repository.Bases;

namespace MichaelKappel.Repositories.SqlRepositoryBase.Tests
{
    public class FormatSqlRepo : SqlRepositoryBase<object>
    {
        public FormatSqlRepo() : base("server=(local);database=Test;trusted_connection=true;")
        {
        }

        protected override object CreateInfoFromReader(SqlDataReader reader)
        {
            return new object();
        }

        public string CallFormatSqlValue(object value)
        {
            var method = typeof(SqlRepositoryBase<object>).GetMethod("FormatSqlValue", BindingFlags.NonPublic | BindingFlags.Instance);
            return (string)method!.Invoke(this, new object?[] { value })!;
        }
    }

    [TestClass]
    public class FormatSqlValue_Tests
    {
        private readonly FormatSqlRepo repo = new();

        [TestMethod]
        public void Null_ReturnsNULL()
        {
            string actual = repo.CallFormatSqlValue(null);
            Assert.AreEqual("NULL", actual);
        }

        [TestMethod]
        public void DBNull_ReturnsNULL()
        {
            string actual = repo.CallFormatSqlValue(DBNull.Value);
            Assert.AreEqual("NULL", actual);
        }

        [TestMethod]
        public void DateTime_Format()
        {
            var dt = new DateTime(2023, 1, 2, 3, 4, 5);
            string actual = repo.CallFormatSqlValue(dt);
            Assert.AreEqual("'2023-01-02 03:04:05'", actual);
        }

        [TestMethod]
        public void DateOnly_Format()
        {
            var dateOnly = new DateOnly(2023, 1, 2);
            string actual = repo.CallFormatSqlValue(dateOnly);
            Assert.AreEqual("'2023-01-02'", actual);
        }

        [TestMethod]
        public void String_EscapesQuotes()
        {
            string actual = repo.CallFormatSqlValue("O'Malley");
            Assert.AreEqual("'O''Malley'", actual);
        }

        [TestMethod]
        public void Int32_ToString()
        {
            string actual = repo.CallFormatSqlValue(123);
            Assert.AreEqual("123", actual);
        }

        [TestMethod]
        public void Decimal_ToString()
        {
            string actual = repo.CallFormatSqlValue(123.45m);
            Assert.AreEqual("123.45", actual);
        }

        [TestMethod]
        public void Double_ToString()
        {
            string actual = repo.CallFormatSqlValue(12.34);
            Assert.AreEqual("12.34", actual);
        }

        [TestMethod]
        public void Float_ToString()
        {
            float val = 1.2f;
            string actual = repo.CallFormatSqlValue(val);
            Assert.AreEqual(val.ToString(), actual);
        }

        [TestMethod]
        public void Guid_ToString()
        {
            var g = new Guid("6f9619ff-8b86-d011-b42d-00cf4fc964ff");
            string actual = repo.CallFormatSqlValue(g);
            Assert.AreEqual(g.ToString(), actual);
        }

        [TestMethod]
        public void Byte_ToString()
        {
            byte b = 255;
            string actual = repo.CallFormatSqlValue(b);
            Assert.AreEqual("255", actual);
        }

        [TestMethod]
        public void Short_ToString()
        {
            short s = -5;
            string actual = repo.CallFormatSqlValue(s);
            Assert.AreEqual("-5", actual);
        }

        [TestMethod]
        public void Long_ToString()
        {
            long l = 9876543210L;
            string actual = repo.CallFormatSqlValue(l);
            Assert.AreEqual("9876543210", actual);
        }

        [TestMethod]
        public void Bool_ToString()
        {
            bool val = true;
            string actual = repo.CallFormatSqlValue(val);
            Assert.AreEqual("True", actual);
        }

        [TestMethod]
        public void Char_ToString()
        {
            char c = 'A';
            string actual = repo.CallFormatSqlValue(c);
            Assert.AreEqual("A", actual);
        }

        [TestMethod]
        public void Enum_ToString()
        {
            DayOfWeek day = DayOfWeek.Monday;
            string actual = repo.CallFormatSqlValue(day);
            Assert.AreEqual("Monday", actual);
        }

        [TestMethod]
        public void TimeSpan_ToString()
        {
            var ts = TimeSpan.FromHours(1);
            string actual = repo.CallFormatSqlValue(ts);
            Assert.AreEqual("01:00:00", actual);
        }

        private class Custom
        {
            public override string ToString() => "custom";
        }

        [TestMethod]
        public void CustomObject_ToString()
        {
            var obj = new Custom();
            string actual = repo.CallFormatSqlValue(obj);
            Assert.AreEqual("custom", actual);
        }
    }
}
