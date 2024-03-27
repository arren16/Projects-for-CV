using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchRename_MainClasses
{
    public class Preset : ICloneable
    {
        public ObservableCollection<IRule> Rules { get; set; } = new ObservableCollection<IRule> { };

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
