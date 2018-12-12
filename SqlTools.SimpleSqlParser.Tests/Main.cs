using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using SqlTools.SimpleSqlParser;

namespace Tests
{

    [TestClass]
    public class Main
    {
        [TestMethod]
        public void TestLoadFile()
        {
            string fileName = @"..\..\..\Doc\test.sql";

            string text = File.ReadAllText(fileName);

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
 
--DROP PROCEDURE [dbo].[Proc1]

IF OBJECT_ID('[dbo].[Proc01]') IS NULL
	EXEC ('CREATE PROCEDURE [dbo].[Proc01] AS BEGIN RETURN END')
GO

-- Proc01
-- 111

/*
ads
*/
ALTER PROCEDURE [dbo].[Proc01] 
	@Param1 INT,
	@Param2 CHAR = NULL
AS 
BEGIN 
	RETURN 
END
GO

GRANT EXEC ON [dbo].[Proc01] TO [login01]
--GO

IF OBJECT_ID('[dbo].[Proc02]') IS NULL
	EXEC ('CREATE PROCEDURE [dbo].[Proc02] AS BEGIN RETURN END')
GO

/* Proc02
222
*/
ALTER PROCEDURE [dbo].[Proc02] 
AS 
BEGIN 
	RETURN 
END
GO";
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

        [TestMethod]
        public void TestParseComment()
        {
            string text =
@"/*
asdsad
sads
sdsd
*/
USE asdas
-- adasd
GO";
            //var lexer = new SqlLexer(text);
            //Token token;
            ////bool isValid = false;
            //while ((token = lexer.Next()) != null)
            //{
            //    Console.WriteLine(token.Type.ToString());
            //};
        }

        [TestMethod]
        public void TestMergeStatement()
        {
            string text =
@";MERGE assds
USING asdasd ON dsad
WHEN MATCHED THEN adsad
;";
            //var lexer = new SqlLexer(text);
            //Token token;
            ////bool isValid = false;
            //while ((token = lexer.Next()) != null)
            //{
            //    Console.WriteLine(token.Type.ToString());
            //};
        }
    }
}
