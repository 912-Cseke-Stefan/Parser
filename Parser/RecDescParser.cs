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
            Console.WriteLine($"Expand: β = {beta}");
        }

        // Advance: Match terminal and move input pointer
        private void Advance()
        {
            Console.WriteLine($"Advance: Matched {input[index]}");
        }

        // Momentary Insuccess: Current input does not match expected
        private void MomentaryInsuccess()
        {
            Console.WriteLine($"Momentary Insuccess: index={index}, β={beta}");
        }

        // Back: Undo last move
        private void Back()
        {
            Console.WriteLine($"Back: index={index}, α={alpha}");
            index--;
            beta.Push(alpha.Pop());
        }

        // Another Try: Retry with alternative rule
        private void AnotherTry()
        {
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
                beta.Push(nonterminal);
            }
            else if (index == 0 && nonterminal == grammar.StartingSymbol)
                state = 'e';
        }

        // Success: Entire input parsed correctly
        private void Success()
        {
            Console.WriteLine("Success: Sequence accepted!");
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
                        if (grammar.Nonterminals.FirstOrDefault(head) != default)  // Head(β) = Non-terminal
                            Expand();
                        else if (index < input.Count && head == input[index])      // Head(β) matches input
                            Advance();
                        else                                                       // Mismatch
                            MomentaryInsuccess();
                    }
                }
                else if (state == 'b') // Backtrack state
                {
                    if (alpha.Count > 0 && grammar.Terminals.FirstOrDefault(alpha.Peek()) != default)  // Head(α) is terminal
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
