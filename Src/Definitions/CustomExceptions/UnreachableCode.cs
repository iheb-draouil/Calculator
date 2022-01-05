namespace MathEval
{
    public class UnreachableCode : Exception
    {
        public string _message;

        public UnreachableCode(string message = "Unreachable Code Reached") : base(message) => _message = message;
    }
    
}