using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rule_GPT
{
    public class JSONUtils<T> where T : class
    {
        public static Tuple<bool, string, T> Deserialize(string jsonString)
        {
            bool success = true;
            string message = "";
            T? result = default(T);

            try
            {
                success = true;
                message = "";
                result = JsonSerializer.Deserialize<T>(jsonString)!;
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
                result
            )!;
        }
    }
}
