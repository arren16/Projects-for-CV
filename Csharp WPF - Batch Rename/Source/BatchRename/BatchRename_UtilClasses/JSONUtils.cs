using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace BatchRename_UtilClassess
{
    public class JSONUtils<T>
    {
        public static Tuple<bool, string, T?> jsonFileReader(string filepath)
        {
            
            bool success = true;
            string message = "";

            var jsonString = File.ReadAllText(filepath);
            
            T? readResult = default(T);

            try
            {
                readResult = JsonSerializer.Deserialize<T>(jsonString);

                if (readResult == null) 
                {
                    success = false;
                    message = string.Concat(
                        "Either null or empty ",
                        typeof(T).Name,
                        " data was read from ",
                        filepath
                    );
                }
                else
                {
                    success = true;
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

            return new Tuple<bool, string, T?>(
                success, 
                message, 
                readResult
            )!; 
        }

        public static Tuple<bool, string> jsonFileWriter(string filepath, T data)
        {
            bool success = true;
            string message = "";

            if (data == null)
            {
                success = false;
                message = string.Concat(
                    "Either null or empty ", 
                    typeof(T).ToString(),
                    " data"
                );
            }
            else
            {
                try
                {
                    var jsonStringOptions = new JsonSerializerOptions 
                    { 
                        WriteIndented = true
                    };

                    string jsonString = JsonSerializer.Serialize<T>(
                        data, 
                        jsonStringOptions
                    );

                    StreamWriter stream = new StreamWriter(filepath);
                    
                    stream.Write(jsonString);
                    stream.Close();

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
            }

            return new Tuple<bool, string>(success, message);
        }
    }
}
