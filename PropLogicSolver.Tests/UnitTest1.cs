using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using PropLogicSolver;

namespace PropLogicSolver.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestTable()
        {
            var table = new TruthTableSolver(new TruthExpression("(A ^ B) v C v D v E v F v G"));
            Console.WriteLine(table.SolveToString());

        }

    }
}