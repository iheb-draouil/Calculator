using System.Collections.Generic;
using System.Linq;
using System;

using FluentAssertions;
using Xunit;

using Source;

namespace Tests
{
    public class ParserTests
    {
        private Parser parser;

        public ParserTests()
        {
            parser = new Parser(
                new Function[] { },
                new Operator[] { },
                new NumberConstant[] { }
            );
        }

        /// <summary>
        /// Expecting an ErrorAtPosition at position 1 where the Name "_abc" is located at,
        /// because a Name can't succeed a number (Number, NumberName or NumberSymbol).
        /// </summary>
        [Fact]
        public void Test1()
        {
            ((Action) (() => parser
            .Preprocess(new List<Token> {
                new NumberSymbol('π', 0),
                new Name("_abc", 1)
            })))
            .Should()
            .Throw<ErrorAtPosition>()
            .Where(e => e._position == 1 && e._length == 4)
            ;
        }

        /// <summary>
        /// A Name can't succeed a number (Number, NumberName or NumberSymbol).
        /// </summary>
        [Fact]
        public void Test2()
        {
            ((Action) (() => parser
            .Preprocess(new List<Token> {
                new Number(1.2, 0, 3),
                new Name("exp", 3),
            })))
            .Should()
            .Throw<ErrorAtPosition>()
            .Where(e => e._position == 3 && e._length == 3)
            ;
        }

        /// <summary>
        /// An OpeningParenthesis can't succeed a number (Number, NumberName or NumberSymbol).
        /// </summary>
        [Fact]
        public void Test3()
        {
            ((Action) (() => parser
            .Preprocess(new List<Token> {
                new Number(1, 0, 1),
                new OpeningParenthesis(1),
            })))
            .Should()
            .Throw<ErrorAtPosition>()
            .Where(e => e._position == 1 && e._length == 1)
            ;
        }

        /// <summary>
        /// Expecting the preprocessor to raise an ErrorAtPosition exception because a Comma
        /// is encountered outside of a context of a function call.
        /// </summary>
        [Fact]
        public void Test4()
        {
            ((Action) (() => parser
            .Preprocess(new List<Token> {
                new Number(123, 0, 3),
                new Comma(3),
                new Number(456, 4, 3),
            })))
            .Should()
            .Throw<ErrorAtPosition>()
            .Where(e => e._position == 3 && e._length == 1)
            ;
        }

        /// <summary>
        /// Expecting the preprocessor to raise an ErrorAtPosition exception because a Comma
        /// is encountered outside of a context of a function call.
        /// </summary>
        [Fact]
        public void Test5()
        {
            ((Action) (() => parser
            .Preprocess(new List<Token> {
                new Name("fn123", 0),
                new Comma(5),
                new Number(456, 6, 3),
            })))
            .Should()
            .Throw<ErrorAtPosition>()
            .Where(e => e._position == 5 && e._length == 1)
            ;
        }

        /// <summary>
        /// Unmatching Parenthesis Exception
        /// </summary>
        [Fact]
        public void Test6()
        {
            ((Action) (() => parser
            .Preprocess(new List<Token> {
                new OpeningParenthesis(0),
                new Number(111, 1, 3),
            })))
            .Should()
            .Throw<UnmatchingParenthesis>()
            ;
        }

        /// <summary>
        /// Expecting the preprocessor to interpret the Name "ln" as a FunctionName because
        /// it is succeeded with a OpeningParenthesis.
        /// </summary>
        [Fact]
        public void Test7()
        {
            parser.Preprocess(new List<Token> {
                new Name("ln", 0),
                new OpeningParenthesis(2),
                new Number(1, 3, 1),
                new ClosingParenthesis(4),
            })
            .Should()
            .BeEquivalentTo(new List<Token> {
                new FunctionName("ln", 0),
                new OpeningParenthesis(2),
                new Number(1, 3, 1),
                new ClosingParenthesis(4),
            })
            ;
        }

        /// <summary>
        /// Expecting the preprocessor to interpret the Name "exp" as a NumberName because
        /// it is not succeeded with a OpeningParenthesis.
        /// </summary>
        [Fact]
        public void Test8()
        {
            parser.Preprocess(new List<Token> {
                new Name("exp", 0),
                new OperatorSymbol('+', 3),
                new Number(1, 4, 1),
            })
            .Should()
            .BeEquivalentTo(new List<Token> {
                new NumberName("exp", 0),
                new OperatorSymbol('+', 3),
                new Number(1, 4, 1),
            })
            ;
        }

    }
    
}