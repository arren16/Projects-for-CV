using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchRename_UtilClasses
{
    public class NameUtils
    {
        public static string InvalidNameErrorMessage => string.Concat(
                "A valid name shouldn't contain any of those characters: ",
                string.Join(", ", _invalidCharacters)
            );
        private static List<string> _invalidCharacters = new List<string>
        {
            "\\", "/", ":", "*", "?", "\"","<", ">", "|"
        };
        public static List<string> InvalidCharacter => _invalidCharacters;
        public static bool IsValidName(string name)
        {
            bool result = true;
        
            foreach (var character in _invalidCharacters)
            {
                if (name.Contains(character))
                {
                    result = false;
                    break;
                }
                else
                {
                    // Do nothing
                }
            }

            return result;
        }
    }
}
