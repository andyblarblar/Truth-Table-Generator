using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.VisualBasic.CompilerServices;

namespace PropLogicSolver
{
    public class TruthExpression
    {
        public Expression<Func<bool[], bool>> InternalExpression { get; set; }

        public byte NumberOfParams { get; set; }

        /// <summary>
        /// Constructs an Expression tree from the given string
        /// </summary>
        /// <param name="strExpr"></param>
        public void Construct(string strExpr)
        {
            var tokens = Tokenize(strExpr);
            InternalExpression = Parse(tokens);
            NumberOfParams = (byte) InternalExpression.Parameters.Count;
        }

        /// <summary>
        /// Parses the tokens passed into an expression tree
        /// </summary>
        /// <param name="tokens">tokens in post-fix form</param>
        /// <returns></returns>
        public static Expression<Func<bool[],bool>> Parse(List<Token> tokens)
        {
            var pe = new List<ParameterExpression>();

            //add parameters
            foreach (var token in tokens) 
            {
                if (token.TokenType == SLToken.AtomicSentence)
                {
                    pe.Add(Expression.Parameter(typeof(bool), token.RawToken.ToString()));
                }
            }

            var stack = new Stack<Expression>();

            //construct InternalExpression tree
            foreach (var token in tokens)
            {
                //push atomic sentences to stack
                if(!IsOperator(token)) stack.Push(Expression.Variable(typeof(bool),token.RawToken.ToString()));

                else
                {
                    //NOT is unary, so we only pop once
                    if (token.TokenType == SLToken.Not)
                    {
                        stack.Push(Expression.Not(stack.Pop()));
                        continue;
                    }

                    var right = stack.Pop();//right node
                    var left = stack.Pop();//left node

                    //handle binary operators by popping the stack twice to get the right and left nodes
                    stack.Push(token.TokenType switch
                    {
                        SLToken.And =>(Expression) Expression.AndAlso(left, right),
                        SLToken.Or => Expression.OrElse(left, right),
                        //NOT done already
                        SLToken.Xor => Expression.ExclusiveOr(left,right),
                        SLToken.Conditional => Expression.OrElse(Expression.Not(left), right),
                        SLToken.Biconditional => Expression.Not(Expression.ExclusiveOr(left,right)),
                        _ => throw new Exception($"got {token.TokenType}, expected a binary operator.")
                    });

                }

            }

            var body = (Expression<Func<bool[], bool>>) stack.Pop();
            
            return Expression.Lambda<Func<bool[], bool>>(body, pe.ToArray());
        }

        /// <summary>
        /// Tokenize the passed string into post-fix form
        /// </summary>
        /// <param name="strExpr"></param>
        /// <returns></returns>
        public static List<Token> Tokenize(string strExpr)
        {
            var tokens = new List<Token>();

            foreach (var c in strExpr)
            {
                switch (c)
                {
                    case '(': case '{': case '[':
                        tokens.Add(new Token {TokenType = SLToken.LParen});
                        break;
                    
                    case ')': case '}': case ']':
                        tokens.Add(new Token {TokenType = SLToken.RParen});
                        break;

                    case '^': case '&':
                        tokens.Add(new Token{TokenType = SLToken.And});
                        break; 
                    
                    case '|': case 'v':
                        tokens.Add(new Token{TokenType = SLToken.Or});
                        break;

                    case '!': case '~':
                        tokens.Add(new Token { TokenType = SLToken.Not });
                        break;
                    
                    case '⊕':
                        tokens.Add(new Token { TokenType = SLToken.Xor });
                        break;

                    case '>': case '→': case '⊃':
                        tokens.Add(new Token{TokenType = SLToken.Conditional});
                        break;

                    case '↔':
                        tokens.Add(new Token{TokenType = SLToken.Biconditional});
                        break;

                    case char a:
                        tokens.Add(new Token{TokenType = SLToken.AtomicSentence, RawToken = a});
                        break;

                    default: 
                        throw new ArgumentException($"Invalid symbol: {c} in {strExpr}");

                }

            }

           
            var postFixTokens = new List<Token>();
            var stack = new Stack<Token>();
            var stackTemp = new List<Token>();

            tokens.Add(new Token{TokenType = SLToken.RParen});
            stack.Push(new Token{TokenType = SLToken.LParen});

            //convert to post-fix form
            foreach (var token in tokens)
            {
                if (IsOperator(token))
                {
                    stackTemp.Clear();

                    while (IsOperator(stack.Peek()))
                    {
                        var next = stack.Pop();

                        if (Precedence(stack.Peek()) <= Precedence(token))
                        {
                            postFixTokens.Add(next);
                            continue;
                        }

                        stackTemp.Add(next);
                    }

                    stackTemp.ForEach(item => stack.Push(item));//rebuild stack
                }

                else if (token.TokenType == SLToken.RParen)
                {
                    while (stack.Peek().TokenType != SLToken.LParen)
                    {
                        postFixTokens.Add(stack.Pop());
                    }

                    stack.Pop();//remove lparen
                }

                else if(token.TokenType == SLToken.LParen)
                {
                    stack.Push(token);    
                }

                else//is operand
                {
                    postFixTokens.Add(token);
                }

            }

            return postFixTokens;

        }

        /// <summary>
        /// returns the precedence of the operator, where a higher number has higher precedence
        /// </summary>
        /// <param name="Operator"></param>
        /// <returns></returns>
        public static int Precedence(Token Operator)
        {
            return Operator.TokenType switch
            {
                SLToken.Not => 5,
                SLToken.And => 4,
                SLToken.Or => 3,
                SLToken.Xor => 3,
                SLToken.Conditional => 2,
                SLToken.Biconditional => 1,
            };

        }

        public static bool IsOperator(Token token)
        {
            return token switch
            {
                { TokenType: SLToken.Xor} => true,
                { TokenType: SLToken.Not} => true,
                { TokenType: SLToken.And} => true,
                { TokenType: SLToken.Or} => true,
                { TokenType: SLToken.Conditional} => true,
                { TokenType: SLToken.Biconditional} => true,
                _ => false,
            };

        }

        public Func<bool[], bool> Compile() => InternalExpression.Compile();

    }
}
