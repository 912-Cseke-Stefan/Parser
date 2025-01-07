namespace Parser
{
    internal class Program
    {
        static void Main()
        {
            try
            {
                var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
                while (directory != null && !directory.GetFiles("*.csproj").Any())
                    directory = directory.Parent;
                Environment.CurrentDirectory = (directory.ToString());

                //RecDescParser parser = new(["a", "a", "c", "b", "c"], "grammars/g1.txt");
                //RecDescParser parser = new(["a", "a", "c", "c", "c"], "grammars/g1.txt");
                //RecDescParser parser = new(["a", "c", "b", "c"], "grammars/g1.txt");
                //RecDescParser parser = new(["a", "c"], "grammars/g1.txt");
                RecDescParser parser = new(["start", "{", "int", "x", ";", "}", "stop"], "grammars/g2.txt");
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
