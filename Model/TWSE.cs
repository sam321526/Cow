using Cow.library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cow.Model
{
   public class TWSE : GalaSoft.MvvmLight.ObservableObject
    {
        private DataTable TWSE_dt = null;
        public DataTable TWSE_Data
        {
            get
            {
                if (TWSE_dt == null)
                {
                    TWSE_dt = SQlite_Module.GetDataTable("SELECT * FROM TWSE");
                }
                return TWSE_dt;
            }
            set
            {
                TWSE_dt = value;
                Set(ref TWSE_dt, value);
            }
        }
        public void ParsingData()
        {
            int diffDate = EndDateTime.Subtract(StartDateTime).Days;
            for (int str = 0; str <= diffDate; str++)
            {
                string date = StartDateTime.AddDays(str).ToString("yyyyMMdd");
                var data = ParsingHTML.GetTWSE(date);
                foreach (var item in data)
                {
                    SQlite_Module.Manipulate($@"INSERT INTO TWSE(""證券代號"", ""證券名稱"", ""成交股數"", ""成交筆數"", ""成交金額"",""開盤價"",""最高價"",""最低價"",""收盤價"",""漲跌(+/-)"",""漲跌價差"",""最後揭示買價"",""最後揭示買量"",""最後揭示賣價"",""最後揭示賣量"",""本益比"") VALUES({string.Join(",", item.Value)})");
                }
            }
            TWSE_Data = SQlite_Module.GetDataTable("SELECT * FROM TWSE");
        }

        #region DatePicker
        private DateTime _startDateTime = DateTime.Today;
        public DateTime StartDateTime
        {
            get
            {
                return _startDateTime;
            }
            set
            {
                Set(ref _startDateTime, value);
            }
        }
        private DateTime _endDateTime = DateTime.Today;
        public DateTime EndDateTime
        {
            get
            {
                return _endDateTime;
            }
            set
            {
                Set(ref _endDateTime, value);
            }
        }
        #endregion
    }
}
