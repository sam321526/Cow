using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Cow.library;
using Cow.Model;
using HandyControl.Controls;

namespace Cow.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #region 取得資料按鈕
        public ICommand GetData => new RelayCommand(ParsingData);
        private DataTable TWSE_dt = null;
        public DataTable TWSE
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
                OnPropertyChanged("TWSE");
            }
        }
        private void ParsingData(object obj)
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
            TWSE = SQlite_Module.GetDataTable("SELECT * FROM TWSE");
        }
        #endregion
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
                _startDateTime = value;
                OnPropertyChanged(nameof(StartDateTime));
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
                _endDateTime = value;
                OnPropertyChanged(nameof(EndDateTime));
            }
        }
        #endregion

        #region Select MenuItem
        public ICommand selectedCommand => new RelayCommand(Selection_Changed);
        public SelectedMenuItem selectedMenuItem = new SelectedMenuItem();
        public void Selection_Changed(object obj)
        {
            if (obj.GetType().Equals(typeof(HandyControl.Data.FunctionEventArgs<object>)))
            {
                var e = (HandyControl.Data.FunctionEventArgs<object>)obj;
                var item = (SideMenuItem)e.Info;
                selectedMenuItem.ItemName = item.Header.ToString();
            }
        }
        #endregion
    }
    public class RelayCommand : ICommand
    {
        #region Fields 
        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;
        #endregion // Fields 
        #region Constructors 
        public RelayCommand(Action<object> execute) : this(execute, null) { }
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute; _canExecute = canExecute;
        }
        #endregion // Constructors 
        #region ICommand Members 
        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public void Execute(object parameter) { _execute(parameter); }
        #endregion // ICommand Members 
    }
}
