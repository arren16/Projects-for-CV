using BatchRename_MainClasses;
using System.ComponentModel;

namespace Rule_MakeAllLowercasedAndNoWhiteSpace
{
    public class MakeAllLowercasedAndNoWhiteSpaceRule : IRule, ICloneable
    {
        public string RuleName => "MakeAllLowercasedAndNoWhiteSpaceRule";

        public Dictionary<string, string> RuleParameters 
        {
            get => new Dictionary<string, string>();
            set
            {
                // Do nothing
            }
        }

        public bool ShouldAnyDialogBeOpened => false;

        public string TextToDisplay => "Convert all characters to lowercase, remove all spaces.";

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
            string result = oldName.Replace(" ", "");
            result = result.ToLower();

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