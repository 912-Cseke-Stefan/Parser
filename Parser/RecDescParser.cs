using Terminal = string;
using Symbol = string;


namespace Parser
{
    internal class RecDescParser
    {
        private readonly Grammar grammar;
        private readonly List<Terminal> input;
        private readonly Stack<string> productions; // To keep track of expansions
        private int index; // Pointer to the current input position
        private Stack<string> alpha; // Partial production rule (α)
        private Stack<Symbol> beta;  // Remaining production (β)
        private char state;   // Current state: q, b, or e

        public RecDescParser(List<string> input, string filename)
        {
            this.input = input;
            grammar = new Grammar(filename);

            productions = new Stack<string>();
            index = 1;
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
                    if (index == input.Length && string.IsNullOrEmpty(beta)) // Success condition
                    {
                        state = 'f';
                        Success();
                    }
                    else if (!string.IsNullOrEmpty(beta))
                    {
                        char head = beta[0];
                        if (char.IsUpper(head)) // Head(β) = Non-terminal
                        {
                            Expand();
                        }
                        else if (index < input.Length && beta[0] == input[index]) // Head(β) matches input
                        {
                            Advance();
                        }
                        else // Mismatch
                        {
                            MomentaryInsuccess();
                        }
                    }
                }
                else if (state == 'b') // Backtrack state
                {
                    if (!string.IsNullOrEmpty(alpha) && alpha[0] == 'a') // Head(α) condition
                    {
                        Back();
                    }
                    else
                    {
                        AnotherTry();
                    }
                }
            }

            if (state == 'e')
            {
                Console.WriteLine("Error: Input could not be parsed!");
            }
        }
    }
}
