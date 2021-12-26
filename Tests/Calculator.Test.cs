using System.Linq;
using System;

using Xunit;

using MathExtensions;

namespace Tests
{
    public class CalculatorTests
    {
        private Calculator calculator;

        public CalculatorTests()
        {
            var ln = new Function("ln", arguments => {
    
                if (arguments.Count() != 1) {
                    throw new Exception("The natural logarithm takes exactly 1 argument");
                }

                return Math.Log(arguments.First());
            });

            var mv2 = new Function("mv2", arguments => {
                
                if (arguments.Count() != 2) {
                    throw new Exception("'mv2' takes exactly 2 arguments");
                }
                
                return arguments[0] + arguments[1];
            });

            var mv3 = new Function("mv3", arguments => {
                
                if (arguments.Count() != 3) {
                    throw new Exception("'mv3' takes exactly 3 arguments");
                }
                
                return arguments[0] + arguments[1] + arguments[1];
            });

            var plus = new Operator('+', (a, b) => a + b, 0);
            var subtract = new Operator('-', (a, b) => a - b, 0);
            var multiply = new Operator('*', (a, b) => a * b, 1);
            var divide = new Operator('/', (a, b) => {
                
                if (b == 0) {
                    throw new Exception("Division by zero is not allowed");
                }

                return a / b;
            }, 1);

            var pi = new NumberConstant("π", Math.PI);

            var lexer = new Lexer();

            var parser = new Parser(
                new[] { ln, mv2, mv3 },
                new[] { plus, subtract, multiply, divide },
                new[] { pi }
            );

            calculator = new Calculator(lexer, parser);
        }

        [Fact]
        public void Test1()
        {
            Assert.Equal(Math.PI, calculator.GetResult("π"));
        }

        [Fact]
        public void Test2()
        {
            Assert.Equal(2*Math.PI+1.0/2, calculator.GetResult("2*π+1/2"));
        }

        [Fact]
        public void Test3()
        {
            Assert.Equal(2, calculator.GetResult("1-2+3"));
        }

        [Fact]
        public void Test4()
        {
            Assert.Equal(0, calculator.GetResult("1+2-3"));
        }

        [Fact]
        public void Test5()
        {
            Assert.Equal(2*Math.PI+1.0/2, calculator.GetResult("(2*π+1/2)"));
        }

        [Fact]
        public void Test6()
        {
            Assert.Equal(1+(Math.Log(2.0/3)-4+5.0/(6+7))/8, calculator.GetResult("1+mv2(ln(2/3)-4,5/(6+7))/8"));
        }

        [Fact]
        public void Test7()
        {
            Assert.Equal(1+(1.0/2/3+5.0/(6+7))/8, calculator.GetResult("1+mv2(1/2/3,5/(6+7))/8"));
        }

        [Fact]
        public void Test8()
        {
            Assert.Equal(1+((2.0/3)/4+5.0/(6+7))/8, calculator.GetResult("1+mv2((2/3)/4,5/(6+7))/8"));
        }

        [Fact]
        public void Test9()
        {
            Assert.Equal(1+((2.0/3)-4+5.0/(6+7))/8, calculator.GetResult("1+mv2((2/3)-4,5/(6+7))/8"));
        }

        [Fact]
        public void Test10()
        {
            Assert.Throws<Exception>(() => calculator.GetResult("1/0"));
        }

        [Fact]
        public void Test11()
        {
            Assert.Equal(1+((2.0/3)-4+5.0/(6+7)+5.0/(6+7))/8, calculator.GetResult("1+mv3((2/3)-4,5/(6+7),5/(6+7))/8"));
        }
    }
    
}