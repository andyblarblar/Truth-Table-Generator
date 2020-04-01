using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace PropLogicSolver
{
    public class TruthTableSolver
    {
        private Delegate TruthExpression { get; set; }

        private TruthExpression sourceExpression { get; }
        public char[] VarNames { get; }

        /// <summary>
        /// Creates truthtable by compiling the passed truthexpression
        /// </summary>
        /// <exception cref="InvalidTruthExpressionException">thrown for errors in expression compilation</exception>
        /// <param name="truthExpression"></param>
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
        private bool SolveSingleCase(IReadOnlyList<bool> args)
        {
            if(args.Count != VarNames.Length) throw new IndexOutOfRangeException("The passed args array is not the same size as the number of args needed.");

            var res = sourceExpression.InternalExpression.Parameters.Count switch 
            {
                1 => TruthExpression.DynamicInvoke(args[0]),
                2 => TruthExpression.DynamicInvoke(args[0], args[1]),
                3 => TruthExpression.DynamicInvoke(args[0], args[1], args[2]),
                4 => TruthExpression.DynamicInvoke(args[0], args[1], args[2], args[3]),
                5 => TruthExpression.DynamicInvoke(args[0], args[1], args[2], args[3], args[4]),
                6 => TruthExpression.DynamicInvoke(args[0], args[1], args[2], args[3], args[4], args[5]),
                7 => TruthExpression.DynamicInvoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6]),
                8 => TruthExpression.DynamicInvoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]),
                9 => TruthExpression.DynamicInvoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7],args[8]),
                10 => TruthExpression.DynamicInvoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7],args[8],args[9]),
                11 => TruthExpression.DynamicInvoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7],args[8],args[9], args[10]),
                12 => TruthExpression.DynamicInvoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7],args[8],args[9], args[10], args[11]),
                13 => TruthExpression.DynamicInvoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7],args[8],args[9], args[10], args[11], args[12]),
                14 => TruthExpression.DynamicInvoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7],args[8],args[9], args[10], args[11], args[12], args[13]),
                15 => TruthExpression.DynamicInvoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7],args[8],args[9], args[10], args[11], args[12], args[13], args[14]),
                16 => TruthExpression.DynamicInvoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7],args[8],args[9], args[10], args[11], args[12], args[13], args[14], args[15])
            };

            return (bool) res;

        }

        /// <summary>
        /// Solves the truthtable and formats to a string.
        /// </summary>
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
                .AppendLine($"| {sourceExpression.OriginalExpression}")
                .AppendLine("----------------------------------------------------------------------------");

            return GenTableWithStringBuilder(builder, args, totalLines).ToString();
        }

        private StringBuilder GenTableWithStringBuilder(StringBuilder stringBuilder, IEnumerable<IEnumerable<bool>> vars, int totalLines)
        {
            //Extract the underlying enumerators for each IEnumerable to prevent multiple enumeration
            var enums = vars.Select(enumerable => enumerable.GetEnumerator()).ToList();

            for (var i = 0; i < totalLines; i++)
            {
                //Add line number
                stringBuilder.Append(i + 1 + " ");

                //Add states to left
                foreach (var enu in enums)
                {
                    enu.MoveNext();
                    stringBuilder.Append($"{enu.Current.ToString().ToUpper()[0]} ");
                }

                //Compute and add result to right
                stringBuilder.Append("| ")
                    .Append(SolveSingleCase(enums.Aggregate(new List<bool>(),
                            (list, enumerator) =>
                            {
                                list.Add(enumerator.Current);
                                return list;
                            })
                    ))
                    .AppendLine();

            }

            return stringBuilder;
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
                //for the far right variable, swap each line.
                if (varIndex == totalVarNum - 1)
                {
                    yield return cycleState;
                    cycleState = !cycleState;
                }
                //for every other variable, cycle 
                else
                {
                    var temp = (float)i / numOfSameState;

                    //start a cycle of the other state every N lines. Cast to float to avoid int flooring
                    if (FloatIsInt(temp) && i != 0) cycleState = !cycleState;

                    yield return cycleState;
                }

            }
            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool FloatIsInt(float x) => Math.Abs(x - (int) x) < float.Epsilon;

    }
}
