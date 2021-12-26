using System.Collections.Generic;
using System.Linq;
using System;

using FluentAssertions;
using Xunit;

using MathExtensions;

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

        [Fact]
        public void Test1()
        {
            ((Action) (() => parser
            .Preprocess(new List<Token> {
                new NumberSymbol('π', 0),
                new Name("_abc", 1)
            })))
            .Should()
            .Throw<ExceptionAtPosition>()
            .Where(e => e._position == 1 && e._length == 4)
            ;
        }

        [Fact]
        public void Test2()
        {
            ((Action) (() => parser
            .Preprocess(new List<Token> {
                new Number(1.2, 0, 3),
                new Name("ln", 3),
            })))
            .Should()
            .Throw<ExceptionAtPosition>()
            .Where(e => e._position == 3 && e._length == 2)
            ;
        }

        [Fact]
        public void Test3()
        {
            ((Action) (() => parser
            .Preprocess(new List<Token> {
                new Number(1, 0, 1),
                new OpeningParenthesis(1),
            })))
            .Should()
            .Throw<ExceptionAtPosition>()
            .Where(e => e._position == 1 && e._length == 1)
            ;
        }

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
            .Throw<ExceptionAtPosition>()
            .Where(e => e._position == 3 && e._length == 1)
            ;
        }

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
            .Throw<ExceptionAtPosition>()
            .Where(e => e._position == 5 && e._length == 1)
            ;
        }
    }
    
}