namespace MathEval
{
    public class ErrorAtPosition : Exception
    {
        public string _message;

        public int _position;

        public int _length;

        public ErrorAtPosition(string message, int position, int length) : base(message)
        {
            _message = message;
            _position = position;
            _length = length;
        }
        
    }

}