using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Reflection;

namespace EdiuxTemplateWebApp
{
    public static class StoredProcedureExtension
    {
        /// <summary>
        /// 命令執行等待時間。
        /// </summary>
        private const int CommandTimeout = 38400;

        /// <summary>
        /// 取得目前資料庫的參數格式。
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        private static string GetParameterFormat(DbContext db)
        {
            DataTable tbl =
                   db.Database.Connection.GetSchema(DbMetaDataCollectionNames.DataSourceInformation);

            string m_parameterMarkerFormat =
                        tbl.Rows[0][DbMetaDataColumnNames.ParameterMarkerFormat] as string;

            if (String.IsNullOrEmpty(m_parameterMarkerFormat))
                m_parameterMarkerFormat = "{0}";

            return m_parameterMarkerFormat;
        }

        public static T ExecuteFunction<T>(this DbContext db,
                                                       string FNName,
                                                       string ParameterNames,
                                                       params object[] paramters)
        {
            try
            {              
                //Had to go this route since EF Code First doesn't support output parameters 
                //returned from sprocs very well at this point

                DbCommand cmd = db.Database.Connection.CreateCommand();
                cmd.CommandTimeout = CommandTimeout;
                cmd.CommandText = FNName;
                cmd.CommandType = CommandType.StoredProcedure;

                if (string.IsNullOrEmpty(ParameterNames))
                {
                    throw new ArgumentNullException(nameof(ParameterNames));
                }

                string[] sqlParameterNames = ParameterNames.Split(',');
                string parameterFormat = GetParameterFormat(db);

                if (sqlParameterNames != null && sqlParameterNames.Length > 0)
                {
                    for (int i = 0; i < sqlParameterNames.Length; i++)
                    {
                        DbParameter parameterObject = cmd.CreateParameter();
                        parameterObject.ParameterName = string.Format(parameterFormat, sqlParameterNames[i]);
                        parameterObject.Direction = ParameterDirection.InputOutput;
                        parameterObject.Value = paramters[i];
                        cmd.Parameters.Add(parameterObject);
                    }
                }

                if (db.Database.Connection.State == ConnectionState.Closed)
                {
                    db.Database.Connection.Open();
                }

                object result = cmd.ExecuteScalar();
                return (T)result;
            }
            catch
            {
                throw;
            }
            finally
            {
                db.Database.Connection.Close();
            }

        }

        public static int ExecuteStoredProcedure(this DbContext db, string SPName,
                                                  string ParameterNames,
                                                  string OutputParamterName,
                                                  params object[] paramters)
        {
            try
            {
                DbCommand cmd = db.Database.Connection.CreateCommand();
                cmd.CommandTimeout = CommandTimeout;
                cmd.CommandText = SPName;
                cmd.CommandType = CommandType.StoredProcedure;

                string[] sqlParameterNames = ParameterNames.Split(',');
                string parameterFormat = GetParameterFormat(db);

                if (sqlParameterNames != null && sqlParameterNames.Length > 0)
                {
                    for (int i = 0; i < sqlParameterNames.Length; i++)
                    {
                        DbParameter parameterObject = cmd.CreateParameter();
                        parameterObject.ParameterName = string.Format(parameterFormat, sqlParameterNames[i]);
                        parameterObject.Direction = ParameterDirection.InputOutput;
                        parameterObject.Value = paramters[i];
                        cmd.Parameters.Add(parameterObject);
                    }
                }

                string[] sqlOutputParameterNames = ParameterNames.Split(',');

                if (sqlOutputParameterNames != null && sqlOutputParameterNames.Length > 0)
                {
                    for (int i = 0; i < sqlOutputParameterNames.Length; i++)
                    {
                        DbParameter parameterObject = cmd.CreateParameter();
                        parameterObject.ParameterName = string.Format(parameterFormat, sqlOutputParameterNames[i]);
                        parameterObject.Direction = ParameterDirection.Output;
                        parameterObject.Value = paramters[i + sqlParameterNames.Length];
                        cmd.Parameters.Add(parameterObject);
                    }
                }

                DbParameter retunValueParameter = cmd.CreateParameter();

                retunValueParameter.Direction = ParameterDirection.ReturnValue;

                retunValueParameter.DbType = DbType.Int32;

                retunValueParameter.ParameterName = string.Format(parameterFormat, "RETURN_VALUE");

                cmd.Parameters.Add(retunValueParameter);

                if (db.Database.Connection.State == ConnectionState.Closed)
                {
                    db.Database.Connection.Open();
                }

                cmd.ExecuteNonQuery();

                return (int)retunValueParameter.Value;
            }
            catch
            {
                throw;
            }
            finally
            {
                db.Database.Connection.Close();
            }
        }

        public static int ExecuteStoredProcedure(this DbContext db,
                                                string SPName,
                                                string ParameterNames,
                                                params object[] paramters)
        {
            return ExecuteStoredProcedure(db, SPName, ParameterNames, "", paramters);
        }

        public static int ExecuteStoredProcedure<T>(this DbContext db,
                                                        string SPName,
                                                        string ParameterNames,
                                                        string OutputParamterName,
                                                        out List<T> QueryResult,
                                                        params object[] paramters)
        {
            try
            {

                DbCommand cmd = db.Database.Connection.CreateCommand();
                cmd.CommandTimeout = CommandTimeout;
                cmd.CommandText = SPName;
                cmd.CommandType = CommandType.StoredProcedure;

                string[] sqlParameterNames = ParameterNames.Split(',');
                string m_parameterMarkerFormat = GetParameterFormat(db);

                if (sqlParameterNames != null && sqlParameterNames.Length > 0)
                {
                    for (int i = 0; i < sqlParameterNames.Length; i++)
                    {
                        DbParameter parameterObject = cmd.CreateParameter();
                        parameterObject.ParameterName = string.Format(m_parameterMarkerFormat, sqlParameterNames[i]);
                        parameterObject.Direction = ParameterDirection.InputOutput;
                        parameterObject.Value = paramters[i];
                        cmd.Parameters.Add(parameterObject);
                    }
                }

                string[] sqlOutputParameterNames = ParameterNames.Split(',');

                if (sqlOutputParameterNames != null && sqlOutputParameterNames.Length > 0)
                {
                    for (int i = 0; i < sqlOutputParameterNames.Length; i++)
                    {
                        DbParameter parameterObject = cmd.CreateParameter();
                        parameterObject.ParameterName = string.Format(m_parameterMarkerFormat, sqlOutputParameterNames[i]);
                        parameterObject.Direction = ParameterDirection.Output;
                        parameterObject.Value = paramters[i + sqlParameterNames.Length];
                        cmd.Parameters.Add(parameterObject);
                    }
                }

                DbParameter retunValueParameter = cmd.CreateParameter();

                retunValueParameter.Direction = ParameterDirection.ReturnValue;
                retunValueParameter.DbType = DbType.Int32;
                retunValueParameter.ParameterName = string.Format(m_parameterMarkerFormat, "RETURN_VALUE");

                cmd.Parameters.Add(retunValueParameter);

                if (db.Database.Connection.State == ConnectionState.Closed)
                {
                    db.Database.Connection.Open();
                }

                List<T> tasks = null;

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        tasks = reader.MapToList<T>();
                    }
                }

                if (sqlOutputParameterNames != null && sqlOutputParameterNames.Length > 0)
                {
                    for (int i = 0; i < sqlOutputParameterNames.Length; i++)
                    {
                        string key = string.Format(m_parameterMarkerFormat, sqlOutputParameterNames[i]);
                        paramters[i + sqlParameterNames.Length] = cmd.Parameters[key].Value;
                    }
                }

                QueryResult = tasks;

                if (QueryResult == null)
                    QueryResult = new List<T>();

                return (int)retunValueParameter.Value;
            }
            catch
            {
                throw;
            }
            finally
            {
                db.Database.Connection.Close();
            }
        }

        public static int ExecuteStoredProcedure<T>(this DbContext db,
                                                string SPName,
                                                string ParameterNames,
                                                out List<T> QueryResult,
                                                params object[] paramters)
        {
            return ExecuteStoredProcedure(db, SPName, ParameterNames, "", out QueryResult, paramters);
        }

        public static List<T> ExecuteStoredProcedure<T>(this DbContext db,
                                                        string SPName,
                                                        string ParameterNames,
                                                        string OutputParamterName = "",
                                                        params object[] paramters)
        {

            try
            {

                DbCommand cmd = db.Database.Connection.CreateCommand();
                cmd.CommandTimeout = CommandTimeout;
                cmd.CommandText = SPName;
                cmd.CommandType = CommandType.StoredProcedure;

                string[] sqlParameterNames = ParameterNames.Split(',');
                string m_parameterMarkerFormat = GetParameterFormat(db);

                if (sqlParameterNames != null && sqlParameterNames.Length > 0)
                {
                    for (int i = 0; i < sqlParameterNames.Length; i++)
                    {
                        DbParameter parameterObject = cmd.CreateParameter();
                        parameterObject.ParameterName = string.Format(m_parameterMarkerFormat, sqlParameterNames[i]);
                        parameterObject.Direction = ParameterDirection.InputOutput;
                        parameterObject.Value = paramters[i];
                        cmd.Parameters.Add(parameterObject);
                    }
                }

                string[] sqlOutputParameterNames = ParameterNames.Split(',');

                if (sqlOutputParameterNames != null && sqlOutputParameterNames.Length > 0)
                {
                    for (int i = 0; i < sqlOutputParameterNames.Length; i++)
                    {
                        DbParameter parameterObject = cmd.CreateParameter();
                        parameterObject.ParameterName = string.Format(m_parameterMarkerFormat, sqlOutputParameterNames[i]);
                        parameterObject.Direction = ParameterDirection.Output;
                        parameterObject.Value = paramters[i + sqlParameterNames.Length];
                        cmd.Parameters.Add(parameterObject);
                    }
                }

                DbParameter retunValueParameter = cmd.CreateParameter();

                retunValueParameter.Direction = ParameterDirection.ReturnValue;
                retunValueParameter.DbType = DbType.Int32;
                retunValueParameter.ParameterName = string.Format(m_parameterMarkerFormat, "RETURN_VALUE");

                cmd.Parameters.Add(retunValueParameter);

                if (db.Database.Connection.State == ConnectionState.Closed)
                {
                    db.Database.Connection.Open();
                }

                List<T> tasks = null;

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        tasks = reader.MapToList<T>();
                    }
                }

                if (sqlOutputParameterNames != null && sqlOutputParameterNames.Length > 0)
                {
                    for (int i = 0; i < sqlOutputParameterNames.Length; i++)
                    {
                        string key = string.Format(m_parameterMarkerFormat, sqlOutputParameterNames[i]);
                        paramters[i + sqlParameterNames.Length] = cmd.Parameters[key].Value;
                    }
                }

                if (tasks == null)
                    tasks = new List<T>();

                return tasks;
            }
            catch
            {
                throw;
            }
            finally
            {
                db.Database.Connection.Close();
            }

        }

        public static List<T> ExecuteStoredProcedure<T>(this DbContext db,
                                                      string SPName,
                                                      string ParameterNames,
                                                      params object[] paramters)
        {

            return ExecuteStoredProcedure<T>(db, SPName, ParameterNames, "", paramters);

        }

        public static Dictionary<string, PropertyInfo> GetProperties<TEntity>(this object obj)
            where TEntity : class
        {
            var entity = typeof(TEntity);
            var propDict = new Dictionary<string, PropertyInfo>();
            var props = entity.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            propDict = props.ToDictionary(p => p.Name.Trim(), p => p);
            return propDict;
        }

        public static Dictionary<string, PropertyInfo> GetProperties(this object obj)
        {
            var entity = obj.GetType();
            var propDict = new Dictionary<string, PropertyInfo>();
            var props = entity.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            propDict = props.ToDictionary(p => p.Name.Trim(), p => p);
            return propDict;
        }
        public static List<T> MapToList<T>(this DbDataReader dr)
        {
            if (dr != null && dr.HasRows)
            {
                var entity = typeof(T);
                var entities = new List<T>();
                var propDict = new Dictionary<string, PropertyInfo>();
                var props = entity.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                propDict = props.ToDictionary(p => p.Name.ToUpper(), p => p);

                return DataReaderToObjectList<T>(dr);
            }
            return null;
        }

        /// <summary>
        /// Creates a list of a given type from all the rows in a DataReader.
        /// 
        /// Note this method uses Reflection so this isn't a high performance
        /// operation, but it can be useful for generic data reader to entity
        /// conversions on the fly and with anonymous types.
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="reader">An open DataReader that's in position to read</param>
        /// <param name="fieldsToSkip">Optional - comma delimited list of fields that you don't want to update</param>
        /// <param name="piList">
        /// Optional - Cached PropertyInfo dictionary that holds property info data for this object.
        /// Can be used for caching hte PropertyInfo structure for multiple operations to speed up
        /// translation. If not passed automatically created.
        /// </param>
        /// <returns></returns>
        public static List<TType> DataReaderToObjectList<TType>(IDataReader reader, string fieldsToSkip = null, Dictionary<string, PropertyInfo> piList = null)
        {
            if (reader == null)
                return null;

            var items = new List<TType>();

            // Create lookup list of property info objects            
            if (piList == null)
            {
                piList = new Dictionary<string, PropertyInfo>();
                var props = typeof(TType).GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (var prop in props)
                    piList.Add(prop.Name.ToLower(), prop);
            }

            while (reader.Read())
            {
                if (typeof(TType).IsValueType || typeof(TType) == typeof(string) || typeof(TType) == typeof(String))
                {
                    //= default(TType);

                    for (int index = 0; index < reader.FieldCount; index++)
                    {
                        if (typeof(TType) == reader.GetFieldType(index))
                        {
                            if (typeof(TType) == typeof(int))
                                items.Add((TType)Convert.ChangeType(reader.GetInt32(index), typeof(TType)));
                            else
                            {
                                var inst = (TType)reader.GetValue(index);
                                items.Add(inst);
                            }

                        }
                    }
                }
                else
                {
                    var inst = Activator.CreateInstance<TType>();
                    DataReaderToObject(reader, inst, fieldsToSkip, piList);
                    items.Add(inst);
                }

            }

            return items;
        }

        /// <summary>
        /// Populates the properties of an object from a single DataReader row using
        /// Reflection by matching the DataReader fields to a public property on
        /// the object passed in. Unmatched properties are left unchanged.
        /// 
        /// You need to pass in a data reader located on the active row you want
        /// to serialize.
        /// 
        /// This routine works best for matching pure data entities and should
        /// be used only in low volume environments where retrieval speed is not
        /// critical due to its use of Reflection.
        /// </summary>
        /// <param name="reader">Instance of the DataReader to read data from. Should be located on the correct record (Read() should have been called on it before calling this method)</param>
        /// <param name="instance">Instance of the object to populate properties on</param>
        /// <param name="fieldsToSkip">Optional - A comma delimited list of object properties that should not be updated</param>
        /// <param name="piList">Optional - Cached PropertyInfo dictionary that holds property info data for this object</param>
        public static void DataReaderToObject(IDataReader reader, object instance, string fieldsToSkip = null, Dictionary<string, PropertyInfo> piList = null)
        {
            if (reader.IsClosed)
                throw new InvalidOperationException("");

            if (string.IsNullOrEmpty(fieldsToSkip))
                fieldsToSkip = string.Empty;
            else
                fieldsToSkip = "," + fieldsToSkip + ",";

            fieldsToSkip = fieldsToSkip.ToLower();

            // create a dictionary of properties to look up
            // we can pass this in so we can cache the list once 
            // for a list operation 
            if (piList == null)
            {
                piList = new Dictionary<string, PropertyInfo>();
                var props = instance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (var prop in props)
                    piList.Add(prop.Name.ToLower(), prop);
            }

            for (int index = 0; index < reader.FieldCount; index++)
            {
                string name = reader.GetName(index).ToLower();
                if (piList.ContainsKey(name))
                {
                    var prop = piList[name];

                    if (fieldsToSkip.Contains("," + name + ","))
                        continue;

                    if ((prop != null) && prop.CanWrite)
                    {
                        var val = reader.GetValue(index);
                        prop.SetValue(instance, (val == DBNull.Value) ? null : val, null);
                    }
                }
            }

            return;
        }
    }
}
