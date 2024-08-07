﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

using System.Threading.Tasks;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;
using System.Reflection;
using MichaelKappel.Repository.Interfaces;
using MichaelKappel.Repository.Interfaces.Models;
using MichaelKappel.Repositories.SqlRepositoryBase.Models;
using System.Runtime.Caching;
using MichaelKappel.Repositories.Common.Models;
using Microsoft.Extensions.Options;
using MichaelKappel.Repositories.Common.Models.Configuration;

namespace MichaelKappel.Repository.Bases
{
    public abstract class SqlRepositoryPagingBase<T> : SqlRepositoryBase<T> where T : class
    {
        private ObjectCache cache = MemoryCache.Default;

        protected internal string SqlPaging = $" OFFSET @PageIndex ROWS FETCH NEXT @PageSize ROWS ONLY";

        protected SqlRepositoryPagingBase(string connectionString) : base(connectionString)
        {
        }

        protected SqlRepositoryPagingBase(IOptions<DatabaseOptionModel> databaseOptions) : base(databaseOptions)
        {
        }

        private (String Sql, SqlParameter[] Parameters) GetRecordCountQuery(String sql, SqlParameter[] parameters)
        {

            List<SqlParameter> countParameters = new();
            foreach (SqlParameter p in parameters)
            {
                countParameters.Add(new SqlParameter(p.ParameterName, p.Value));
            }

            String innerQuery = sql.Replace("  ", " ");

            Match match = Regex.Match(innerQuery, @"(?i)\bSELECT\b(?>[^()]|\((?<Depth>)|\)(?<-Depth>))*(?(Depth)(?!))\bFROM\b", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (match.Success)
            {
                innerQuery = innerQuery.Remove(match.Index, match.Length)
                                         .Insert(match.Index, "SELECT 1 AS FakeColumn FROM");
            }

            String sqlOrderBy = "ORDER BY";
            if (innerQuery.Contains(sqlOrderBy, StringComparison.OrdinalIgnoreCase))
            {
                String[] sqlSplit = Regex.Split(innerQuery, sqlOrderBy, RegexOptions.IgnoreCase);

                innerQuery = sqlSplit[0];
            }

            String countSql = @$"SELECT COUNT(cteRecordCount.FakeColumn) FROM ({innerQuery}) AS cteRecordCount";

            return (countSql, countParameters.ToArray());
        }


        private int GetRecordCount(String sql, SqlParameter[] parameters, bool isCountCached = true)
        {
            if (isCountCached)
            {
                String cacheKey = GenerateCacheKey(sql, parameters);

                if (cache.Contains(cacheKey))
                {
                    return (int)cache.Get(cacheKey);
                }
                else
                {
                    int recordCount = ExecuteCountQuery(sql, parameters);
                    CacheItemPolicy cachePolicy = new CacheItemPolicy
                    {
                        SlidingExpiration = TimeSpan.FromHours(1)
                    };
                    cache.Add(cacheKey, recordCount, cachePolicy);
                    return recordCount;
                }
            }
            else
            {
                return ExecuteCountQuery(sql, parameters);
            }
        }

        private int ExecuteCountQuery(String sql, SqlParameter[] parameters)
        {
            (String Sql, SqlParameter[] Parameters) countingQuery = GetRecordCountQuery(sql, parameters);
            return ExecuteScalar<int>(countingQuery.Sql, CommandType.Text, countingQuery.Parameters);
        }

        private async Task<int> ExecuteCountQueryAsync(String sql, SqlParameter[] parameters)
        {
            (String Sql, SqlParameter[] Parameters) countingQuery = GetRecordCountQuery(sql, parameters);

            return await ExecuteScalarAsync<int>(countingQuery.Sql, CommandType.Text, countingQuery.Parameters);
        }

        private String GenerateCacheKey(String sql, SqlParameter[] parameters)
        {
            String parameterKey = String.Join("_", parameters.Select(p => p.ParameterName + "=" + p.Value));
            return sql + "_" + parameterKey;
        }

        private async Task<int> GetRecordCountAsync(String sql, SqlParameter[] parameters, bool isCountCached = true)
        {
            if (isCountCached)
            {
                String cacheKey = GenerateCacheKey(sql, parameters);

                if (cache.Contains(cacheKey))
                {
                    return (int)cache.Get(cacheKey);
                }
                else
                {
                    int recordCount = await ExecuteCountQueryAsync(sql, parameters);
                    CacheItemPolicy cachePolicy = new CacheItemPolicy
                    {
                        SlidingExpiration = TimeSpan.FromHours(1)
                    };
                    cache.Add(cacheKey, recordCount, cachePolicy);
                    return recordCount;
                }
            }
            else
            {
                return await ExecuteCountQueryAsync(sql, parameters);
            }
        }

        protected void AddPaging(IPaging pageing, ref CommandType commandType, ref String sql, ref SqlParameter[] parameters)
        {
            if (commandType != CommandType.Text)
            {
                throw new Exception("SqlRepositoryPagingBase currently only supports commandType of Text");
            }

            if (!sql.Contains(SqlPaging))
            {
                sql += SqlPaging;
            }

            List<SqlParameter> parametersWithPaging = parameters.ToList();

            parametersWithPaging.Add(new SqlParameter("@PageIndex", pageing.PageIndex));
            parametersWithPaging.Add(new SqlParameter("@PageSize", pageing.PageSize));

            parameters = parametersWithPaging.ToArray();
        }

        protected virtual PagingResultsModel<T> GetPagingResults(IPaging pageing, String sql, CommandType commandType, IList<SqlParameter> parameters)
        {
            return this.GetPagingResults(pageing, sql, commandType, parameters.ToArray());
        }

        protected virtual PagingResultsModel<T> GetPagingResults(IPaging pageing, String sql, CommandType commandType, params SqlParameter[] parameters)
        {
            int totalRecordCount = GetRecordCount(sql, parameters);

            AddPaging(pageing, ref commandType, ref sql, ref parameters);

            IList<T> rawResults = GetModels(sql, commandType, parameters);

            return new PagingResultsModel<T>(pageing, totalRecordCount, rawResults);
        }
        protected virtual async Task<PagingResultsModel<T>> GetPagingResultsAsync(IPaging pageing, String sql, CommandType commandType, IList<SqlParameter> parameters)
        {
            return await this.GetPagingResultsAsync(pageing, sql, commandType, parameters.ToArray());
        }

        protected virtual async Task<PagingResultsModel<T>> GetPagingResultsAsync(IPaging pageing, String sql, CommandType commandType, params SqlParameter[] parameters)
        {
            Task<int> totalRecordCount = GetRecordCountAsync(sql, parameters);

            AddPaging(pageing, ref commandType, ref sql, ref parameters);

            Task<IList<T>> rawResults = GetModelsAsync(sql, commandType, parameters);

            await Task.WhenAll(totalRecordCount, rawResults);

            return new PagingResultsModel<T>(pageing, await totalRecordCount, await rawResults);
        }
    }
}
