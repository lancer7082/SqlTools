using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

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
            string test = 
@"USE [Test]
GO

ALTER PROCEDURE[dbo].[Test]
AS
BEGIN
  BEGIN
    SELECT '1'
  END
END
;";

        }
    }
}
