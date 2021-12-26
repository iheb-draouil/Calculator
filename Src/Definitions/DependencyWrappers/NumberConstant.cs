namespace Source
{
    public class NumberConstant
    {
        public string _identifier;

        public double _value;

        public NumberConstant(char symbol, double value)
        {
            var identifier = symbol.ToString();
            
            if (!Lexer._name.IsMatch(identifier)) {
                throw new Exception($"Invalid constant name '{identifier}'");
            }
            
            _identifier = identifier;
            _value = value;
        }

        public NumberConstant(string name, double value)
        {
            if (!Lexer._numberSymbols.Contains(name[0])) {
                throw new Exception($"Invalid constant symbol '{name}'");
            }
            
            _identifier = name;
            _value = value;
        }
        
    }

}