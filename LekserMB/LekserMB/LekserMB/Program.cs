using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LekserMB
{
    class Program
    {
        //utworzenie listy tokenów
        static List<Token> tokenList = new List<Token>();
        static List<Token> tokenListToParser = new List<Token>();
        static List<Token> finalList = new List<Token>();
        static bool check = false;

        //stworzenie tzw. grup nazwanych dla poszczególnych regexów
        private static string pattern =
            @"(?<operator>\+|\-|\/|\*)|" +
            @"(?<variable>[a-zA-Z_$][a-zA-Z0-9_$]*)|" +
            @"(?<float>([0-9]+\.[0-9]+))|" +
            @"(?<digit>([0-9]+))|" +
            @"(?<bracket>\(|\))|" +
            @"(?<invalid>[^\s])";


        static void Main(string[] args)
        {
            string expression = " 1 3a.bc + 12 - 33 /2";
            expression = Regex.Replace(expression, @"\s+", "");
            Console.WriteLine("Wyrażenie: " + expression);
            Lexer(expression);
            Parser();
        }

        public static void ShowError(int i)
        {
            check = true;
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine("Error");
            Console.ResetColor();
            Console.WriteLine("Błąd występuje po tokenie " + i);
            
        }

        public static void ShowOk()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Green;
            Console.WriteLine("Ok :) ");
            Console.ResetColor();

        }


        public static void Lexer(string expression)
        {
            //utworzenie obiektu regexPattern klasy Regex, przekazujemy pattern
            Regex regexPattern = new Regex(pattern);

            //wyrażenie do sprawdzenia z patternem
            MatchCollection input = regexPattern.Matches(expression);

            //przelecenie po całym wyrażeniu z input i odpowiednie dopasowanie
            //poszczególnych częsci wyrażenia do grup z patternu
            foreach (Match m in input)
            {
                int i = 0;
                foreach (Group g in m.Groups)
                {
                    string matchValue = g.Value;

                    //testowanie wyniku dopasowania z własnością Success, jeśli jest true i i>2
                    if (g.Success && i > 2)
                    {
                        //wykorzystanie własności GroupNameFromNumber dla utworzonego wcześniej obiektu
                        //pobranie nazwy grupy odpowiadającej numerowi zmiennej i
                        string groupName = regexPattern.GroupNameFromNumber(i);
                        //utworzenie nowego tokenu i dodanie go do listy tokenów
                        tokenList.Add(new Token(groupName, matchValue));
                    }
                    i++;
                }
            }
        }

        public static void Parser()
        {
            int lBracket = 0;
            int rBracket = 0;
            //Parser 
            for (int i = 0; i < tokenList.Count; i++)
            {
                if (tokenList[i].Name != "invalid")
                {
                    if (i < tokenList.Count - 1)
                    {
                        if (tokenList[i].Name == tokenList[i + 1].Name)
                        {
                            ShowError(i);
                            break;
                        }
                    }
                    if (tokenList[0].Name == "operator")
                    {
                        ShowError(i);
                        break;
                    }
                    if (i==tokenList.Count-1 && tokenList[tokenList.Count - 1].Name == "operator")
                    {
                        ShowError(tokenList.Count - 1);
                        break;
                        
                    }
                    if (tokenList[i].Name == "bracket")
                    {
                        if (tokenList[i].Value == "(")
                        {
                            lBracket++;
                        }
                        else
                        {
                            rBracket++;
                        }
                    }
                    if (i == tokenList.Count && lBracket != rBracket)
                    {
                        ShowError(i);
                        break;
                    }

                    if (i < tokenList.Count-1)
                    {
                        if (tokenList[i].Name == "operator" && tokenList[i + 1].Name == "bracket") 
                        {
                            ShowError(i);
                            break;
                        }
                    }
                    tokenListToParser.Add(new Token(tokenList[i].Name, tokenList[i].Value));
                }
                else
                {
                    ShowError(i);
                    break;
                }
                
            }

            if (check==false)
            {
                ShowOk();
            }

            //foreach (var item in tokenListToParser)
            //{
            //    Console.WriteLine(item.Name + " " + item.Value);
            //}
        }
        
    }
}
