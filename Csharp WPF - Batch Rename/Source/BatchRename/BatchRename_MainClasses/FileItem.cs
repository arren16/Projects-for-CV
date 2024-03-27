using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchRename_MainClasses
{
    public class FileItem : INotifyPropertyChanged
    {
        public string OldName { get; set; } = "";
        public string NewName { get; set; } = "";
        public string Location { get; set; } = "";
        public string ErrorAndWarning { get; set; } = "";
        public bool IsFolder { get; set; } = false;

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
