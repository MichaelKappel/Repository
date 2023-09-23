
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;

using System;
using System.Text;

namespace MichaelKappel.Repositories.EntityFrameworkRepositoryBase
{
    public static class MigrationBuilderExtensions
    {
        public static void AddPrimaryKeyIfNotExists(
        this MigrationBuilder migrationBuilder,
        string name,
        string table,
        string column,
        string? schema = null)
        {
            string schemaPrefix = schema != null ? $"[{schema}]." : "";

            string checkPrimaryKeySql = $@"
                IF NOT EXISTS (
                    SELECT * 
                    FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
                    WHERE TABLE_NAME = '{table}' 
                    AND CONSTRAINT_TYPE = 'PRIMARY KEY' 
                    AND CONSTRAINT_NAME = '{name}'
                )
                BEGIN
                    ALTER TABLE {schemaPrefix}[{table}] ADD CONSTRAINT [{name}] PRIMARY KEY CLUSTERED ([{column}]);
                END";

            migrationBuilder.Sql(checkPrimaryKeySql);
        }
        public static void ForceAlterColumn<T>(
            this MigrationBuilder migrationBuilder,
            String name,
            String table,
            String? type = null,
            bool? unicode = null,
            int? maxLength = null,
            bool rowVersion = false,
            String? schema = null,
            bool nullable = false,
            object? defaultValue = null,
            String? defaultValueSql = null,
            String? computedColumnSql = null,
            Type? oldClrType = null,
            String? oldType = null,
            bool? oldUnicode = null,
            int? oldMaxLength = null,
            bool oldRowVersion = false,
            bool oldNullable = false,
            object? oldDefaultValue = null,
            String? oldDefaultValueSql = null,
            String? oldComputedColumnSql = null,
            bool? fixedLength = null,
            bool? oldFixedLength = null,
            String? comment = null,
            String? oldComment = null,
            String? collation = null,
            String? oldCollation = null,
            int? precision = null,
            int? oldPrecision = null,
            int? scale = null,
            int? oldScale = null,
            bool? stored = null,
            bool? oldStored = null)
        {
                // Drop default constraint if exists
                String dropDependentDefaultConstraintSql = $@"
                    DECLARE @constraintName sysname;
                    SELECT @constraintName = [d].[name]
                    FROM [sys].[default_constraints] [d]
                    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
                    WHERE ([d].[parent_object_id] = OBJECT_ID(N'{table}') AND [c].[name] = N'{name}');
                    IF @constraintName IS NOT NULL EXEC(N'ALTER TABLE [{table}] DROP CONSTRAINT [' + @constraintName + '];');";

                migrationBuilder.Sql(dropDependentDefaultConstraintSql);

            // Build the ALTER COLUMN SQL statement
            String alterColumnSql = $"ALTER TABLE [{table}] ALTER COLUMN [{name}] ";

            if (type != null)
            {
                alterColumnSql += $"{type} ";
            }

            if (maxLength.HasValue && type != null && !type.Contains("("))
            {
                alterColumnSql += $"({maxLength.Value}) ";
            }

            alterColumnSql += (nullable ? "NULL " : "NOT NULL ");

            // Add a default value if necessary
            if (!nullable && defaultValue == null && type != null)
            {
                if (type.Contains("int", StringComparison.InvariantCultureIgnoreCase))
                {
                    defaultValue = 0;
                }
                else if (type == "float" || type.Contains("decimal", StringComparison.InvariantCultureIgnoreCase))
                {
                    defaultValue = 0.0;
                }
                else if (type.Contains("char", StringComparison.InvariantCultureIgnoreCase))
                {
                    defaultValue = "''";
                }
                else if (type.Contains("date", StringComparison.InvariantCultureIgnoreCase))
                {
                    defaultValue = "GETUTCDATE()";
                }
                else if (type == "bit")
                {
                    defaultValue = 0;
                }
                else
                {
                    throw new InvalidOperationException($"Default value for column type '{type}' is not handled.");
                }

            }

            if (!nullable)
            {
                String defaultStringValue = $"{defaultValue}";
                if (type.Contains("date", StringComparison.InvariantCultureIgnoreCase))
                {
                    defaultStringValue = $"'{DateTimeOffset.UtcNow.ToString()}'";
                }
                else if (String.IsNullOrWhiteSpace(defaultStringValue))
                {
                    if (type.Contains("char", StringComparison.InvariantCultureIgnoreCase))
                    {
                        defaultStringValue = "''";
                    }
                    else if (type.Contains("float", StringComparison.InvariantCultureIgnoreCase) || type.Contains("decimal", StringComparison.InvariantCultureIgnoreCase))
                    {
                        defaultStringValue = "0.0";
                    }
                    else if (type.Contains("int", StringComparison.InvariantCultureIgnoreCase))
                    {
                        defaultStringValue = "0";
                    }
                }

                alterColumnSql = $"UPDATE [{table}] SET [{name}] = {defaultStringValue} WHERE ISNULL([{name}],'N/A') = 'N/A'; {alterColumnSql}";


                alterColumnSql += @$"IF EXISTS(SELECT *
                        INNER JOIN sys.tables t ON c.object_id = t.object_id
                        WHERE c.is_identity = 1
                        AND t.name = '{table}' AND  c.name ='{name}' )
                    BEGIN 
                        ALTER TABLE [{{table}}] ADD CONSTRAINT DF_{{table}}_{{name}} DEFAULT ({{defaultValue}}) FOR [{{name}}];
                    END";

                 
            }
            else if (!nullable && defaultValue == null)
            {
                throw new InvalidOperationException($"Could not determine a default value for column '{name}' in table '{table}' with type '{type}'.");
            }

            alterColumnSql = alterColumnSql.Replace("DEFAULT ()", "DEFAULT('')").Replace("(1/1/0001 12:00:00 AM +00:00)", "GETUTCDATE()");
      
            if (alterColumnSql.Contains("DEFAULT ()"))
            {
                alterColumnSql += $"Debug Info: {nameof(name)} = {name}, {nameof(table)} = {table}, {nameof(type)} = {type}, {nameof(unicode)} = {unicode}, {nameof(maxLength)} = {maxLength}, {nameof(rowVersion)} = {rowVersion}, {nameof(schema)} = {schema}, {nameof(nullable)} = {nullable}, {nameof(defaultValue)} = {defaultValue}, {nameof(defaultValueSql)} = {defaultValueSql}, {nameof(computedColumnSql)} = {computedColumnSql}, {nameof(oldClrType)} = {oldClrType}, {nameof(oldType)} = {oldType}, {nameof(oldUnicode)} = {oldUnicode}, {nameof(oldMaxLength)} = {oldMaxLength}, {nameof(oldRowVersion)} = {oldRowVersion}, {nameof(oldNullable)} = {oldNullable}, {nameof(oldDefaultValue)} = {oldDefaultValue}, {nameof(oldDefaultValueSql)} = {oldDefaultValueSql}, {nameof(oldComputedColumnSql)} = {oldComputedColumnSql}, {nameof(fixedLength)} = {fixedLength}, {nameof(oldFixedLength)} = {oldFixedLength}, {nameof(comment)} = {comment}, {nameof(oldComment)} = {oldComment}, {nameof(collation)} = {collation}, {nameof(oldCollation)} = {oldCollation}, {nameof(precision)} = {precision}, {nameof(oldPrecision)} = {oldPrecision}, {nameof(scale)} = {scale}, {nameof(oldScale)} = {oldScale}, {nameof(stored)} = {stored}, {nameof(oldStored)} = {oldStored}";
            }

            String checkTableExistsSql = $@"IF OBJECT_ID('{table}', 'U') IS NOT NULL 
                                                BEGIN 
                                                    IF EXISTS (SELECT *  FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{table}' AND COLUMN_NAME = '{name}') 
                                                        BEGIN
                                                            
                                                            DECLARE @sql NVARCHAR(MAX) = '{alterColumnSql.Replace("'","''")}'
                                                            EXEC sp_executesql @sql;
                                                        END 
                                                END";

            // Execute the SQL command to alter the column
            migrationBuilder.Sql(checkTableExistsSql);
        }

        public static OperationBuilder<SqlOperation> CreateIndexIfNotExists(
           this MigrationBuilder migrationBuilder,
           String name,
           String table,
           String column,
           String? schema = null,
           bool unique = false,
           String? filter = null)
        {
            if (String.IsNullOrEmpty(name) || String.IsNullOrEmpty(table) || String.IsNullOrEmpty(column))
            {
                throw new ArgumentNullException("Name, table, and column must be provided.");
            }

            String sqlStatement = BuildSqlStatementForIndex("IF NOT EXISTS", name, table, schema);
            String uniqueString = unique ? "UNIQUE" : "";
            String filterString = filter != null ? $"WHERE {filter}" : "";
            String createIndexCommand = $"CREATE {uniqueString} NONCLUSTERED INDEX [{name}] ON [{table}] ([{column}] ASC) {filterString}";

            return migrationBuilder.Sql(sqlStatement + " BEGIN " + createIndexCommand + GetIndexOptions() + " END");
        }

        public static void DropIndexIfExists(
        this MigrationBuilder migrationBuilder,
        String name,
        String? table = null,
        String? schema = null)
        {
            String sqlStatement = BuildSqlStatementForIndex("IF EXISTS", name, table, schema);
            String dropIndexCommand = $"DROP INDEX [{name}]";

            if (!String.IsNullOrEmpty(table))
            {
                dropIndexCommand += $" ON [{table}]";
            }

            migrationBuilder.Sql(sqlStatement + " BEGIN " + dropIndexCommand + " END");
        }

        private static String BuildSqlStatementForIndex(String condition, String name, String? table, String? schema)
        {
            String sqlStatement = $"{condition} (SELECT * FROM sys.indexes WHERE name = '{name}'";

            if (!String.IsNullOrEmpty(table))
            {
                sqlStatement += $" AND object_id = OBJECT_ID('{table}')";
            }

            if (!String.IsNullOrEmpty(schema))
            {
                sqlStatement += $" AND OBJECT_SCHEMA_NAME(object_id) = '{schema}'";
            }

            sqlStatement += ")";

            return sqlStatement;
        }


        private static String GetIndexOptions()
        {
            return " WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]";
        }


            public static void AddForeignKeyIfNotExists(
               this MigrationBuilder migrationBuilder,
               String name,
               String table,
               String column,
               String principalTable,
               String? schema = null,
               String? principalSchema = null,
               String? principalColumn = null,
               ReferentialAction onUpdate = ReferentialAction.NoAction,
               ReferentialAction onDelete = ReferentialAction.NoAction)
            {
                if (String.IsNullOrEmpty(name) || String.IsNullOrEmpty(table) || String.IsNullOrEmpty(column) || String.IsNullOrEmpty(principalTable))
                {
                    throw new ArgumentNullException("Name, table, column, and principalTable must be provided.");
                }

                principalColumn = String.IsNullOrEmpty(principalColumn) ? column : principalColumn;

                String sqlStatement = BuildSqlStatementForForeignKey("IF NOT EXISTS", name, table, schema);
                String updateAction = GetReferentialActionString(onUpdate);
                String deleteAction = GetReferentialActionString(onDelete); 
                String addForeignKeyCommand = $@"ALTER TABLE [{table}] ADD CONSTRAINT [{name}] 
                                                 FOREIGN KEY ([{column}]) REFERENCES [{principalTable}] ([{principalColumn}])
                                                 ON UPDATE {updateAction} ON DELETE {deleteAction}";

                addForeignKeyCommand = $@" BEGIN 
                                                {sqlStatement}
                                                {addForeignKeyCommand.Replace("RESTRICT", "NO ACTION")}  
                                            END ";


            String checkTableExistsSql = $@"IF OBJECT_ID('{table}', 'U') IS NOT NULL 
                                                BEGIN 
                                                    IF NOT EXISTS (SELECT *  FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{table}' AND COLUMN_NAME = '{column}') 
                                                        BEGIN
                                                           {addForeignKeyCommand}
                                                        END 
                                                END";

            migrationBuilder.Sql(checkTableExistsSql);
        }

        public static void DropForeignKeyIfExists(
           this MigrationBuilder migrationBuilder,
           String name,
           String table,
           String? schema = null)
        {
            if (String.IsNullOrEmpty(name) || String.IsNullOrEmpty(table))
            {
                throw new ArgumentNullException("Name and table must be provided.");
            }

            String sqlStatement = BuildSqlStatementForForeignKey("IF EXISTS", name, table, schema);
            String dropForeignKeyCommand = $"ALTER TABLE [{table}] DROP CONSTRAINT [{name}]";

            migrationBuilder.Sql(sqlStatement + " BEGIN " + dropForeignKeyCommand + " END");
        }

        private static String BuildSqlStatementForForeignKey(String condition, String name, String? table, String? schema)
        {
            String sqlStatement = $"{condition} (SELECT 1 FROM sys.foreign_keys fk INNER JOIN sys.objects obj ON fk.parent_object_id = obj.object_id WHERE fk.name = '{name}'";

            if (!String.IsNullOrEmpty(table))
            {
                sqlStatement += $" AND obj.name = '{table}'";
            }

            if (!String.IsNullOrEmpty(schema))
            {
                sqlStatement += $" AND OBJECT_SCHEMA_NAME(obj.object_id) = '{schema}'";
            }

            sqlStatement += ")";

            return sqlStatement;
        }


        private static String GetReferentialActionString(ReferentialAction action)
        {
            return action switch
            {
                ReferentialAction.Cascade => "CASCADE",
                ReferentialAction.Restrict => "RESTRICT",
                ReferentialAction.SetNull => "SET NULL",
                ReferentialAction.SetDefault => "SET DEFAULT",
                _ => "NO ACTION"
            };
        }

        public static void DropColumnIfExists(
         this MigrationBuilder migrationBuilder,
         String name,
         String table,
         String? schema = null)
        {
            if (String.IsNullOrEmpty(name) || String.IsNullOrEmpty(table))
            {
                throw new ArgumentNullException("Name and table must be provided.");
            }

            // Drop dependent default constraint if it exists
            String dropDependentDefaultConstraintSql = $@"
        DECLARE @constraintName sysname;
        SELECT @constraintName = [d].[name]
        FROM [sys].[default_constraints] [d]
        INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
        WHERE ([d].[parent_object_id] = OBJECT_ID(N'{table}') AND [c].[name] = N'{name}');
        IF @constraintName IS NOT NULL EXEC(N'ALTER TABLE [{table}] DROP CONSTRAINT [' + @constraintName + '];');";

            migrationBuilder.Sql(dropDependentDefaultConstraintSql);

            // Drop the column if it exists
            String sqlStatement = $"IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{table}' AND COLUMN_NAME = '{name}'";

            if (!String.IsNullOrEmpty(schema))
            {
                sqlStatement += $" AND TABLE_SCHEMA = '{schema}'";
            }

            sqlStatement += $") BEGIN ALTER TABLE [{table}] DROP COLUMN [{name}] END";

            migrationBuilder.Sql(sqlStatement);
        }

    }
}
