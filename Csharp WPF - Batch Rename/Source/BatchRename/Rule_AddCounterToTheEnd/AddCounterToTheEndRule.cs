using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Text.RegularExpressions;
using BatchRename_MainClasses;
using BatchRename_UtilClasses;

namespace Rule_AddCounterToTheEnd
{
    public class AddCounterToTheEndRule : IRule, ICloneable, INotifyPropertyChanged
    {
        public string RuleName => "AddCounterToTheEndRule";
        private Dictionary<string, string> _ruleParameters =
            new Dictionary<string, string> {
                {"Start", "0"},
                {"Step", "1" },
                {"The number of digits", "0" }
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

        public string TextToDisplay => "Add counter to the end of the file (Could have padding like 01, 02, 03, ..., 10, ..., 99)";

        public bool HaveGlobalAffect => true;

        public event PropertyChangedEventHandler? PropertyChanged;

        public object Clone()
        {
            var rule = new AddCounterToTheEndRule();
            rule.RuleParameters = new Dictionary<string, string>(
                this.RuleParameters
            );

            return rule;
        }
        private int FileCounter { get; set; } = int.MinValue;
        private int FolderCounter { get; set; } = int.MinValue;
        public Tuple<bool, string, string> Rename(string oldName, bool isFolder)
        {
            bool success = true;
            string message = "";
            string result = oldName;

            string extension = "";
            string name = "";

            string padding = $"D{RuleParameters["The number of digits"]}";

            int Counter = 0;

            if (!string.IsNullOrEmpty(oldName))
            {
                int step = System.Convert.ToInt32(RuleParameters["Step"]);

                if (isFolder)
                {
                    if (FolderCounter == int.MinValue)
                    {
                        FolderCounter = System.Convert.ToInt32(RuleParameters["Start"]);
                    }
                    else
                    {
                        // Do nothing
                    }

                    Counter = FolderCounter;
                    name = oldName;
                    extension = "";
                    
                    FolderCounter = FolderCounter + step;
                }
                else
                {
                    if (FileCounter == int.MinValue)
                    {
                        FileCounter = System.Convert.ToInt32(RuleParameters["Start"]);
                    }
                    else
                    {
                        // Do nothing
                    }

                    Counter = FileCounter;
                    var extensionSplitter = oldName.Split('.');
                    extension = extensionSplitter.Last();
                    name = string.Join(".", extensionSplitter.SkipLast(1));

                    FileCounter = FileCounter + step;
                }

            
                name = string.Concat(name, " ", Counter.ToString(padding));
            }
            else
            {
                name = Counter.ToString(padding);
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


            if (paramKey == "Start" || paramKey == "Step")
            {
                Regex intRegex = new Regex("^-?\\d+$"); // integer

                isValid = intRegex.IsMatch(paramValue);

                if (!isValid)
                {
                    message = "This field should be an integer";
                }
                else
                {
                    // Do nothing
                }
            }
            else if (paramKey == "The number of digits")
            {
                Regex uintRegex = new Regex("^\\d+$"); // unsigned integer

                isValid = uintRegex.IsMatch(paramValue);

                if (!isValid)
                {
                    message = "This field should be an unsigned integer";
                }
                else
                {
                    // Do nothing
                }
            }

            return new Tuple<bool, string>
            (
                isValid,
                message
            );
        }

        public void ResetGlobalParameter()
        {
            FolderCounter = int.MinValue;
            FileCounter = int.MinValue;
        }
    }
}
