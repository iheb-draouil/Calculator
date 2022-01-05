namespace MathEval
{
    class FunctionCall : Node
    {
        Function _function;

        public Stack<Node> _arguments = new Stack<Node>();
        
        // opportunity to use source gen to add different signature functions based on argument count
        public FunctionCall(Function function) => _function = function;

        public override double Evaluate()
        {
            if (!_arguments.Any()) {
                throw new Exception("Unreachable code: Evaluating a function call node with zero arguments");
            }

            return _function._mapping(_arguments.Reverse().Select(e => e.Evaluate()).ToList());
        }
        
    }
    
}