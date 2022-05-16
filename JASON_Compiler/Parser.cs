using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JASON_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();

        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public Node root;

        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = new Node("Program");
            root.Children.Add(Program());
            return root;
        }
        Node Program()
        {
            Node programN = new Node("Program");
            programN.Children.Add(Program_Dash());
            programN.Children.Add(Main());
            MessageBox.Show("Success");
            return programN;
        }

        Node Program_Dash()
        {
            Node programdashN = new Node("ProgramDash");
            if ((TokenStream[InputPointer].token_type == Token_Class.stringk || TokenStream[InputPointer].token_type == Token_Class.Integer || TokenStream[InputPointer].token_type == Token_Class.Float) 
                && TokenStream[InputPointer + 1].token_type != Token_Class.Main)
            {
                programdashN.Children.Add(Function_Statement());
                programdashN.Children.Add(Program_Dash());
            }
            else
            {
                return null;
            }
            return programdashN;
        }
        Node Function_body()
        {
            Node FunctionBodyN = new Node("Function Body");
            FunctionBodyN.Children.Add(match(Token_Class.LeftBraces));
            FunctionBodyN.Children.Add(Statements());
            FunctionBodyN.Children.Add(return_statement());
            FunctionBodyN.Children.Add(match(Token_Class.RightBraces));
            return FunctionBodyN;
        }
        Node Statements()
        {
            Node StatementsN = new Node("Statements");
            StatementsN.Children.Add(statement());
            StatementsN.Children.Add(statementsdash());
            return StatementsN;
        }
        Node statementsdash()
        {
            Node StatementsdashN = new Node("Statements dash");
            if ((TokenStream[InputPointer].token_type == Token_Class.Identifier && TokenStream[InputPointer + 1].token_type == Token_Class.LeftParentheses)||
                (TokenStream[InputPointer].token_type == Token_Class.stringk || TokenStream[InputPointer].token_type == Token_Class.Integer || TokenStream[InputPointer].token_type == Token_Class.Float)||
                (TokenStream[InputPointer].token_type == Token_Class.write)|| (TokenStream[InputPointer].token_type == Token_Class.read)|| (TokenStream[InputPointer].token_type == Token_Class.repeat)||
                (TokenStream[InputPointer].token_type == Token_Class.Identifier)|| (TokenStream[InputPointer].token_type == Token_Class.Identifier)|| (TokenStream[InputPointer].token_type == Token_Class.ifk))
            {
                StatementsdashN.Children.Add(statement());
                StatementsdashN.Children.Add(statementsdash());
            }
            else
            {
                return null;
            }
            return StatementsdashN;
        }
        Node statement()
        {
            Node statementN = new Node("Statement");
            if(TokenStream[InputPointer].token_type==Token_Class.Identifier&& TokenStream[InputPointer+1].token_type == Token_Class.LeftParentheses)
            {
                statementN.Children.Add(function_call());
            }
            else if(TokenStream[InputPointer].token_type == Token_Class.stringk|| TokenStream[InputPointer].token_type == Token_Class.Integer|| TokenStream[InputPointer].token_type == Token_Class.Float)
            {
                statementN.Children.Add(declaration_statement());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.write)
            {
                statementN.Children.Add(write_statement());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.read)
            {
                statementN.Children.Add(read_statement());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.repeat)
            {
                statementN.Children.Add(repeat_statement());
            }
            else if(TokenStream[InputPointer].token_type == Token_Class.Identifier)
            {
                statementN.Children.Add(assignment_statement());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.ifk)
            {
                statementN.Children.Add(if_statement());
            }
            else
            {
                return null;
            }
            return statementN;
        }
        Node assignment_statement()
        {
            Node assignmentstN = new Node("Assignment Statement");
            assignmentstN.Children.Add(match(Token_Class.Identifier));
            assignmentstN.Children.Add(match(Token_Class.Equal));
            assignmentstN.Children.Add(Expression());
            assignmentstN.Children.Add(match(Token_Class.Semicolon));

            return assignmentstN;
        }

        Node Expression()
        {
            Node expressionN = new Node("Expression");
            if (TokenStream[InputPointer].token_type == Token_Class.String) {
                expressionN.Children.Add(match(Token_Class.String));
            }
            else if ((TokenStream[InputPointer].token_type == Token_Class.Number || TokenStream[InputPointer].token_type == Token_Class.Identifier)
                && (TokenStream[InputPointer+1].token_type == Token_Class.Add || TokenStream[InputPointer+1].token_type == Token_Class.Subtract
                || TokenStream[InputPointer+1].token_type == Token_Class.Divide || TokenStream[InputPointer+1].token_type == Token_Class.Multiply))
            {
                expressionN.Children.Add(Equation());
            }
            else if(TokenStream[InputPointer].token_type == Token_Class.Number|| TokenStream[InputPointer].token_type == Token_Class.Identifier||
               TokenStream[InputPointer].token_type == Token_Class.Identifier&& TokenStream[InputPointer+1].token_type == Token_Class.LeftParentheses)
            {
                expressionN.Children.Add(Term());
            }
            
            return expressionN;
        }

        Node Term()
        {
            Node TermN = new Node("Term");
            if(TokenStream[InputPointer].token_type == Token_Class.Number)
            {
                TermN.Children.Add(match(Token_Class.Number));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Identifier && TokenStream[InputPointer + 1].token_type == Token_Class.LeftParentheses)
            {
                TermN.Children.Add(function_call());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Identifier)
            {
                TermN.Children.Add(match(Token_Class.Identifier));
            }
            
            return TermN;
        }


        Node function_call()
        {
            Node functioncallN = new Node("Function Call");
            functioncallN.Children.Add(match(Token_Class.Identifier));
            functioncallN.Children.Add(match(Token_Class.LeftParentheses));
            functioncallN.Children.Add(parameters());
            functioncallN.Children.Add(match(Token_Class.RightParentheses));

            return functioncallN;
        }

        Node parameters()
        {
            Node parametersN = new Node("parameters");
            if(TokenStream[InputPointer].token_type == Token_Class.stringk || TokenStream[InputPointer].token_type == Token_Class.Integer 
                || TokenStream[InputPointer].token_type == Token_Class.Float || TokenStream[InputPointer].token_type == Token_Class.Identifier)
            {
                parametersN.Children.Add(parameter());
                parametersN.Children.Add(param());
            }
            else
            {
                return null;
            }
            return parametersN;
        }

        Node param()
        {
            Node paramN = new Node("Param");
            if (TokenStream[InputPointer].token_type == Token_Class.comma)
            {
                paramN.Children.Add(match(Token_Class.comma));
                paramN.Children.Add(parameter());
                paramN.Children.Add(param());
            }
            else { return null; }
            return paramN;
        }

        Node parameter()
        {
            Node ParameterN = new Node("Parameter");
            if (TokenStream[InputPointer].token_type == Token_Class.stringk || TokenStream[InputPointer].token_type == Token_Class.Integer
                || TokenStream[InputPointer].token_type == Token_Class.Float)
            {
                ParameterN.Children.Add(Datatype());
                ParameterN.Children.Add(match(Token_Class.Identifier));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Identifier)
            {
                ParameterN.Children.Add(match(Token_Class.Identifier));
            }

            return ParameterN;
        }

        Node Equation()
        {
            Node EquationN = new Node("Equation");
            EquationN.Children.Add(Term());
            EquationN.Children.Add(arithmetic_operator());
            EquationN.Children.Add(Term());
            EquationN.Children.Add(Eq());
            return EquationN;
        }

        Node Eq()
        {
            Node EqN = new Node("Eq");
            if (TokenStream[InputPointer].token_type == Token_Class.Add || TokenStream[InputPointer].token_type == Token_Class.Subtract
                || TokenStream[InputPointer].token_type == Token_Class.Divide || TokenStream[InputPointer].token_type == Token_Class.Multiply)
            {
                EqN.Children.Add(arithmetic_operator());
                EqN.Children.Add(Eq());
            }
            else
            {
                return null;
            }
            return EqN;
        }

        Node arithmetic_operator()
        {
            Node arithmeticopN = new Node("Arithmetic Operator");
            if(TokenStream[InputPointer].token_type == Token_Class.Add)
            {
                arithmeticopN.Children.Add(match(Token_Class.Add));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Subtract)
            {
                arithmeticopN.Children.Add(match(Token_Class.Subtract));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Multiply)
            {
                arithmeticopN.Children.Add(match(Token_Class.Multiply));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Divide)
            {
                arithmeticopN.Children.Add(match(Token_Class.Divide));
            }
            return arithmeticopN;
        }
        Node declaration_statement()
        {
            Node declarationstN = new Node("Declaration Statement");
            declarationstN.Children.Add(Datatype());
            declarationstN.Children.Add(match(Token_Class.Identifier));
            declarationstN.Children.Add(Extension());

            return declarationstN;
        }

        Node Extension()
        {
            Node ExtensionN = new Node("Extension");
            if (TokenStream[InputPointer].token_type == Token_Class.Semicolon)
            {
                ExtensionN.Children.Add(match(Token_Class.Semicolon));
            }
            else
            {
                ExtensionN.Children.Add(extensiondash());
            }

            return ExtensionN;
        }

        Node extensiondash() {
            Node extensiondashN = new Node("Extension Dash");
            if (TokenStream[InputPointer].token_type == Token_Class.Equal)
            {
                extensiondashN.Children.Add(match(Token_Class.Equal));
                extensiondashN.Children.Add(Expression());
                extensiondashN.Children.Add(extensiondash());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.comma)
            {
                extensiondashN.Children.Add(match(Token_Class.comma));
                extensiondashN.Children.Add(match(Token_Class.Identifier));
                extensiondashN.Children.Add(Extension());
            }
            else
            {
                return null;
            }
            return extensiondashN;
        }

        Node write_statement()
        {
            Node wrstN = new Node("Write Statement");
            wrstN.Children.Add(match(Token_Class.write));
            if(TokenStream[InputPointer].token_type == Token_Class.endl)
            {
                wrstN.Children.Add(match(Token_Class.endl));
            }
            else
            {
                wrstN.Children.Add(Expression());
            }
            wrstN.Children.Add(match(Token_Class.Semicolon));
            return wrstN;
        }

        Node read_statement()
        {
            Node rdN = new Node("Read Statement");
            rdN.Children.Add(match(Token_Class.read));
            rdN.Children.Add(match(Token_Class.Identifier));
            rdN.Children.Add(match(Token_Class.Semicolon));
            return rdN;
        }

        Node if_statement()
        {
            Node ifstN = new Node("If Statement");
            ifstN.Children.Add(match(Token_Class.ifk));
            ifstN.Children.Add(condition_statement());
            ifstN.Children.Add(match(Token_Class.then));
            ifstN.Children.Add(Statements());
            ifstN.Children.Add(if_rest());
            return ifstN;
        }

        Node condition_statement()
        {
            Node condStN = new Node("Condition Statement");
            condStN.Children.Add(condition());
            condStN.Children.Add(condition_statementdash());
            return condStN;
        }

        Node condition_statementdash()
        {
            Node cndstdshN = new Node("Condition Statement Dash");
            if (TokenStream[InputPointer].token_type == Token_Class.And || TokenStream[InputPointer].token_type == Token_Class.Or)
            {
                cndstdshN.Children.Add(boolean_operators());
                cndstdshN.Children.Add(condition());
                cndstdshN.Children.Add(condition_statementdash());
            }
            else
            {
                return null;
            }
            return cndstdshN;
        }

        Node condition()
        {
            Node condN = new Node("Condition");
            condN.Children.Add(match(Token_Class.Identifier));
            condN.Children.Add(condition_operator());
            condN.Children.Add(Term());
            return condN;
        }

        Node condition_operator()
        {
            Node conopN = new Node("Condition Operator");
            if (TokenStream[InputPointer].token_type == Token_Class.lessThan)
            {
                conopN.Children.Add(match(Token_Class.lessThan));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.MoreThan)
            {
                conopN.Children.Add(match(Token_Class.MoreThan));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Equal)
            {
                conopN.Children.Add(match(Token_Class.Equal));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.notEqual)
            {
                conopN.Children.Add(match(Token_Class.notEqual));
            }
            return conopN;
        }

        Node boolean_operators()
        {
            Node boolopN = new Node("boolean Operator");
            if (TokenStream[InputPointer].token_type == Token_Class.And)
            {
                boolopN.Children.Add(match(Token_Class.And));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Or)
            {
                boolopN.Children.Add(match(Token_Class.Or));
            }
            return boolopN;
        }
        Node if_rest()
        {
            Node ifrstN = new Node("If Rest");
            if (TokenStream[InputPointer].token_type == Token_Class.end)
            {
                ifrstN.Children.Add(match(Token_Class.end));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.elsek)
            {
                ifrstN.Children.Add(else_statement());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.elseif)
            {
                ifrstN.Children.Add(else_if_statement());
            }
            return ifrstN;
        }

        Node else_if_statement()
        {
            Node elifN = new Node("Else If Statement");
            elifN.Children.Add(match(Token_Class.elseif));
            elifN.Children.Add(condition_statement());
            elifN.Children.Add(match(Token_Class.then));
            elifN.Children.Add(Statements());
            elifN.Children.Add(elseIf());
            return elifN;
        }

        Node elseIf()
        {
            Node elseIfN = new Node("Else If");
            if (TokenStream[InputPointer].token_type == Token_Class.elseif)
            {
                elseIfN.Children.Add(match(Token_Class.elseif));
                elseIfN.Children.Add(condition_statement());
                elseIfN.Children.Add(match(Token_Class.then));
                elseIfN.Children.Add(Statements());
                elseIfN.Children.Add(elseIf());
            }
            else
            {
                return null;
            }
            return elseIfN;
        }

        Node else_statement()
        {
            Node elseN = new Node("Else Statement");
            elseN.Children.Add(match(Token_Class.elsek));
            elseN.Children.Add(Statements());
            elseN.Children.Add(match(Token_Class.end));

            return elseN;
        }
        Node repeat_statement()
        {
            Node repN = new Node("repeat Statement");
            repN.Children.Add(match(Token_Class.repeat));
            repN.Children.Add(Statements());
            repN.Children.Add(match(Token_Class.until));
            repN.Children.Add(condition_statement());
            return repN;
        }

        Node return_statement()
        {
            Node retN = new Node("return Statement");
            if (TokenStream[InputPointer].token_type == Token_Class.returnk)
            {
                retN.Children.Add(match(Token_Class.returnk));
                retN.Children.Add(Expression());
                retN.Children.Add(match(Token_Class.Semicolon));
            }
            else
            {
                return null;
            }
            return retN;
        }

        Node Function_Statement()
        {
            Node fnstN = new Node("Function Statement");
            fnstN.Children.Add(function_declaration());
            fnstN.Children.Add(Function_Body());
            return fnstN;
        }

        Node function_declaration()
        {
            Node fndecN = new Node("Function Declaration");
            fndecN.Children.Add(Datatype());
            fndecN.Children.Add(function_name());
            fndecN.Children.Add(match(Token_Class.LeftParentheses));
            fndecN.Children.Add(parameters());
            fndecN.Children.Add(match(Token_Class.RightParentheses));
            return fndecN;
        }

        Node function_name()
        {
            Node fnnameN = new Node("Function Name");
            fnnameN.Children.Add(match(Token_Class.Identifier));
            return fnnameN;
        }
        Node Main()
        {
            Node mainN = new Node("Main");
            mainN.Children.Add(Datatype());

            mainN.Children.Add(match(Token_Class.Main));
            mainN.Children.Add(match(Token_Class.LeftParentheses));
            mainN.Children.Add(match(Token_Class.RightParentheses));
            mainN.Children.Add(Function_Body());

            return mainN;
        }

        Node Datatype()
        {
            Node datatypeN = new Node("Datatype");
            if (TokenStream[InputPointer].token_type == Token_Class.stringk)
            {
                datatypeN.Children.Add(match(Token_Class.stringk));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Integer)
            {
                datatypeN.Children.Add(match(Token_Class.Integer));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Float)
            {
                datatypeN.Children.Add(match(Token_Class.Float));
            }
            else
            {
                return null;
            }
            return datatypeN;
        }

        Node Function_Body()
        {
            Node functionbodyN = new Node("Function Body");
            functionbodyN.Children.Add(match(Token_Class.LeftBraces));
            functionbodyN.Children.Add(Statements());
            functionbodyN.Children.Add(return_statement());
            functionbodyN.Children.Add(match(Token_Class.RightBraces));

            return functionbodyN;
        }



        // Implement your logic here

        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;

                }

                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + "\r\n");
                InputPointer++;
                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}
