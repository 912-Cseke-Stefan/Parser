using Terminal = string;
using Nonterminal = string;
using Symbol = string;


namespace Parser
{
    internal class RecDescParser
    {
        private readonly Grammar grammar;
        private readonly List<Terminal> input;
        private int index; // Pointer to the current input position
        private Stack<string> alpha; // Partial production rule (α)
        private Stack<Symbol> beta;  // Remaining production (β)
        private char state;   // Current state: q, b, e of f

        public RecDescParser(List<string> input, string filename)
        {
            this.input = input;
            grammar = new Grammar(filename);

            index = 0;
            state = 'q';
            alpha = new Stack<string>();
            beta = new Stack<Terminal>();
            beta.Push(grammar.StartingSymbol);
        }

        // Expand: Expand non-terminal at the head of β
        private void Expand()
        {
            string head = beta.Pop();
            List<Symbol> production = grammar.Productions[head][0];
            int number_of_production = 0;

            alpha.Push(head + "~" + number_of_production);


            for (int i = production.Count - 1; i >= 0; i--)
            {
                beta.Push(production[i]);
            }

            Console.WriteLine($"Expand: Non-terminal {head} expanded using production {number_of_production}");
        }


        // Advance: Match terminal and move input pointer
        private void Advance()
        {
            string terminal = beta.Pop();
            if (terminal != "")
                index++;

            alpha.Push(terminal);

            Console.WriteLine($"Advance: Matched terminal '{terminal}', index now at {index}");
        }


        // Momentary Insuccess: Current input does not match expected
        private void MomentaryInsuccess()
        {
            state = 'b';

            if (index < input.Count && beta.Count > 0)
                Console.WriteLine($"Momentary Insuccess: Input at index {index} ({input[index]}) does not match head of β ({beta.Peek()}).");
            else
                Console.WriteLine("Momentary Insuccess");
        }


        // Back: Undo last move
        private void Back()
        {
            Console.WriteLine($"Back: index is {index}, head of α is {alpha.Peek()}");

            if (alpha.Peek() != "")
                index--;
            beta.Push(alpha.Pop());
        }
        

        // Another Try: Retry with alternative rule
        private void AnotherTry()
        {
            string production_of_nonterminal = alpha.Pop();
            int number_of_production = int.Parse(production_of_nonterminal.Split('~')[1]);
            Nonterminal nonterminal = production_of_nonterminal.Split('~')[0];
            List<Symbol> current_production = grammar.Productions[nonterminal][number_of_production];

            foreach (Symbol current in current_production)
                if (current == beta.Peek())  // little safeguard
                    beta.Pop();
                else
                    Console.WriteLine("Something went horribly wrong");

            if (number_of_production < grammar.Productions[nonterminal].Count - 1)
            {
                Console.WriteLine($"Another Try 1: Trying another production of nonterminal {nonterminal}");

                alpha.Push(nonterminal + "~" + (number_of_production + 1));  //?

                List<Symbol> new_production = grammar.Productions[nonterminal][number_of_production + 1];
                for (int aux = new_production.Count - 1; aux >= 0; aux--)
                    beta.Push(new_production[aux]);

                state = 'q';
            }
            else if (number_of_production == grammar.Productions[nonterminal].Count - 1)
            {
                Console.WriteLine($"Another Try 2: Finished all productions of nonterminal {nonterminal}");

                beta.Push(nonterminal);

                if (index == 0 && beta.Peek() == grammar.StartingSymbol)
                    state = 'e';
            }
        }

        // Success: Entire input parsed correctly
        private void Success()
        {
            Console.WriteLine("Success: The input sequence was parsed successfully!");
            Console.WriteLine($"Final State: index = {index}, α = [{alpha}], β = []");
        }


        public string GenerateSyntaxTreeTable()
        {
            List<string> table = [];

            Stack<string> reversed_alpha = new(alpha);
            Stack<int> parents = [];
            parents.Push(0);
            Stack<int> no_of_descendants = [];
            no_of_descendants.Push(1);
            Stack<int> left_siblings = [];
            left_siblings.Push(0);
            int index = 1;

            foreach (string element_of_alpha in reversed_alpha)
            {
                if (grammar.Nonterminals.FirstOrDefault(v => v == element_of_alpha.Split('~')[0]) != default)
                {
                    int number_of_production = int.Parse(element_of_alpha.Split('~')[1]);
                    Nonterminal nonterminal = element_of_alpha.Split('~')[0];

                    table.Add($"{index} | {nonterminal} | {parents.Peek()} | {left_siblings.Peek()}");

                    no_of_descendants.Push(no_of_descendants.Pop() - 1);
                    if (no_of_descendants.Peek() == 0)
                    {
                        no_of_descendants.Pop();
                        parents.Pop();
                        left_siblings.Pop();
                    }
                    else
                    {
                        left_siblings.Pop();
                        left_siblings.Push(index);
                    }

                    parents.Push(index++);
                    no_of_descendants.Push(grammar.Productions[nonterminal][number_of_production].Count);
                    left_siblings.Push(0);
                }
                else
                {
                    if (element_of_alpha == "")
                        table.Add($"{index} | epsilon | {parents.Peek()} | {left_siblings.Peek()}");
                    else
                        table.Add($"{index} | {element_of_alpha} | {parents.Peek()} | {left_siblings.Peek()}");

                    no_of_descendants.Push(no_of_descendants.Pop() - 1);
                    if (no_of_descendants.Peek() == 0)
                    {
                        no_of_descendants.Pop();
                        parents.Pop();
                        left_siblings.Pop();
                        index++;
                    }
                    else
                    {
                        left_siblings.Pop();
                        left_siblings.Push(index++);
                    }
                }
            }

            string ret = "";
            foreach (string row in table)
                ret += row + '\n';
            return ret;
        }

        // Main parsing method
        public void Parse()
        {
            
            while (state != 'e' && state != 'f') // e = error, f = success
            {
                if (state == 'q')
                {
                    if (index == input.Count && beta.Count == 0) // Success condition
                    {
                        state = 'f';
                        Success();
                    }
                    else if (beta.Count > 0)
                    {
                        Symbol head = beta.Peek();
                        if (grammar.Nonterminals.FirstOrDefault(v => v == head) != default)    // Head(β) = Non-terminal
                            Expand();
                        else if (index < input.Count && (head == input[index] || head == ""))  // Head(β) matches input
                            Advance();
                        else                                                                   // Mismatch
                            MomentaryInsuccess();
                    }
                    else if (beta.Count == 0)  // would happen when word is not part of the language
                        MomentaryInsuccess();
                }
                else if (state == 'b') // Backtrack state
                {
                    if (alpha.Count > 0 && grammar.Terminals.FirstOrDefault(v => v == alpha.Peek()) != default)  // Head(α) is terminal
                        Back();
                    else
                        AnotherTry();
                }
            }

            if (state == 'e')
            {
                Console.WriteLine("Error: Input could not be parsed!");
            }
        }

        public void SetState(int index, Stack<string> alpha, Stack<Symbol> beta, char state)
        {
            this.index = index;
            this.alpha = alpha;
            this.beta = beta;
            this.state = state;
        }
    }
}
