using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;


namespace PropLogicSolver
{
    /// <summary>
    /// Encapsulates a Sentential Logic expression, to be compiled to a .net lambda
    /// </summary>
    public class TruthExpression
    {
        /// <summary>
        /// The interal expression tree
        /// </summary>
        public LambdaExpression InternalExpression { get; set; }

        /// <summary>
        /// The last parsed molecular sentence in post-fix form
        /// </summary>
        public List<Token> Tokens { get; set; }

        public string OriginalExpression { get; } 

        private const int MaxVariables = 16;

        public TruthExpression()
        {
        }

        /// <summary>
        /// Creates a truth expression from the passed string
        /// </summary>
        /// <exception cref="InvalidTruthExpressionException"></exception>
        /// <param name="expr">A well formed sentential logic expression</param>
        public TruthExpression(string expr)
        {
            OriginalExpression = expr;
            Construct(expr);
        }

        /// <summary>
        /// Constructs an Expression tree from the given string
        /// </summary>
        /// <exception cref="InvalidTruthExpressionException"></exception>
        /// <param name="strExpr"></param>
        public void Construct(string strExpr)
        {
            Tokens = Tokenize(strExpr);
            InternalExpression = Parse(Tokens);
            var numberOfParams = (byte) InternalExpression.Parameters.Count;
            if(numberOfParams > MaxVariables) throw new InvalidTruthExpressionException($"More than {MaxVariables} atomic sentences are not supported at this time.");
        }
        
        /// <summary>
        /// Parses the tokens passed into an expression tree
        /// </summary>
        /// <param name="tokens">tokens in post-fix form</param>
        private static LambdaExpression Parse(IReadOnlyCollection<Token> tokens)
        {
            var pe = new List<ParameterExpression>();

            //add parameters
            foreach (var token in tokens) 
            {
                if (token.TokenType == SLToken.AtomicSentence && pe.All(per => per.Name != token.RawToken.ToString()))
                {
                    pe.Add(Expression.Parameter(typeof(bool), token.RawToken.ToString()));
                }
            }

            var stack = new Stack<Expression>();

            try
            {
                //construct InternalExpression tree
                foreach (var token in tokens)
                {
                    //push atomic sentences to stack
                    if (!IsOperator(token)) stack.Push(pe.Single(p => p.Name == token.RawToken.ToString()));

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
                            SLToken.And => (Expression)Expression.AndAlso(left, right),
                            SLToken.Or => Expression.OrElse(left, right),
                            //NOT done already
                            SLToken.Xor => Expression.ExclusiveOr(left, right),
                            SLToken.Conditional => Expression.OrElse(Expression.Not(left), right),
                            SLToken.Biconditional => Expression.Not(Expression.ExclusiveOr(left, right)),
                            _ => throw new InvalidTruthExpressionException($"got {token.TokenType}, expected a binary operator.")
                        });

                    }

                }
            }
            catch (InvalidOperationException)
            {
                throw new InvalidTruthExpressionException("An operator is missing an operand");
            }

            var body = (Expression) stack.Pop();
            
            return Expression.Lambda(body, pe.ToArray());
        }

        /// <summary>
        /// Tokenize the passed string into post-fix form
        /// </summary>
        /// <param name="strExpr"></param>
        /// <returns></returns>
        private static List<Token> Tokenize(string strExpr)
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
                    case '.':
                    case '•':
                        tokens.Add(new Token{TokenType = SLToken.And});
                        break; 
                    
                    case '|': case 'v':
                        tokens.Add(new Token{TokenType = SLToken.Or});
                        break;

                    case '!': case '~':
                    case '¬':
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

                    case char a when !char.IsWhiteSpace(a):
                        tokens.Add(new Token{TokenType = SLToken.AtomicSentence, RawToken = a});
                        break;

                    //Ignore whitespaces
                    case char a when char.IsWhiteSpace(a):
                        break;

                    default: 
                        throw new InvalidTruthExpressionException($"Invalid symbol: {c} in {strExpr}");

                }

            }

           
            var postFixTokens = new List<Token>();
            var stack = new Stack<Token>();

            tokens.Add(new Token{TokenType = SLToken.RParen});
            stack.Push(new Token{TokenType = SLToken.LParen});

            //convert to post-fix form
            foreach (var token in tokens)
            {
                if (IsOperator(token))
                {
                    while (stack.Count > 0 && IsOperator(stack.Peek()))
                    {

                        if (Precedence(token) <= Precedence(stack.Peek()))
                        {
                            postFixTokens.Add(stack.Pop());
                        }

                    }

                    stack.Push(token);
                }

                else if (token.TokenType == SLToken.RParen)
                {
                    while (stack.Count > 0 && stack.Peek().TokenType != SLToken.LParen)
                    {
                        postFixTokens.Add(stack.Pop());
                    }

                    if (stack.Count > 0 && stack.Peek().TokenType != SLToken.LParen)
                    {
                        throw new InvalidTruthExpressionException("Too many right parentheses"); 
                    }

                    stack.Pop(); //remove lparen  
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
        private static int Precedence(Token Operator)
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

        private static bool IsOperator(Token token)
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

        public Delegate Compile() => InternalExpression.Compile();

    }
}
