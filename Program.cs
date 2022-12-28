using MiniDown.AST;
using System.Text;

namespace MiniDown
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Insufficient arguments; please supply a minidown source.");
                return;
            }
            if (!File.Exists(args[0]))
            {
                Console.WriteLine($"Source {args[0]} doesn't exist.");
                return;
            }

            string source = File.ReadAllText(args[0]);

            try
            {
                List<IAstElement> miniDownElems = Parsing.Parser.ParseMinidown(source);
                StringBuilder output = new();
                miniDownElems.ForEach((elem) => elem.EmitHTML(output));
                File.WriteAllText("out.html", output.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}