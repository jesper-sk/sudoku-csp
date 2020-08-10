using System.Collections.Generic;
using System.Text;

namespace Sudoku2
{
    /// <summary>
    /// Implements the forward checking backtracking algorithm with most constrained variable heuristics to solve sudokus.
    /// </summary>
    static class ForwardCheckingMcvBacktrackSolver
    {
        #region Solve
        /// <summary>
        /// Solves a sudoku with our standard implementation of Forward Checking with MCV heuristics.
        /// </summary>
        /// <param name="sudo">The sudoku to solve</param>
        /// <param name="expanded">Keeps track of the amount of iterations/node expansions of the algorithm</param>
        public static void Solve(Sudoku sudo, out long expanded)
        {
            int varCount = sudo.VarCount;
            List<Point> variables = sudo.GetVariables();            // Get the variables of the sudoku
            List<int> varTrials = new List<int>(new int[varCount]); // Initialize varTrials list with varCount zeros
            Stack<int> previs = new Stack<int>();                   // Instead of using i for backtracking we now need to keep track of the last indices, since we take variables in a non-linear way. 
                                                                    // A stack is the best data structure in this case because we need to get the most recent (First in, First out)
            int totaldone = 0;                                      // Used to determine how many variables are filled in
            bool backtrack = false;                                 // Used to determine if we need to do backtracking
            expanded = 0;                                           // Initialize node expansions variable
            while (totaldone >= 0 && totaldone < varCount)          // If the total of tiles done becomes less than zero, we know that there is no possible value, if it becomes equal to the variables count, we filled in every variable -> solution
            {
                if (backtrack)                                      // First handle backtracking because of the hashsets
                {
                    sudo.SetTile(variables[previs.Pop()], 0);       // Get the last tile that had a value assigned and reset it, removing it from the stack
                    backtrack = false;                              // Disable backtrack
                }
                else
                {
                    int i = sudo.GetSmallestDomainIndex(variables);     // Get the index of the tile with the smalles domain
                    Point v = variables[i];                             // Set that tile as the current tile

                    List<int> domain = sudo.CalculateDomainList(v);     // Get the domain of the tile
                    bool valueFound = false;                            // Enables us to check whether we found a value that causes no empty domains
                    while (varTrials[i] < domain.Count)                 // Try values in the domain
                    {
                        sudo.SetTile(v, domain[varTrials[i]]);          // Set tile to value
                        varTrials[i]++;                                 // Update varTrial to ensure we get the next value in the domain next time
                        if (!sudo.CheckIfEmptyDomains(v))               // Check if we caused any variables to have an empty domain
                        {
                            valueFound = true;                          // No empty domains, we can keep this value
                            break;                                      // Stop trying other values
                        }
                        else sudo.SetTile(v, 0);                        // Empty domain(s) found, reset tile
                    }
                    if (valueFound)                 // We found a value that caused no empty domains
                    {
                        totaldone++;                // We assigned a new value, so this can be upped once
                        previs.Push(i);             // Since we assigned a value, we need to push it to the stack
                    }
                    else                            // No values found that cause no empty domain, we need to do backtracking
                    {
                        varTrials[i] = 0;           // Reset the varTrial for the tile
                        backtrack = true;           // Enable backtrack
                        totaldone--;                // Lower the amount of variables that has a value, since we are going to do backtracking
                    }
                    expanded++;                     // Another node expanded
                }
            }
        }

        /// <summary>
        /// Solves a sudoku with our extended implementation of Forward Checking with MCV heuristics.
        /// </summary>
        /// <param name="sudo">The sudoku to solve</param>
        /// <param name="expanded">Keeps track of the amount of iterations/node expansions of the algorithm</param>
        public static void SolveLL(Sudoku sudo, out long expanded)
        {
            // This implementation doesn't use the list of variables, therefore we don't have an index to use for the varTrials.
            // However since we do have a Point representing the variable we are checking, we can just use a 2-dimensional array for the varTrials.
            int[,] varTrials = new int[sudo.N, sudo.N];
            for (int x = 0; x < sudo.N; x++)
            {
                for (int y = 0; y < sudo.N; y++)
                {
                    varTrials[x, y] = 0;
                }
            }
            Stack<Point> prevs = new Stack<Point>();            // Although we don't have a list of variables, we still get Points/tiles which assign a value to
                                                                // Therefore we can still use a stack to keep track of the points we assigned a value to
            int totaldone = 0;                                  // Used to determine how many variables are filled in
            bool backtrack = false;                             // Used to determine if we need to do backtracking
            expanded = 0;                                       // Initialize node expansions variable
            while (totaldone >= 0 && totaldone < sudo.VarCount) // If the total of tiles done becomes less than zero, we know that there is no possible value, if it becomes equal to the variables count, we filled in every variable -> solution
            {
                if (backtrack)                                  // First handle backtracking because of the hashsets
                {
                    sudo.ResetAndUpdate(prevs.Pop());           // Get the value to which a value is assigned most recent, reset it while removing it from the stack
                    backtrack = false;                          // Disable backtrack
                }
                else
                {
                    Sudoku.DomainHashSet domain = sudo.GetSmallestDomain(out Point v);  // Get the domain and Point of the tile that has the smallest domain
                    bool valueFound = false;                                            // Enables us to check whether we found a value that causes no empty domains
                    int varTrial = varTrials[v.X, v.Y];                                 // Get the varTrial of the tile
                    int t = 0;                                                          // Enumerator for the hashset
                    for (int n = 1; n <= sudo.N; n++)                                   // Check numbers 1-N
                    {
                        if (domain.Contains(n))                                         // First check if the domain contains the value
                        {
                            if (t >= varTrial)                                          // Only set the value if we get the varTrial-th value from the domain
                            {
                                sudo.SetAndUpdate(v, n);                                // Set the value of the tile
                                varTrials[v.X, v.Y]++;                                  // Up the varTrial to ensure we get the next value of the domain next time
                                if (!sudo.ExistsEmptyDomain())                          // Check if we caused any empty domains
                                {
                                    valueFound = true;                                  // No empty domains, we can keep this value
                                    break;                                              // Stop trying other values
                                }
                                else sudo.ResetAndUpdate(v);                            // Empty domain(s) found, reset tile
                            }
                            t++;                                                        // Up the enumerator only when the value is in the domain
                        }
                    }
                    if (valueFound)                 // We found a value that caused no empty domains
                    {
                        totaldone++;                // We assigned a new value, so this can be upped once
                        prevs.Push(v);              // Since we assigned a value, we need to push it to the stack
                    }
                    else                            // No values found that cause no empty domain, we need to do backtracking
                    {
                        varTrials[v.X, v.Y] = 0;    // Reset the varTrial for the tile
                        backtrack = true;           // Enable backtrack
                        totaldone--;                // Lower the amount of variables that has a value, since we are going to do backtracking
                    }
                    expanded++;                     // Another node expanded
                }
            }
        }
        #endregion

        #region SolveTrace
        /// <summary>
        /// Solves a sudoku with our standard implementation of Forward Checking with MCV heuristics.
        /// Differs from Solve() by showing a visualizer that gives a good impression of the solving process.
        /// </summary>
        /// <param name="sudo">The sudoku to solve</param>
        /// <param name="expanded">Keeps track of the amount of iterations/node expansions of the algorithm</param>
        public static void SolveTrace(Sudoku sudo, out long expanded)   // For full functionality of the code see Solve()
        {
            sudo.StartVisualizer();     // Start the visualizer
            List<Point> variables = sudo.GetVariables();
            List<int> varTrials = new List<int>(new int[sudo.VarCount]);
            Stack<int> previs = new Stack<int>();

            int totaldone = 0;
            bool backtrack = false;
            expanded = 0;
            while (totaldone >= 0 && totaldone < sudo.VarCount)
            {
                if (backtrack)
                {
                    int prev = previs.Pop();
                    sudo.SetTile(variables[prev], 0);
                    backtrack = false;
                }

                int i = sudo.GetSmallestDomainIndex(variables);
                Point v = variables[i];

                List<int> domain = sudo.CalculateDomainList(v);
                bool valueFound = false;
                while (varTrials[i] < domain.Count)
                {
                    sudo.SetTile(v, domain[varTrials[i]]);
                    varTrials[i]++;
                    if (!sudo.CheckIfEmptyDomains(v))
                    {
                        valueFound = true;
                        break;
                    }
                    else sudo.SetTile(v, 0);
                }
                if (valueFound)
                {
                    totaldone++;
                    previs.Push(i);
                }
                else
                {
                    varTrials[i] = 0;
                    backtrack = true;
                    totaldone--;
                }
                expanded++;
            }
            sudo.CloseVisualizer();     // Close the visualizer
        }

        /// <summary>
        /// Solves a sudoku with our extended implementation of Forward Checking with MCV heuristics.
        /// Differs from SolveLL() by showing a visualizer that gives a good impression of the solving process.
        /// </summary>
        /// <param name="sudo">The sudoku to solve</param>
        /// <param name="expanded">Keeps track of the amount of iterations/node expansions of the algorithm</param>
        public static void SolveTraceLL(Sudoku sudo, out long expanded) // For full functionality of the code see SolveLL()
        {
            sudo.StartVisualizer();     // Start the visualizer
            int[,] varTrials = new int[sudo.N, sudo.N];
            for (int x = 0; x < sudo.N; x++)
            {
                for (int y = 0; y < sudo.N; y++)
                {
                    varTrials[x, y] = 0;
                }
            }
            Stack<Point> prevs = new Stack<Point>();

            int totaldone = 0;
            bool backtrack = false;
            expanded = 0;
            while (totaldone >= 0 && totaldone < sudo.VarCount)
            {
                if (backtrack)
                {
                    Point b = prevs.Pop();
                    sudo.ResetAndUpdate(b);
                    backtrack = false;
                }

                Sudoku.DomainHashSet domain = sudo.GetSmallestDomain(out Point v);
                bool valueFound = false;
                int varTrial = varTrials[v.X, v.Y];
                int t = 0;
                for (int n = 1; n <= sudo.N; n++)
                {
                    if (domain.Contains(n))
                    {
                        if (t >= varTrial)
                        {
                            sudo.SetAndUpdate(v, n);
                            varTrials[v.X, v.Y]++;
                            if (!sudo.ExistsEmptyDomain())
                            {
                                valueFound = true;
                                break;
                            }
                            else sudo.ResetAndUpdate(v);
                        }
                        t++;
                    }
                }
                if (valueFound)
                {
                    totaldone++;
                    prevs.Push(v);
                }
                else
                {
                    varTrials[v.X, v.Y] = 0;
                    backtrack = true;
                    totaldone--;
                }
                expanded++;
            }
            sudo.CloseVisualizer();     // Close the visualizer
        }
        #endregion

        #region SolveExtendedTrace
        /// <summary>
        /// Solves a sudoku with our standard implementation of Forward Checking with MCV heuristics.
        /// Differs from SolveTrace() by keeping a log in which all steps are logged.
        /// </summary>
        /// <param name="sudo">The sudoku to solve</param>
        /// <param name="expanded">Keeps track of the amount of iterations/node expansions of the algorithm</param>
        /// <param name="log">The stringbuilder in which the steps are logged</param>
        public static void SolveExtendedTrace(Sudoku sudo, out long expanded, StringBuilder log)    // For full functionality of the code see Solve()
        {
            sudo.StartVisualizer();     // Start the visualizer
            List<Point> variables = sudo.GetVariables();
            List<int> varTrials = new List<int>(new int[sudo.VarCount]);
            Stack<int> previs = new Stack<int>();

            int totaldone = 0;
            bool backtrack = false;
            expanded = 0;
            while (totaldone >= 0 && totaldone < sudo.VarCount)
            {
                log.AppendLine($"\ntotaldone={totaldone}");

                if (backtrack)
                {
                    int prev = previs.Pop();
                    log.AppendLine($"Backtrack, resetting point {variables[prev]}.");
                    sudo.SetTile(variables[prev], 0);
                    backtrack = false;
                }

                int i = sudo.GetSmallestDomainIndex(variables);
                Point v = variables[i];
                log.AppendLine($"Checking point {v}:");

                List<int> domain = sudo.CalculateDomainList(v);
                if (domain.Count > 0)
                {
                    bool valueFound = false;
                    while (varTrials[i] < domain.Count)
                    {
                        log.AppendLine($"Trying value of {domain[varTrials[i]]} for tile {v}.");
                        sudo.SetTile(v, domain[varTrials[i]]);
                        varTrials[i]++;
                        if (!sudo.CheckIfEmptyDomains(v))
                        {
                            log.AppendLine("No empty domains!");
                            valueFound = true;
                            break;
                        }
                        else
                        {
                            log.AppendLine("Empty domain detected!");
                            sudo.SetTile(v, 0);
                        }
                    }
                    if (valueFound)
                    {
                        totaldone++;
                        previs.Push(i);
                    }
                    else
                    {
                        log.AppendLine($"No value found, resetting trials of tile {v}, starting backtrack.");
                        varTrials[i] = 0;
                        backtrack = true;
                        totaldone--;
                    }
                }
                else
                {
                    log.AppendLine($"No value found, resetting trials of tile {v}, starting backtrack.");
                    varTrials[i] = 0;
                    backtrack = true;
                    totaldone--;
                }
                expanded++;
            }
            if (totaldone == -1) log.AppendLine("No solution found, returning...");
            else log.AppendLine("Solution found!");
            sudo.CloseVisualizer();     // Close the visualizer
        }

        /// <summary>
        /// Solves a sudoku with our extended implementation of Forward Checking with MCV heuristics.
        /// Differs from SolveTraceLL() by keeping a log in which all steps are logged.
        /// </summary>
        /// <param name="sudo">The sudoku to solve</param>
        /// <param name="expanded">Keeps track of the amount of iterations/node expansions of the algorithm</param>
        /// <param name="log">The stringbuilder in which the steps are logged</param>
        public static void SolveExtendedTraceLL(Sudoku sudo, out long expanded, StringBuilder log)  // For full functionality of the code see SolveLL()
        {
            sudo.StartVisualizer();     // Start the visualizer
            int[,] varTrials = new int[sudo.N, sudo.N];
            for (int x = 0; x < sudo.N; x++)
            {
                for (int y = 0; y < sudo.N; y++)
                {
                    varTrials[x, y] = 0;
                }
            }
            Stack<Point> prevs = new Stack<Point>();

            int totaldone = 0;
            bool backtrack = false;
            expanded = 0;
            while (totaldone >= 0 && totaldone < sudo.VarCount)
            {
                log.AppendLine($"\ntotaldone={totaldone}");

                if (backtrack)
                {
                    Point b = prevs.Pop();
                    log.AppendLine($"Backtrack, resetting point {b}.");
                    sudo.ResetAndUpdate(b);
                    backtrack = false;
                }

                Sudoku.DomainHashSet domain = sudo.GetSmallestDomain(out Point v);
                if (domain.Count > 0)
                {
                    bool valueFound = false;
                    int varTrial = varTrials[v.X, v.Y];
                    int t = 0;
                    for (int n = 1; n <= sudo.N; n++)
                    {
                        if (domain.Contains(n))
                        {
                            if (t >= varTrial)
                            {
                                log.AppendLine($"Trying value of {n} for tile {v}.");
                                sudo.SetAndUpdate(v, n);
                                varTrials[v.X, v.Y]++;
                                if (!sudo.ExistsEmptyDomain())
                                {
                                    log.AppendLine("No empty domains!");
                                    valueFound = true;
                                    break;
                                }
                                else
                                {
                                    log.AppendLine("Empty domain detected!");
                                    sudo.ResetAndUpdate(v);
                                }
                            }
                            t++;
                        }
                    }
                    if (valueFound)
                    {
                        totaldone++;
                        prevs.Push(v);
                    }
                    else
                    {
                        log.AppendLine($"No value found, resetting trials of tile {v}, starting backtrack.");
                        varTrials[v.X, v.Y] = 0;
                        backtrack = true;
                        totaldone--;
                    }
                }
                else
                {
                    log.AppendLine($"No value found, resetting trials of tile {v}, starting backtrack.");
                    varTrials[v.X, v.Y] = 0;
                    backtrack = true;
                    totaldone--;
                }
                expanded++;
            }
            if (totaldone == -1) log.AppendLine("No solution found, returning...");
            else log.AppendLine("Solution found!");
            sudo.CloseVisualizer();     // Close the visualizer
        }
        #endregion
    }
}
