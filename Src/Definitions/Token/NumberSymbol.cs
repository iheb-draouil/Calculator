namespace Source
{
    class NumberSymbol : Token
    {
        public char _value;

        public NumberSymbol(char value, int position) : base(position, 1) => _value = value;
    }
    
}