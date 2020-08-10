using System.Collections.Generic;
using System.Text;

namespace Sudoku2
{
    /// <summary>
    /// Implements the chronological backtracking algorithm to solve sudokus.
    /// </summary>
    static class ChronologicalBacktrackSolver
    {
        #region Solve
        /// <summary>
        /// Solves a sudoku with our standard implementation of Chronological Backtracking.
        /// </summary>
        /// <param name="sudo">The sudoku to solve</param>
        /// <param name="expanded">Keeps track of the amount of iterations/node expansions of the algorithm</param>
        public static void Solve(Sudoku sudo, out long expanded)
        {
            int varCount = sudo.VarCount;
            List<Point> variables = sudo.GetVariables();            // Get the variables of the sudoku
            List<int> varTrials = new List<int>(new int[varCount]); // Initialize varTrials list with varCount zeros

            int i = 0;                      // Used to determine which variable we need to pick
            bool backtrack = false;         // Used to determine if we need to do backtracking
            expanded = 0;                   // Initialize node expansions variable
            while (i >= 0 && i < varCount)  // If i is out of range of the variables we know we that we a solution of failure
            {
                Point v = variables[i];     // Coordinates of tile we are going to check

                if (backtrack)              // First handle backtracking because of the hashsets
                {
                    sudo.SetTile(v, 0);     // Reset value of tile (thus removing the value from the hashsets)
                    backtrack = false;      // Disable backtrack
                }
                else
                {
                    List<int> domain = sudo.CalculateDomainList(v);         // Get the domain of the tile
                    if (domain.Count > 0 && varTrials[i] < domain.Count)    // Check if the domain is empty, or if we have checked all values
                    {
                        sudo.SetTile(v, domain[varTrials[i]]);              // Set the value of the tile to the varTrial-th element that we are checking from the domain of this tile
                        varTrials[i]++;                                     // Upp the varTrial to ensure we get the next value of the domain next time
                        i++;                                                // Up i so we take the next variable next time
                    }
                    else                                                    // We can't set the tile to a value, so we need to backtrack
                    {
                        varTrials[i] = 0;                                   // Reset the varTrial
                        backtrack = true;                                   // Enable backtrack
                        i--;                                                // Make sure we get the previous variable next time
                    }
                    expanded++;                                             // Another node expanded
                }
            }
        }

        /// <summary>
        /// Solves a sudoku with our extended implementation of Chronological Backtracking.
        /// </summary>
        /// <param name="sudo">The sudoku to solve</param>
        /// <param name="expanded">Keeps track of the amount of iterations/node expansions of the algorithm</param>
        public static void SolveLL(Sudoku sudo, out long expanded)
        {
            int varCount = sudo.VarCount;
            List<Point> variables = sudo.GetVariables();            // Get the variables of the sudoku
            List<int> varTrials = new List<int>(new int[varCount]); // Initialize varTrials list with varCount zeros

            int i = 0;                      // Used to determine which variable we need to pick
            bool backtrack = false;         // Used to determine if we need to do backtracking
            expanded = 0;                   // Initialize node expansions variable
            while (i >= 0 && i < varCount)  // If i is out of range of the variables we know we that we a solution of failure
            {
                Point v = variables[i];     // Coordinates of tile we are going to check

                if (backtrack)
                {
                    sudo.ResetAndUpdate(v); // Reset the value of tile
                    backtrack = false;      // Disable backtrack
                }
                else
                {
                    Sudoku.DomainHashSet domain = sudo.GetDomain(v);        // Get the domain of the tile
                    if (domain.Count > 0 && varTrials[i] < domain.Count)    // Check if 
                    {
                        int t = 0;                                          // Enumerator for the hashset
                        for (int n = 1; n <= sudo.N; n++)                   // Check numbers 1-N
                        {
                            if (domain.Contains(n))                         // First check if the domain contains the value 
                            {
                                if (t >= varTrials[i])                      // Only set the value if we get the varTrial-th value from the domain
                                {
                                    sudo.SetAndUpdate(v, n);                // Set the value of the tile
                                    varTrials[i]++;                         // Up the varTrial to ensure we get the next value of the domain next time
                                    i++;                                    // Up i so we take the next variable next time
                                    break;                                  // If we set a value we don't need to check the next values
                                }
                                t++;                                        // Up the enumerator only when the value is in the domain
                            }
                        }
                    }
                    else                    // We can't set the tile to a value, so we need to backtrack 
                    {
                        varTrials[i] = 0;   // Reset the varTrial
                        backtrack = true;   // Enable backtrack
                        i--;                // Make sure we get the previous variable next time
                    }
                    expanded++;             // Another node expanded
                }
            }
        }
        #endregion

        #region SolveTrace
        /// <summary>
        /// Solves a sudoku with our standard implementation of Chronological Backtracking.
        /// Differs from Solve() by showing a the visualizer that gives a good impression of the solving process.
        /// </summary>
        /// <param name="sudo">The sudoku to solve</param>
        /// <param name="expanded">Keeps track of the amount of iterations/node expansions of the algorithm</param>
        public static void SolveTrace(Sudoku sudo, out long expanded) // For full functionality of the code see Solve()
        {
            sudo.StartVisualizer();     // Start the visualizer
            int varCount = sudo.VarCount;
            List<Point> variables = sudo.GetVariables();
            List<int> varTrials = new List<int>(new int[varCount]);

            int i = 0;
            bool backtrack = false;
            expanded = 0;
            while (i >= 0 && i < varCount)
            {
                Point v = variables[i];

                if (backtrack)
                {
                    sudo.SetTile(v, 0);
                    backtrack = false;
                }

                List<int> domain = sudo.CalculateDomainList(v);
                if (domain.Count > 0 && varTrials[i] < domain.Count)
                {
                    sudo.SetTile(v, domain[varTrials[i]]);
                    varTrials[i]++;
                    i++;
                }
                else
                {
                    varTrials[i] = 0;
                    backtrack = true;
                    i--;
                }
                expanded++;
            }
            sudo.CloseVisualizer();     // Close the visualizer
        }

        /// <summary>
        /// Solves a sudoku with our extended implementation of Chronological Backtracking.
        /// Differs from SolveLL() by showing a visualizer that gives a good impression of the solving process.
        /// </summary>
        /// <param name="sudo">The sudoku to solve</param>
        /// <param name="expanded">Keeps track of the amount of iterations/node expansions of the algorithm</param>
        public static void SolveTraceLL(Sudoku sudo, out long expanded)  // For full functionality of the code see SolveLL()
        {
            sudo.StartVisualizer();     // Start the visualizer
            int varCount = sudo.VarCount;
            List<Point> variables = sudo.GetVariables();
            List<int> varTrials = new List<int>(new int[varCount]);

            int i = 0;
            bool backtrack = false;
            expanded = 0;
            while (i >= 0 && i < varCount)
            {
                Point v = variables[i];

                if (backtrack)
                {
                    sudo.ResetAndUpdate(v);
                    backtrack = false;
                }

                Sudoku.DomainHashSet domain = sudo.GetDomain(v);
                if (domain.Count > 0 && varTrials[i] < domain.Count)
                {
                    int t = 0;
                    for (int n = 1; n <= sudo.N; n++)
                    {
                        if (domain.Contains(n))
                        {
                            if (t >= varTrials[i])
                            {
                                sudo.SetAndUpdate(v, n);
                                varTrials[i]++;
                                i++;
                                break;
                            }
                            t++;
                        }
                    }
                }
                else
                {
                    varTrials[i] = 0;
                    backtrack = true;
                    i--;
                }
                expanded++;
            }
            sudo.CloseVisualizer();     // Close the visualizer
        }
        #endregion

        #region SolveExtendedTrace
        /// <summary>
        /// Solves a sudoku with our standard implementation of Chronological Backtracking.
        /// Differs from SolveTrace() by keeping a log in which all steps are logged.
        /// </summary>
        /// <param name="sudo">The sudoku to solve</param>
        /// <param name="expanded">Keeps track of the amount of iterations/node expansions of the algorithm</param>
        /// <param name="log">The stringbuilder in which the steps are logged</param>
        public static void SolveExtendedTrace(Sudoku sudo, out long expanded, StringBuilder log)    // For full functionality of the code see Solve()
        {
            sudo.StartVisualizer();     // Start the visualizer
            int varCount = sudo.VarCount;
            List<Point> variables = sudo.GetVariables();
            List<int> varTrials = new List<int>(new int[varCount]);

            int i = 0;
            bool backtrack = false;
            expanded = 0;
            while (i >= 0 && i < varCount)
            {
                log.AppendLine($"\ni={i}");

                Point v = variables[i];
                log.AppendLine($"Checking point {v}:");

                if (backtrack)
                {
                    log.AppendLine($"Backtrack, resetting point {v}.");
                    sudo.SetTile(v, 0);
                    backtrack = false;
                }

                List<int> domain = sudo.CalculateDomainList(v);
                if (domain.Count > 0 && varTrials[i] < domain.Count)
                {
                    log.AppendLine($"Setting value of tile {v} to {domain[varTrials[i]]}.");
                    sudo.SetTile(v, domain[varTrials[i]]);
                    varTrials[i]++;
                    i++;
                }
                else
                {
                    log.AppendLine($"Resetting trials of tile {v}, starting backtrack.");
                    varTrials[i] = 0;
                    backtrack = true;
                    i--;
                }
                expanded++;
            }
            if (i == -1) log.AppendLine("No solution found, returning...");
            else log.AppendLine("Solution found!");
            sudo.CloseVisualizer();     // Close the visualizer
        }

        /// <summary>
        /// Solves a sudoku with our extended implementation of Chronological Backtracking.
        /// Differs from SolveTraceLL() by keeping a log in which all steps are logged.
        /// </summary>
        /// <param name="sudo">The sudoku to solve</param>
        /// <param name="expanded">Keeps track of the amount of iterations/node expansions of the algorithm</param>
        /// <param name="log">The stringbuilder in which the steps are logged</param>
        public static void SolveExtendedTraceLL(Sudoku sudo, out long expanded, StringBuilder log) // For full functionality of the code see SolveLL()
        {
            sudo.StartVisualizer();     // Start the visualizer
            int varCount = sudo.VarCount;
            List<Point> variables = sudo.GetVariables();
            List<int> varTrials = new List<int>(new int[varCount]);

            int i = 0;
            bool backtrack = false;
            expanded = 0;
            while (i >= 0 && i < varCount)
            {
                log.AppendLine($"\ni={i}");

                Point v = variables[i];
                log.AppendLine($"Checking point {v}:");

                if (backtrack)
                {
                    log.AppendLine($"Backtrack, resetting point {v}.");
                    sudo.ResetAndUpdate(v);
                    backtrack = false;
                }

                Sudoku.DomainHashSet domain = sudo.GetDomain(v);
                if (domain.Count > 0 && varTrials[i] < domain.Count)
                {
                    int t = 0;
                    for (int n = 1; n <= sudo.N; n++)
                    {
                        if (domain.Contains(n))
                        {
                            if (t >= varTrials[i])
                            {
                                log.AppendLine($"Setting value of tile {v} to {n}.");
                                sudo.SetAndUpdate(v, n);
                                varTrials[i]++;
                                i++;
                                break;
                            }
                            t++;
                        }
                    }
                }
                else
                {
                    log.AppendLine($"Resetting trials of tile {v}, starting backtrack.");
                    varTrials[i] = 0;
                    backtrack = true;
                    i--;
                }
                expanded++;
            }
            if (i == -1) log.AppendLine("No solution found, returning...");
            else log.AppendLine("Solution found!");
            sudo.CloseVisualizer();     // Close the visualizer    
        }
        #endregion
    }
}
