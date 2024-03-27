using BatchRename_MainClasses;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Rule_GPT
{
    public class GPTRule : IRule, ICloneable
    {
        string IRule.RuleName => "GPTRule";

        public Dictionary<string, string> RuleParameters
        {
            get => new Dictionary<string, string>();
            set
            {
                // Do nothing
            }
        }

        public bool ShouldAnyDialogBeOpened => false;

        public string TextToDisplay => "Fix grammar errors, redundant and inappropriate characters with the power of ChatGPT";

        public bool HaveGlobalAffect => false;

        public event PropertyChangedEventHandler? PropertyChanged;

        public object Clone()
        {
            return MemberwiseClone();
        }

        private async Task<Tuple<bool, string, string>> AsyncRename(string oldName, bool isFolder)
        {
            bool success = true;
            string message = "";
            string result = oldName;

            string name = "";
            string extension = "";

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

                name = name.Replace(oldValue: "\\", newValue: "");

                var prompt = string.Concat(
                    $"My file name is '{name}' ",
                    "(the file extension is hidden). ",
                    "Let's detect errors in this (like ",
                    "grammar errors, ",
                    "redundant letters, ",
                    "inapproriate characters for file name like ",
                    "colons, ?, slashes, semicolons, commas, +, =, <, >, quotes, double quotes, |, [, ], etc', ",
                    "then give me a new file name with these errors fixed ",
                    "(the new name should be based on the old name) ",
                    "(place that new file name between two square brackets ",
                    "like this: 'The new file name is: [place_the_new_file_name_here] ', ",
                    "so that I can use code to parse it) ",
                    "(of course, with no file extension)"
                );

                OpenAIAPIDTO openAIAPIDTO = new OpenAIAPIDTO();
                (success, message, openAIAPIDTO) = await GPTUtils.Prompt(prompt);

                if (success)
                {
                    string responseMessage = openAIAPIDTO.choices[0].message.content;

                    // [...] or "..." or '...'
                    string pattern = @"(\[([^:/?\\;,\+=<>\""\|;\[\]\'])+\])|(\""([^:/?\\;,\+=<>\""\|;\[\]\'])+\"")|(\'([^:/?\\;,\+=<>\""\|;\[\]\'])+\')";

                    Regex regex = new Regex(pattern);

                    Match match = regex.Match(responseMessage);

                    if (match.Success)
                    {
                        success = true;
                        name = match.Value;
                        name = name.Substring(startIndex: 1, length: name.Length - 2);
                    }
                    else
                    {
                        success = false;
                        message = $"The response from GPT might be in a wrong format: {responseMessage}";
                    }
                }
                else
                {
                    // Do nothing
                }

            }
            else
            {
                success = false;
                message = "The name might be null or empty";
                result = oldName;
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
        public Tuple<bool, string, string> Rename(string oldName, bool isFolder)
        {

            bool success = true;
            string message = "";
            string result = "";

            try
            {
                Task<Tuple<bool, string, string>> asyncTask = AsyncRename(oldName, isFolder);
                asyncTask.Wait();

                (success, message, result) = asyncTask.Result; 
            }
            catch (Exception e)
            {
                success = false;
                message = e.Message;
            }

            return new Tuple<bool, string, string>(
                success, 
                message, 
                result
            );
        }
        public Tuple<bool, string> ValidateParameters(string paramKey, string paramValue)
        {
            throw new NotSupportedException();
        }

        public void ResetGlobalParameter()
        {
            throw new NotSupportedException();
        }
    }
}