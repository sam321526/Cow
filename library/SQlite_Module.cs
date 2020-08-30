using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cow.library
{
    class SQlite_Module
    {
        public static string dbPath = "";
        public static string cnStr = "data source=" + dbPath;
        /// <summary>建立資料庫連線</summary>
        /// <param name="database">資料庫名稱</param>
        /// <returns></returns>
        public SQLiteConnection OpenConnection(string database)
        {
            var conntion = new SQLiteConnection()
            {
                ConnectionString = $"Data Source={database};Version=3;New=False;Compress=True;"
            };
            if (conntion.State == ConnectionState.Open) conntion.Close();
            conntion.Open();
            return conntion;
        }
        /// <summary>建立新資料庫</summary>
        /// <param name="database">資料庫名稱</param>
        public void CreateDatabase(string database)
        {
            var connection = new SQLiteConnection()
            {
                ConnectionString = $"Data Source=Data/{database};Version=3;New=True;Compress=True;"
            };
            connection.Open();
            connection.Close();
        }
        /// <summary>建立新資料表</summary>
        /// <param name="database">資料庫名稱</param>
        /// <param name="sqlCreateTable">建立資料表的 SQL 語句</param>
        public void CreateTable(string database, string sqlCreateTable)
        {
            var connection = OpenConnection(database);
            //connection.Open();
            var command = new SQLiteCommand(sqlCreateTable, connection);
            var mySqlTransaction = connection.BeginTransaction();
            try
            {
                command.Transaction = mySqlTransaction;
                command.ExecuteNonQuery();
                mySqlTransaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlTransaction.Rollback();
                throw (ex);
            }
            if (connection.State == ConnectionState.Open) connection.Close();
        }
        /// <summary>新增\修改\刪除資料</summary>
        /// <param name="database">資料庫名稱</param>
        /// <param name="sqlManipulate">資料操作的 SQL 語句</param>
        public void Manipulate(string database, string sqlManipulate)
        {
            var connection = OpenConnection(database);
            var command = new SQLiteCommand(sqlManipulate, connection);
            var mySqlTransaction = connection.BeginTransaction();
            try
            {
                command.Transaction = mySqlTransaction;
                command.ExecuteNonQuery();
                mySqlTransaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlTransaction.Rollback();
                throw (ex);
            }
            if (connection.State == ConnectionState.Open) connection.Close();
        }
        /// <summary>讀取資料</summary>
        /// <param name="database">資料庫名稱</param>
        /// <param name="sqlQuery">資料查詢的 SQL 語句</param>
        /// <returns></returns>
        public DataTable GetDataTable(string database, string sqlQuery)
        {
            var connection = OpenConnection(database);
            var dataAdapter = new SQLiteDataAdapter(sqlQuery, connection);
            var myDataTable = new DataTable();
            var myDataSet = new DataSet();
            myDataSet.Clear();
            dataAdapter.Fill(myDataSet);
            myDataTable = myDataSet.Tables[0];
            if (connection.State == ConnectionState.Open) connection.Close();
            return myDataTable;
        }

        public void test()
        {
            // 建立 SQLite 資料庫
            if (!File.Exists("data.db"))
            {
                CreateDatabase("data.db");
                // 建立資料表 TestTable
                var createtablestring = @"CREATE TABLE TestTable (日期 VARCHAR(32),公司代號 VARCHAR(32),收盤 VARCHAR(32),
                                                            開盤 VARCHAR(32),周轉率 VARCHAR(32),盤中高 VARCHAR(32),盤中低 VARCHAR(32),
                                                            上漲 VARCHAR(32),下跌 VARCHAR(32),公司名稱 VARCHAR(32));";
                CreateTable("data.db", createtablestring);
            }

            // 讀取資料
            var dataTable = GetDataTable("data.db", @"SELECT * FROM TestTable");
        }
    }
}
