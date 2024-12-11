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

                Parser parser = new("grammars/g2.txt");
                Console.WriteLine(parser);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.ReadLine();
        }
    }
}
