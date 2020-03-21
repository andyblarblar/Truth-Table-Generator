using System;
using System.Collections.Generic;
using System.Text;

namespace PropLogicSolver
{
    class TruthTableSolver
    {
        private Delegate TruthExpression { get; set; }

        public char[] VarNames { get; }

        public TruthTableSolver(Delegate truthExpression)
        {
            TruthExpression = truthExpression;
            VarNames = new char[truthExpression.Method.GetParameters().Length];

            //Extract variable names
            for (var i = 0; i < truthExpression.Method.GetParameters().Length; i++)
            {
                VarNames[i] = truthExpression.Method.GetParameters()[i].Name[0];
            }

        }

        /// <summary>
        /// Solves the truth expression given the passed states. 
        /// </summary>
        /// <param name="args">A bool array that represents the state of all variables. It must be the exact size of all args.</param>
        /// <returns></returns>
        public bool SolveSingleCase(bool[] args)
        {
            if(args.Length != TruthExpression.Method.GetParameters().Length) throw new IndexOutOfRangeException("The passed args array is not the same size as the number of args needed.");

            var res = TruthExpression switch
            {
                Func<bool,bool> func => TruthExpression.DynamicInvoke(args[0]),
                Func<bool,bool,bool> func => TruthExpression.DynamicInvoke(args[0], args[1]),
                Func<bool,bool,bool,bool> func => TruthExpression.DynamicInvoke(args[0], args[1], args[2]),
                Func<bool,bool,bool,bool,bool> func => TruthExpression.DynamicInvoke(args[0], args[1], args[2], args[3]),
                Func<bool,bool,bool,bool,bool,bool> func => TruthExpression.DynamicInvoke(args[0], args[1], args[2], args[3], args[4]),
                Func<bool,bool,bool,bool,bool,bool,bool> func => TruthExpression.DynamicInvoke(args[0], args[1], args[2], args[3], args[4], args[5]),
                Func<bool,bool,bool,bool,bool,bool,bool,bool> func => TruthExpression.DynamicInvoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6]),
                Func<bool,bool,bool,bool,bool,bool,bool,bool,bool> func => TruthExpression.DynamicInvoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]),
                Func<bool,bool,bool,bool,bool,bool,bool,bool,bool,bool> func => TruthExpression.DynamicInvoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7],args[8]),
                Func<bool,bool,bool,bool,bool,bool,bool,bool,bool,bool,bool> func => TruthExpression.DynamicInvoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7],args[8],args[9]),
                Func<bool,bool,bool,bool,bool,bool,bool,bool,bool,bool,bool,bool> func => TruthExpression.DynamicInvoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7],args[8],args[9], args[10]),
                Func<bool,bool,bool,bool,bool,bool,bool,bool,bool,bool,bool,bool,bool> func => TruthExpression.DynamicInvoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7],args[8],args[9], args[10], args[11]),
                Func<bool,bool,bool,bool,bool,bool,bool,bool,bool,bool,bool,bool,bool,bool> func => TruthExpression.DynamicInvoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7],args[8],args[9], args[10], args[11], args[12]),
                Func<bool,bool,bool,bool,bool,bool,bool,bool,bool,bool,bool,bool,bool,bool,bool> func => TruthExpression.DynamicInvoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7],args[8],args[9], args[10], args[11], args[12], args[13]),
                Func<bool,bool,bool,bool,bool,bool,bool,bool,bool,bool,bool,bool,bool,bool,bool,bool> func => TruthExpression.DynamicInvoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7],args[8],args[9], args[10], args[11], args[12], args[13], args[14]),
                Func<bool,bool,bool,bool,bool,bool,bool,bool,bool,bool,bool,bool,bool,bool,bool,bool,bool> func => TruthExpression.DynamicInvoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7],args[8],args[9], args[10], args[11], args[12], args[13], args[14], args[15]),

            };

            return (bool) res;

        }

        public string Solve()
        {
            //contains all truth states to solve
            var args = new List<List<bool>>();

            var totalLines = 1 << VarNames.Length;

            //Add a list of states for each var
            foreach (var _ in VarNames)
            {
                args.Add(new List<bool>());
            }

            //add generators for each var
            for (var i = 0; i < args.Count; i++)
            {
                var i1 = i;
                args.ForEach(list => list.AddRange(GenerateBool(i1,totalLines,VarNames.Length)));
            }

            var builder = new StringBuilder();

            builder.Append(stackalloc[] {' ', ' '});

            //Add vars to left
            foreach (var varName in VarNames)
            {
                builder.Append(varName + ' ');
            }

            builder.Append($" | {TruthExpression.Method}");//can change to better represent user input if needed
            
            //TODO create Line genorator and compleate solve method.



        }

        private void AddTableEntry(ref StringBuilder stringBuilder)
        {


        }

        /// <summary>
        /// Iterates through all values for a variable given its position
        /// </summary>
        /// <param name="varIndex">the placement of the variable. Ie, far left is 0</param>
        /// <param name="totalLines">the total lines in the table</param>
        /// <param name="totalVarNum">the total number of variables</param>
        /// <returns></returns>
        private static IEnumerable<bool> GenerateBool(int varIndex, int totalLines, int totalVarNum)
        {
            //the number of lines before a true changes to a false in the cycle
            var numOfSameState = 1 << (totalVarNum - 1 - varIndex);

            var cycleState = true;

            for (var i = 0; i < totalLines; i++)
            {
                //for the first variable, swap each line.
                if (varIndex == totalVarNum)
                {
                    yield return i % totalLines != 0;
                }
                //for every other variable, cycle 
                else
                {
                    //start a cycle of the other state every N lines
                    if (i / numOfSameState == 1) cycleState = !cycleState;

                    yield return cycleState;
                }

            }
            
        }




    }
}
