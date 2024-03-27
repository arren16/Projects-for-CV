using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchRename_UtilClasses
{
    public class PathUtils
    {
        public static Tuple<string, string> 
            FullFilePathToLocationAndFileName(string filepath)
        {
            string location = "";

            string filename = "";

            char delim = '/';
            
            var sampleLocation = AppDomain.CurrentDomain.BaseDirectory;

            if (sampleLocation.Contains("/"))
            {
                delim = '/';
            }
            else if (sampleLocation.Contains("\\")) 
            {
                delim = '\\';
            }
            else
            {
                delim = sampleLocation.Last();
            }

            string[] parser = filepath.Split(delim);
            location = string.Join(delim, parser.SkipLast(1));
            filename = parser.Last();

            var result = new Tuple<string, string>(
                location,
                filename
            );

            return result;
        }
        public static string 
            FileLocationAndFileNameToFilePathConverter(string location, string filename)
        {

            string folder = AppDomain.CurrentDomain.BaseDirectory;
            char delim = folder.Last();
            char unwantedDelim = (delim == '/') ? '\\' : '/';

            string filepath = string.Concat(location, delim, filename);
            filepath.Replace(unwantedDelim, delim);

            return filepath;
        }
        public static Tuple<string, string> FileNameToNameAndExtension(string filename) 
        {
            string name = "";
            string extension = "";

            string? splitter = string.Join("", filename.Split('.').SkipLast(1));
            name = string.IsNullOrEmpty(splitter) 
                ? "" : splitter;

            splitter = filename.Split('.').Last();
            extension = string.IsNullOrEmpty(splitter)
                ? "" : splitter;

            return new Tuple<string, string>(name, extension);
        }
    }
}
