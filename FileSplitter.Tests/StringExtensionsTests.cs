using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileSplitter.Tests {
    [TestClass]
    public class StringExtensionsTests {
        [TestMethod]
        public void ContainsWildcard_ContainsAsterisk_ReturnsTrue() {
            // Arrange
            string strToTest = "*.txt";
            // Act
            bool result = strToTest.ContainsWildcard();
            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ContainsWildcard_ContainsQuestionMark_ReturnsTrue() {
            // Arrange
            string strToTest = "?index.txt";
            // Act
            bool result = strToTest.ContainsWildcard();
            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ContainsWildcard_ContainsAsteriskAndQuestionMark_ReturnsTrue() {
            // Arrange
            string strToTest = "?index*.txt";
            // Act
            bool result = strToTest.ContainsWildcard();
            // Assert
            Assert.IsTrue(result);
        }
    }
}
