using System;
using System.Collections.Generic;
using System.Text;

namespace PropLogicSolver
{
    class InvalidTruthExpressionException : Exception
    {
        public InvalidTruthExpressionException(string message) : base(message)
        {

        }

    }
}
