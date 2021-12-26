namespace MathExtensions
{

    public class Calculator
    {
        private Lexer _lexer;
        private Parser _parser;

        public Calculator(Lexer lexer, Parser parser)
        {
            _lexer = lexer;
            _parser = parser;
        }

        public double GetResult(string input)
        {
            if (input.Equals("")) {
                throw new Exception("Empty Expressions are not allowed");
            }
            
            var tokens = _parser.Preprocess(_lexer.Tokenize(input));

            return _parser.BuildSyntaxTree(tokens)
            .Evaluate()
            ;
        }
    }
    
}