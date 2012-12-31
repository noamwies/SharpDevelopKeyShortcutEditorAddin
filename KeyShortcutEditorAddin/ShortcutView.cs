using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyShortcutEditorAddin
{
    public class ShortcutView : INotifyPropertyChanged
    {
        private string _operation;

        private string _key;

        private bool _shouldExport;

        public string Operation {
            get { return _operation; }
            set { _operation = value; OnPropertyChanged("Operation"); }
        }

        public string Key {
            get { return _key; }
            set { _key = value; OnPropertyChanged("Key"); }
        }

        public bool ShouldExport {
            get { return _shouldExport; }
            set { _shouldExport = value; OnPropertyChanged("ShouldExport"); }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
