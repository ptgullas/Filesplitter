using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileSplitter;

namespace FileSplitter.Tests {
    [TestClass]
    public class MathHelpersTests {
        [TestMethod]
        public void CountDigitsInNumber_Valid_ReturnsNumber() {
            // Arrange
            int numberToTest = 40000;
            int expected = 5;
            // Act
            int result = MathHelpers.CountDigits(numberToTest);
            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void CountDigitsInNumber_Zero_ReturnsOne() {
            // Arrange
            int numberToTest = 0;
            int expected = 1;
            // Act
            int result = MathHelpers.CountDigits(numberToTest);
            // Assert
            Assert.AreEqual(expected, result);
        }

        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        [TestMethod]
        public void CountDigitsInNumber_Negative_ThrowsException() {
            // Arrange
            int numberToTest = -3;
            // Act
            int result = MathHelpers.CountDigits(numberToTest);
            // Assert
        }

        [TestMethod]
        public void GetNumericDecimalFormatString_Valid_ReturnsString() {
            // Arrange
            int numberToTest = 23;
            string expected = "D2";
            // Act
            string result = MathHelpers.GetNumericDecimalFormatString(numberToTest);
            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetNumericDecimalFormatString_Zero_ReturnsString() {
            // Arrange
            int numberToTest = 0;
            string expected = "D1";
            // Act
            string result = MathHelpers.GetNumericDecimalFormatString(numberToTest);
            // Assert
            Assert.AreEqual(expected, result);
        }

        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        [TestMethod]
        public void GetNumericDecimalFormatString_Negative_ThrowsException() {
            // Arrange
            int numberToTest = -99;
            string expected = "D1";
            // Act
            string result = MathHelpers.GetNumericDecimalFormatString(numberToTest);
            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void CalculateNumberOfFiles_Valid_ReturnsNumber() {
            // Arrange
            int totalLines = 10;
            int maxLinesPerFile = 5;
            int expected = 2;
            // Act
            int result = MathHelpers.CalculateNumberOfFiles(totalLines, maxLinesPerFile);
            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void CalculateNumberOfFiles_Uneven_ReturnsNumber() {
            // Arrange
            int totalLines = 10;
            int maxLinesPerFile = 6;
            int expected = 2;
            // Act
            int result = MathHelpers.CalculateNumberOfFiles(totalLines, maxLinesPerFile);
            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void CalculateNumberOfFiles_MaxLinesPerFileGreaterThanTotalLines_ReturnsOne() {
            // Arrange
            int totalLines = 10;
            int maxLinesPerFile = 16;
            int expected = 1;
            // Act
            int result = MathHelpers.CalculateNumberOfFiles(totalLines, maxLinesPerFile);
            // Assert
            Assert.AreEqual(expected, result);
        }


    }
}
