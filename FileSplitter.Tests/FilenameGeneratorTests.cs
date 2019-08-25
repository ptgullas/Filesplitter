using System;
using System.IO;
using FileSplitter.Services;
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

        [TestMethod]
        public void GenerateFilenameWithPath_StartingFilename_GeneratesCorrectly() {
            // Arrange
            string indexFile = "c:\\temp\\IA_2019_index.txt";
            int totalLines = 400;
            int linesPerFile = 50;
            string expected = @"c:\temp\IA_2019_index_1_001-050.txt";

            string targetFolder = Path.GetDirectoryName(indexFile);

            FilenameGenerator fg = new FilenameGenerator(indexFile, totalLines, linesPerFile, targetFolder);

            // Act
            string result = fg.GenerateFilenameWithPath();

            // Assert
            Assert.AreEqual(expected, result);
        }


    }
}
