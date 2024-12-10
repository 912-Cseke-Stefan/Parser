using Nonterminal = string;
using Terminal = string;

namespace Parser
{
    public class Parser
    {
        private List<Terminal> terminals = [];
        private List<Nonterminal> nonterminals = [];
        private Nonterminal startingSymbol = "";
        private Dictionary<Nonterminal, List<List<string>>> productions = [];

        public Parser(string filename) 
        {
            StreamReader sr = new StreamReader(filename);

            string? line = sr.ReadLine()?.Trim();
            while (line != null)
            {
                string element = line.Split('=')[0].Trim();
                if (element == line)
                {
                    // we are processing a production
                    // they can be scattered across the entire file for what I am concerned
                    string nonter = line.Split("->")[0];
                    string refElem = nonterminals.FirstOrDefault(nonter);
                }
                else
                    switch (element) 
                    {
                        case "N":
                            string nonterminals = line.Split('=')[1].Trim();
                            foreach (string nonterminal in nonterminals.Split(" "))
                                this.nonterminals.Add(nonterminal);
                            break;
                        case "E":
                            string terminals = line.Split('=')[1].Trim();
                            foreach (string termimal in terminals.Split(" "))
                                this.terminals.Add(termimal);
                            break;
                        case "S":
                            string start = line.Split('=')[1].Trim();
                            startingSymbol = start;
                            break;
                        case "P":
                            break;
                    }

                line = sr.ReadLine();
            }
            
            sr.Close();
        }

        public override string ToString()
        {
            string result = "";
            result += "N = ";
            foreach (string nonterminal in nonterminals)
                result += nonterminal + " ";
            result += "\nE = ";
            foreach(string terminal in terminals)
                result += terminal + " ";
            result += "\nS = ";
            result += startingSymbol;
            result += "\nP = ";

            return result;
        }
    }
}
