namespace Parser
{
    internal class Program
    {
        static void Main()
        {
            Parser parser = new("D:\\Sample.txt");
            Console.WriteLine(parser);
            Console.ReadLine();
        }
    }
}
