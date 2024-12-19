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
                RecDescParser parser = new(["a", "a", "c", "c", "c"], "grammars/g1.txt");
                parser.Parse();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.ReadLine();
        }
    }
}
