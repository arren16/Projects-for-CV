using BatchRename_MainClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Rule_RemoveBeginningAndEndingSpace
{
    public class RemoveBeginningAndEndingSpaceRule : IRule, ICloneable
    {
        public string RuleName => "RemoveBeginningAndEndingSpaceRule";

        public bool ShouldAnyDialogBeOpened => false;

        public string TextToDisplay => "Remove all space from the beginning and the ending of the filename.";

        public Dictionary<string, string> RuleParameters
        {
            get => new Dictionary<string, string>();
            set 
            {
                // Do nothing
            }
        }

        public bool HaveGlobalAffect => false;

        public event PropertyChangedEventHandler? PropertyChanged;

        public object Clone()
        {
            return MemberwiseClone();
        }

        public void ResetGlobalParameter()
        {
            throw new NotSupportedException();
        }

        public Tuple<bool, string, string> Rename(string oldName, bool isFolder)
        {
            bool success = true;
            string message = "";
            string result = "";

            if (string.IsNullOrEmpty(oldName))
            {
                result = "";
            }
            else 
            {
                Regex regex = new Regex(@"^\s+|\s+$");
                
                string[] splitResult = oldName.Split('.');
                
                if (isFolder)
                {
                    result = regex.Replace(oldName, "");
                }    
                else
                {
                    string extension = splitResult.Last();
                    oldName = string.Join(".", splitResult.SkipLast(1));
                    result = regex.Replace(oldName, "") + "." + extension;
                }
            }

            return new Tuple<bool, string, string>
            (
                success, message, result
            );
        }

        public Tuple<bool, string> ValidateParameters(string paramKey, string paramValue)
        {
            throw new NotSupportedException();
        }
    }
}
