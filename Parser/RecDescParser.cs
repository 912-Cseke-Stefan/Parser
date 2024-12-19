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
        private char state;   // Current state: q, b, or e

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

            Console.WriteLine($"Expand: Non-terminal {head} expanded using production {production}");
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

            Console.WriteLine($"Momentary Insuccess: Input at index {index} does not match head of β.");
        }


        // Back: Undo last move
        private void Back()
        {
            Console.WriteLine($"Back: index={index}, α={alpha}");
            if (alpha.Peek() != "")
                index--;
            beta.Push(alpha.Pop());
        }
        //private static int counter = 0;

        // Another Try: Retry with alternative rule
        private void AnotherTry()
        {
            //counter++;
            //if (counter >= 5)
            //    Console.WriteLine("BaLLs");
            string production_of_nonterminal = alpha.Pop();
            int number_of_production = int.Parse(production_of_nonterminal.Split('~')[1]);
            Nonterminal nonterminal = production_of_nonterminal.Split('~')[0];
            if (number_of_production < grammar.Productions[nonterminal].Count - 1)
            {
                Console.WriteLine("Another Try 1");
                alpha.Push(nonterminal + "~" + (number_of_production + 1));  //?
                List<Symbol> current_production = grammar.Productions[nonterminal][number_of_production];
                foreach (Symbol current in current_production)
                    if (current == beta.Peek())  // little safeguard
                        beta.Pop();
                    else
                        Console.WriteLine("Something went horribly wrong");

                List<Symbol> new_production = grammar.Productions[nonterminal][number_of_production + 1];
                for (int aux = new_production.Count - 1; aux >= 0; aux--)
                    beta.Push(new_production[aux]);

                state = 'q';
            }
            else if (number_of_production == grammar.Productions[nonterminal].Count - 1)
            {
                Console.WriteLine("Another Try 2");
                List<Symbol> current_production = grammar.Productions[nonterminal][number_of_production];
                foreach (Symbol current in current_production)
                    if (current == beta.Peek())  // little safeguard
                        beta.Pop();
                    else
                        Console.WriteLine("Something went horribly wrong");

                beta.Push(nonterminal);
            }
            else if (index == 0 && nonterminal == grammar.StartingSymbol)
                state = 'e';
        }

        // Success: Entire input parsed correctly
        private void Success()
        {
            Console.WriteLine("Success: The input sequence was parsed successfully!");
            Console.WriteLine($"Final State: index = {index}, α = [{alpha}], β = []");
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
                        foreach (string asdf in alpha)
                            Console.WriteLine(asdf);
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
                    /*foreach (string a in grammar.Terminals)
                        Console.WriteLine(a);
                    List<string> asdf = ["a", "b", "=", "int"];
                    Stack<string> zxcv = new();
                    zxcv.Push("qwer");
                    zxcv.Push("int");

                    foreach (string a in asdf)
                        if (a == "int")
                            Console.WriteLine(a);

                    Console.WriteLine(asdf.FirstOrDefault(v => v == zxcv.Peek()));*/
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
