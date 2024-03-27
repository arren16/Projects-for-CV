using BatchRename_MainClasses;
using BatchRename_UtilClassess;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchRename_UtilClasses
{
    public class PresetUtils
    {
        public static Tuple<bool, string> WritePreset(string filepath, Preset preset)
        {
            bool success = true;
            string message = string.Empty;

            try
            {
                (success, message) = JSONUtils<ObservableCollection<IRule>>
                    .jsonFileWriter(
                        filepath,
                        preset.Rules
                    );
            }
            catch (Exception ex)
            {
                success = false;
                message = string.Concat(
                    ex.Message,
                    ": ",
                    ex.StackTrace
                );
            }

            return new Tuple<bool, string>(success, message);
        }

        public static Tuple<bool, string, Preset> ReadPreset(string filepath)
        {
            bool success = true;
            string message = string.Empty;

            List<MinimalRule>? readResult = new List<MinimalRule>() { };
            ObservableCollection<IRule> rules = new ObservableCollection<IRule> { };

            try
            {
                (success, message, readResult) = JSONUtils<List<MinimalRule>>
                    .jsonFileReader(
                        filepath
                    );

                if (success && (readResult != null))
                {
                    foreach (var minimalRule in readResult) 
                    {
                        IRule? rule = default(IRule);

                        bool factorySuccess = true;
                        string factoryMessage = "";

                        (factorySuccess, factoryMessage, rule) =
                            Factory<IRule>
                                .Instance()
                                .Produce(minimalRule);

                        success = success && factorySuccess;
                        if (factorySuccess)
                        {
                            rules.Add(rule!);
                        }
                        else
                        {
                            message = string.Concat(message, "\n", factoryMessage);
                        }
                    }
                }
                else
                {
                    // Do nothing
                }
            }
            catch (Exception ex)
            {
                success = false;
                message = string.Concat(
                    ex.Message,
                    ": ",
                    ex.StackTrace
                );
            }

            var response = new Tuple<bool, string, Preset>(
                success,
                message,
                new Preset
                {
                    Rules = rules
                }
            );

            return response;
        }
    }
}
