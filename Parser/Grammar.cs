using System.Text;
using Nonterminal = string;
using Terminal = string;


namespace Parser
{
    public class Grammar
    {
        private readonly List<Terminal> terminals = [];
        private readonly List<Nonterminal> nonterminals = [];
        private readonly Nonterminal startingSymbol = "";
        private readonly Dictionary<Nonterminal, List<List<string>>> productions = [];


        public Grammar(string filename) 
        {
            StreamReader sr = new(filename);

            string? line = sr.ReadLine()?.Trim();
            while (line != null)
            {
                string element = line.Split(":=")[0].Trim();
                if (element == line)
                {
                    // we are processing a production
                    // they can be scattered across the entire file for what I am concerned
                    string nonter = line.Split("->")[0].Trim();
                    CFGCheck(nonter);

                    List<string> productions = [.. line.Split("->")[1].Split("| ")];
                    for (int i = 0; i < productions.Count; i-=-1)
                        productions[i] = productions[i].Trim();

                    foreach (string production in productions)
                    {
                        List<string> elems = [.. production.Split(" ")];
                        for (int i = 0; i < elems.Count; i -= -1)
                            elems[i] = elems[i].Trim();
                        this.productions[nonter].Add(elems);
                    }
                }
                else
                    switch (element) 
                    {
                        case "N":
                            string nonterminals = line.Split('=')[1].Trim();
                            foreach (string nonterminal in nonterminals.Split(" "))
                            {
                                this.nonterminals.Add(nonterminal.Trim());
                                this.productions.Add(nonterminal.Trim(), []);
                            }
                            break;
                        case "E":
                            string terminals = line.Split('=')[1].Trim();
                            foreach (string termimal in terminals.Split(" "))
                                this.terminals.Add(termimal.Trim());
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

        private void CFGCheck(string nonter)
        {
            if (nonter.Split(' ').ToList().Count > 1)
                throw new Exception("Not Context-free grammar");
        }

        public override string ToString()
        {
            StringBuilder result = new();
            result.Append("N = ");
            foreach (string nonterminal in nonterminals)
                result.Append($"{nonterminal} ");

            result.Append("\nE = ");
            foreach(string terminal in terminals)
                result.Append($"{terminal} ");

            result.Append($"\nS = {startingSymbol}");

            result.Append("\nP = \n");
            foreach (string nonterminal in nonterminals)
                result.Append(ProductionsOf(nonterminal));

            result.Append('\n');

            return result.ToString();
        }


        public string ProductionsOf(Nonterminal nonterminal)
        {
            StringBuilder result = new();

            if (productions.TryGetValue(nonterminal, out List<List<string>>? value))
            {
                result.Append($"{nonterminal} -> ");
                foreach (List<string> production in value)
                {
                    StringBuilder productionStr = new();
                    foreach (string elem in production)
                       productionStr.Append($"{elem} ");

                    result.Append($"{productionStr}| ");
                }
                result.Append("\b\b\b   ");
                result.Append('\n');
                return result.ToString();
            }
            return "";
        }
    }
}
