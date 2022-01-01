namespace Source
{
    public class NumberConstant
    {
        public string _identifier;

        public double _value;

        public NumberConstant(char symbol, double value)
        {
            if (!Lexer._numberSymbols.Contains(symbol)) {
                throw new Exception($"Invalid constant symbol '{symbol}'");
            }
            
            _identifier = symbol.ToString();
            _value = value;
        }

        public NumberConstant(string name, double value)
        {
            if (!Lexer._name.IsMatch(name)) {
                throw new Exception($"Invalid constant name '{name}'");
            }
            
            _identifier = name;
            _value = value;
        }
        
    }

}