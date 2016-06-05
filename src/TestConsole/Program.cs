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
            var command1 = CommandParser.ParseCommand(args);

            var command2 = CommandParser.ParseCommand(string.Join(" ",args));


            Console.WriteLine(string.Join(Environment.NewLine, command1.Output));
            Console.WriteLine("------------------------");
            Console.WriteLine(string.Join(Environment.NewLine, command2.Output));


            //Console.ReadLine();
        }


    }




}
