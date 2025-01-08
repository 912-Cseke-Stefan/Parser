namespace Parser
{
    internal partial class RecDescParser
    { 
        public class RecDescParserTests
        {
            private RecDescParser CreateParser(List<string> input)
            {
                var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
                while (directory != null && !directory.GetFiles("*.csproj").Any())
                    directory = directory.Parent;
                Environment.CurrentDirectory = (directory.ToString());

                string mockGrammarFile = "grammars/g1.txt";
                return new RecDescParser(mockGrammarFile, input);
            }


            public void ExpandTests()
            {
                var input = new List<string>();
                var parser = CreateParser(input);
                parser.SetState(0, new Stack<string>(), new Stack<string>(new[] { "S" }), 'q');

                parser.Expand();

                if (!parser.alpha.Contains("S~0"))
                {
                    Console.WriteLine("Test failed: First production is not in alpha.");
                    return;
                }

                var elem1 = parser.beta.Pop();
                var elem2 = parser.beta.Pop();
                var elem3 = parser.beta.Pop();
                var elem4 = parser.beta.Pop();

                if ( elem1 != "a" || elem2 != "S" || elem3 != "b" || elem4 != "S") 
                {
                    Console.WriteLine("Test failed: Beta stack does not match expected.");
                    return;
                }

                Console.WriteLine("Test passed: Expand worked as expected.");
            }


            public void AdvanceTests()
            {
                var input = new List<string> { "a", "b" };
                var parser = CreateParser(input);
                parser.SetState(0, new Stack<string>(), new Stack<string>(new[] { "a" }), 'q');

                parser.Advance();

                if (parser.index != 1)
                {
                    Console.WriteLine("Test failed: Input index did not advance.");
                    return;
                }
                if (!parser.alpha.Contains("a"))
                {
                    Console.WriteLine("Test failed: Matched terminal 'a' not in alpha.");
                    return;
                }
                if (parser.beta.Count != 0)
                {
                    Console.WriteLine("Test failed: Beta stack is not empty.");
                    return;
                }

                Console.WriteLine("Test passed: Advance worked as expected.");
            }


            public void MomentaryInsuccessTests()
            {
                var input = new List<string> { "a", "c" };
                var parser = CreateParser(input);
                parser.SetState(0, new Stack<string>(), new Stack<string>(new[] { "b" }), 'q'); // 'b' doesn't match 'a'

                parser.MomentaryInsuccess();

                if (parser.state != 'b')
                {
                    Console.WriteLine("Test failed: State did not change to 'b'.");
                    return;
                }

                parser.SetState(0, new Stack<string>(), new Stack<string>(new[] { "a" }), 'q');

                parser.MomentaryInsuccess();

                if (parser.state != 'b')
                {
                    Console.WriteLine("Test failed: State did not change to 'b'.");
                    return;
                }

                Console.WriteLine("Test passed: Momentary Insuccess worked as expected.");
            }


            public void BackTests()
            {
                var input = new List<string> { "a", "b" };
                var parser = CreateParser(input);
                var alpha = new Stack<string>(new[] { "a" });
                parser.SetState(1, alpha, new Stack<string>(), 'b'); // 'a' was consumed

                parser.Back();

                if (parser.index != 0)
                {
                    Console.WriteLine("Test failed: Index did not backtrack.");
                    return;
                }
                if (!parser.beta.Contains("a"))
                {
                    Console.WriteLine("Test failed: Terminal 'a' was not restored to beta.");
                    return;
                }

                Console.WriteLine("Test passed: Back worked as expected.");
            }


            public void AnotherTryTests()
            {
                var input = new List<string> { "a", "c", "b" };
                var parser = CreateParser(input);
                var alpha = new Stack<string>(new[] { "S~0" }); // Using first production
                var beta = new Stack<string>(new[] { "S", "b", "S", "a" });
                parser.SetState(1, alpha, beta, 'b');

                parser.AnotherTry();


                if (!parser.alpha.Contains("S~1"))
                {
                    Console.WriteLine("Test failed: Next production was not tried.");
                    return;
                }

                var betaVal1 = parser.beta.Pop();
                var betaVal2 = parser.beta.Pop();

                if (betaVal1 != "a" || betaVal2 != "S")
                {
                    Console.WriteLine("Test failed: Beta stack did not update correctly.");
                    return;
                }

                if (parser.state != 'q')
                {
                    Console.WriteLine("Test failed: State did not return to 'q'.");
                    return;
                }

                Console.WriteLine("Test passed: Another Try worked as expected.");
            }


            public void SuccessTests()
            {
                var input = new List<string> { };
                var parser = CreateParser(input);

                parser.Success();

                if (parser.state != 'f')
                {
                    Console.WriteLine("Test failed: Parsing did not succeed, state is not 'f'.");
                    return;
                }

                Console.WriteLine("Test passed: Success worked as expected.");
            }


            public void RunTests()
            {
                ExpandTests();
                AdvanceTests();
                MomentaryInsuccessTests();
                BackTests();
                AnotherTryTests();
                SuccessTests();
            }
        }
    }
}
