namespace Source
{
    public class Parser
    {
        private Dictionary<char, Operator> _operators;

        private Dictionary<string, Function> _functions;

        private Dictionary<string, NumberConstant> _constants;

        public Parser(
            IEnumerable<Function> functions,
            IEnumerable<Operator> operators,
            IEnumerable<NumberConstant> constants
        ) {
            _functions = functions.ToDictionary(fn => fn._name);
            _operators = operators.ToDictionary(op => op._symbol);

            _constants = constants.ToDictionary(c => {

                if (c._identifier.Length != 1) {

                    if (_functions.TryGetValue(c._identifier, out _)) {
                        throw new Exception($"Constant '{c._identifier}' has the same name as a function");
                    }

                }

                else {

                    if (_operators.TryGetValue(c._identifier[0], out _)) {
                        throw new Exception($"Constant '{c._identifier}' has the same symbol as an operator");
                    }

                }

                return c._identifier;
            });
        }
        
        public List<Token> Preprocess(List<Token> tokens)
        {
            var result = new List<Token>();

            if (tokens[0] is not Name
                && tokens[0] is not Number
                && tokens[0] is not NumberSymbol
                && tokens[0] is not OpeningParenthesis) {
                throw new ErrorAtPosition("Unexpected Token", tokens[0]._position, tokens[0]._length);
            }

            if (tokens.Count == 1) {

                if (tokens[0] is Name name && tokens.Count > 1 && tokens[1] is not OpeningParenthesis) {
                    result.Add(new NumberName(name._value, name._position));
                }

                else {
                    result.Add(tokens[0]);
                }

                return result;
            }

            var functionCalls = new Stack<int>();
            
            var openingParenthesisMatch = new Stack<int>();
            var closingParenthesisMatch = new Stack<int>();

            for (int i = 0; i < tokens.Count-1; i++)
            {
                var token1 = tokens[i];
                var token2 = tokens[i+1];
                
                if ((token1 is Number && token2 is not OperatorSymbol && token2 is not Comma && token2 is not ClosingParenthesis)
                    || (token1 is NumberSymbol && token2 is not OperatorSymbol && token2 is not Comma && token2 is not ClosingParenthesis)
                    || (token1 is ClosingParenthesis && token2 is not OperatorSymbol && token2 is not Comma && token2 is not ClosingParenthesis)
                    || (token1 is Comma && token2 is not Number && token2 is not Name && token2 is not NumberSymbol && token2 is not OpeningParenthesis)
                    || (token1 is OperatorSymbol && token2 is not Number && token2 is not Name && token2 is not NumberSymbol && token2 is not OpeningParenthesis)
                    || (token1 is OpeningParenthesis && token2 is not Number && token2 is not Name && token2 is not NumberSymbol && token2 is not OpeningParenthesis)
                    || (token1 is Name && token2 is not OpeningParenthesis && token2 is not OperatorSymbol && token2 is not Comma && token2 is not ClosingParenthesis)) {
                    throw new ErrorAtPosition("Unexpected Token", token2._position, token2._length);
                }

                if (token1 is Name name) {
                
                    if (token2 is OpeningParenthesis) {
                        result.Add(new FunctionName(name._value, name._position));
                        functionCalls.Push(token2._position);
                    }
                    
                    else {
                        result.Add(new NumberName(name._value, name._position));
                    }

                }

                else if (token1 is Comma) {

                    if (!functionCalls.Any() || functionCalls.Peek() != openingParenthesisMatch.Peek()) {
                        throw new ErrorAtPosition("Unexpected Token", token1._position, token1._length);
                    }

                    result.Add(token1);
                }

                else {
                    result.Add(token1);
                }

                if (token1 is OpeningParenthesis) {
                    openingParenthesisMatch.Push(token1._position);
                }
                
                if (token2 is ClosingParenthesis) {
                    
                    if (openingParenthesisMatch.Any()) {

                        if (functionCalls.Any() && functionCalls.Peek() == openingParenthesisMatch.Peek()) {
                            functionCalls.Pop();
                        }

                        openingParenthesisMatch.Pop();
                    }

                    else {
                        closingParenthesisMatch.Push(token2._position);
                    }

                }

            }

            if (tokens.Last() is not Name
                && tokens.Last() is not Number
                && tokens.Last() is not NumberSymbol
                && tokens.Last() is not ClosingParenthesis) {
                throw new ErrorAtPosition("Unexpected Token", tokens.Last()._position, tokens.Last()._length);
            }

            result.Add(tokens.Last());

            if (openingParenthesisMatch.Any() || closingParenthesisMatch.Any() ) {
                throw new UnmatchingParenthesis();
            }

            return result;
        }

        public Node BuildSyntaxTree(List<Token> tokens)
        {
            var views = new Stack<Node>();

            for (var i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];
                
                if (token is Number || token is NumberName || token is NumberSymbol) {

                    Value value;
                    
                    if (token is Number number) {
                        value = new Value(number._value);
                    }

                    else if (token is NumberName numberName) {
                        value = new Value(_constants[numberName._value]._value);
                    }

                    else if (token is NumberSymbol numberSymbol) {
                        value = new Value(_constants[numberSymbol._value.ToString()]._value);
                    }

                    else {
                        throw new UnreachableCode();
                    }
                    
                    if (!views.Any()) {
                        views.Push(value);
                    }

                    else if (views.Peek() is Parenthesis parenthesis) {
                        parenthesis._child = value;
                        views.Push(value);
                    }

                    else if (views.Peek() is Operation operation) {
                        operation._roperand = value;
                    }

                    else if (views.Peek() is FunctionCall fcall) {
                        fcall._arguments.Push(value);
                        views.Push(value);
                    }

                    else {
                        throw new UnreachableCode("Nestable node expected");
                    }

                }

                else if (token is FunctionName fname) {

                    Function? function;
                    
                    if (!_functions.TryGetValue(fname._value, out function)) {
                        throw new ErrorAtPosition($"Function Not Found", fname._position, fname._length);
                    }

                    var fcall = new FunctionCall(function);

                    if (views.Any()) {

                        if (views.Peek() is Parenthesis parenthesis) {
                            parenthesis._child = fcall;
                        }

                        else if (views.Peek() is Operation operation) {
                            operation._roperand = fcall;
                        }

                        else {
                            throw new UnreachableCode("An Operation or Parenthesis node expected");
                        }
                    
                    }
                    
                    views.Push(fcall);
                }

                else if (token is OperatorSymbol operatorSymbol) {

                    Operator? op;
                    
                    if (!_operators.TryGetValue(operatorSymbol._value, out op)) {
                        throw new ErrorAtPosition($"Operator Not Found", operatorSymbol._position, operatorSymbol._length);
                    }
                    
                    var operation = new Operation(op);
                    
                    if (views.Peek() is Value value) {
                        
                        operation._loperand = views.Pop();

                        if (views.Any()) {

                            if (views.Peek() is FunctionCall fcall) {
                                fcall._arguments.Pop();
                                fcall._arguments.Push(operation);
                            }

                            else if (views.Peek() is Parenthesis parenthesis) {
                                parenthesis._child = operation;
                            }

                            else {
                                throw new UnreachableCode("A FunctionCall or Parenthesis node expected");
                            }

                        }

                    }

                    else if (views.Peek() is Operation parentOperation1) {
                        
                        if (operation._operator._precedence > parentOperation1._operator._precedence) {
                            operation._loperand = parentOperation1._roperand;
                            parentOperation1._roperand = operation;
                        }

                        else {

                            operation._loperand = views.Pop();

                            if (views.Any()) {

                                if (views.Peek() is FunctionCall fcall) {
                                    fcall._arguments.Pop();
                                    fcall._arguments.Push(operation);
                                }

                                else if (views.Peek() is Parenthesis parenthesis) {
                                    parenthesis._child = operation;
                                }

                                else {
                                    throw new UnreachableCode("A FunctionCall or Parenthesis node expected");
                                }

                            }

                        }

                    }

                    else if (views.Peek() is FunctionCall || views.Peek() is Parenthesis) {
                        
                        operation._loperand = views.Pop();

                        if (views.Any()) {

                            if (views.Peek() is Operation parentOperation2) {
                                parentOperation2._roperand = operation;
                            }

                            else if (views.Peek() is FunctionCall parentFcall) {
                                parentFcall._arguments.Pop();
                                parentFcall._arguments.Push(operation);
                            }

                            else if (views.Peek() is Parenthesis parentParenthesis) {
                                parentParenthesis._child = operation;
                            }

                            else {
                                throw new UnreachableCode("Nestable node expected");
                            }

                        }

                    }

                    else {
                        throw new UnreachableCode("Unrecognized node type");
                    }

                    views.Push(operation);
                }

                else if (token is OpeningParenthesis openingParenthesis) {

                    var parenthesis = new Parenthesis();

                    if (views.Any()) {

                        if (views.Peek() is Parenthesis parentParenthesis) {
                            parentParenthesis._child = parenthesis;
                        }

                        else if (views.Peek() is Operation operation) {
                            operation._roperand = parenthesis;
                        }

                        else if (views.Peek() is FunctionCall fcall) {
                            fcall._arguments.Push(parenthesis);
                        }

                        else {
                            throw new UnreachableCode("Nestable node expected");
                        }

                    }

                    views.Push(parenthesis);
                }

                else if (token is ClosingParenthesis closingParenthesis) {
                    
                    while (views.Any() && views.Peek() is not FunctionCall && views.Peek() is not Parenthesis) {
                        views.Pop();
                    }

                    if (views.Count > 1) {
                        views.Pop();
                    }
                    
                }

                else if (token is Comma comma) {
                    
                    while (views.Any() && views.Peek() is not FunctionCall) {
                        views.Pop();
                    }

                    var parenthesis = new Parenthesis();
                    ((FunctionCall) views.Peek())._arguments.Push(parenthesis);

                    views.Push(parenthesis);
                }

                else {
                    throw new UnreachableCode("Unrecognized token type");
                }
                
            }
            
            return views.Last();
        }
        
    }
    
}