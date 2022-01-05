namespace MathEval
{
    class Parenthesis : Node
    {
        public Node? _child;

        public override double Evaluate()
        {
            if (_child is null) {
                throw new Exception("Unreachable code: Evaluating an empty parenthesis node");
            }

            return _child.Evaluate();
        }
        
    }

}