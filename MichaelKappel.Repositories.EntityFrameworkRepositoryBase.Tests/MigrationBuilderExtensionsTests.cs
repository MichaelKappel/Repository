using MichaelKappel.Repositories.EntityFrameworkRepositoryBase;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace MichaelKappel.Repositories.EntityFrameworkRepositoryBase.Tests;

[TestClass]
public class MigrationBuilderExtensionsTests
{
    private static string SingleSql(MigrationBuilder migrationBuilder)
    {
        var sqlOperation = migrationBuilder.Operations.OfType<SqlOperation>().Single();
        return sqlOperation.Sql;
    }

    [TestMethod]
    public void AddPrimaryKeyIfNotExists_GeneratesConditionalSql()
    {
        var migrationBuilder = new MigrationBuilder("SqlServer");

        migrationBuilder.AddPrimaryKeyIfNotExists("PK_Customers", "Customers", "Id", "dbo");

        var sql = SingleSql(migrationBuilder);
        StringAssert.Contains(sql, "IF NOT EXISTS");
        StringAssert.Contains(sql, "TABLE_NAME = 'Customers'");
        StringAssert.Contains(sql, "ALTER TABLE [dbo].[Customers] ADD CONSTRAINT [PK_Customers] PRIMARY KEY");
    }

    [TestMethod]
    public void CreateIndexIfNotExists_WithUniqueAndFilter_BuildsExpectedSql()
    {
        var migrationBuilder = new MigrationBuilder("SqlServer");

        migrationBuilder.CreateIndexIfNotExists("IX_Customers_Email", "Customers", "Email", schema: "dbo", unique: true, filter: "[Email] IS NOT NULL");

        var sql = SingleSql(migrationBuilder);
        StringAssert.Contains(sql, "IF NOT EXISTS");
        StringAssert.Contains(sql, "name = 'IX_Customers_Email'");
        StringAssert.Contains(sql, "OBJECT_SCHEMA_NAME(object_id) = 'dbo'");
        StringAssert.Contains(sql, "CREATE UNIQUE NONCLUSTERED INDEX [IX_Customers_Email] ON [Customers] ([Email] ASC) WHERE [Email] IS NOT NULL");
    }

    [TestMethod]
    public void DropIndexIfExists_WithTable_BuildsExpectedSql()
    {
        var migrationBuilder = new MigrationBuilder("SqlServer");

        migrationBuilder.DropIndexIfExists("IX_Customers_Email", "Customers", "dbo");

        var sql = SingleSql(migrationBuilder);
        StringAssert.Contains(sql, "IF EXISTS");
        StringAssert.Contains(sql, "DROP INDEX [IX_Customers_Email] ON [Customers]");
    }

    [TestMethod]
    public void AddForeignKeyIfNotExists_FormatsReferentialActions()
    {
        var migrationBuilder = new MigrationBuilder("SqlServer");

        migrationBuilder.AddForeignKeyIfNotExists(
            name: "FK_Order_Customer",
            table: "Orders",
            column: "CustomerId",
            principalTable: "Customers",
            schema: "dbo",
            principalSchema: "dbo",
            principalColumn: "Id",
            onUpdate: ReferentialAction.Cascade,
            onDelete: ReferentialAction.SetNull);

        var sql = SingleSql(migrationBuilder);
        StringAssert.Contains(sql, "IF OBJECT_ID('Orders', 'U') IS NOT NULL");
        StringAssert.Contains(sql, "IF NOT EXISTS (SELECT *  FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Orders' AND COLUMN_NAME = 'CustomerId')");
        StringAssert.Contains(sql, "ON UPDATE CASCADE ON DELETE SET NULL");
    }

    [TestMethod]
    public void DropForeignKeyIfExists_BuildsExpectedSql()
    {
        var migrationBuilder = new MigrationBuilder("SqlServer");

        migrationBuilder.DropForeignKeyIfExists("FK_Order_Customer", "Orders", "dbo");

        var sql = SingleSql(migrationBuilder);
        StringAssert.Contains(sql, "IF EXISTS");
        StringAssert.Contains(sql, "fk.name = 'FK_Order_Customer'");
        StringAssert.Contains(sql, "ALTER TABLE [Orders] DROP CONSTRAINT [FK_Order_Customer]");
    }

    [TestMethod]
    public void DropColumnIfExists_AddsDropConstraintAndDropColumnSql()
    {
        var migrationBuilder = new MigrationBuilder("SqlServer");

        migrationBuilder.DropColumnIfExists("Legacy", "Orders", "dbo");

        var sqlStatements = migrationBuilder.Operations.OfType<SqlOperation>().Select(operation => operation.Sql).ToList();
        Assert.AreEqual(2, sqlStatements.Count);
        StringAssert.Contains(sqlStatements[0], "default_constraints");
        StringAssert.Contains(sqlStatements[1], "ALTER TABLE [Orders] DROP COLUMN [Legacy]");
    }

    [TestMethod]
    public void ForceAlterColumn_AddsDropConstraintAndAlterSql()
    {
        var migrationBuilder = new MigrationBuilder("SqlServer");

        migrationBuilder.ForceAlterColumn<int>("Score", "Orders", type: "int", nullable: false);

        var sqlStatements = migrationBuilder.Operations.OfType<SqlOperation>().Select(operation => operation.Sql).ToList();
        Assert.AreEqual(2, sqlStatements.Count);
        StringAssert.Contains(sqlStatements[0], "default_constraints");
        StringAssert.Contains(sqlStatements[1], "IF OBJECT_ID('Orders', 'U') IS NOT NULL");
        StringAssert.Contains(sqlStatements[1], "ALTER TABLE [Orders] ALTER COLUMN [Score] int NOT NULL");
    }

    [TestMethod]
    public void CreateIndexIfNotExists_MissingRequiredParameters_Throws()
    {
        var migrationBuilder = new MigrationBuilder("SqlServer");

        Assert.ThrowsException<ArgumentNullException>(() => migrationBuilder.CreateIndexIfNotExists(string.Empty, "Customers", "Email"));
    }

    [TestMethod]
    public void DropForeignKeyIfExists_MissingRequiredParameters_Throws()
    {
        var migrationBuilder = new MigrationBuilder("SqlServer");

        Assert.ThrowsException<ArgumentNullException>(() => migrationBuilder.DropForeignKeyIfExists(string.Empty, "Orders"));
        Assert.ThrowsException<ArgumentNullException>(() => migrationBuilder.DropForeignKeyIfExists("FK_Order_Customer", string.Empty));
    }
}
