namespace Source
{
    class Operation : Node
    {
        public Operator _operator;

        public Node? _loperand;

        public Node? _roperand;

        public Operation(Operator op) => _operator = op;
        
        public override double Evaluate()
        {
            if (_loperand is null || _roperand is null) {
                throw new Exception("Unreachable code: Operation node evaluated while missing the left and/or right hand operands");
            }

            return _operator._mapping(_loperand.Evaluate(), _roperand.Evaluate());
        }
        
    }
    
}