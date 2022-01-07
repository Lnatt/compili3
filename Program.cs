using System;
using Irony;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Irony.Parsing;
using System.Diagnostics;

namespace compili3
{
    public class EquasionDescription
    {
        public string name { get; }

        public string hash { get; }

        public string left { get; }
        public string right { get; }

        public string comment { get; }
        public List<string> presentedIDs { get; }

        public EquasionDescription(string number, string l, string r, string c, List<string> presentedIDs)
        {
            name = number;
            left = l;
            right = r;
            comment = c;
            this.presentedIDs = presentedIDs;
            hash = Regex.Replace(l, @"\s", "");
        }


    }

    public class MyGrammar : Grammar
    {

        public MyGrammar()
        {
            #region Initial setup of the grammar

            // define all the terminals and non-terminals
            var equasionList = new NonTerminal("equasions");
            var equasion = new NonTerminal("equasion");
            var equasionNumber = new IdentifierTerminal("equasionNumber");
            var leftSide = new FreeTextLiteral("leftSide", "=>");
            var rightSide = new FreeTextLiteral("rightSide", "\n", "{");
            var comment = new CommentTerminal("comment", "{", "}");

            // specify the non-terminal which is the root of the AST

            #endregion

            #region Define the grammar

            equasion.Rule = equasionNumber + ":" + leftSide + "=>" + rightSide + comment;

            equasionList.Rule = MakePlusRule(equasionList, null, equasion);

            this.Root = equasionList;

            this.MarkPunctuation(":", "=>", "{", "}");

            #endregion
        }

         public static List<EquasionDescription> TransformsFromAST(ParseTree AST)
         {
             List<EquasionDescription> equations = new List<EquasionDescription>();
             foreach (ParseTreeNode n in AST.Root.ChildNodes)
             //foreach (ParseTreeNode n in AST.Root.ChildNodes)
             {
                 string num = n.ChildNodes[0].Token.ValueString;
                 string left = n.ChildNodes[1].Token.ValueString;
                 string right = n.ChildNodes[2].Token.ValueString;
                 string comment = n.ChildNodes[3].Token.ValueString;
                 string pattern = @"[_a-zA-Z][_a-zA-Z0-9]*";
                 RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Compiled;

                 Regex optionRegex = new Regex(pattern, options);
                 var matchesLeft = optionRegex.Matches(@left);
                 var matchesRight = optionRegex.Matches(@right);
                 List<string> presentedIDsLeft = (from object match in matchesLeft select match.ToString()).ToList();
                 List<string> presentedIDsRight = (from object match in matchesRight select match.ToString()).ToList();
                 List<string> presentedIDs = presentedIDsRight.Where(presentedId => presentedIDsLeft.Contains(presentedId)).ToList();


                 EquasionDescription eq = new EquasionDescription(num, left, right, comment, presentedIDs);
                 equations.Add(eq);
             }
             return equations;
         }


        public void TransformsFromAST_2(string s)
        {
            string equasionNumber = "";
            string leftSide = "";
            string rightSide = "";
            string comment = "";

            int n = 0;
            int l = s.Length;
            if(l > n)
            {
                //equasionNumber + ":" + leftSide + "=>" + rightSide + comment;
                if (s.Contains(":") & s.IndexOf(":") > 0) //если двоеточие есть и не первый символ
                {
                    equasionNumber = s.Substring(0, s.IndexOf(":"));
                    n = s.IndexOf(":");
                }
                if (s.Contains("=>") & s.IndexOf("=>") > n) //если двоеточие есть и после equasionNumber
                {
                    leftSide = s.Substring(n + 1, s.IndexOf("=>") - n - 1);
                    n = s.IndexOf("=>") + 1;
                }
                if (l > n) //если правая сторона есть
                {
                    if (s.Contains("{")) //если есть комментарии
                    {
                        rightSide = s.Substring(n + 1, l - s.IndexOf("{") - 2);
                        n = s.IndexOf("{");
                    }
                    else
                    {
                        rightSide = s.Substring(n + 1, l + 2);
                        n = l;
                    }
                }
                if (l > n) //если есть комментарии
                {
                    comment = s.Substring(n + 1, l - n - 2);
                    n = l;
                }
            }
            Console.WriteLine($"Исходная строка: {s}");
            Console.WriteLine($"equasionNumber: {equasionNumber}");
            Console.WriteLine($"leftSide: {leftSide}");
            Console.WriteLine($"rightSide: {rightSide}");
            Console.WriteLine($"comment: {comment}");

        }
    }

    public class MyParser
    {
        public Parser _parser = null;
        public MyParser()
        {
            MyGrammar g = new MyGrammar();
            _parser = new Parser(g);
        }
        /*public ParseTree Parse(string text, string s)
        {
            return _parser.Parse(text);
        }*/
        public ParseTree Parse(string text)
        {
            Console.WriteLine(_parser.Parse(text, "sss").Root);
            return _parser.Parse(text, "sss");
        }
    }


    class Program
    {
        static void Main()
        {
            MyParser p = new MyParser();

            //////////ParseTree t = p.Parse("1: a + b => deio3sf3 { nonsense }");
            ///////////////MyGrammar.TransformsFromAST(t);
            ///
            //MyGrammar gg = new MyGrammar();
            //LanguageData languageData = new LanguageData(gg);
            // ParseTree t1 = new ParseTree("Sffa", "snnnn");
            //ParseTree t = p.Parse("1: a + b => deioh33hesf3 { nonsense }");
            //t.Root = languageData.Grammar.Root;
            MyGrammar grammar = new MyGrammar();
            grammar.TransformsFromAST_2("561: a + b => deio3sf3 { nonsense }");


            Console.WriteLine();
        }
    }
}
