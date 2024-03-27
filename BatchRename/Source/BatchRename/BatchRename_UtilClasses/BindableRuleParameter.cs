using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace BatchRename_UtilClasses
{
    public class BindableRuleParameter : INotifyPropertyChanged
    {
        public string ParamKey { get; set; } = "";
        public string ParamValue { get; set; } = "";
        public string RuleError { get; set; } = "";
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
