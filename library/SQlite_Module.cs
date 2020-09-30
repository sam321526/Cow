using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cow.library
{
    class SQlite_Module
    {
        public static string dbPath = @"Stock.db";
        public static string cnStr = "data source=" + dbPath;
        private static string Version = "DBVersion";
        private static int _DBVersion = 0;
        public static int DBVersion
        {
            set
            {
                _DBVersion = value;
                if (JCSharp.Winform.SettingConfig.existkeyname(Version))
                    JCSharp.Winform.SettingConfig.modifyitem(Version, _DBVersion.ToString());
                else
                    JCSharp.Winform.SettingConfig.additem(Version, _DBVersion.ToString());
            }
            get
            {
                if (JCSharp.Winform.SettingConfig.existkeyname(Version))
                    int.TryParse(JCSharp.Winform.SettingConfig.getitemvalue(Version), out _DBVersion);
                return _DBVersion;
            }
        }
        /// <summary>建立資料庫連線</summary>        
        /// <returns></returns>
        public static SQLiteConnection OpenConnection()
        {
            var conntion = new SQLiteConnection()
            {
                ConnectionString = $"Data Source={dbPath};Version=3;New=False;Compress=True;"
            };
            if (conntion.State == ConnectionState.Open) conntion.Close();
            conntion.Open();
            return conntion;
        }
        /// <summary>建立新資料庫</summary>        
        public static void CreateDatabase()
        {
            var connection = new SQLiteConnection()
            {
                ConnectionString = $"Data Source=Data/{dbPath};Version=3;New=True;Compress=True;"
            };
            //connection.Open();
            connection.Close();
        }
        /// <summary>建立新資料表</summary>
        /// <param name="sqlCreateTable">建立資料表的 SQL 語句</param>
        public static void CreateTable(string sqlCreateTable)
        {
            var connection = OpenConnection();
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
        public static void Manipulate(string sqlManipulate)
        {
            var connection = OpenConnection();
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
        public static DataTable GetDataTable(string sqlQuery)
        {
            var connection = OpenConnection();
            var dataAdapter = new SQLiteDataAdapter(sqlQuery, connection);
            var myDataTable = new DataTable();
            var myDataSet = new DataSet();
            myDataSet.Clear();
            dataAdapter.Fill(myDataSet);
            myDataTable = myDataSet.Tables[0];
            if (connection.State == ConnectionState.Open) connection.Close();
            return myDataTable;
        }
        /// <summary>
        /// 確認資料庫版本
        /// </summary>
        public static void CheckVersion()
        {
            switch (DBVersion)
            {
                case 0:
                    CreateDatabase();
                    // 建立資料表 TestTable
                    var createtablestring = $@"CREATE TABLE ""TWSE"" (
                ""ID""    INTEGER,
	            ""證券代號""  TEXT,
	            ""證券名稱""  TEXT,
	            ""成交股數""  NUMERIC,
	            ""成交筆數""  NUMERIC,
	            ""成交金額""  NUMERIC,
	            ""開盤價""   NUMERIC,
	            ""最高價""   NUMERIC,
	            ""最低價""   NUMERIC,
	            ""收盤價""   NUMERIC,
	            ""漲跌(+/-)""   TEXT,
	            ""漲跌價差""  NUMERIC,
	            ""最後揭示買價""    NUMERIC,
	            ""最後揭示買量""    NUMERIC,
	            ""最後揭示賣價""    NUMERIC,
	            ""最後揭示賣量""    NUMERIC,
	            ""本益比""   NUMERIC,
	            PRIMARY KEY(""ID"" AUTOINCREMENT)
            );";
                    CreateTable(createtablestring);
                    DBVersion++;
                    break;
            }
            //// 讀取資料
            //var dataTable = GetDataTable(dbPath, @"SELECT * FROM TestTable");
        }
    }
}
