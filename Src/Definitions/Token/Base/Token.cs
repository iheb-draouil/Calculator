namespace MathEval
{
    public abstract class Token
    {
        public int _position;

        public int _length;

        public Token(int position, int length)
        {
            _position = position;
            _length = length;
        }
        
    }

}