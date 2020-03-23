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
            var table = new TruthTableSolver(new TruthExpression("A > B"));
            Console.WriteLine(table.SolveToString());

        }

        [Test]
        public void TestBoolGenorator()
        {
            //var lst = new List<IEnumerable<bool>>();
            //lst.Add(TruthTableSolver.GenerateBool(0, 4, 2));
            //lst.Add(TruthTableSolver.GenerateBool(1, 4, 2));
            var seq1 = TruthTableSolver.GenerateBool(0, 4, 2);
            var seq2 = TruthTableSolver.GenerateBool(1, 4, 2);

            //            for (int i = 0; i < 4; i++)
            //            {
            //              //  lst.ForEach(enu => Console.WriteLine(enu.GetEnumerator().MoveNext()));
            //              Console.WriteLine(seq1.GetEnumerator().MoveNext());
            //
            //            } 

            var enumerable = seq1.GetEnumerator();
            enumerable.MoveNext();
            Console.WriteLine(enumerable.Current);
            enumerable.MoveNext();
            Console.WriteLine(enumerable.Current);
            enumerable.MoveNext();
            Console.WriteLine(enumerable.Current);
            enumerable.MoveNext();
            Console.WriteLine(enumerable.Current);
            enumerable.Dispose();

        }
    }
}