﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace COMPARE
{
    public class Database
    {
        public SqlConnection conn;
        public Database()
        {
            string sqlserver = "mt-return.ddns.me,3030";

            try
            {
                conn = new SqlConnection();
                conn.ConnectionString = "Server=" + sqlserver + ";Database=Humbuger;User ID=tt;Password=tt135135;Max Pool Size=250; Connection Timeout=300";
                conn.Open();
            }
            catch
            {
                throw;
            }
        }
        private void openConnect()
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
        }
        private void closeConnect()
        {
            if (conn.State == ConnectionState.Open)
                conn.Close();
        }
        private DataSet execQuery(string sql)
        {
            DataSet ds = null;
            try
            {
                openConnect();
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                da.SelectCommand.CommandTimeout = 1000;
                ds = new DataSet();
                da.Fill(ds);
                closeConnect();
            }
            catch (Exception)
            {
                throw;
            }
            finally { conn.Close(); }
            return ds;
        }
        private int execNonQuery(string sql)
        {
            int ret = 0;
            try
            {
                openConnect();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandTimeout = 500;
                ret = cmd.ExecuteNonQuery();
                closeConnect();
            }
            catch
            {
                //throw;
            }
            return ret;
        }
        public long getServerDate()
        {
            DataTable dt = execQuery("select datediff(s,'1970-1-1', getdate())").Tables[0];
            return long.Parse(dt.Rows[0][0].ToString());
        }
        public string getServerStringDate()
        {
            DataTable dt = execQuery("select GETDATE() AS [DateTime]").Tables[0];
            return dt.Rows[0][0].ToString();
        }
        public int doInsertTicket(string type, string member, string bettype, string hdp, string home, string away, string keo, string odd, string money, string usd, string betgroup, string phieuchung)
        {
            string sql = string.Format("'{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', {7}, {8}, {9}, '{10}', '{11}'", type, member, bettype, hdp, home, away, keo, odd, money, usd, betgroup, phieuchung);
            return execNonQuery(@"INSERT INTO [BetList]
                           ([type]
                           ,[member]
                           ,[bettype]
                           ,[hdp]
                           ,[home]
                           ,[away]
                           ,[keo]
                           ,[odd],[money],[usd],[betgroup],[phieuchung])
                     VALUES
                           (" + sql + ")");
        }
        public void UpdateInfoCompare(string Group, string data, bool isLive)
        {
            if (isLive)
            {
                string sql = "UPDATE CompareAccount SET CompareLive = '" + data.Replace("'", "''") + "' WHERE id= '" + Group + "'";
                execNonQuery(sql);
            }
            else
            {
                string sql = "UPDATE CompareAccount SET CompareNonLive = '" + data.Replace("'", "''") + "' WHERE id= '" + Group + "'";
                execNonQuery(sql);
            }
        }
        public void doInsertUnder(string data)
        {
            if (data.Trim() != "")
            {
                string sql = "INSERT INTO Under(data) values('" + data.Replace("'", "''") + "')";
                execNonQuery(sql);
            }
        }

        public DataTable getMinus()
        {
            return execQuery(@"SELECT [id]
                                  ,[keo]
                                  ,[data1]
                                  ,[data2]
                                  ,[systemdate]
                              FROM [Humbuger].[dbo].[Minus] where DATEDIFF(s,'1970-1-1',GETDATE())-DATEDIFF(s,'1970-1-1',systemdate) <= 43200").Tables[0];
        }

        public void doInsertMinus(string keo, string data1)
        {
            if (data1.Trim() != "")
            {
                string sql = "INSERT INTO Minus(keo, data1) values('"+keo.Replace("'", "''") + "', '" + data1.Replace("'", "''") + "')";
                execNonQuery(sql);
            }
        }
        public void doUpdateMinus(string keo, string data2)
        {
            if (data2.Trim() != "")
            {
                string sql = "UPDATE Minus set data2 = '" + data2.Replace("'", "''") + "' Where keo = '" + keo.Replace("'", "''") + "'";
                execNonQuery(sql);
            }
            else
            {
                string sql = "UPDATE Minus set data2 = null Where keo = '" + keo.Replace("'", "''") + "'";
                execNonQuery(sql);
            }
        }
        public void doDeleteMinus()
        {
            string sql = "Delete from Minus";
            execNonQuery(sql);
        }
        public DataTable getCompareAccount()
        {
            return execQuery("Select * from CompareAccount").Tables[0];
        }
        public string Login(string username, string password)
        {
            try
            {
                DataTable dt = execQuery("select top 1 username from [User] where username = '" + username.Replace("'", "''") + "' and password = '" + password.Replace("'", "''") + "' and status='1'").Tables[0];
                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0][0].ToString();
                }
                return "";
            }
            catch (Exception)
            {
                return "";
            }
        }

    }
}

