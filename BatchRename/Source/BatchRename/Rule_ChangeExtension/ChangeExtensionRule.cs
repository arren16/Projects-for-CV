using System.ComponentModel;
using BatchRename_MainClasses;
using BatchRename_UtilClasses;

namespace Rule_ChangeExtension
{
    public class ChangeExtensionRule : IRule, ICloneable, INotifyPropertyChanged
    {
        public string RuleName => "ChangeExtensionRule";
        private Dictionary<string, string> _ruleParameters =
            new Dictionary<string, string> {
                {"New extension", "init-extension"}
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

        public string TextToDisplay => "Change the extension to another extension (no conversion, force renaming extension)";

        public bool HaveGlobalAffect => false;

        public event PropertyChangedEventHandler? PropertyChanged;

        public object Clone()
        {
            ChangeExtensionRule rule = new ChangeExtensionRule();
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

            var newExtension = RuleParameters["New extension"];

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
                    extension = newExtension;
                    name = string.Join(".", extensionSplitter.SkipLast(1));
                }
            }
            else
            {
                // Do nothing
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

            if (paramKey == "New extension")
            {
                if (string.IsNullOrEmpty(paramValue))
                {
                    isValid = false;
                    message = "Extension can't be empty; ";
                }
                else
                {
                    isValid = NameUtils.IsValidName(paramValue);

                    if (!isValid)
                    {
                        message = NameUtils.InvalidNameErrorMessage;
                        message = string.Concat(message, "; ");
                    }
                    else
                    {
                        // Do nothing
                    }

                    if (paramValue.Contains(" "))
                    {
                        isValid = false;
                        message = "An extension can't contain spaces; ";
                    }
                    else
                    {
                        // Do nothing
                    }
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
