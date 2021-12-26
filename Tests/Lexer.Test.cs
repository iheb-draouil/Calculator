using System.Collections.Generic;
using System.Linq;
using System;

using FluentAssertions;
using Xunit;

using MathExtensions;

namespace Tests
{
    public class LexerTests
    {
        private Lexer lexer;

        public LexerTests()
        {
            lexer = new Lexer();
        }

        [Fact]
        public void testing_symbols_always_forming_tokens_by_their_own()
        {
            lexer.Tokenize("(π,)")
            .Should()
            .BeEquivalentTo(new List<Token> {
                new OpeningParenthesis(0),
                new NumberSymbol('π', 1),
                new Comma(2),
                new ClosingParenthesis(3),
            }, options => options.RespectingRuntimeTypes())
            ;
        }

        [Fact]
        public void testing_names_never_starting_with_symbols()
        {
            lexer.Tokenize("π_abc")
            .Should()
            .BeEquivalentTo(new List<Token> {
                new NumberSymbol('π', 0),
                new Name("_abc", 1)
            }, options => options.RespectingRuntimeTypes())
            ;
        }

        [Fact]
        public void testing_numbers_prepended_with_underscore_considered_as_a_name_1()
        {
            lexer.Tokenize("_123")
            .First()
            .Should()
            .BeEquivalentTo(new Name("_123", 0))
            ;
        }

        [Fact]
        public void testing_numbers_prepended_with_underscore_considered_as_a_name_2()
        {
            lexer.Tokenize("abc_123")
            .First()
            .Should()
            .BeEquivalentTo(new Name("abc_123", 0))
            ;
        }

        [Fact]
        public void testing_names_never_starting_with_numbers()
        {
            lexer.Tokenize("123_abc")
            .Should()
            .BeEquivalentTo(new List<Token> {
                new Number(123, 0, 3),
                new Name("_abc", 3)
            }, options => options.RespectingRuntimeTypes())
            ;
        }

        [Fact]
        public void testing_names_never_starting_with_decimal_numbers()
        {
            lexer.Tokenize("1.23abc")
            .Should()
            .BeEquivalentTo(new List<Token> {
                new Number(1.23, 0, 4),
                new Name("abc", 4)
            }, options => options.RespectingRuntimeTypes())
            ;
        }

        [Fact]
        public void Test7()
        {
            lexer.Tokenize("1.2_3+")
            .Should()
            .BeEquivalentTo(new List<Token> {
                new Number(1.2, 0, 3),
                new Name("_3", 3),
                new OperatorSymbol('+', 5)
            }, options => options.RespectingRuntimeTypes())
            ;
        }

        [Fact]
        public void Test8()
        {
            ((Action) (() => lexer.Tokenize("1.xxx+")))
            .Should()
            .Throw<ExceptionAtPosition>()
            .Where(e => e._position == 0 && e._length == 3)
            ;
        }

        [Fact]
        public void Test9()
        {
            lexer.Tokenize("123+2_3+")
            .Should()
            .BeEquivalentTo(new List<Token> {
                new Number(123, 0, 3),
                new OperatorSymbol('+', 3),
                new Number(2, 4, 1),
                new Name("_3", 5),
                new OperatorSymbol('+', 7),
            }, options => options.RespectingRuntimeTypes())
            ;
        }

        [Fact]
        public void Test10()
        {
            lexer.Tokenize("1+1_")
            .Should()
            .BeEquivalentTo(new List<Token> {
                new Number(1, 0, 1),
                new OperatorSymbol('+', 1),
                new Number(1, 2, 1),
                new Name("_", 3),
            }, options => options.RespectingRuntimeTypes())
            ;
        }

        [Fact]
        public void Test11()
        {
            lexer.Tokenize("1abc+2")
            .Should()
            .BeEquivalentTo(new List<Token> {
                new Number(1, 0, 1),
                new Name("abc", 1),
                new OperatorSymbol('+', 4),
                new Number(2, 5, 1),
            }, options => options.RespectingRuntimeTypes())
            ;
        }

        [Fact]
        public void Test12()
        {
            lexer.Tokenize("_abc 1.3 abc")
            .Should()
            .BeEquivalentTo(new List<Token> {
                new Name("_abc", 0),
                new Number(1.3, 5, 3),
                new Name("abc", 9),
            }, options => options.RespectingRuntimeTypes())
            ;
        }

        [Fact]
        public void Test13()
        {
            ((Action) (() => lexer.Tokenize("_abc1.3abc")))
            .Should()
            .Throw<ExceptionAtPosition>()
            .Where(e => e._position == 5 && e._length == 1)
            ;
        }

        [Fact]
        public void Test14()
        {
            lexer.Tokenize("(")
            .Should()
            .BeEquivalentTo(new List<Token> {
                new OpeningParenthesis(0)
            }, options => options.RespectingRuntimeTypes())
            ;
        }

        [Fact]
        public void Test15()
        {
            ((Action) (() => lexer.Tokenize("1.")))
            .Should()
            .Throw<ExceptionAtPosition>()
            .Where(e => e._position == 1 && e._length == 1)
            ;
        }

        [Fact]
        public void Test16()
        {
            ((Action) (() => lexer.Tokenize(".")))
            .Should()
            .Throw<ExceptionAtPosition>()
            .Where(e => e._position == 0 && e._length == 1)
            ;
        }

        [Fact]
        public void Test17()
        {
            lexer.Tokenize("π")
            .First()
            .Should()
            .BeEquivalentTo(new NumberSymbol('π', 0))
            ;
        }

        [Fact]
        public void Test18()
        {
            lexer.Tokenize("1+2")
            .Should()
            .BeEquivalentTo(new List<Token> {
                new Number(1, 0, 1),
                new OperatorSymbol('+', 1),
                new Number(2, 2, 1),
            }, options => options.RespectingRuntimeTypes())
            ;
        }

        [Fact]
        public void Test19()
        {
            lexer.Tokenize("1+")
            .Should()
            .BeEquivalentTo(new List<Token> {
                new Number(1, 0, 1),
                new OperatorSymbol('+', 1),
            }, options => options.RespectingRuntimeTypes())
            ;
        }

        [Fact]
        public void Test20()
        {
            lexer.Tokenize("+++")
            .Should()
            .BeEquivalentTo(new List<Token> {
                new OperatorSymbol('+', 0),
                new OperatorSymbol('+', 1),
                new OperatorSymbol('+', 2),
            }, options => options.RespectingRuntimeTypes())
            ;
        }

        [Fact]
        public void Test21()
        {
            lexer.Tokenize("1.2ln")
            .Should()
            .BeEquivalentTo(new List<Token> {
                new Number(1.2, 0, 3),
                new Name("ln", 3),
            }, options => options.RespectingRuntimeTypes())
            ;
        }

        [Fact]
        public void Test22()
        {
            lexer.Tokenize("1(")
            .Should()
            .BeEquivalentTo(new List<Token> {
                new Number(1, 0, 1),
                new OpeningParenthesis(1),
            }, options => options.RespectingRuntimeTypes())
            ;
        }

        [Fact]
        public void Test23()
        {
            lexer.Tokenize("123,456")
            .Should()
            .BeEquivalentTo(new List<Token> {
                new Number(123, 0, 3),
                new Comma(3),
                new Number(456, 4, 3),
            }, options => options.RespectingRuntimeTypes())
            ;
        }

        [Fact]
        public void Test24()
        {
            lexer.Tokenize("fn123,456")
            .Should()
            .BeEquivalentTo(new List<Token> {
                new Name("fn123", 0),
                new Comma(5),
                new Number(456, 6, 3),
            }, options => options.RespectingRuntimeTypes())
            ;
        }

        [Fact]
        public void Test25()
        {
            lexer.Tokenize("ln(1/(2+3))*(1+2)/(2+3)")
            .Should()
            .BeEquivalentTo(new List<Token> {
                new Name("ln", 0),
                new OpeningParenthesis(2),
                new Number(1, 3, 1),
                new OperatorSymbol('/', 4),
                new OpeningParenthesis(5),
                new Number(2, 6, 1),
                new OperatorSymbol('+', 7),
                new Number(3, 8, 1),
                new ClosingParenthesis(9),
                new ClosingParenthesis(10),
                new OperatorSymbol('*', 11),
                new OpeningParenthesis(12),
                new Number(1, 13, 1),
                new OperatorSymbol('+', 14),
                new Number(2, 15, 1),
                new ClosingParenthesis(16),
                new OperatorSymbol('/', 17),
                new OpeningParenthesis(18),
                new Number(2, 19, 1),
                new OperatorSymbol('+', 20),
                new Number(3, 21, 1),
                new ClosingParenthesis(22),
            }, options => options.RespectingRuntimeTypes())
            ;
        }
    }
    
}