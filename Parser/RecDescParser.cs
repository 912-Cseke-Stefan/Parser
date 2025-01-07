using Terminal = string;
using Nonterminal = string;
using Symbol = string;


namespace Parser
{
    internal class RecDescParser
    {
        private readonly Grammar grammar;
        private readonly List<Terminal> input;
        public int index { get; set; } // Pointer to the current input position
        public Stack<string> alpha { get; set; } // Partial production rule (α)
        public Stack<Symbol> beta { get; set; }  // Remaining production (β)
        public char state { get; set; }   // Current state: q, b, e of f

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
        public void Expand()
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
        public void Advance()
        {
            string terminal = beta.Pop();
            if (terminal != "")
                index++;

            alpha.Push(terminal);

            Console.WriteLine($"Advance: Matched terminal '{terminal}', index now at {index}");
        }


        // Momentary Insuccess: Current input does not match expected
        public void MomentaryInsuccess()
        {
            state = 'b';

            if (index < input.Count && beta.Count > 0)
                Console.WriteLine($"Momentary Insuccess: Input at index {index} ({input[index]}) does not match head of β ({beta.Peek()}).");
            else
                Console.WriteLine("Momentary Insuccess");
        }


        // Back: Undo last move
        public void Back()
        {
            Console.WriteLine($"Back: index is {index}, head of α is {alpha.Peek()}");

            if (alpha.Peek() != "")
                index--;
            beta.Push(alpha.Pop());
        }
        

        // Another Try: Retry with alternative rule
        public void AnotherTry()
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
        public void Success()
        {
            state = 'f';
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
