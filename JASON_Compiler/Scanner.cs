using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum Token_Class
{
    Number, String, Comment, Identifier, Add, Subtract, Divide, Multiply,Main,end,
    And, Or, lessThan, MoreThan, notEqual, Equal, Integer, Float, stringk,
    read, write, repeat, until, ifk, elseif, elsek, then, returnk, endl, LeftBraces, RightBraces, LeftParentheses, RightParentheses, Semicolon, comma
}
namespace JASON_Compiler
{

    public class Token
    {
        public string lex;
        public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> ArithmaticOperators = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> BooleanOperators = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> ConditionOperators = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Brackets = new Dictionary<string, Token_Class>();


        public Scanner()
        {
            ReservedWords.Add("int", Token_Class.Integer);
            ReservedWords.Add("float", Token_Class.Float);
            ReservedWords.Add("string", Token_Class.stringk);
            ReservedWords.Add("read", Token_Class.read);
            ReservedWords.Add("write", Token_Class.write);
            ReservedWords.Add("repeat", Token_Class.repeat);
            ReservedWords.Add("until", Token_Class.until);
            ReservedWords.Add("if", Token_Class.ifk);
            ReservedWords.Add("elseif", Token_Class.elseif);
            ReservedWords.Add("else", Token_Class.elsek);
            ReservedWords.Add("then", Token_Class.then);
            ReservedWords.Add("return", Token_Class.returnk);
            ReservedWords.Add("endl", Token_Class.endl);
            ReservedWords.Add("main", Token_Class.Main);
            ReservedWords.Add("end", Token_Class.end);

            ConditionOperators.Add("<", Token_Class.lessThan);
            ConditionOperators.Add(">", Token_Class.MoreThan);
            ConditionOperators.Add(":=", Token_Class.Equal);
            ConditionOperators.Add("<>", Token_Class.notEqual);

            BooleanOperators.Add("&&", Token_Class.And);
            BooleanOperators.Add("||", Token_Class.Or);

            ArithmaticOperators.Add("+", Token_Class.Add);
            ArithmaticOperators.Add("-", Token_Class.Subtract);
            ArithmaticOperators.Add("/", Token_Class.Divide);
            ArithmaticOperators.Add("*", Token_Class.Multiply);
            Brackets.Add("{", Token_Class.LeftBraces);
            Brackets.Add("}", Token_Class.RightBraces);
            Brackets.Add("(", Token_Class.LeftParentheses);
            Brackets.Add(")", Token_Class.RightParentheses);
            Brackets.Add(";", Token_Class.Semicolon);
            Brackets.Add(",", Token_Class.comma);



        }

        public void StartScanning(string SourceCode)
        {
            // i: Outer loop to check on lexemes.
            for (int i = 0; i < SourceCode.Length; i++)
            {
                // j: Inner loop to check on each character in a single lexeme.
                int j = i;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();

                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n' || CurrentChar == '\t')
                {
                    continue;
                }

                if (char.IsLetterOrDigit(CurrentChar))
                {
                    // The possible Token Classes that begin with a character are
                    // an Idenifier or a Reserved Word.

                    // (1) Update the CurrentChar and validate its value.
                    CurrentChar = SourceCode[j];
                    while (char.IsLetterOrDigit(CurrentChar) || CurrentChar == '.')
                    {

                        if (j < SourceCode.Length - 1)
                        {

                            j++;
                            CurrentChar = SourceCode[j];
                            if (CurrentChar == '(' || CurrentChar == ';' || CurrentChar == ',' || CurrentChar == '{' || CurrentChar == '+' || CurrentChar == '-' || CurrentChar == '*' || CurrentChar == '/' || CurrentChar == ')' || CurrentChar == ':')
                            {
                                j--;
                                break;
                            }
                            if (CurrentChar == ' ')
                            {
                                break;
                            }
                            CurrentLexeme += CurrentChar.ToString();
                            Console.WriteLine(CurrentChar);

                        }
                        else
                        {
                            break;
                        }
                    }


                    // (2) Iterate to build the rest of the lexeme while satisfying the
                    // conditions on how the Token Classes should be.
                    // (2.1) Append the CurrentChar to CurrentLexeme.
                    // (2.2) Update the CurrentChar.

                    // (3) Call FindTokenClass on the CurrentLexeme.

                    // (4) Update the outer loop pointer (i) to point on the next lexeme.

                }
                else if (char.IsDigit(CurrentChar))
                {
                    CurrentChar = SourceCode[j];
                    while (char.IsDigit(CurrentChar))
                    {
                        j++;
                        CurrentChar = SourceCode[j];
                        if (CurrentChar == '(' || CurrentChar == ';' || CurrentChar == ',' || CurrentChar == '{' || CurrentChar == '+' || CurrentChar == '-' || CurrentChar == '*' || CurrentChar == '/' || CurrentChar == ')' || CurrentChar == ':')
                        {
                            j--;
                            break;
                        }
                        if (j < SourceCode.Length - 1)
                        {
                            CurrentLexeme += CurrentChar.ToString();
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else if (char.IsSymbol(CurrentChar) || CurrentChar == ':')
                {
                    CurrentChar = SourceCode[j];
                    while (char.IsSymbol(CurrentChar) || CurrentChar == ':')
                    {

                        Console.WriteLine("Hello");
                        if (j < SourceCode.Length - 1)
                        {
                            j++;
                            CurrentChar = SourceCode[j];
                            if (char.IsLetterOrDigit(CurrentChar))
                            {
                                j--;
                                break;
                            }
                            if (CurrentChar != ' ')
                            {
                                CurrentLexeme += CurrentChar.ToString();
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }


                else if (CurrentChar == '/' && SourceCode[j + 1] == '*')
                {
                    int x = 0;
                    while (true)
                    {
                        if (SourceCode[j] == '/' && x != 0)
                        {
                            break;
                        }
                        if (j < SourceCode.Length - 1)
                        {
                            j++;
                            CurrentChar = SourceCode[j];
                            CurrentLexeme += CurrentChar.ToString();
                            x = 1;
                        }
                        else
                        {
                            break;
                        }

                    }

                }
                else if (CurrentChar == '"')
                {
                    int x = 0;
                    while (true)
                    {
                        if (CurrentChar == '"' && x != 0)
                        {
                            break;
                        }
                        if (j < SourceCode.Length - 1)
                        {
                            j++;
                            CurrentChar = SourceCode[j];
                            CurrentLexeme += CurrentChar.ToString();
                            x = 1;
                        }
                        else
                        {
                            break;
                        }

                    }

                }
                else
                {

                }
                FindTokenClass(CurrentLexeme);

                i = j;

            }

            JASON_Compiler.TokenStream = Tokens;
        }

        void FindTokenClass(string Lex)
        {
            Token Tok = new Token();
            Tok.lex = Lex;
            if (Lex == "repeat\r")
            {
                Tok.token_type = Token_Class.repeat;
                Tokens.Add(Tok);
            }
            //Is it a reserved word?
            else if (ReservedWords.ContainsKey(Lex))
            {
                Tok.token_type = ReservedWords[Lex];
                Tokens.Add(Tok);
            }
            else if (isString(Lex))
            {
                Tok.token_type = Token_Class.String;
                Tokens.Add(Tok);
            }
            else if (isComment(Lex))
            {
                Tok.token_type = Token_Class.Comment;
                Tokens.Add(Tok);

            }

            else if (Brackets.ContainsKey(Lex))
            {
                Tok.token_type = Brackets[Lex];
                Tokens.Add(Tok);
            }
            else if (ArithmaticOperators.ContainsKey(Lex))
            {
                Tok.token_type = ArithmaticOperators[Lex];
                Tokens.Add(Tok);
            }
            else if (BooleanOperators.ContainsKey(Lex))
            {
                Tok.token_type = BooleanOperators[Lex];
                Tokens.Add(Tok);
            }
            else if (ConditionOperators.ContainsKey(Lex))
            {
                Tok.token_type = ConditionOperators[Lex];
                Tokens.Add(Tok);
            }

            //Is it an identifier?
            else if (isIdentifier(Lex))
            {
                Tok.token_type = Token_Class.Identifier;
                Tokens.Add(Tok);
            }




            //Is it a Constant?
            else if (isConstant(Lex))
            {
                Tok.token_type = Token_Class.Number;
                Tokens.Add(Tok);
            }


            else
            {
                Errors.Error_List.Add(Lex + " Unrecognized token");
            }

            //Is it an operator?

            //Is it an undefined?

        }

        bool isIdentifier(string lex)
        {
            var rx = new Regex(@"^[_a-zA-Z][_a-zA-Z0-9]*$", RegexOptions.Compiled);
            // Check if the lex is an identifier or not.
            if (rx.IsMatch(lex))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        bool isConstant(string lex)
        {

            var rx = new Regex(@"^[0-9]*(.[0-9]*)?$", RegexOptions.Compiled);
            if (rx.IsMatch(lex))
            {
                return true;
            }
            else
            {
                return false;
            }
            // Check if the lex is a constant (Number) or not.
        }

        bool isString(string lex)
        {
            var rx = new Regex("^\".*?\"$", RegexOptions.Compiled);
            if (rx.IsMatch(lex))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        bool isComment(string lex)
        {
            var rx = new Regex(@"^[/\*].*?[\*/]$", RegexOptions.Compiled);
            if (rx.IsMatch(lex))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
