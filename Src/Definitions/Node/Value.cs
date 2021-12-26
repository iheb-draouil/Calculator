namespace MathExtensions
{
    class Value : Node
    {
        double _value;
        
        public Value(double value) => _value = value;
        
        public override double Evaluate()
        {
            return _value;
        }
    }
    
}