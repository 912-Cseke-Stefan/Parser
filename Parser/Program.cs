using Parser;

namespace Parser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Parser parser = new Parser("D:\\Sample.txt");
            Console.WriteLine(parser);
            Console.ReadLine();
        }
    }
}
