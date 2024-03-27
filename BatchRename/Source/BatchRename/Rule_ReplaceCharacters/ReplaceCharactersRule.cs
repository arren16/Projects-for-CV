using System.ComponentModel;
using BatchRename_MainClasses;
using BatchRename_UtilClasses;

namespace Rule_ReplaceCharacters
{
    public class ReplaceCharactersRule : IRule, ICloneable, INotifyPropertyChanged
    {
        public string RuleName => "ReplaceCharactersRule";
        private Dictionary<string, string> _ruleParameters =
            new Dictionary<string, string> {
                {"Characters to be replaced", ""},
                {"Change to", ""}
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

        public string TextToDisplay => "Replace certain characters";

        public bool HaveGlobalAffect => false;

        public event PropertyChangedEventHandler? PropertyChanged;

        public object Clone()
        {
            var rule = new ReplaceCharactersRule();
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

            var toBeReplaced = RuleParameters["Characters to be replaced"];
            var changeTo = RuleParameters["Change to"];

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

                if (!string.IsNullOrEmpty(toBeReplaced))
                {
                    name = name.Replace(toBeReplaced, changeTo);
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

            if (paramKey == "Change to")
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
