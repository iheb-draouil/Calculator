namespace Source
{
    class NumberName : Token
    {
        public string _value;

        public NumberName(string value, int position) : base(position, value.Length) => _value = value;
    }
}