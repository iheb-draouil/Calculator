namespace Source
{
    public class Operator
    {
        public char _symbol;

        public Func<double, double, double> _mapping;
        
        public int _precedence;
        
        public Operator(char symbol, Func<double, double, double> mapping, int precedence)
        {
            if (!Lexer._operatorSymbols.Contains(symbol)) {
                throw new Exception($"Invalid operator symbol '{symbol}'");
            }

            _symbol = symbol;
            _mapping = mapping;
            _precedence = precedence;
        }
        
    }

}