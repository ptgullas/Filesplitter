using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileSplitter.Tests {
    [TestClass]
    public class FilenameGeneratorTests {
        [TestMethod]
        public void GenerateFilename_StartingFilename_GeneratesCorrectly() {
            // Arrange
            string indexFile = "c:\\temp\\IA_2019_index.txt";
            int totalLines = 400;
            int linesPerFile = 50;
            string expected = "IA_2019_index_1_001-050.txt";

            FilenameGenerator fg = new FilenameGenerator(indexFile, totalLines, linesPerFile);

            // Act
            string result = fg.GenerateFilename();

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}
