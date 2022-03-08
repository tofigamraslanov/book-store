using BulkyBook.DataAccess.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using BulkyBook.DataAccess.Repositories.Abstract;
using Microsoft.Data.SqlClient;

namespace BulkyBook.DataAccess.Repositories.Concrete
{
    public class StoredProcedureCall : IStoredProcedureCall
    {
        private readonly ApplicationDbContext _context;
        private static string _connectionString = "";

        public StoredProcedureCall(ApplicationDbContext context)
        {
            _context = context;
            _connectionString = _context.Database.GetDbConnection().ConnectionString;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public T Single<T>(string procedureName, DynamicParameters param = null)
        {
            using SqlConnection sqlConnection = new SqlConnection(_connectionString);

            sqlConnection.Open();
            return (T)Convert.ChangeType(
                sqlConnection.ExecuteScalar<T>(procedureName, param,
                    commandType: System.Data.CommandType.StoredProcedure), typeof(T));
        }

        public void Execute(string procedureName, DynamicParameters param = null)
        {
            using SqlConnection sqlConnection = new SqlConnection(_connectionString);

            sqlConnection.Open();
            sqlConnection.Execute(procedureName, param, commandType: System.Data.CommandType.StoredProcedure);
        }

        public T OneRecord<T>(string procedureName, DynamicParameters param = null)
        {
            using var sqlConnection = new SqlConnection(_connectionString);

            sqlConnection.Open();
            var value = sqlConnection.Query<T>(procedureName, param,
                commandType: System.Data.CommandType.StoredProcedure);
            return (T)Convert.ChangeType(value.FirstOrDefault(), typeof(T));
        }

        public IEnumerable<T> List<T>(string procedureName, DynamicParameters param = null)
        {
            using var sqlConnection = new SqlConnection(_connectionString);

            sqlConnection.Open();
            return sqlConnection.Query<T>(procedureName, param, commandType: System.Data.CommandType.StoredProcedure);
        }

        public Tuple<IEnumerable<T1>, IEnumerable<T2>> List<T1, T2>(string procedureName,
            DynamicParameters param = null)
        {
            using var sqlConnection = new SqlConnection(_connectionString);

            sqlConnection.Open();
            var result = sqlConnection.QueryMultiple(procedureName, param,
                commandType: System.Data.CommandType.StoredProcedure);
            var item1 = result.Read<T1>().ToList();
            var item2 = result.Read<T2>().ToList();

            return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(item1, item2);
        }
    }
}