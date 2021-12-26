namespace MathExtensions
{
    class OperatorSymbol : Token
    {
        public char _value;

        public OperatorSymbol(char value, int position) : base(position, 1) => _value = value;
    }

}