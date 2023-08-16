using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

using System.Threading.Tasks;
using MichaelKappel.Repository.Interfaces;

namespace MichaelKappel.Repository.Bases
{
    public abstract class SqlRepositoryBase<T>
    {


        protected readonly string _connectionString;
        protected readonly IConnectionStringConfiguration? _databaseOptions;

        public SqlRepositoryBase(string connectionString)
        {
            _connectionString = connectionString;
        }

        public SqlRepositoryBase(IConnectionStringConfiguration databaseOptions)
        {
            _databaseOptions = databaseOptions;
            _connectionString = string.Empty;
        }

        protected SqlConnection GetSqlConnection()
        {
            if (!string.IsNullOrWhiteSpace(_connectionString))
            {
                return new SqlConnection(_connectionString);
            }
            else
            {
                return new SqlConnection(_databaseOptions!.ConnectionString);
            }
        }

        protected virtual T GetFirstModel(string storedProcedure, params SqlParameter[] parameters)
        {
            return GetFirstModel(storedProcedure, CommandType.StoredProcedure, parameters);
        }

        protected virtual T GetFirstModel(string storedProcedure, CommandType commandType, params SqlParameter[] parameters)
        {
            T result = default;
            var results = GetModels(storedProcedure, commandType, parameters);
            if (results != default(List<T>))
            {
                result = results[0];
            }
            return result;
        }


        protected virtual async Task<T> GetModelAsync(string storedProcedure, params SqlParameter[] parameters)
        {
            return await GetModelAsync(storedProcedure, CommandType.StoredProcedure, parameters);
        }

        protected virtual async Task<T> GetModelAsync(string storedProcedure, CommandType commandType, params SqlParameter[] parameters)
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

        protected virtual async Task<IList<T>> GetModelsAsync(string storedProcedure, params SqlParameter[] parameters)
        {
            return await GetModelsAsync(storedProcedure, CommandType.StoredProcedure, parameters);
        }

        protected virtual async Task<IList<T>> GetModelsAsync(string storedProcedure, CommandType commandType, params SqlParameter[] parameters)
        {
            var results = default(List<T>);
            using (SqlConnection connection = GetSqlConnection())
            {
                using (SqlCommand command = new SqlCommand(storedProcedure, connection))
                {
                    command.CommandType = commandType;
                    command.CommandTimeout = 600;
                    if (parameters != null && parameters.Count() > 0)
                    {
                        foreach (var parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
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

        protected virtual T GetModel(string storedProcedure, params SqlParameter[] parameters)
        {
            return GetModel(storedProcedure, CommandType.StoredProcedure, parameters);
        }

        protected virtual T GetModel(string sql, CommandType commandType, params SqlParameter[] parameters)
        {
            T result = default;
            var results = GetModels(sql, commandType, parameters);
            if (results != default(List<T>) && results.Any())
            {
                if (results.Count == 1)
                {
                    result = results[0];
                }
                else if (results.Count > 0)
                {
                    throw new Exception(sql + " result count greater than 1");
                }
            }
            return result;
        }

        protected virtual IList<T> GetModels(string storedProcedure, params SqlParameter[] parameters)
        {
            return GetModels(storedProcedure, CommandType.StoredProcedure, parameters);
        }

        protected virtual IList<T> GetModels(string sql, CommandType commandType, params SqlParameter[] parameters)
        {
            var results = default(List<T>);
            using (SqlConnection connection = GetSqlConnection())
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = commandType;
                    command.CommandTimeout = 600;
                    if (parameters != null && parameters.Count() > 0)
                    {
                        foreach (var parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
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
        protected virtual int ExecuteNonQuery(string sql, CommandType commandType, params SqlParameter[] parameters)
        {
            int result = default;
            using (SqlConnection connection = GetSqlConnection())
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = commandType;
                    command.CommandTimeout = 600;
                    if (parameters != null && parameters.Count() > 0)
                    {
                        foreach (var parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }

                    connection.Open();
                    result = command.ExecuteNonQuery();
                }
            }
            return result;
        }

        protected virtual async Task<int> ExecuteNonQueryAsync(string sql, CommandType commandType, params SqlParameter[] parameters)
        {
            int result = default;
            using (SqlConnection connection = GetSqlConnection())
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = commandType;
                    command.CommandTimeout = 600;
                    if (parameters != null && parameters.Count() > 0)
                    {
                        foreach (var parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }

                    connection.Open();
                    result = await command.ExecuteNonQueryAsync();
                }
            }
            return result;
        }


        protected virtual Ts ExecuteScalar<Ts>(string sql, CommandType commandType, params SqlParameter[] parameters)
        {
            Ts result = default;
            using (SqlConnection connection = GetSqlConnection())
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = commandType;
                    command.CommandTimeout = 600;
                    if (parameters != null && parameters.Count() > 0)
                    {
                        foreach (var parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }

                    connection.Open();
                    result = (Ts)command.ExecuteScalar();
                }
            }
            return result;
        }

        protected virtual async Task<Ts> ExecuteScalarAsync<Ts>(string sql, CommandType commandType, params SqlParameter[] parameters)
        {
            Ts result = default;
            using (SqlConnection connection = GetSqlConnection())
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = commandType;
                    command.CommandTimeout = 600;
                    if (parameters != null && parameters.Count() > 0)
                    {
                        foreach (var parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }

                    connection.Open();
                    object? resultRaw = await command.ExecuteScalarAsync();
                    result = (Ts)resultRaw!;
                }
            }
            return result;
        }

        protected virtual int Execute(string storedProcedure, params SqlParameter[] parameters)
        {
            return ExecuteNonQuery(storedProcedure, CommandType.StoredProcedure, parameters);
        }
        protected virtual DT ReadAs<DT>(IDataReader reader, string fieldName)
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
                if (expectedType == typeof(DateTime) && rawFieldValue.GetType() == typeof(string))
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
                else if (expectedType == typeof(bool) && rawFieldValue.GetType() == typeof(string))
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
                else if ((expectedType == typeof(decimal?) || expectedType == typeof(decimal)) && rawFieldValue.GetType() == typeof(double))
                {
                    string valueAsString = rawFieldValue.ToString()!;
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
                throw new Exception($"fieldName {fieldName} value {rawFieldValue}", e);
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
                    string errorProcedure = ReadAs<string>(reader, "ErrorProcedure");
                    int errorLine = ReadAs<int>(reader, "ErrorLine");
                    string errorMessage = ReadAs<string>(reader, "ErrorMessage");

                    HandleSqlError(errorNumber, errorSeverity, errorState, errorProcedure, errorLine, errorMessage);
                }
                return true;
            }
            return false;
        }
        protected virtual void HandleSqlError(int ErrorNumber, int ErrorSeverity, int ErrorState, string ErrorProcedure, int ErrorLine, string ErrorMessage)
        {
            throw new Exception(ErrorMessage);
        }
        public static bool ReaderContainsColumn(IDataReader reader, string name)
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
