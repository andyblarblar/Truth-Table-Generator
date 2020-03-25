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
        public void TablesParseToString()
        {
            var table = new TruthTableSolver(new TruthExpression("(A v B)"));
            Console.WriteLine(table.SolveToString());

        }

        [Test]
        public void TestBoolGenorator()
        {
        }
    }
}