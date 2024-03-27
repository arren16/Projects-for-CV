using System.ComponentModel;
using BatchRename_MainClasses;
using BatchRename_UtilClasses;

namespace Rule_AddingPrefix
{
    public class AddingPrefixRule : IRule, ICloneable, INotifyPropertyChanged
    {
        public string RuleName => "AddingPrefixRule";
        private Dictionary<string, string> _ruleParameters =
            new Dictionary<string, string> {
                {"Prefix", "InitPrefix"}
            };
        public Dictionary<string, string> RuleParameters
        {
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

        public string TextToDisplay => "Adding a prefix to all the files";

        public bool HaveGlobalAffect => false;

        public event PropertyChangedEventHandler? PropertyChanged;

        public object Clone()
        {
            var rule = new AddingPrefixRule();
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

            var prefix = RuleParameters["Prefix"];

            if (!string.IsNullOrEmpty(oldName))
            {
                if (isFolder)
                {
                    name = oldName;
                    extension = "";
                }
                else
                {
                    var extensionSplitter = oldName.Split('.');
                    extension = extensionSplitter.Last();
                    name = string.Join(".", extensionSplitter.SkipLast(1));
                }

                name = string.Concat(prefix, name);
            }
            else
            {
                name = prefix;
            }

            result = string.Concat(
                name,
                isFolder ? "" : ".",
                extension
            );

            return new Tuple<bool, string, string>
            (
                success, message, result
            );
        }

        public Tuple<bool, string> ValidateParameters(string paramKey, string paramValue)
        {
            bool isValid = true;
            string message = "";

            if (paramKey == "Prefix")
            {
                isValid = NameUtils.IsValidName(paramValue);

                if (!isValid)
                {
                    message = NameUtils.InvalidNameErrorMessage;
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
