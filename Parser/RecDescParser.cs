using Terminal = string;
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
        }

        // Another Try: Retry with alternative rule
        private void AnotherTry()
        {
            Console.WriteLine("Another Try: Trying alternative rule");
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
    }
}
