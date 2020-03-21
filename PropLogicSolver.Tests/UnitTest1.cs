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
        public void Test1()
        {
            var prop = new TruthExpression();

            prop.Construct("~A");

            Console.WriteLine(prop.InternalExpression);
            Console.WriteLine(prop.InternalExpression.Parameters[0].Name);
            Console.WriteLine(prop.InternalExpression.Body);
            
            var del = prop.Compile();

            var parameters = del.Method.GetParameters();

            var inP = new List<bool>();

            parameters.ToList().ForEach(p => inP.Add(true));

            var resBool = del switch
            {
                Func<bool, bool> func => del.DynamicInvoke(inP[0]),
                Func<bool, bool, bool> func => del.DynamicInvoke(inP[0], inP[1]),
                Func<bool, bool, bool, bool> func => del.DynamicInvoke(inP[0], inP[1], inP[2])

            };

            Console.WriteLine((bool) resBool);
            Assert.Pass();
        }
    }
}