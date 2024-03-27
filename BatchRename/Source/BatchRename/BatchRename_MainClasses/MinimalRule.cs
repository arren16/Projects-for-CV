using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchRename_MainClasses
{
    public class MinimalRule
    {
        public string RuleName { get; set; } = "";
        public Dictionary<string, string>? RuleParameters { get; set; }
    }
}
