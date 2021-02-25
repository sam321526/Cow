using GalaSoft.MvvmLight;
using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cow.Model
{
    public class SelectedMenuItem : ObservableObject
    {
        private string _ItemName;
        public string ItemName
        {
            get { return _ItemName; }
            set { Set(ref _ItemName, value); }
        }
                
        public void Selection_Changed(object obj)
        {
            if (obj.GetType().Equals(typeof(HandyControl.Data.FunctionEventArgs<object>)))
            {
                var e = (HandyControl.Data.FunctionEventArgs<object>)obj;
                var item = (SideMenuItem)e.Info;
                ItemName = item.Header.ToString();                
            }
        }
    }
}
