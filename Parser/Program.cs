namespace Parser
{
    internal class Program
    {
        static void Main()
        {
            try
            {
                //var tests = new RecDescParser.RecDescParserTests();
                //tests.RunTests();
            
                var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
                while (directory != null && !directory.GetFiles("*.csproj").Any())
                    directory = directory.Parent;
                Environment.CurrentDirectory = (directory.ToString());

                //RecDescParser parser = new("grammars/g1.txt", sequence: ["a", "a", "c", "b", "c"]);
                //RecDescParser parser = new(["a", "a", "c", "c", "c"], "grammars/g1.txt");
                //RecDescParser parser = new(["a", "c", "b", "c"], "grammars/g1.txt");
                //RecDescParser parser = new(["a", "c"], "grammars/g1.txt");
                RecDescParser parser = new("grammars/g2_numeric.txt", pifPath: "pifs/pif.out");
                //RecDescParser parser = new("grammars/g2.txt", sequence: ["start", "{", "int", "x", "(", ")", ";", "}", "stop"]);
                //RecDescParser parser = new("grammars/g2.txt", sequence: ["start", "{", "int", "x", ";", "if", "x", "==", "0", "{", "x", "=", "1", ";", "}", "}", "stop"]);
                //RecDescParser parser = new("grammars/g2.txt", sequence: ["start", "{", "if", "x", "==", "0", "{", "x", "=", "1", ";", "}", "}", "stop"]);
                //RecDescParser parser = new("grammars/g2.txt", sequence: ["start", "{", "if", "true", "{", "x", "=", "1", ";", "}", ";", "}", "stop"]);

                //Grammar parser = new("grammars/g1.txt");
                //Console.WriteLine(parser);
                parser.Parse();

                StreamWriter sw = new("out.out");
                sw.Write(parser.GenerateSyntaxTreeTable());
                sw.Close();
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.ReadLine();
        }
    }
}
