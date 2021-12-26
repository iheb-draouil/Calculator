using System.Text.RegularExpressions;

namespace MathExtensions
{
    public class Lexer
    {
        public static readonly char[] _operatorSymbols = { '+', '-', '*', '/', '^', '%', ':', '&', '#', '@', '>', '<' };

        public static readonly char[] _numberSymbols = { 'α', 'χ', 'δ', 'β', 'ε', 'φ', 'γ', 'η', 'ι', 'κ', 'λ', 'μ', 'ν', 'ο', 'π', 'θ', 'ρ', 'σ', 'τ', 'υ', 'ω', 'ξ', 'ψ', 'ζ' };

        public static readonly char[] _symbols = _operatorSymbols.Concat(_numberSymbols).ToArray();

        private static readonly Regex _startsNumber = new Regex(@"^([0-9]+|[0-9]+\.[0-9]*)$", RegexOptions.Compiled);

        private static readonly Regex _whiteSpaces = new Regex(@"^[ ]+$", RegexOptions.Compiled);

        public static readonly Regex _number = new Regex(@"^([0-9]+|[0-9]+\.[0-9]+)$", RegexOptions.Compiled);
        
        public static readonly Regex _name = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$", RegexOptions.Compiled);

        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }
        
        private bool IsSpace(char c)
        {
            return c == ' ';
        }

        private bool IsAlphabet(char c)
        {
            return c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z';
        }

        private bool IsComma(char c)
        {
            return c == ',';
        }

        private bool IsSymbol(char c)
        {
            return _symbols.Contains(c);
        }

        private bool IsNumberSymbol(char c)
        {
            return _numberSymbols.Contains(c);
        }

        private bool IsOperatorSymbol(char c)
        {
            return _operatorSymbols.Contains(c);
        }

        private bool IsDecimalPoint(char c)
        {
            return c == '.';
        }

        private bool IsUnderscore(char c)
        {
            return c == '_';
        }

        private bool IsParenthesis(char c)
        {
            return c == '(' || c == ')';
        }

        private bool IsLegalCharacter(char c)
        {
            return IsParenthesis(c)
            || IsDecimalPoint(c)
            || IsUnderscore(c)
            || IsAlphabet(c)
            || IsSymbol(c)
            || IsComma(c)
            || IsDigit(c)
            || IsSpace(c)
            ;
        }

        private Token ToToken(string sequence, int position)
        {
            if (sequence.Length == 1 && sequence[0] == '(') {
                return new OpeningParenthesis(position);
            }

            else if (sequence.Length == 1 && sequence[0] == ')') {
                return new ClosingParenthesis(position);
            }

            else if (sequence.Length == 1 && IsNumberSymbol(sequence[0])) {
                return new NumberSymbol(sequence[0], position);
            }

            else if (sequence.Length == 1 && IsOperatorSymbol(sequence[0])) {
                return new OperatorSymbol(sequence[0], position);
            }

            else if (sequence.Length == 1 && IsComma(sequence[0])) {
                return new Comma(position);
            }

            else if (_name.IsMatch(sequence)) {
                return new Name(sequence, position);
            }

            else if (_number.IsMatch(sequence)) {
                return new Number(Double.Parse(sequence), position, sequence.Length);
            }
            
            throw new UnreachableCode();
        }

        private string GetNextSequence(string input, int from)
        {
            for (int i = from; i < input.Length; i++)
            {
                if (!IsLegalCharacter(input[i])) {
                    throw new ExceptionAtPosition("Illegal character", i, 1);
                }

                var sequence = input.Substring(from, (i-from)+1);

                if (sequence.Length == 1
                    && (IsComma(sequence[0]) || IsSymbol(sequence[0]) || IsParenthesis(sequence[0]))) {
                    return sequence;
                }

                else if (_name.IsMatch(sequence) || _whiteSpaces.IsMatch(sequence)) {
                    continue;
                }
                
                else if (_startsNumber.IsMatch(sequence)) {
                    
                    if (i == input.Length-1 && !_number.IsMatch(sequence)) {
                        return sequence.Substring(0, sequence.Length-1);
                    }
                    
                    continue;
                }
                
                else {
                    
                    var subsequence = sequence.Substring(0, sequence.Length-1);

                    if (_number.IsMatch(subsequence) || _name.IsMatch(subsequence) || _whiteSpaces.IsMatch(subsequence)) {
                        return subsequence;
                    }

                    throw new ExceptionAtPosition("Illegal Sequence", from, sequence.Length);
                }

            }
            
            return input.Substring(from, input.Length-from);
        }

        public List<Token> Tokenize(string input)
        {
            var tokens = new List<Token>();

            var cursor = 0;

            while (cursor < input.Length)
            {
                var sequence = GetNextSequence(input, cursor);
                
                if (!sequence.Contains(" ")) {
                    tokens.Add(ToToken(sequence, cursor));
                }

                cursor += sequence.Length;
            }

            return tokens;
        }
    }
    
}