using System;
using Snip.CLI;
using snip.CLI;
using Snip.Lexer;

namespace Snip
{
    public static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Compilation");
            string? source = null;
            var cli = new CLI.CLI();
            try
            {
                source = cli.Parse(args);
                if (source == null)
                {
                    return;
                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            catch (InvalidArgumentException e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            var lexer = new Lexer.Lexer(source);
            lexer.Tokenize();
            Console.WriteLine(lexer.ToString());
            Console.Write("Finish Compilation");
        }
    }
}
