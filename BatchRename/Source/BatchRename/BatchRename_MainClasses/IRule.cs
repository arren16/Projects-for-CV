using System.ComponentModel;
using System.Text.Json.Serialization;

namespace BatchRename_MainClasses
{
    public interface IRule : INotifyPropertyChanged, ICloneable
    {
        public string RuleName { get; }
        public Dictionary<string, string> RuleParameters { get; set; }
        
        [JsonIgnore]
        public bool ShouldAnyDialogBeOpened { get; }

        [JsonIgnore]
        public string TextToDisplay { get; }
        public Tuple<bool, string> ValidateParameters(string paramKey, string paramValue);
        public Tuple<bool, string, string> Rename(string oldName, bool isFolder);
        [JsonIgnore]
        public bool HaveGlobalAffect { get; }
        public void ResetGlobalParameter();
    }
}
