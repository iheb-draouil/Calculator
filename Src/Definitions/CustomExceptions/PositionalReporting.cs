namespace MathExtensions
{
    public class ExceptionAtPosition : Exception
    {
        public string _message;

        public int _position;

        public int _length;

        public ExceptionAtPosition(string message, int position, int length) : base(message)
        {
            _message = message;
            _position = position;
            _length = length;
        }
    }

}