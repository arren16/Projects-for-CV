using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchRename_MainClasses
{
    public class Project : ICloneable
    {
        public ObservableCollection<FileItem> Files { get; set; } = new ObservableCollection<FileItem>();
        public Preset RuleList { get; set; } = new Preset();
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
