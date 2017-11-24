using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    public static class SqlHelper
    {
        public static IEnumerable<T> ExecuteQuery<T>(this ApplicationDbContext _context, string storeName, List<SqlParameter> param = null) where T : new()
        {
            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = $"dbo.[{storeName}]";
                cmd.CommandType = CommandType.StoredProcedure;
                if (param != null && param.Any())
                    cmd.Parameters.AddRange(param.ToArray());
                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
                IList<T> result = new List<T>();
                Hashtable hashtable = new Hashtable();
                foreach (PropertyInfo info in properties)
                {
                    hashtable[info.Name.ToUpper()] = info;
                }
                using (var dataReader = cmd.ExecuteReader())
                {

                    while (dataReader.Read())
                    {
                        T item = new T();
                        for (int index = 0; index < dataReader.FieldCount; index++)
                        {
                            PropertyInfo info = (PropertyInfo)
                                                hashtable[dataReader.GetName(index).ToUpper()];
                            if ((info != null) && info.CanWrite)
                            {
                                info.SetValue(item, dataReader.GetValue(index) == DBNull.Value ? null : dataReader.GetValue(index), null);
                            }
                        }
                        result.Add(item);
                    }
                }
                cmd.Connection.Close();
                return result;
            }
        }
        public static async Task<IList<T>> ExecuteQueryAsync<T>(this ApplicationDbContext _context, string storeName, List<SqlParameter> param = null) where T : new()
        {

            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = $"dbo.[{storeName}]";
                cmd.CommandType = CommandType.StoredProcedure;
                if (param.Any() && param != null)
                    cmd.Parameters.AddRange(param.ToArray());
                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();
                IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
                IList<T> result = new List<T>();
                Hashtable hashtable = new Hashtable();
                foreach (PropertyInfo info in properties)
                {
                    hashtable[info.Name.ToUpper()] = info;
                }
                using (var dataReader = await cmd.ExecuteReaderAsync())
                {
                    while (await dataReader.ReadAsync())
                    {
                        T item = new T();
                        for (int index = 0; index < dataReader.FieldCount; index++)
                        {
                            PropertyInfo info = (PropertyInfo)
                                                hashtable[dataReader.GetName(index).ToUpper()];
                            if ((info != null) && info.CanWrite)
                            {
                                info.SetValue(item, dataReader.GetValue(index) == DBNull.Value ? null : dataReader.GetValue(index), null);
                            }
                        }
                        result.Add(item);
                    }
                }
                cmd.Connection.Close();
                return result;
            }
        }
        public static IEnumerable<object> ExecuteQueryDynamicObject(this ApplicationDbContext _context, string storeName, List<SqlParameter> param = null)
        {
            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = $"dbo.[{storeName}]";
                cmd.CommandType = CommandType.StoredProcedure;
                if (param != null && param.Any())
                    cmd.Parameters.AddRange(param.ToArray());
                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();
                var retObject = new List<dynamic>();
                using (var dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        var dataRow = new ExpandoObject() as IDictionary<string, object>;
                        for (var iFiled = 0; iFiled < dataReader.FieldCount; iFiled++)
                        {

                            dataRow.Add(
                                dataReader.GetName(iFiled),
                                dataReader.IsDBNull(iFiled) ? null : dataReader[iFiled] // use null instead of {}
                            );
                        }
                        retObject.Add((ExpandoObject)dataRow);
                    }
                }
                cmd.Connection.Close();
                return retObject;
            }
        }
        public static async Task<IEnumerable<object>> ExecuteQueryDynamicObjectAsync(this ApplicationDbContext _context, string storeName, List<SqlParameter> param = null)
        {
            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = $"dbo.[{storeName}]";
                cmd.CommandType = CommandType.StoredProcedure;
                if (param.Any() && param != null)
                    cmd.Parameters.AddRange(param.ToArray());
                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();
                var retObject = new List<dynamic>();
                using (var dataReader = await cmd.ExecuteReaderAsync())
                {
                    while (await dataReader.ReadAsync())
                    {
                        var dataRow = new ExpandoObject() as IDictionary<string, object>;
                        for (var iFiled = 0; iFiled < dataReader.FieldCount; iFiled++)
                        {
                            dataRow.Add(
                                dataReader.GetName(iFiled),
                                dataReader.IsDBNull(iFiled) ? null : dataReader[iFiled] // use null instead of {}
                            );
                        }

                        retObject.Add((ExpandoObject)dataRow);
                    }
                    return retObject;
                }
            }
        }
        public static int ExecuteNonQuery(this ApplicationDbContext _context, string storeName, List<SqlParameter> param = null)
        {
            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = $"dbo.[{storeName}]";
                cmd.CommandType = CommandType.StoredProcedure;
                if (param != null && param.Any())
                    cmd.Parameters.AddRange(param.ToArray());
               
                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();
                return (int)cmd.ExecuteScalar();
            }
        }
        public  static async Task<int> ExecuteNonQueryAsync(this ApplicationDbContext _context, string storeName, List<SqlParameter> param = null)
        {
            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = $"dbo.[{storeName}]";
                cmd.CommandType = CommandType.StoredProcedure;
                if (param != null && param.Any())
                    cmd.Parameters.AddRange(param.ToArray());

                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();
                return (int)await cmd.ExecuteScalarAsync();
            }
        }
    }
}
