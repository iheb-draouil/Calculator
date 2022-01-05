namespace MathEval
{
    class Number : Token
    {
        public double _value;
        
        public Number(double value, int position, int length) : base(position, length) => _value = value;
    }
}