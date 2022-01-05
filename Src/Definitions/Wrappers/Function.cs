namespace MathEval
{
    public class Function
    {
        public string _name;

        public Func<List<double>, double> _mapping;

        public Function(string name, Func<List<double>, double> mapping)
        {
            if (!Lexer._name.IsMatch(name)) {
                throw new Exception($"Invalid function name '{name}'");
            }

            _name = name;
            _mapping = mapping;
        }
        
    }

}