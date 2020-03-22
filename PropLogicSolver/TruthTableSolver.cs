using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PropLogicSolver
{
    public class TruthTableSolver
    {
        private Delegate TruthExpression { get; set; }

        private TruthExpression sourceExpression { get; }
        public char[] VarNames { get; }

        public TruthTableSolver(TruthExpression truthExpression)
        {
            sourceExpression = truthExpression;
            TruthExpression = truthExpression.Compile();
            VarNames = new char[truthExpression.InternalExpression.Parameters.Count];

            //Extract variable names
            for (var i = 0; i < VarNames.Length; i++)
            {
                VarNames[i] = truthExpression.InternalExpression.Parameters[i].Name[0];
            }

        }

        /// <summary>
        /// Solves the truth expression given the passed states. 
        /// </summary>
        /// <param name="args">A bool array that represents the state of all variables. It must be the exact size of all args.</param>
        /// <returns></returns>
        private bool SolveSingleCase(bool[] args)
        {
            if(args.Length != VarNames.Length) throw new IndexOutOfRangeException("The passed args array is not the same size as the number of args needed.");

            var res = TruthExpression switch //TODO change to a switch on param count
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

        public string SolveToString()
        {
            //contains all truth states to solve
            var args = new List<IEnumerable<bool>>();

            var totalLines = 1 << VarNames.Length;

            //Add a list of states for each var
            for (var i = 0; i < VarNames.Length; i++)
            {
                var i1 = i;
                args.Add(GenerateBool(i1, totalLines, VarNames.Length));
            }

            var builder = new StringBuilder();

            builder.Append("  ");

            //Add vars to left
            foreach (var varName in VarNames)
            {
                builder.Append(varName + " ");
            }

            builder
                .AppendLine($"| {sourceExpression.InternalExpression.Body}")
                .AppendLine("----------------------------------------------------------------------------");

            GenTableWithStringBuilder(ref builder, ref args, totalLines);

            return builder.ToString();
        }

        private void GenTableWithStringBuilder(ref StringBuilder stringBuilder, ref List<IEnumerable<bool>> vars, int totalLines)
        {
            for (var i = 0; i < totalLines; i++)
            {
                //Add line number
                stringBuilder.Append(++i + " ");
                
                //Add states
                foreach (var var in vars)
                {
                    stringBuilder.Append($"{var.GetEnumerator().Current.ToString().ToUpper()[0]} ");
                }

                //Super fancy way of getting all of the values of the vars for input into the solver function, then printing
                stringBuilder.Append("| ")
                    .Append(SolveSingleCase(vars.Aggregate(new List<bool>(),
                            (bools, enumerable) =>
                            {
                                bools.Add(enumerable.GetEnumerator().Current);
                                return bools;
                            })
                        .ToArray()))
                    .AppendLine();

                //Advance to next line states
                foreach (var var in vars)
                { 
                    var.GetEnumerator().MoveNext();
                }

            }

        }

        /// <summary>
        /// Creates an IEnumerable that contains all of the true false values for a variable on the left side of the table.
        /// </summary>
        /// <param name="varIndex">the placement of the variable. Ie, far left is 0</param>
        /// <param name="totalLines">the total lines in the table</param>
        /// <param name="totalVarNum">the total number of variables</param>
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

        /// <summary>
        /// Iterates through the range of start to end, inclusive. No clue why this inst in the std library.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private static IEnumerable<int> Range(int start, int end)
        {
            for (var i = start; i < end; i++)
            {
                yield return i++;
            }
            
        }



    }
}
