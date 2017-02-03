using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class AspNetDbEntities2
    {
        public virtual System.Web.Security.MembershipCreateStatus aspnet_Membership_CreateUser(string applicationName, string userName, string password, string passwordSalt, string email, string passwordQuestion, string passwordAnswer, Nullable<bool> isApproved, Nullable<System.DateTime> currentTimeUtc, Nullable<System.DateTime> createDate, Nullable<int> uniqueEmail, Nullable<int> passwordFormat, out Guid userId)
        {
            ICollection<object> outputParams = new Collection<object>();

            int status = ExecuteStoredProcedureOrSqlFunction("aspnet_Membership_CreateUser",
                "ApplicationName,UserName,Password,PasswordSalt,eMail,PasswordQuestion, PasswordAnswer, IsApproved, CurrentTimeUtc,CreateDate,UniqueEmail,PasswordFormat,UserId",
                out outputParams,
                applicationName, userName,
                password, passwordSalt,
                email,
                passwordQuestion, passwordAnswer, isApproved,
                currentTimeUtc,
                createDate,
                uniqueEmail,
                passwordFormat);

            userId = Guid.Empty;

            if (outputParams?.Count > 0)
            {
                userId = (Guid)outputParams.First();
            }
            return (System.Web.Security.MembershipCreateStatus)Enum.Parse(typeof(System.Web.Security.MembershipCreateStatus), string.Format("{0}", status));
            //switch (status)
            //{
            //    case 0:
            //        return System.Web.Security.MembershipCreateStatus.Success;
            //    case 1:
            //        return System.Web.Security.MembershipCreateStatus.InvalidUserName;
            //    case 2:
            //        return System.Web.Security.MembershipCreateStatus.InvalidPassword;
            //    case 3:
            //        return System.Web.Security.MembershipCreateStatus.InvalidQuestion;

            //}
        }

        public virtual int aspnet_Membership_GetNumberOfUsersOnline(string applicationName,
           int MinutesSinceLastInActive, DateTime CurrentTimeUtc)
        {
            try
            {
                int returnvalue;

                returnvalue = ExecuteStoredProcedureOrSqlFunction("aspnet_Membership_GetNumberOfUsersOnline", "@ApplicationName, @MinutesSinceLastInActive, @CurrentTimeUtc",
                       applicationName, MinutesSinceLastInActive, CurrentTimeUtc);

                return returnvalue;
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                throw ex;
            }
        }

        public virtual int ExecuteStoredProcedureOrSqlFunction(string spName, string paramterNames, params object[] values)
        {
            try
            {
                DbCommand cmd = Database.Connection.CreateCommand();

                cmd.Connection = Database.Connection as SqlConnection;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = spName;
                cmd.Transaction = Database.CurrentTransaction?.UnderlyingTransaction as SqlTransaction;

                scanForSqlParamterNames(cmd, paramterNames);
                scanForSqlParamters(cmd, values);

                openDatabaseConnection();

                var reader = cmd.ExecuteReader();

                reader.Close();

                closeDatabaseConnection();

                int returnCode = fetchReturnValue(cmd);

                return returnCode;
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                throw ex;
            }
        }
        public virtual int ExecuteStoredProcedureOrSqlFunction<ResultModel>(string spName, string paramterNames, out ICollection<ResultModel> result, params object[] values)
        {
            try
            {
                DbCommand cmd = Database.Connection.CreateCommand();

                cmd.Connection = Database.Connection as SqlConnection;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = spName;
                cmd.Transaction = Database.CurrentTransaction?.UnderlyingTransaction as SqlTransaction;

                scanForSqlParamterNames(cmd, paramterNames);
                scanForSqlParamters(cmd, values);

                openDatabaseConnection();

                var reader = cmd.ExecuteReader();

                result = fetchResultRows<ResultModel>(reader);

                reader.Close();

                closeDatabaseConnection();

                int returnCode = fetchReturnValue(cmd);

                return returnCode;
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                throw ex;
            }
        }
        /// <summary>
        /// 執行不支援的預存程序類型。
        /// </summary>
        /// <typeparam name="ResultModel">回傳的資料列型別。</typeparam>
        /// <param name="spName">預存程序名稱。</param>
        /// <param name="paramterNames">預存程序參數名稱清單，以'，'分隔。各參數方向請以空格分開。</param>
        /// <param name="result">回傳的資料列集合。</param>
        /// <param name="outputParamterValues">傳出的輸出參數集合。</param>
        /// <param name="values">傳入的參數值。</param>
        /// <returns>預存程序回傳的執行結果代碼。</returns>
        public virtual int ExecuteStoredProcedureOrSqlFunction<ResultModel>(string spName, string paramterNames, out ICollection<ResultModel> result, out ICollection<object> outputParamterValues, params object[] values)
        {
            try
            {
                DbCommand cmd = Database.Connection.CreateCommand();

                cmd.Connection = Database.Connection as SqlConnection;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = spName;
                cmd.Transaction = Database.CurrentTransaction?.UnderlyingTransaction as SqlTransaction;

                scanForSqlParamterNames(cmd, paramterNames);
                scanForSqlParamters(cmd, values);

                openDatabaseConnection();

                var reader = cmd.ExecuteReader();

                result = fetchResultRows<ResultModel>(reader);

                reader.Close();

                closeDatabaseConnection();

                scanForOutputParamters(out outputParamterValues, cmd);

                int returnCode = fetchReturnValue(cmd);

                return returnCode;
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                throw ex;
            }
        }

        private static int fetchReturnValue(DbCommand cmd)
        {
            try
            {
                int returnCode = -1;

                if (cmd?.Parameters["@return_value"]?.Value != null && cmd?.Parameters["@return_value"]?.Direction == ParameterDirection.ReturnValue)
                {
                    returnCode = (int)cmd?.Parameters["@return_value"]?.Value;
                }

                return returnCode;
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                throw;
            }

        }

        private static void scanForOutputParamters(out ICollection<object> result, DbCommand cmd)
        {
            try
            {
                result = new Collection<object>();

                if (cmd.Parameters.Count > 0)
                {
                    object rowReturn = Activator.CreateInstance<object>();

                    for (int x = 0; x < cmd.Parameters.Count; x++)
                    {
                        if (cmd.Parameters[x].Direction == ParameterDirection.Output)
                        {

                            Type t = rowReturn.GetType();
                            PropertyInfo propinfo = t.GetProperty(cmd.Parameters[x].ParameterName.Trim('@'));

                            if (propinfo == null)
                            {
                                continue;
                            }
                            propinfo.SetValue(rowReturn, cmd.Parameters[x].Value);
                        }

                    }

                    result.Add(rowReturn);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                throw;
            }


        }

        private void closeDatabaseConnection()
        {
            if (Database.Connection.State == ConnectionState.Open)
                Database.Connection.Close();
        }

        private void openDatabaseConnection()
        {
            if (Database.Connection.State == ConnectionState.Closed)
                Database.Connection.Open();
        }

        private static ICollection<ResultModel> fetchResultRows<ResultModel>(DbDataReader reader)
        {
            ICollection<ResultModel> result = Activator.CreateInstance(typeof(Collection<>).MakeGenericType(typeof(ResultModel))) as Collection<ResultModel>;
            do
            {
                ResultModel row = Activator.CreateInstance<ResultModel>();
                Type t = row.GetType();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    PropertyInfo propinfo = t.GetProperty(reader.GetName(i));

                    if (propinfo == null)
                    {
                        continue;
                    }

                    propinfo.SetValue(row, reader.GetValue(i));
                    result.Add(row);
                }

            } while (reader.Read());
            return result;
        }

        public virtual ICollection<ResultModel> ExecuteStoredProcedureOrSqlFunction<ResultModel>(string spName, string paramterNames, out int returnValue, params object[] values)
        {
            try
            {
                DbCommand cmd = Database.Connection.CreateCommand();

                cmd.Connection = Database.Connection as SqlConnection;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = spName;
                cmd.Transaction = Database.CurrentTransaction?.UnderlyingTransaction as SqlTransaction;

                scanForSqlParamterNames(cmd, paramterNames);
                scanForSqlParamters(cmd, values);

                openDatabaseConnection();

                var reader = cmd.ExecuteReader();

                ICollection<ResultModel> result = fetchResultRows<ResultModel>(reader);

                reader.Close();

                closeDatabaseConnection();

                returnValue = fetchReturnValue(cmd);

                return result;
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                throw ex;
            }
        }

        private static void scanForSqlParamterNames(DbCommand cmd, string paramterNames)
        {
            string[] names = paramterNames.Split(',');

            if (names != null && names.Length > 0)
            {
                for (int i = 0; i < names.Length; i++)
                {
                    try
                    {
                        string[] directionInNames = names[i].Split(' ');

                        if (directionInNames?.Length >= 2)
                        {
                            string paramterName = directionInNames[0];
                            string paramterDirection = directionInNames[1].ToLowerInvariant().Trim();

                            SqlParameter paramterValue = new SqlParameter();
                            paramterValue.ParameterName = string.Format("@{0}", paramterName.Trim('@'));

                            switch (paramterDirection)
                            {
                                case "out":
                                case "output":
                                    paramterValue.Direction = ParameterDirection.Output;
                                    break;
                                default:
                                    paramterValue.Direction = ParameterDirection.Input;
                                    break;
                            }

                            cmd.Parameters.Add(paramterValue);
                        }
                        else
                        {
                            SqlParameter paramterValue = new SqlParameter();
                            paramterValue.ParameterName = string.Format("@{0}", names[i].Trim('@'));
                            paramterValue.Direction = ParameterDirection.Input;
                            cmd.Parameters.Add(paramterValue);
                        }
                    }
                    catch (Exception ex)
                    {
                        Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                        continue;
                    }

                }
            }
        }

        private static void scanForSqlParamters(DbCommand cmdparams, object[] values)
        {
            if (values?.Length > 0)
            {
                for (int i = 0; i < cmdparams.Parameters.Count; i++)
                {
                    cmdparams.Parameters[i].Value = values[i];
                }
            }

            var returnValue = new SqlParameter();

            returnValue.ParameterName = "@return_value";
            returnValue.SqlDbType = SqlDbType.Int;
            returnValue.Direction = ParameterDirection.Output;

            cmdparams.Parameters.Add(returnValue);
        }
    }
}