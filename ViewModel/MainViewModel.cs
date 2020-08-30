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
        private DataTable dt = null;
        public DataTable Data
        {
            get
            {
                if (dt == null)
                {
                    dt = new DataTable();
                    dt.Columns.Add("CodeName");
                    dt.Columns.Add("Name");
                    dt.Columns.Add("Sum");
                    dt.Rows.Add(new string[] { "101", "asdf", "111111" });
                    dt.Rows.Add(new string[] { "101", "asdf", "111111" });
                    dt.Rows.Add(new string[] { "101", "asdf", "111111" });
                }
                return dt;
            }
            set
            {
                dt = value;
                OnPropertyChanged("Data");
            }
        }
        private void ParsingData(object obj)
        {
            dt.Rows.Add(new string[] { "101", "asdf", "222222" });
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
