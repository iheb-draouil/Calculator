using System.Collections.Generic;
using System.Linq;
using System;

using FluentAssertions;
using Xunit;

using MathEval;

namespace Tests
{
    public class LexerTests
    {
        private Lexer lexer;

        public LexerTests()
        {
            lexer = new Lexer();
        }

        /// <summary>
        /// Expecting symbols, parenthesis and commas to each be interprested as their
        /// respective token types.
        /// </summary>
        [Fact]
        public void Test1()
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

        /// <summary>
        /// Expecting the sequence "π" to be interpreted as a NumberSymbol while the sequence
        /// "_xxx" to be interpreted as a Name.
        /// </summary>
        [Fact]
        public void Test2()
        {
            lexer.Tokenize("π_xxx")
            .Should()
            .BeEquivalentTo(new List<Token> {
                new NumberSymbol('π', 0),
                new Name("_xxx", 1)
            }, options => options.RespectingRuntimeTypes())
            ;
        }

        /// <summary>
        /// Expecting the entire sequence to be interpreted as a Name.
        /// </summary>
        [Fact]
        public void Test3()
        {
            lexer.Tokenize("_123")
            .First()
            .Should()
            .BeEquivalentTo(new Name("_123", 0))
            ;
        }

        /// <summary>
        /// Expecting the entire sequence to be interpreted as a Name.
        /// </summary>
        [Fact]
        public void Test4()
        {
            lexer.Tokenize("xxx_123")
            .First()
            .Should()
            .BeEquivalentTo(new Name("xxx_123", 0))
            ;
        }

        /// <summary>
        /// Expecting the "123" sequence to be interpreted as a Number while the "_xxx" sequence to be
        /// a Name.
        /// </summary>
        [Fact]
        public void Test5()
        {
            lexer.Tokenize("123_xxx")
            .Should()
            .BeEquivalentTo(new List<Token> {
                new Number(123, 0, 3),
                new Name("_xxx", 3)
            }, options => options.RespectingRuntimeTypes())
            ;
        }

        /// <summary>
        /// Expecting the "1.23" sequence to be interpreted as a Number while the "_xxx" sequence to be
        /// a Name.
        /// </summary>
        [Fact]
        public void Test6()
        {
            lexer.Tokenize("1.23xxx")
            .Should()
            .BeEquivalentTo(new List<Token> {
                new Number(1.23, 0, 4),
                new Name("xxx", 4)
            }, options => options.RespectingRuntimeTypes())
            ;
        }

        /// <summary>
        /// Expecting the "1.2" sequence to be interpreted as a Number, the "_3" sequence to
        /// be interpreted as a Name while the sequence "π" as an OperatorSymbol.
        /// </summary>
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

        /// <summary>
        /// Expecting the "1.2" sequence to be interpreted as a Number while the "ln" sequence to
        /// be interpreted as a Name
        /// </summary>
        [Fact]
        public void Test8()
        {
            lexer.Tokenize("1.2ln")
            .Should()
            .BeEquivalentTo(new List<Token> {
                new Number(1.2, 0, 3),
                new Name("ln", 3),
            }, options => options.RespectingRuntimeTypes())
            ;
        }

        /// <summary>
        /// Expecting the lexer to raise an ErrorAtPosition exception at position
        /// 5 concerning the sequence "." since the decimal point can't appear as the first character
        /// of any valid sequence.
        /// </summary>
        [Fact]
        public void Test9()
        {
            ((Action) (() => lexer.Tokenize("_xxx1.3xxx")))
            .Should()
            .Throw<ErrorAtPosition>()
            .Where(e => e._position == 5 && e._length == 1)
            ;
        }
        
        /// <summary>
        /// Expecting the sequence "_xxx" to yield a Name, 1.3 to yield a Number and xxx to yield
        /// another Name.
        /// </summary>
        [Fact]
        public void Test10()
        {
            lexer.Tokenize("_xxx 1.3 xxx")
            .Should()
            .BeEquivalentTo(new List<Token> {
                new Name("_xxx", 0),
                new Number(1.3, 5, 3),
                new Name("xxx", 9),
            }, options => options.RespectingRuntimeTypes())
            ;
        }

        /// <summary>
        /// Expecting the lexer to raise an ErrorAtPosition exception at position
        /// 0 because the subsequence "1." of the sequence "1.x" is an invalid sequence by itself.
        /// </summary>
        [Fact]
        public void Test11()
        {
            ((Action) (() => lexer.Tokenize("1.xxx+")))
            .Should()
            .Throw<ErrorAtPosition>()
            .Where(e => e._position == 0 && e._length == 3)
            ;
        }
        
        /// <summary>
        /// Expecting the sequences "123", "+", "2", "_3" and "+" to yield a Number, an OperatorSymbol,
        /// a Number, a Name and an OperatorSymbol repectively.
        /// </summary>
        [Fact]
        public void Test12()
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

        /// <summary>
        /// Expecting the sequences "1", "+", "1" and "_" to yield a Number, an OperatorSymbol,
        /// a Number and a Name repectively.
        /// </summary>
        [Fact]
        public void Test13()
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

        /// <summary>
        /// Expecting an OpeningParenthesis
        /// </summary>
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

        /// <summary>
        /// Expecting the lexer to raise an ErrorAtPosition exception at position
        /// 0 because the subsequence "1." is invalid.
        /// </summary>
        [Fact]
        public void Test15()
        {
            ((Action) (() => lexer.Tokenize("1.")))
            .Should()
            .Throw<ErrorAtPosition>()
            .Where(e => e._position == 0 && e._length == 2)
            ;
        }

        /// <summary>
        /// Expecting the lexer to raise an ErrorAtPosition exception at position
        /// 0 because the subsequence "." is invalid.
        /// </summary>
        [Fact]
        public void Test16()
        {
            ((Action) (() => lexer.Tokenize(".")))
            .Should()
            .Throw<ErrorAtPosition>()
            .Where(e => e._position == 0 && e._length == 1)
            ;
        }

        /// <summary>
        /// Expecting a single NumberSymbol.
        /// </summary>
        [Fact]
        public void Test17()
        {
            lexer.Tokenize("π")
            .First()
            .Should()
            .BeEquivalentTo(new NumberSymbol('π', 0))
            ;
        }

        /// <summary>
        /// Expecting the sequence "1" to yield a Number and the "+" sequence to yield an OperatorSymbol.
        /// </summary>
        [Fact]
        public void Test18()
        {
            lexer.Tokenize("1+")
            .Should()
            .BeEquivalentTo(new List<Token> {
                new Number(1, 0, 1),
                new OperatorSymbol('+', 1),
            }, options => options.RespectingRuntimeTypes())
            ;
        }

        /// <summary>
        /// Expecting 3 OperatorSymbols.
        /// </summary>
        [Fact]
        public void Test19()
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

        /// <summary>
        /// Expecting 3 OperatorSymbols.
        /// </summary>
        [Fact]
        public void Test20()
        {
            lexer.Tokenize("1 1")
            .Should()
            .BeEquivalentTo(new List<Token> {
                new Number(1, 0, 1),
                new Number(1, 2, 1),
            }, options => options.RespectingRuntimeTypes())
            ;
        }

        /// <summary>
        /// Expecting a Number, a comma and a Number that correspond to the sequence "123", "," and "456"
        /// respectively.
        /// </summary>
        [Fact]
        public void Test21()
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

        /// <summary>
        /// Expecting a Number, an OperatorSymbol and Number that corresponds to the sequence "1", "+" and "2"
        /// respectively.
        /// </summary>
        [Fact]
        public void Test22()
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

        /// <summary>
        /// Testing a complex expression comprizing all token classes.
        /// </summary>
        [Fact]
        public void Test23()
        {
            lexer.Tokenize("mv(π/(2+3),(1+2)/(2+3))")
            .Should()
            .BeEquivalentTo(new List<Token> {
                new Name("mv", 0),
                new OpeningParenthesis(2),
                new NumberSymbol('π', 3),
                new OperatorSymbol('/', 4),
                new OpeningParenthesis(5),
                new Number(2, 6, 1),
                new OperatorSymbol('+', 7),
                new Number(3, 8, 1),
                new ClosingParenthesis(9),
                new Comma(10),
                new OpeningParenthesis(11),
                new Number(1, 12, 1),
                new OperatorSymbol('+', 13),
                new Number(2, 14, 1),
                new ClosingParenthesis(15),
                new OperatorSymbol('/', 16),
                new OpeningParenthesis(17),
                new Number(2, 18, 1),
                new OperatorSymbol('+', 19),
                new Number(3, 20, 1),
                new ClosingParenthesis(21),
                new ClosingParenthesis(22),
            }, options => options.RespectingRuntimeTypes())
            ;
        }
        
    }
    
}