using BatchRename_MainClasses;
using System.Reflection;

namespace BatchRename_UtilClassess
{
    public class DLLUtils
    {
        public static Tuple<bool, string, List<InterfaceType>> 
            getAllImplementableClassInstanceFromDLLs<InterfaceType>(
            string dllFolder
        ) {
            bool success = true;
            string message = "";

            List<InterfaceType> instances = new List<InterfaceType>() { };

            try
            {
                var folderInfo = new DirectoryInfo(dllFolder);
                var dllFiles = folderInfo.GetFiles("*.dll");

                foreach (var file in dllFiles)
                {
                    var assembly = Assembly.LoadFrom(file.FullName);
                    var types = assembly.GetTypes();

                    foreach (var type in types)
                    {
                        if (type.IsClass && typeof(InterfaceType).IsAssignableFrom(type))
                        {
                            InterfaceType rule = (InterfaceType)Activator
                                .CreateInstance(type)!;
                            
                            instances.Add(rule);
                        }
                    }
                }

                success = true;
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

            Tuple<bool, string, List<InterfaceType>> result 
                = new Tuple<bool, string, List<InterfaceType>>(
                    success, 
                    message, 
                    instances
                );

            return result;
        }
    }
}