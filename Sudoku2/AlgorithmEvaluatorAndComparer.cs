using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LatexFormatting;
using System.Diagnostics;

namespace Sudoku2
{
    static class AlgorithmEvaluatorAndComparer
    {
        /// <summary>
        /// Evaluates the performance of all the variables on a set of Sudokus.
        /// </summary>
        /// <param name="dir">The directory to pull the Sudokus from</param>
        /// <param name="n">The amount of Sudokus available</param>
        /// <param name="size">The size of the sudokus [9/16]</param>
        /// <param name="ltm">Will be filled with the results</param>
        /// <param name="log">Will be formatted and filled with the results</param>
        public static void EvaluateAndCompare(string dir, int n, int size, bool[] inc, out LatexTabularMaker ltm, out StringBuilder log)
        {
            int numAlgs = 6;                                                                                           // The number of algorithms to evaluate and compare
            foreach (bool b in inc) if (!b) numAlgs--;
            Sudoku[][] sudos = new Sudoku[numAlgs][];                                                                       // We will parse each sudoku numAlgs times, since the Sudokus will be solved in place 
            switch (size)                                                                                                   // and therefore can't be reused
            {
                case (9): for (int i = 0; i < numAlgs; i++) sudos[i] = Parser.Parse9(dir, n); break;
                case (16): for (int i = 0; i < numAlgs; i++) sudos[i] = Parser.Parse16(dir, n); break;
                default: throw new InvalidArgumentException("Invalid size");
            }

            ltm = new LatexTabularMaker(numAlgs * 2 + 1);                                                                                                       // Initiating the header for the LatexTabularMaker
            string[] algs = { "", "cbt", "cbtll", "fc", "fcll", "fcmcv", "fcmcvll" };
            List<string> al = new List<string>();
            al.Add("");
            for (int i = 1; i < algs.Length; i++) if (inc[i - 1]) al.Add(algs[i]);
            int[] columnS = new int[al.Count];
            string[] h2 = new string[2 * al.Count - 1];
            columnS[0] = 1;
            h2[0] = "s\\#";
            for(int i = 1; i < al.Count; i++)
            {
                columnS[i] = 2;
                h2[i * 2 - 1] = "n";
                h2[i * 2] = "t (ms)";
            }

            ltm.AddMultiColumnRow(al.ToArray(), columnS);
            ltm.AddRow(h2, true);

            log = new StringBuilder();                                                                                                                          // Intiating the header for the StringBuilder
            log.AppendLine("\tCBT\t\tCBT-LL\t\tFC\t\tFC-LL\t\tFC-MCV\t\tFC-MCV-LL");
            log.AppendLine("Sudoku\tNodes exp.\tTime(ms)\tNodes exp.\tTime(ms)\tNodes exp.\tTime(ms)\tNodes exp.\tTime(ms)\tNodes exp.\tTime(ms)\tNodes exp.\tTime(ms)");

            long[,] nodes = new long[numAlgs, n];                                                                            // Will contain the number of expanded nodes, such that nodes[a, s] contains the 
                                                                                                                             // expanded nodes for algorithm a and sudoku s
            double[,] times = new double[numAlgs, n];                                                                        // Will contain the time the solving process took, in milliseconds 

            #region Evaluating
            int alg = 0;
            int j = 0;
            if (inc[alg])
            {
                Console.WriteLine("Evaluating CBT...");
                for (int i = 0; i < n; i++)                                                                                      // Iterating through all sudokus
                {
                    Console.Write($"\tSudoku {i}");

                    var start = Process.GetCurrentProcess().TotalProcessorTime;                                                 // By using this expression, we made sure that we only count the time this process
                    ChronologicalBacktrackSolver.Solve(sudos[j][i], out long exp);                                            // uses the CPU, so the results from this method should be independent from PC
                    var stop = Process.GetCurrentProcess().TotalProcessorTime;                                                  // power.
                    double time = (stop - start).TotalMilliseconds;

                    nodes[j, i] = exp;
                    times[j, i] = time;
                    Console.WriteLine($": {exp} nodes exp., {time}ms elapsed.");
                }
                j++;
            }

            #region This works the same
            if (inc[++alg])
            {
                Console.WriteLine("Evaluating CBT-LL...");
                for (int i = 0; i < n; i++)
                {
                    Console.Write($"\tSudoku {i}");

                    var start = Process.GetCurrentProcess().TotalProcessorTime;
                    ChronologicalBacktrackSolver.SolveLL(sudos[j][i], out long exp);
                    var stop = Process.GetCurrentProcess().TotalProcessorTime;
                    double time = (stop - start).TotalMilliseconds;

                    nodes[j, i] = exp;
                    times[j, i] = time;
                    Console.WriteLine($": {exp} nodes exp., {time}ms elapsed.");
                }
                j++;
            }

            if (inc[++alg])
            {
                Console.WriteLine("Evaluating FC...");
                for (int i = 0; i < n; i++)
                {
                    Console.Write($"\tSudoku {i}");

                    var start = Process.GetCurrentProcess().TotalProcessorTime;
                    ForwardCheckingBacktrackSolver.Solve(sudos[j][i], out long exp);
                    var stop = Process.GetCurrentProcess().TotalProcessorTime;
                    double time = (stop - start).TotalMilliseconds;

                    nodes[j, i] = exp;
                    times[j, i] = time;
                    Console.WriteLine($": {exp} nodes exp., {time}ms elapsed.");
                }
                j++;
            }

            if (inc[++alg])
            {
                Console.WriteLine("Evaluating FC-LL...");
                for (int i = 0; i < n; i++)
                {
                    Console.Write($"\tSudoku {i}");

                    var start = Process.GetCurrentProcess().TotalProcessorTime;
                    ForwardCheckingBacktrackSolver.SolveLL(sudos[j][i], out long exp);
                    var stop = Process.GetCurrentProcess().TotalProcessorTime;
                    double time = (stop - start).TotalMilliseconds;

                    nodes[j, i] = exp;
                    times[j, i] = time;
                    Console.WriteLine($": {exp} nodes exp., {time}ms elapsed.");
                }
                j++;
            }


            if (inc[++alg])
            {
                Console.WriteLine("Evaluating FC-MCV...");
                for (int i = 0; i < n; i++)
                {
                    Console.Write($"\tSudoku {i}");

                    var start = Process.GetCurrentProcess().TotalProcessorTime;
                    ForwardCheckingMcvBacktrackSolver.Solve(sudos[j][i], out long exp);
                    var stop = Process.GetCurrentProcess().TotalProcessorTime;
                    double time = (stop - start).TotalMilliseconds;

                    nodes[j, i] = exp;
                    times[j, i] = time;
                    Console.WriteLine($": {exp} nodes exp., {time}ms elapsed.");
                }
                j++;
            }

            if (inc[++alg])
            {
                Console.WriteLine("Evaluating FC-MCV-LL...");
                for (int i = 0; i < n; i++)
                {
                    Console.Write($"\tSudoku {i}");

                    var start = Process.GetCurrentProcess().TotalProcessorTime;
                    ForwardCheckingMcvBacktrackSolver.SolveLL(sudos[j][i], out long exp);
                    var stop = Process.GetCurrentProcess().TotalProcessorTime;
                    double time = (stop - start).TotalMilliseconds;

                    nodes[j, i] = exp;
                    times[j, i] = time;
                    Console.WriteLine($": {exp} nodes exp., {time}ms elapsed.");
                }
                j++;
            }
            #endregion
            #endregion

            for (int s = 0; s < n; s++)                                                                                      // Formatting the results, again iterating through sudokus (or rows)
            {
                string[] entries = new string[numAlgs * 2 + 1];
                entries[0] = s.ToString();
                log.Append($"{s.ToString()}");
                int e = 1;
                for(int i = 0; i < numAlgs; i++)
                {
                    double time = times[i, s];
                    entries[e++] = nodes[i, s].ToString();
                    entries[e++] = (time > 0) ? time.ToString() : $"< 15.625";                                              // If the time is equal to 0, this means that the algorithm was too fast for the timer
                                                                                                                            // to detect a time difference. In this case the time is lower than 15.625ms which is the
                                                                                                                            // resolution of the timer
                    log.Append($"\t{nodes[i, s].ToString()}");
                    log.Append($"\t{((time > 0) ? time.ToString() : $" < 15.625")}");
                }
                log.Append("\n");
                ltm.AddRow(entries);
            }
        }
    }
}
