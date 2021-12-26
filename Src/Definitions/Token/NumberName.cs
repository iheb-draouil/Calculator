namespace MathExtensions
{
    class NumberName : Token
    {
        public string _value;

        public NumberName(string value, int position, int length) : base(position, length) => _value = value;
    }
}