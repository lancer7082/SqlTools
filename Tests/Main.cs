using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using SqlTools.Common;

namespace Tests
{
    [TestClass]
    public class Main
    {
        [TestMethod]
        public void TestLoadFile()
        {
            string fileName = @"..\..\..\Doc\test.sql";

            string text = System.IO.File.ReadAllText(fileName);

            //using (var reader = new StreamReader(fileName))
            //{
            //    string line;
            //    while ((line = reader.ReadLine()) != null)
            //    {
            //        System.Diagnostics.Debug.WriteLine(line);
            //        //System.Console.WriteLine(line);
            //    }
            //    reader.Close();
            //    //System.Console.WriteLine("Press a key");
            //    //System.Console.ReadLine();               
            //}
        }

        [TestMethod]
        public void TestParse()
        {
            string text = 
@"USE [Test]
GO

ALTER PROCEDURE [dbo].[Test]
    @Param1 INT,
    @Param2 INT
AS
BEGIN
  BEGIN
    SELECT '1'
  END
END
;";
            var parser = new SqlParser(text);
            parser.Parse();
        }

        [TestMethod]
        public void TestParseStringConstant()
        {
            string text =
@"/*
asdsad
sads
sdsd
*/
ALTER PROCEDURE Test
AS
BEGIN
  '''''asd''''
GO'
END
GO";
            var parser = new SqlParser(text);
            parser.Parse();
        }
    }
}
