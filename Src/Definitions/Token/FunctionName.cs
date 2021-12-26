namespace MathExtensions
{
    class FunctionName : Token
    {
        public string _value;

        public FunctionName(string value, int position, int length) : base(position, length) => _value = value;
    }

}