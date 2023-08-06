using System;
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

namespace MichaelKappel.Repository.Bases
{
    public abstract class SqlRepositoryPagingBase<T> : SqlRepositoryBase<T> where T : class
    {

        protected internal string SqlPaging = $" OFFSET @PageIndex ROWS FETCH NEXT @PageSize ROWS ONLY";

        protected SqlRepositoryPagingBase(string connectionString) : base(connectionString)
        {
        }

        protected SqlRepositoryPagingBase(IConnectionStringConfiguration databaseOptions) : base(databaseOptions)
        {
        }



        private Task<int> GetRecordCountAsync(string sql, SqlParameter[] parameters)
        {
            return Task<int>.Factory.StartNew(() =>
            {
                return GetRecordCount(sql, parameters);
            });
        }

        private int GetRecordCount(string sql, SqlParameter[] parameters)
        {
            string innerQuery = sql.Replace("  ", " ");

            innerQuery = Regex.Replace(innerQuery, @"(?i)\bSELECT\b(?>[^()]|\((?<Depth>)|\)(?<-Depth>))*(?(Depth)(?!))\bFROM\b", "SELECT 1 AS FakeColumn FROM", RegexOptions.Singleline | RegexOptions.IgnoreCase);

            string sqlOrderBy = "ORDER BY";
            if (innerQuery.Contains(sqlOrderBy, StringComparison.OrdinalIgnoreCase))
            {
                string[] sqlSplit = Regex.Split(innerQuery, sqlOrderBy, RegexOptions.IgnoreCase);

                innerQuery = sqlSplit[0];
            }

            string countSql = @$"SELECT COUNT(cteRecordCount.FakeColumn) FROM ({innerQuery}) AS cteRecordCount";


            return Execute(countSql, CommandType.Text, parameters);
        }

        protected void AddPaging(IPaging pageing, ref CommandType commandType, ref string sql, ref SqlParameter[] parameters)
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

        protected virtual IPagingResults<T> GetPagingResults(IPaging pageing, string sql, CommandType commandType, params SqlParameter[] parameters)
        {
            int totalRecordCount = GetRecordCount(sql, parameters);

            AddPaging(pageing, ref commandType, ref sql, ref parameters);

            IList<T> rawResults = GetModels(sql, commandType, parameters.ToArray());

            return new PagingResultModel<T>(pageing, totalRecordCount, rawResults);
        }

        protected virtual async Task<IPagingResults<T>> GetPagingResultsAsync(IPaging pageing, string sql, CommandType commandType, params SqlParameter[] parameters)
        {
            Task<int> totalRecordCount = GetRecordCountAsync(sql, parameters);

            AddPaging(pageing, ref commandType, ref sql, ref parameters);

            Task<IList<T>> rawResults = GetModelsAsync(sql, commandType, parameters.ToArray());

            await Task.WhenAll(totalRecordCount, rawResults);

            return new PagingResultModel<T>(pageing, await totalRecordCount, await rawResults);
        }
    }
}
