using BatchRename_MainClasses;
using System.ComponentModel;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Rule_PascalCase
{
    public class PascalCaseRule : IRule, ICloneable, INotifyPropertyChanged
    {
        public string RuleName => "PascalCaseRule";
        private Dictionary<string, string> _ruleParameters =
            new Dictionary<string, string> {
                {"Delimiter", " "}
            };
        public Dictionary<string, string> RuleParameters {
            get => _ruleParameters;
            set 
            {
                foreach (var key in value.Keys)
                {
                    _ruleParameters[key] = value[key];
                }
            } 
        }

        public bool ShouldAnyDialogBeOpened => true;

        public string TextToDisplay => "Convert filename to PascalCase";

        public bool HaveGlobalAffect => false;

        public event PropertyChangedEventHandler? PropertyChanged;

        public object Clone()
        {
            var rule = new PascalCaseRule();
            rule.RuleParameters = new Dictionary<string, string>(
                this.RuleParameters
            );
            
            return rule;
        }

        public Tuple<bool, string, string> Rename(string oldName, bool isFolder)
        {
            bool success = true;
            string message = "";
            string result = oldName;

            string extension = "";
            string name = "";

            if (!string.IsNullOrEmpty(oldName))
            {
                if (isFolder)
                {
                    name = oldName;
                }
                else
                {
                    var extensionSplitter = oldName.Split('.');
                    extension = extensionSplitter.Last();
                    name = string.Join(".", extensionSplitter.SkipLast(1));
                }

                var pascalSplitter = name.Split(RuleParameters["Delimiter"]);

                StringBuilder builder = new StringBuilder();

                foreach (string item in pascalSplitter)
                {
                    if (item.Length > 0)
                    {
                        builder.Append(
                            item[0].ToString().ToUpper()
                        );

                        builder.Append(item.Substring(1));
                    }
                    else
                    {
                        // Do nothing
                    }
                }

                if (isFolder)
                {
                    result = builder.ToString();
                }    
                else
                {
                    result = string.Concat(
                        builder.ToString(),
                        ".",
                        extension
                    );
                }

                success = true;
            }
            else
            {
                success = false;
                message = "The name is null or empty";
            }

            return new Tuple<bool, string, string>
            (
                success, message, result
            );
        }

        public Tuple<bool, string> ValidateParameters(string paramKey, string paramValue)
        {
            bool isValid = true;
            string message = "";

            if (paramKey == "Delimiter")
            {
                if (string.IsNullOrEmpty(paramValue))
                {
                    isValid = false;
                    message = "Delimiter can't be null or empty";
                }
                else
                {
                    // Do nothing
                }
            }
            else
            {
                // Do nothing
            }

            return new Tuple<bool, string>
            (
                isValid,
                message
            );
        }

        public void ResetGlobalParameter()
        {
            throw new NotSupportedException();
        }
    }
}