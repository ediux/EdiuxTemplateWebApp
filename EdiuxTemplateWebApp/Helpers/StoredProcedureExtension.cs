using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Reflection;

namespace EdiuxTemplateWebApp
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OutputParameterAttribute : Attribute
    {
        public int Order { get; set; }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class InputParameterMappingAttribute : Attribute
    {
        private string _inputParameters = "";
        public InputParameterMappingAttribute(string InputParamters)
        {
            _inputParameters = InputParamters;
        }

        public string InputParameters { get { return _inputParameters; } }

        public string OutputParameters { get; set; }

        private string _returnParameter = "";
        public string ReturnParameter { get { return _returnParameter; } set { _returnParameter = value; } }
    }
    public static class StoredProcedureExtension
    {
        /// <summary>
        /// 命令執行等待時間。
        /// </summary>
        private const int CommandTimeout = 38400;



        public static T ExecuteFunction<T>(this DbContext db,
                                                       string FNName,
                                                       params object[] paramters)
        {
            try
            {
                //Had to go this route since EF Code First doesn't support output parameters 
                //returned from sprocs very well at this point

                if (string.IsNullOrEmpty(FNName))
                {
                    throw new ArgumentNullException(nameof(FNName));
                }

                DbCommand cmd = db.Database.Connection.CreateCommand();
                cmd.CommandTimeout = CommandTimeout;
                cmd.CommandText = FNName;
                cmd.CommandType = CommandType.StoredProcedure;

                string m_parameterMarkerFormat = string.Empty;

                ScanforParameters(db, paramters, cmd, out m_parameterMarkerFormat);


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
                if (db.Database.Connection.State == ConnectionState.Open)
                    db.Database.Connection.Close();
            }

        }

        public static int ExecuteStoredProcedure(this DbContext db,
                                                string SPName,
                                                params object[] paramters)
        {
            try
            {
                DbCommand cmd = db.Database.Connection.CreateCommand();
                cmd.CommandTimeout = CommandTimeout;
                cmd.CommandText = SPName;
                cmd.CommandType = CommandType.StoredProcedure;

                string m_parameterMarkerFormat = string.Empty;

                ScanforParameters(db, paramters, cmd, out m_parameterMarkerFormat);

                DbParameter retunValueParameter = MakeReturnValueParameter(cmd, m_parameterMarkerFormat);

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
                if (db.Database.Connection.State == ConnectionState.Open)
                    db.Database.Connection.Close();
            }
        }

        public static int ExecuteStoredProcedure<T>(this DbContext db,
                                               string SPName,
                                               out IEnumerable<T> QueryResult,
                                               params object[] paramters)
        {
            try
            {

                DbCommand cmd = db.Database.Connection.CreateCommand();
                cmd.CommandTimeout = CommandTimeout;
                cmd.CommandText = SPName;
                cmd.CommandType = CommandType.StoredProcedure;

                string m_parameterMarkerFormat = string.Empty;

                ScanforParameters(db, paramters, cmd, out m_parameterMarkerFormat);

                DbParameter retunValueParameter = MakeReturnValueParameter(cmd, m_parameterMarkerFormat);

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

                QueryResult = tasks.AsEnumerable();

                if (QueryResult == null)
                    QueryResult = new List<T>().AsEnumerable();

                return (int)retunValueParameter.Value;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (db.Database.Connection.State == ConnectionState.Open)
                    db.Database.Connection.Close();
            }
        }

        public static void ExecuteStoredProcedure<O>(this DbContext db,
                                                string SPName,
                                                out O outputparameter,
                                                out int ReturnValue,
                                                params object[] paramters)
        {
            try
            {

                DbCommand cmd = db.Database.Connection.CreateCommand();
                cmd.CommandTimeout = CommandTimeout;
                cmd.CommandText = SPName;
                cmd.CommandType = CommandType.StoredProcedure;

                string m_parameterMarkerFormat = string.Empty;

                ScanforParameters(db, paramters, cmd, out m_parameterMarkerFormat);

                Type outputCLRType;

                ScanOutputParameters(out outputparameter, paramters, cmd, m_parameterMarkerFormat, out outputCLRType);

                DbParameter retunValueParameter = MakeReturnValueParameter(cmd, m_parameterMarkerFormat);

                if (db.Database.Connection.State == ConnectionState.Closed)
                {
                    db.Database.Connection.Open();
                }

                DbDataReader reader = cmd.ExecuteReader();

                outputparameter = makeOutputParameters(outputparameter, paramters, cmd, m_parameterMarkerFormat, outputCLRType);

                ReturnValue = (int)retunValueParameter.Value;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (db.Database.Connection.State == ConnectionState.Open)
                    db.Database.Connection.Close();
            }
        }

        public static IEnumerable<T> ExecuteStoredProcedure<T>(this DbContext db,
                                                    string SPName,
                                                    out int ReturnValue,
                                                    params object[] paramters)
        {
            try
            {

                DbCommand cmd = db.Database.Connection.CreateCommand();
                cmd.CommandTimeout = CommandTimeout;
                cmd.CommandText = SPName;
                cmd.CommandType = CommandType.StoredProcedure;

                string m_parameterMarkerFormat = string.Empty;

                ScanforParameters(db, paramters, cmd, out m_parameterMarkerFormat);

                DbParameter retunValueParameter = MakeReturnValueParameter(cmd, m_parameterMarkerFormat);

                checkDbConnectionOpen(db);

                List<T> tasks = null;

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        tasks = reader.MapToList<T>();
                    }
                }

                ReturnValue = (int)retunValueParameter.Value;

                return tasks.AsEnumerable();
            }
            catch
            {
                throw;
            }
            finally
            {
                closeDbConnection(db);
            }
        }



        public static IEnumerable<T> ExecuteStoredProcedure<T, O>(this DbContext db,
                                                        string SPName,
                                                        out O outputparameter,
                                                        out int ReturnValue,
                                                        params object[] paramters)
        {
            try
            {

                DbCommand cmd = db.Database.Connection.CreateCommand();
                cmd.CommandTimeout = CommandTimeout;
                cmd.CommandText = SPName;
                cmd.CommandType = CommandType.StoredProcedure;

                string m_parameterMarkerFormat = string.Empty;

                ScanforParameters(db, paramters, cmd, out m_parameterMarkerFormat);

                Type outputCLRType;

                ScanOutputParameters(out outputparameter, paramters, cmd, m_parameterMarkerFormat, out outputCLRType);

                DbParameter retunValueParameter = MakeReturnValueParameter(cmd, m_parameterMarkerFormat);

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

                outputparameter = makeOutputParameters(outputparameter, paramters, cmd, m_parameterMarkerFormat, outputCLRType);

                ReturnValue = (int)retunValueParameter.Value;

                if (tasks == null)
                    tasks = new List<T>();

                return tasks.AsEnumerable();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (db.Database.Connection.State == ConnectionState.Open)
                    db.Database.Connection.Close();
            }
        }





        #region Helper functions
        private static void closeDbConnection(DbContext db)
        {
            if (db.Database.Connection.State == ConnectionState.Open)
                db.Database.Connection.Close();
        }

        private static void checkDbConnectionOpen(DbContext db)
        {
            if (db.Database.Connection.State == ConnectionState.Closed)
            {
                db.Database.Connection.Open();
            }
        }

        private static Dictionary<int, string> GetParamterForSP(DbContext db, string SPName, out Dictionary<int, ParameterDirection> parameterDirections, string sys_SPName = "sp_procedure_params_rowset")
        {

            try
            {
                int i = 0;
                Dictionary<int, string> names = new Dictionary<int, string>();
                parameterDirections = new Dictionary<int, ParameterDirection>();

                DbCommand cmd = db.Database.Connection.CreateCommand();
                cmd.CommandTimeout = CommandTimeout;
                cmd.CommandText = sys_SPName;
                cmd.CommandType = CommandType.StoredProcedure;

                DbParameter p1 = cmd.CreateParameter();
                p1.ParameterName = "procedure_name";
                p1.Direction = ParameterDirection.Input;
                p1.Value = SPName;

                cmd.Parameters.Add(p1);

                checkDbConnectionOpen(db);

                DbDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Type FT = reader.GetFieldType(reader.GetOrdinal("ORDINAL_POSITION"));
                    int ordinal = (int)Convert.ChangeType(reader.GetValue(reader.GetOrdinal("ORDINAL_POSITION")), typeof(int));
                    if (i != ordinal)
                        ordinal = i;

                    int DirCode = (int)Convert.ChangeType(reader.GetValue(reader.GetOrdinal("PARAMETER_TYPE")), typeof(int));
                    switch (DirCode)
                    {
                        case 1:
                            parameterDirections.Add(ordinal, ParameterDirection.Input);
                            break;

                        case 3:
                            parameterDirections.Add(ordinal, ParameterDirection.InputOutput);
                            break;

                        case 2:
                        case 4:
                            continue;
                    }

                    string name = reader.GetString(reader.GetOrdinal("PARAMETER_NAME")).Substring(1);
                    names.Add(ordinal, name);

                    i++;
                }

                reader.Close();

                return names;
            }
            catch
            {
                throw;
            }
            finally
            {
                closeDbConnection(db);
            }
        }
        /// <summary>
        /// 取得目前資料庫的參數格式。
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        private static string GetParameterFormat(DbContext db)
        {
            if (db.Database.Connection.State == ConnectionState.Closed)
                db.Database.Connection.Open();

            DataTable tbl =
                   db.Database.Connection.GetSchema(DbMetaDataCollectionNames.DataSourceInformation);

            string m_parameterMarkerFormat =
                        tbl.Rows[0][DbMetaDataColumnNames.ParameterMarkerFormat] as string;

            if (String.IsNullOrEmpty(m_parameterMarkerFormat))
                m_parameterMarkerFormat = "{0}";

            return m_parameterMarkerFormat;
        }

        private static void ScanforParameters(DbContext db, object[] paramters, DbCommand cmd, out string parameterFormat)
        {
            parameterFormat = GetParameterFormat(db);

            if (paramters.Length > 0)
            {
                for (int i = 0; i < paramters.Length; i++)
                {
                    Type type = paramters[i].GetType();

                    if (type.IsClass)
                    {
                        Dictionary<string, PropertyInfo> props = paramters[i].GetProperties();

                        foreach (string k in props.Keys)
                        {
                            DbParameter parameterObject = cmd.CreateParameter();

                            parameterObject.ParameterName = string.Format(parameterFormat, props[k].Name);
                            parameterObject.Direction = ParameterDirection.Input;
                            parameterObject.Value = props[k].GetValue(paramters[i], null);

                            cmd.Parameters.Add(parameterObject);
                        }

                    }
                    else
                    {
                        DbParameter parameterObject = cmd.CreateParameter();

                        parameterObject.ParameterName = string.Format(parameterFormat, paramters[i]);
                        parameterObject.Direction = ParameterDirection.Input;
                        parameterObject.Value = paramters[i];
                        cmd.Parameters.Add(parameterObject);
                    }

                }
            }
        }

        private static DbParameter MakeReturnValueParameter(DbCommand cmd, string parameterFormat)
        {
            DbParameter retunValueParameter = cmd.CreateParameter();

            retunValueParameter.Direction = ParameterDirection.ReturnValue;

            retunValueParameter.DbType = DbType.Int32;

            retunValueParameter.ParameterName = string.Format(parameterFormat, "RETURN_VALUE");

            cmd.Parameters.Add(retunValueParameter);
            return retunValueParameter;
        }

        private static O makeOutputParameters<O>(O outputparameter, object[] paramters, DbCommand cmd, string m_parameterMarkerFormat, Type outputCLRType)
        {
            if (outputCLRType.IsValueType)
            {
                outputparameter = (O)cmd.Parameters[paramters.Length].Value;
            }
            else
            {
                if (outputCLRType.IsClass)
                {
                    outputparameter = Activator.CreateInstance<O>();
                    var sqlOutputParameterNames = outputparameter.GetProperties();
                    foreach (var k in sqlOutputParameterNames.Keys)
                    {
                        string parameterName = string.Format(m_parameterMarkerFormat, k);
                        PropertyInfo parameterProperty = outputCLRType.GetProperty(k);
                        if (parameterProperty != null)
                        {
                            parameterProperty.SetValue(outputparameter, cmd.Parameters[parameterName].Value);
                        }
                    }
                }
            }

            return outputparameter;
        }

        private static void ScanOutputParameters<O>(out O outputparameter, object[] paramters, DbCommand cmd, string m_parameterMarkerFormat, out Type outputCLRType)
        {
            outputparameter = Activator.CreateInstance<O>();
            outputCLRType = typeof(O);
            if (outputCLRType.IsValueType)
            {
                DbParameter parameterObject = cmd.CreateParameter();
                parameterObject.ParameterName = string.Format(m_parameterMarkerFormat, string.Format("p{0}", paramters.Length));
                parameterObject.Direction = ParameterDirection.Output;
                parameterObject.Value = default(O);
                cmd.Parameters.Add(parameterObject);
            }
            else
            {
                if (outputCLRType.IsClass)
                {
                    var sqlOutputParameterNames = outputparameter.GetProperties();

                    foreach (var k in sqlOutputParameterNames.Keys)
                    {
                        DbParameter parameterObject = cmd.CreateParameter();

                        parameterObject.ParameterName = string.Format(m_parameterMarkerFormat, k);
                        parameterObject.Direction = ParameterDirection.Output;
                        parameterObject.Value = default(O);
                        object outprarm = sqlOutputParameterNames[k].GetValue(outputparameter, null);
                        if (outprarm is Guid)
                        {
                            parameterObject.DbType = DbType.Guid;
                        }

                        OutputParameterAttribute outputParamAttr = sqlOutputParameterNames[k].GetCustomAttribute<OutputParameterAttribute>();

                        if (outputParamAttr != null)
                        {
                            cmd.Parameters.Insert(outputParamAttr.Order, parameterObject);
                        }
                        else
                        {
                            cmd.Parameters.Add(parameterObject);
                        }
                    }
                }
            }
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
        #endregion

    }
}
