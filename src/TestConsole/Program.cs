using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ChatopsBot.Commands;
using CommandLine;
using CommandLine.Text;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var command = CommandParser.ParseCommand(string.Join(" ", args));
            Console.WriteLine(string.Join("", command.Output));

            //var input = "help build";
            //var command = CommandParser.ParseCommand(input);

            //Console.WriteLine(command.Output);

            //var input1 = "set-alias -n \"alias Name\" \"queue-build 42\"";
            //var command1 = CommandParser.ParseCommand(input1);

            //var input2 = "set-alias -n \"alias Name\" -c \"queue-build 42\"";
            //var command2 = CommandParser.ParseCommand(input2);

            //Console.ReadLine();
        }


    }




}
