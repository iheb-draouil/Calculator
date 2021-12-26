namespace Source
{
    class FunctionName : Token
    {
        public string _value;

        public FunctionName(string value, int position) : base(position, value.Length) => _value = value;
    }

}