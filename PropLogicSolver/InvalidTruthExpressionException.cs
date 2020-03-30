using System;
using System.Collections.Generic;
using System.Text;

namespace PropLogicSolver
{
    public class InvalidTruthExpressionException : Exception
    {
        public InvalidTruthExpressionException(string message) : base(message)
        {

        }

    }
}
