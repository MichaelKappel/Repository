using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

using System.Threading.Tasks;
using MichaelKappel.Repository.Interfaces;
using Microsoft.Extensions.Options;
using MichaelKappel.Repositories.Common.Models.Configuration;

namespace MichaelKappel.Repository.Bases
{
    public abstract class SqlRepositoryBase<T>
    {


        protected readonly string _connectionString;
        protected readonly DatabaseOptionModel? _databaseOptions;

        public SqlRepositoryBase(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public SqlRepositoryBase(IOptions<DatabaseOptionModel> databaseOptions)
        {
            this._databaseOptions = databaseOptions.Value;
            this._connectionString = string.Empty;
        }

        protected SqlConnection GetSqlConnection()
        {
            if (!string.IsNullOrWhiteSpace(_connectionString))
            {
                return new SqlConnection(_connectionString);
            }
            else
            {
                return new SqlConnection(_databaseOptions!.DefaultConnection);
            }
        }
        private SqlParameter AdjustParameterType(SqlParameter originalParameter)
        {
            // Create a copy of the original parameter with all relevant properties
            SqlParameter adjustedParameter = new SqlParameter
            {
                ParameterName = originalParameter.ParameterName,
                Direction = originalParameter.Direction,
                Size = originalParameter.Size
            };


            // Adjust the value if it is of type DateOnly
            if (originalParameter.Value is DateOnly dateOnlyValue)
            {
                adjustedParameter.Value = new DateTime(dateOnlyValue.Year, dateOnlyValue.Month, dateOnlyValue.Day);
                adjustedParameter.SqlDbType = SqlDbType.DateTime;  // Ensure the SqlDbType matches the expected type
            }
            else
            {
                adjustedParameter.Value = originalParameter.Value;
                adjustedParameter.SqlDbType = originalParameter.SqlDbType;
            }

            return adjustedParameter;
        }

        private String FormatSqlParameter(SqlParameter param)
        {
            // Handle formatting based on data type
            switch (param.SqlDbType)
            {
                case SqlDbType.DateTime:
                case SqlDbType.Date:
                case SqlDbType.Time:
                case SqlDbType.DateTime2:
                    return $"'{param.Value:yyyy-MM-dd HH:mm:ss}'";  // Standard SQL datetime formatting
                case SqlDbType.Char:
                case SqlDbType.VarChar:
                case SqlDbType.NChar:
                case SqlDbType.NVarChar:
                case SqlDbType.Text:
                case SqlDbType.NText:
                    return $"'{param.Value?.ToString()?.Replace("'", "''") ?? ""}'";  // Handle single quotes in strings
                default:
                    return param.Value.ToString();
            }
        }

        private String GetSqlType(SqlParameter param)
        {
            // Adding length for variable character types
            if (param.SqlDbType == SqlDbType.NVarChar || param.SqlDbType == SqlDbType.VarChar || param.SqlDbType == SqlDbType.NChar || param.SqlDbType == SqlDbType.Char)
            {
                Int32 size = param.Size > 0 ? param.Size : param.Value?.ToString()?.Length ?? 0;
                return $"{param.SqlDbType}({size})";
            }

            return param.SqlDbType.ToString();
        }

        protected String GetFullSql(String sql, IList<SqlParameter> parameters)
        {
            return this.GetFullSql(sql, parameters.ToArray());
        }

        protected String GetFullSql(String sql, params SqlParameter[] parameters)
        {
            List<SqlParameter> commandParameters = new();

            StringBuilder result = new("\r\n\r\n");

            if (parameters != null && parameters.Any())
            {
                foreach (SqlParameter parameterOriginal in parameters)
                {
                    commandParameters.Add(AdjustParameterType(parameterOriginal));
                }

                foreach (var commandParameter in commandParameters)
                {
                    String value = commandParameter.Value != null ? FormatSqlParameter(commandParameter) : "NULL";

                    String parameterNamePart = commandParameter
                        .ParameterName
                        .Replace("@", "")
                        .Replace(" ", String.Empty);

                    result.Append($"DECLARE @{parameterNamePart} AS {this.GetSqlType(commandParameter)} = {value}; \r\n");
                }
            }

            result.Append(sql);

            return result.ToString();
        }

        protected virtual T GetFirstModel(String storedProcedure, params SqlParameter[] parameters)
        {
            return GetFirstModel(storedProcedure, CommandType.StoredProcedure, parameters);
        }

        protected virtual T GetFirstModel(String storedProcedure, CommandType commandType, params SqlParameter[] parameters)
        {
            T result = default;
            var results = GetModels(storedProcedure, commandType, parameters);
            if (results != default(List<T>))
            {
                result = results[0];
            }
            return result;
        }


        protected virtual async Task<T> GetModelAsync(String storedProcedure, params SqlParameter[] parameters)
        {
            return await GetModelAsync(storedProcedure, CommandType.StoredProcedure, parameters);
        }

        protected virtual async Task<T> GetModelAsync(String storedProcedure, CommandType commandType, params SqlParameter[] parameters)
        {
            T result = default;
            var results = await GetModelsAsync(storedProcedure, commandType, parameters);
            if (results != default(List<T>) && results.Any())
            {
                if (results.Count == 1)
                {
                    result = results[0];
                }
                else if (results.Count > 0)
                {
                    throw new Exception(storedProcedure + " result count greater than 1");
                }
            }
            return result;
        }

        protected virtual async Task<IList<T>> GetModelsAsync(String storedProcedure, params SqlParameter[] parameters)
        {
            return await GetModelsAsync(storedProcedure, CommandType.StoredProcedure, parameters);
        }

        protected virtual async Task<IList<T>> GetModelsAsync(String storedProcedure, CommandType commandType, params SqlParameter[] parameters)
        {
            var results = default(List<T>);
            using (SqlConnection connection = GetSqlConnection())
            {
                using (SqlCommand command = new SqlCommand(storedProcedure, connection))
                {
                    command.CommandType = commandType;
                    command.CommandTimeout = 600;

                    if (parameters != null && parameters.Any())
                    {
                        foreach (SqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(AdjustParameterType(parameter));
                        }
                    }

                    connection.Open();
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        try
                        {
                            if (reader.HasRows)
                            {
                                if (HasError(reader))
                                {
                                    return null;
                                }

                                results = new List<T>();
                                while (reader.Read())
                                {
                                    results.Add(CreateInfoFromReader(reader));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            reader.Close();
                        }
                    }
                }
            }
            return (results ?? new List<T>()).AsReadOnly();
        }

        protected virtual T GetModel(String storedProcedure, IList<SqlParameter> parameters)
        {
            return GetModel(storedProcedure, parameters.ToArray());
        }

        protected virtual T GetModel(String storedProcedure, params SqlParameter[] parameters)
        {
            return GetModel(storedProcedure, CommandType.StoredProcedure, parameters);
        }

        protected virtual T GetModel(String sql, CommandType commandType, IList<SqlParameter> parameters)
        {
            return GetModel(sql, commandType, parameters.ToArray());
        }

        protected virtual T GetModel(String sql, CommandType commandType, params SqlParameter[] parameters)
        {
            IList<T> results = GetModels(sql, commandType, parameters);
            if (results != default(List<T>) && results.Any())
            {
                if (results.Count == 1)
                {
                    return (T)results[0];
                }
                else if (results.Count > 0)
                {
                    throw new Exception($" {sql} result count of {results.Count} expected 1");
                }
            }
            string className = this.GetType().Name;  // Gets the name of the current instance type

#if DEBUG
            throw new Exception($"{className} No result found for {this.GetFullSql(sql, parameters)}");
#else
            throw new Exception($"No result found on GetModel");
#endif
        }

        protected virtual IList<T> GetModels(String storedProcedure, IList<SqlParameter> parameters)
        {
            return GetModels(storedProcedure, parameters.ToArray());
        }

        protected virtual IList<T> GetModels(String storedProcedure, params SqlParameter[] parameters)
        {
            return GetModels(storedProcedure, CommandType.StoredProcedure, parameters);
        }
        protected virtual IList<T> GetModels(String sql, CommandType commandType, IList<SqlParameter> parameters)
        {
            return GetModels(sql, commandType, parameters.ToArray());
        }

        protected virtual IList<T> GetModels(String sql, CommandType commandType, params SqlParameter[] parameters)
        {
            var results = default(List<T>);
            using (SqlConnection connection = GetSqlConnection())
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = commandType;
                    command.CommandTimeout = 600;

                    if (parameters != null && parameters.Any())
                    {
                        foreach (SqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(AdjustParameterType(parameter));
                        }
                    }

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        //try
                        //{
                        if (reader.HasRows)
                        {
                            if (HasError(reader))
                            {
                                return null;
                            }

                            results = new List<T>();
                            while (reader.Read())
                            {
                                results.Add(CreateInfoFromReader(reader));
                            }
                        }
                        //}
                        //catch (Exception ex)
                        //{
                        //    throw ex;
                        //}
                        //finally
                        //{
                        //    reader.Close();
                        //}
                    }
                }
            }
            return results ?? new List<T>();
        }
        protected virtual int ExecuteNonQuery(String sql, CommandType commandType, params SqlParameter[] parameters)
        {
            try
            {

                int result = default;
                using (SqlConnection connection = GetSqlConnection())
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.CommandType = commandType;
                        command.CommandTimeout = 600;

                        if (parameters != null && parameters.Any())
                        {
                            foreach (SqlParameter parameter in parameters)
                            {
                                command.Parameters.Add(AdjustParameterType(parameter));
                            }
                        }

                        connection.Open();
                        result = command.ExecuteNonQuery();
                    }
                }
                return result;
            }
            catch
            {
#if DEBUG
                throw new Exception(this.GetFullSql(sql, parameters));
#else
                            throw;
#endif
            }

        }

        protected virtual async Task<int> ExecuteNonQueryAsync(String sql, CommandType commandType, params SqlParameter[] parameters)
        {
            try
            {

                int result = default;
                using (SqlConnection connection = GetSqlConnection())
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.CommandType = commandType;
                        command.CommandTimeout = 600;

                        if (parameters != null && parameters.Any())
                        {
                            foreach (SqlParameter parameter in parameters)
                            {
                                command.Parameters.Add(AdjustParameterType(parameter));
                            }
                        }

                        connection.Open();
                        result = await command.ExecuteNonQueryAsync();
                    }
                }
                return result;
            }
            catch
            {
#if DEBUG
                throw new Exception(this.GetFullSql(sql, parameters));
#else
                            throw;
#endif
            }
        }

        protected virtual Ts ExecuteScalar<Ts>(String sql, CommandType commandType, params SqlParameter[] parameters)
        {
            try
            {

                Ts result = default;
                using (SqlConnection connection = GetSqlConnection())
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.CommandType = commandType;
                        command.CommandTimeout = 600;

                        if (parameters != null && parameters.Any())
                        {
                            foreach (SqlParameter parameter in parameters)
                            {
                                command.Parameters.Add(AdjustParameterType(parameter));
                            }
                        }

                        connection.Open();
                        result = (Ts)command.ExecuteScalar();
                    }
                }
                return result;
            }
            catch
            {
#if DEBUG
                throw new Exception(this.GetFullSql(sql, parameters));
#else
                  throw;
#endif
            }
        }

        protected virtual async Task<Ts> ExecuteScalarAsync<Ts>(String sql, CommandType commandType, params SqlParameter[] parameters)
        {
            try
            {
                Ts result = default;
                using (SqlConnection connection = GetSqlConnection())
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.CommandType = commandType;
                        command.CommandTimeout = 600;
                        if (parameters != null && parameters.Any())
                        {
                            foreach (SqlParameter parameter in parameters)
                            {
                                command.Parameters.Add(AdjustParameterType(parameter));
                            }
                        }
                        connection.Open();
                        object? resultRaw = await command.ExecuteScalarAsync();
                        result = (Ts)resultRaw!;
                    }
                }
                return result;
            }
            catch
            {
#if DEBUG
                throw new Exception(this.GetFullSql(sql, parameters));
#else
                            throw;
#endif
            }
        }

        protected virtual int Execute(String storedProcedure, params SqlParameter[] parameters)
        {
            return ExecuteNonQuery(storedProcedure, CommandType.StoredProcedure, parameters);
        }
        protected virtual DT ReadAs<DT>(IDataReader reader, String fieldName)
        {
            Type expectedType = typeof(DT);
            var rawFieldValue = reader[fieldName];
            try
            {
                if (rawFieldValue == DBNull.Value)
                {
                    return default;
                }
                if (expectedType == typeof(bool) && rawFieldValue.GetType() == typeof(byte))
                {
                    return (DT)(object)((byte)rawFieldValue == 1 ? true : false);
                }
                if (expectedType == typeof(DateTime) && rawFieldValue.GetType() == typeof(String))
                {
                    DateTime dt;
                    if (DateTime.TryParse(rawFieldValue.ToString(), out dt))
                    {
                        return (DT)(object)dt;
                    }
                    else
                    {
                        return (DT)(object)default(DateTime);
                    }
                }
                else if (expectedType == typeof(DateOnly))
                {
                    if (rawFieldValue.GetType() == typeof(string))
                    {
                        if (DateOnly.TryParse(rawFieldValue.ToString(), out DateOnly dol))
                        {
                            return (DT)(object)dol;
                        }
                        else
                        {
                            if (DateTime.TryParse(rawFieldValue.ToString(), out DateTime dt))
                            {
                                return (DT)(object)DateOnly.FromDateTime(dt);
                            }
                            else
                            {
                                return (DT)(object)default(DateOnly);
                            }
                        }
                    }
                    else
                    {
                        return (DT)(object)DateOnly.FromDateTime((DateTime)rawFieldValue);
                    }
                }
                else if (expectedType == typeof(bool) && rawFieldValue.GetType() == typeof(String))
                {
                    return (DT)(object)(rawFieldValue.ToString().ToLower() == "true" || rawFieldValue.ToString().ToLower() == "y" ? true : false);
                }
                else if (expectedType == typeof(bool) && (rawFieldValue.GetType() == typeof(int) || rawFieldValue.GetType() == typeof(short) || rawFieldValue.GetType() == typeof(byte)))
                {
                    return (DT)(object)(rawFieldValue.ToString() == "0" ? false : true);
                }
                else if (expectedType == typeof(int) && rawFieldValue.GetType() == typeof(byte))
                {
                    return (DT)(object)int.Parse(rawFieldValue.ToString()!);
                }
                else if (expectedType == typeof(int) && rawFieldValue.GetType() == typeof(short))
                {
                    return (DT)(object)int.Parse(rawFieldValue.ToString()!);
                }
                else if (expectedType == typeof(int))
                {
                    return (DT)(object)int.Parse((rawFieldValue.ToString() ?? String.Empty).Trim());
                }
                else if ((expectedType == typeof(decimal?) || expectedType == typeof(decimal)) && rawFieldValue.GetType() == typeof(double))
                {
                    String valueAsString = rawFieldValue.ToString()!;
                    if (valueAsString.Contains("E-"))
                    {
                        return (DT)(object)0M;
                    }
                    return (DT)(object)Math.Round(decimal.Parse(valueAsString), 6, MidpointRounding.AwayFromZero);
                }
                else
                {
                    return (DT)rawFieldValue;
                }
            }
            catch (Exception e)
            {
                string className = this.GetType().Name;  // Gets the name of the current instance type
                throw new Exception($"Error in {className}: fieldName {fieldName}, value {rawFieldValue}, Databse type {rawFieldValue.GetType()} C# type {typeof(DT)}", e);
            }
        }
        protected bool HasError(IDataReader reader)
        {
            if (ReaderContainsColumn(reader, "ErrorNumber"))
            {
                while (reader.Read())
                {
                    int errorNumber = ReadAs<int>(reader, "ErrorNumber");
                    int errorSeverity = ReadAs<int>(reader, "ErrorSeverity");
                    int errorState = ReadAs<int>(reader, "ErrorState");
                    String errorProcedure = ReadAs<String>(reader, "ErrorProcedure");
                    int errorLine = ReadAs<int>(reader, "ErrorLine");
                    String errorMessage = ReadAs<String>(reader, "ErrorMessage");

                    HandleSqlError(errorNumber, errorSeverity, errorState, errorProcedure, errorLine, errorMessage);
                }
                return true;
            }
            return false;
        }
        protected virtual void HandleSqlError(int ErrorNumber, int ErrorSeverity, int ErrorState, String ErrorProcedure, int ErrorLine, String ErrorMessage)
        {
            throw new Exception(ErrorMessage);
        }
        public static bool ReaderContainsColumn(IDataReader reader, String name)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(name, StringComparison.CurrentCultureIgnoreCase)) return true;
            }
            return false;
        }

        protected abstract T CreateInfoFromReader(SqlDataReader reader);
    }
}
