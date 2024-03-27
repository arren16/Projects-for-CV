using Rule_GPT;
using System.Xml.Linq;

//var prompt = "Give me a fullname of a famous people (the name should be in between the two squares like this: [name])";
//prompt = "Give me some famous song name in separated lines";

//string name = "old_file_namej  _009942gijwgd))()())-didi";

//var prompt = string.Concat(
//    $"My file name is '{name}' ",
//    "(the file extension is hidden). ",
//    "Let's detect errors in this (like ",
//    "grammar errors, redundant letters, inapproriate characters for file name error, etc, ",
//    "and any other errors that you can detect) ",
//    "then give me a new file name with these errors fixed ",
//    "(the new name should be between two squares,",
//    "for example: [file_name]) ",
//    "(of course, with no file extension)"
//);

//Console.WriteLine(prompt.Length);

////Console.WriteLine(prompt);

//var result = await GPTUtils.Prompt(prompt);
//Console.WriteLine(result.Item2);
//Console.WriteLine(result.Item3.choices[0].message.content);


//Console.WriteLine();
//Console.WriteLine();
//Console.WriteLine();
//Console.WriteLine();
//Console.WriteLine();

var r = new GPTRule();


var (sucess, message, newName) = r.Rename("This is the olde name \\ / ??--309t-284614-601490-)))()()(LKLFJLD';l'.'''.pdf", false);

Console.WriteLine("A");
Console.WriteLine(sucess);
Console.WriteLine("B");
Console.WriteLine(message);
Console.WriteLine("C");
Console.WriteLine(newName);