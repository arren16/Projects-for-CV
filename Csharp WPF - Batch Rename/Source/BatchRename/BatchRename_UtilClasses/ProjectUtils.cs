using BatchRename_MainClasses;
using BatchRename_UtilClassess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace BatchRename_UtilClasses
{
    public class ProjectUtils
    {
        public static Tuple<bool, string> WriteProject(string filepath, Project project)
        {
            bool success = true;
            string message = string.Empty;

            try
            {
                string filesJSONString = JsonSerializer
                    .Serialize<ObservableCollection<FileItem>>(
                    project.Files
                );

                string ruleListJSONString = JsonSerializer
                    .Serialize<ObservableCollection<IRule>>(
                    project.RuleList.Rules
                );

                (success, message) = JSONUtils<DeserializableProject>
                    .jsonFileWriter(
                        filepath,
                        new DeserializableProject
                        {
                            Files = filesJSONString,
                            RuleList = ruleListJSONString
                        }
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

        public static Tuple<bool, string, Project> ReadProject(string filepath)
        {
            bool success = true;
            string message = string.Empty;

            ObservableCollection<FileItem>? files = new ObservableCollection<FileItem> { };
            ObservableCollection<IRule> rules = new ObservableCollection<IRule> { };

            try
            {
                // Read Deserializable Project
                
                DeserializableProject? readProject = new DeserializableProject();

                (success, message, readProject) = JSONUtils<DeserializableProject>
                    .jsonFileReader(
                        filepath
                    );

                if (success && readProject != null ) 
                {
                    // Extract file list from the Deserializable Project

                    files = JsonSerializer.Deserialize<ObservableCollection<FileItem>>(
                        readProject.Files
                    );

                    // Extract rule list from the Deserializable Project

                    List<MinimalRule>? readRuleList = new List<MinimalRule>() { };

                    readRuleList = JsonSerializer.Deserialize<List<MinimalRule>>(
                        readProject.RuleList
                    );
                    

                    if (readRuleList != null)
                    {
                        foreach (var minimalRule in readRuleList)
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

            var response = new Tuple<bool, string, Project>(
                success,
                message,
                new Project
                {
                    Files = (files == null) 
                        ? new ObservableCollection<FileItem> { } 
                        : files,

                    RuleList = new Preset 
                    { 
                        Rules = rules
                    }
                }
            );

            return response;
        }

    }
}
