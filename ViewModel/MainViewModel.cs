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
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using HandyControl.Controls;

namespace Cow.ViewModel
{
    public class MainViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        public MainViewModel()
        {
            
        }
        private TWSE _twse;
        public TWSE twse
        {
            get { if (_twse == null) _twse = new TWSE(); return _twse; }
            set { Set(ref _twse, value); }
        }
        #region 取得資料按鈕
        public ICommand GetData => new RelayCommand(twse.ParsingData);

        #endregion

        private SelectedMenuItem _selectedMenuItem;
        public SelectedMenuItem selectedMenuItem
        {
            get { if (_selectedMenuItem == null) selectedMenuItem = new SelectedMenuItem(); return _selectedMenuItem; }
            set { Set(ref _selectedMenuItem, value); }
        }
        #region Select MenuItem
        public ICommand selectedCommand => new RelayCommand<object>(selectedMenuItem.Selection_Changed);
        #endregion
    }
}
