using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CalcC.Tests
{
    [TestClass]
    public class BasicTests
    {
        private static readonly object monitor = new();

        [TestMethod]
        public void CanCompileToCil()
        {
            // Arrange
            const string code = "3 3 +";

            // Act
            var calcc = new CalcC();
            calcc.CompileToCil(code);
            var cil = calcc.Cil;

            // Assert
            Assert.AreNotEqual(string.Empty, cil);
        }

        [DataTestMethod]
        [DataRow("4 3 -", "-1", "subtraction")]
        [DataRow("3 4 +", "7", "addition")]
        [DataRow("3 4 *", "12", "multiplication")]
        [DataRow("4 15 /", "3", "division")]
        [DataRow("5 7 %", "2", "modulus")]
        [DataRow("16 sqrt", "4", "sqrt")]
        [DataRow("-1 -1 *", "1", "negative operands")]
        public void TestManySimpleExpressions(string expr, string expectedResult, string label)
        {
            lock (monitor)
            {
                // Arrange handled by DataRows

                // Act
                var calcc = new CalcC();
                calcc.CompileToCil(expr);
                calcc.AssembleToObjectCode();
                var result = calcc.ExecuteObjectCode();

                // Assert
                Assert.AreEqual(expectedResult, result, label);
            }
        }

        [DataTestMethod]
        [DataRow("-16 sqrt", typeof(OverflowException), "sqrt of negative number")]
        public void TestForExceptions(string expr, Type expectedExceptionType, string label)
        {
            lock (monitor)
            {
                // Arrange handled by DataRows

                // Act and Assert
                var exception = Assert.ThrowsException<System.Reflection.TargetInvocationException>(() =>
                {
                    var calcc = new CalcC();
                    calcc.CompileToCil(expr);
                    calcc.AssembleToObjectCode();
                    calcc.ExecuteObjectCode();
                }, label);

                // Assert
                Assert.IsInstanceOfType(exception.InnerException, expectedExceptionType);
            }
        }
    }
}
