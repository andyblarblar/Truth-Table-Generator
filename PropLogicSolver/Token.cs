using System;
using System.Collections.Generic;
using System.Text;

namespace PropLogicSolver
{
    public enum SLToken
    {
        LParen,
        RParen,
        And,
        Or,
        Xor,
        Biconditional,
        Conditional,
        Not,
        AtomicSentence,
    }

    public struct Token
    {
        public SLToken TokenType { get; set; }

        public char RawToken { get; set; }

    }

}
