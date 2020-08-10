// To enter debug mode, uncomment the line below
//#define DEB

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using LatexFormatting;

/**
 * Dirs:
 * ?dir ./Sudokus/9_9/50sudokus.txt ?alg fcmcvll ?n 50 ?size 9 ?trace
 * ?dir ./Sudokus/9_9/50sudokus.txt ?alg fcmcvll ?n 50 ?size 9 ?extendedtrace
 * ?dir ./Sudokus/16_16/16x16ex.txt ?alg fcmcv ?n 1 ?size 16
 * ?dir ./Sudokus/9_9/hard.txt ?alg fcnll ?n 1 ?size 9 
 * ?dir ./Sudokus/16_16/16x16ex.txt ?alg fcnll ?n 1 ?size 16
 * 
 * ?dir ./Sudokus/9_9/50sudokus.txt ?alg eac-cbt-fc ?n 50 ?size 9
 */

namespace Sudoku2
{
    /*
     * 
     */ 
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            // If DEB is defined (see top of file), this block of code is executed instead of the normal routine
#if DEB
            Console.WriteLine("Debug");
            string Alg = "eac";
            string[] exc = Alg.Split('-');
            string[] algs = { "cbt", "cbtll", "fc", "fcll", "fcmcv", "fcmcvll" };
            Console.WriteLine(Util.ArrToString(exc));
            bool[] inc = { true, true, true, true, true, true };
            for (int i = 1; i < exc.Length; i++)
            {
                //Console.WriteLine($"{i}, eval {algs[i].Trim()}");
                for (int j = 0; j < inc.Length; j++)
                {
                    string a = exc[i].Trim();
                    if (a == algs[j]) inc[j] = false;
                }
            }
            foreach (bool b in inc) Console.WriteLine(b);
            Console.ReadKey();

#else
            // The normal routine, basicly everything is done within the class SudokuCalculator
            while (true)
            {
                Console.Title = $"SudokuSolver - Enter command";
                Application.EnableVisualStyles();
                using (Process p = Process.GetCurrentProcess()) p.PriorityClass = ProcessPriorityClass.High;
                SudokuCalculator calc = new SudokuCalculator();
                calc.ParseAndCalculate();
            }
#endif
        }

        class SudokuCalculator
        {
            // After calling the method FillArguments() (near the bottom of the file), this dictionary is filled with the user input arguments, so that
            // ?[argument] [value] --> args[argument] = value (for more information about the ?[arg] [value] notation, read our report)
            Dictionary<string, string> args;

            // Within this region, every argument a user can input gets converted into usable variables
            #region Variables from args
            // ?size [int]
            int Size
            {
                get
                {
                    try { return int.Parse(args["size"]); }
                    catch (FormatException e) { throw new InvalidArgumentException(e.Message); }
                    catch (KeyNotFoundException) { throw new MissingArgumentExeption("size"); }
                }
            }

            // ?dir [string]
            string Dir
            {
                get
                {
                    try { return args["dir"]; }
                    catch (KeyNotFoundException) { throw new MissingArgumentExeption("dir"); }
                }
            }

            // ?n [int]
            int N
            {
                get
                {
                    try { return int.Parse(args["n"]); }
                    catch (FormatException e) { throw new InvalidArgumentException(e.Message); }
                    catch (KeyNotFoundException) { throw new MissingArgumentExeption("n"); }
                }
            }

            // ?alg [string]
            string Alg
            {
                get
                {
                    try { return args["alg"]; }
                    catch (KeyNotFoundException) { throw new MissingArgumentExeption("alg"); }
                }
            }
            
            // ?parallel
            bool UseParallel
            {
                get { return args.ContainsKey("parallel"); }
            }

            string FileName
            {
                get
                {
                    string[] d = Dir.Split('\\');
                    string[] d1 = d[d.Length - 1].Split('/');
                    string file = d1[d1.Length - 1];
                    int l = file.Length;
                    return file.Substring(0, l - 4);
                }
            }

            // ?dest [string]
            string Destination
            {
                get
                {
                    try { return args["dest"]; }
                    catch (KeyNotFoundException) { return Directory.GetCurrentDirectory(); }
                }
            }

            // ?trace for regular trace, ?extendedtrace for extended trace
            TraceMode TraceMode
            {
                get
                {
                    if (args.ContainsKey("extendedtrace")) return TraceMode.Extended;
                    else if (args.ContainsKey("trace")) return TraceMode.Normal;
                    else return TraceMode.None;
                }
            }
            #endregion

            public void ParseAndCalculate()
            {
                try
                {
                    Console.WriteLine("Sudoku solver. Enter command or type 'help':");
                    string inp = Console.ReadLine();
                    if (inp == "help")      // Print a 'brief' overview of all the possible commands
                    {
                        Console.WriteLine("Required commands:");
                        Console.WriteLine("\t?dir: \t\tThe directory of the sudokus");
                        Console.WriteLine("\t?size: \t\tThe size of the sudokus");
                        Console.WriteLine("\t?n: \t\tThe number of sudokus");
                        Console.WriteLine("\t?alg: \t\t[cbt/cbtll/fc/fcll/fcmcv//fcmcvll/eac] The algorithm used");
                        Console.WriteLine("Optional commands:");
                        Console.WriteLine("\t?parallel: \tUse Parallel.For loop to solve the sudokus (only useable without trace command)");
                        Console.WriteLine("\t?trace: \tShow visualizer");
                        Console.WriteLine("\t?extendedtrace:\tShow visualizer and log every step");
                        Console.WriteLine("\t?dest: \t\tThe desination directory to save the log files (for trace)");
                        Console.WriteLine("\tclear:\t\tClears the console");
                        Console.WriteLine("\tforecolor:\tUse this to change the text color of the console, \n\t\t\ttype \'forecolor\' and then \'help\' for instructions how to use");
                        Console.WriteLine("\tbackcolor:\tUse this to change the background color of the console (it's more like a text-marker idea)");
                        Console.WriteLine("Examples:");
                        Console.WriteLine("---");
                    }
                    else if (inp == "clear") Console.Clear();
                    #region Color
                    else if (inp == "forecolor")
                    {
                    ForeColorStart:
                        Console.WriteLine("Please type in one of the ConsoleColors, type help for all possibilities, reset to reset color, or exit to stop choosing color.");
                        string chosencolor = Console.ReadLine().ToLower();
                        if (chosencolor == "help")
                        {
                            ConsoleColor prevcolor = Console.ForegroundColor;
                            foreach (ConsoleColor clr in Enum.GetValues(typeof(ConsoleColor)))
                            {
                                Console.ForegroundColor = clr;
                                Console.WriteLine(clr);
                            }
                            Console.ForegroundColor = prevcolor;
                            goto ForeColorStart;
                        }
                        else if (chosencolor == "reset") Console.ForegroundColor = ConsoleColor.White;
                        else if (chosencolor == "exit") return;
                        else
                        {
                            try
                            {
                                ConsoleColor color = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), chosencolor, true);
                                Console.ForegroundColor = color;
                            }
                            catch
                            {
                                goto ForeColorStart;
                            }
                        }

                    }
                    else if (inp == "backcolor")
                    {
                    BackColorStart:
                        Console.WriteLine("Please type in one of the ConsoleColors, type help for all possibilities, reset to reset color, or exit to stop choosing color.");
                        string chosencolor = Console.ReadLine().ToLower();
                        if (chosencolor == "help")
                        {
                            ConsoleColor prevcolor = Console.BackgroundColor;
                            foreach (ConsoleColor clr in Enum.GetValues(typeof(ConsoleColor)))
                            {
                                Console.BackgroundColor = clr;
                                Console.WriteLine(clr);
                            }
                            Console.BackgroundColor = prevcolor;
                            goto BackColorStart;
                        }
                        else if (chosencolor == "reset") Console.BackgroundColor = ConsoleColor.Black;
                        else if (chosencolor == "exit") return;
                        else
                        {
                            try
                            {
                                ConsoleColor color = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), chosencolor, true);
                                Console.BackgroundColor = color;
                            }
                            catch
                            {
                                goto BackColorStart;
                            }
                        }
                    }
                    #endregion
                    // Here we actually start with parsing the arguments and solving the sudoku 
                    else
                    {
                        Console.WriteLine("---");
                        FillArguments(inp);     // Parse the input and put it in the dictionary 'args'
                        if (Alg.StartsWith("eac"))
                        {
                            string[] exc = Alg.Split('-');
                            string[] algs = { "cbt", "cbtll", "fc", "fcll", "fcmcv", "fcmcvll" };
                            bool[] inc = { true, true, true, true, true, true };
                            for (int i = 1; i < exc.Length; i++)
                            {
                                for (int j = 0; j < inc.Length; j++)
                                {
                                    string a = exc[i].Trim();
                                    if (a == algs[j]) inc[j] = false;
                                }
                            }
                            Console.Title = $"SudokuSolver - Evaluating algorithms on {FileName}...";
                            AlgorithmEvaluatorAndComparer.EvaluateAndCompare(Dir, N, Size, inc, out LatexTabularMaker lt, out StringBuilder log);
                            string root = @".\Tabulars";
                            if (!Directory.Exists(root)) Directory.CreateDirectory(root);
                            string dir;
                            Directory.CreateDirectory(dir = $@"{root}\{DateTime.Now.ToString("dd.M.yy H\\hmm\\mss\\s")} ({FileName})");
                            File.WriteAllText($@"{dir}\latex.txt", lt.ReturnTable());
                            File.WriteAllText($@"{dir}\plain.txt", log.ToString());
                            Console.Title = $"SudokuSolver - Enter command";
                            Process.Start(dir);

                            return;
                        }
                        Console.WriteLine("Parsing sudokus...");
                        Sudoku[] sudos;         // Will contain all the sudokus to solve
                        switch (Size)           // Parsing the sudokus depends on the size
                        {
                            case (9): sudos = Parser.Parse9(Dir, N); break;
                            case (16): sudos = Parser.Parse16(Dir, N); break;
                            default: throw new InvalidArgumentException("Unknown size.");   // If this happens, the user has defined a size we didn't account for. Therefore, we raise an exception
                        }
                        Console.WriteLine("Starting solving process...\n---");
                        TraceMode mode = TraceMode;     //What will we trace?
                        switch (mode)
                        {
                            case (TraceMode.Extended): SolveExtendedTrace(sudos, inp); break;     // Solve the sudoku with extended trace.
                            case (TraceMode.Normal): SolveTrace(sudos, inp); break;              // Solve the sudoku with trace
                            default: Solve(sudos, inp); break;                                  // If the user doesn't define a tracemode, we take the normal ie no trace mode
                        }
                    }
                    /// <summary>
                    /// Interprets the user input, and puts it in the args dictionary
                    /// </summary>
                    /// <param name="pars">The user input</param>
                    void FillArguments(string pars)
                    {
                        string[] split = pars.Split('?');                       // Every argument starts with a '?', so we split the string on that.
                        args = new Dictionary<string, string>(split.Length);
                        foreach (string par in split)
                        {
                            string p = par.Trim();                              // We don't need white spaces
                            int cursor = 0;
                            for (; cursor < par.Length; cursor++)
                            {
                                if (par[cursor] == ' ') break;                  // Separate the argument from the value by looking at a space
                            }
                            args[par.Substring(0, cursor).Trim().ToLower()] = cursor < par.Length ? par.Substring(cursor).Trim() : string.Empty; // If a value is found, set is as the value for the argument as key.
                        }                                                                                                                        // If no value is found, we give string.Empty as value (we don't need it anyway)
                    }
                }
                catch (MissingArgumentExeption e)   // If this exception is thrown, the user didn't specify an argument that was required. 
                {
                    Console.WriteLine($"Missing parameter: {e.CustomMessage}");
                }
                catch (InvalidArgumentException e)  // If this exception  is thrown the user specified an argument the wrong way, eg typing errors or an invalid range.
                {
                    Console.WriteLine($"Invalid parameter: {e.CustomMessage}");
                }
                catch (FileNotFoundException e)      // If this exception is thrown, the program can't find the directory given at ?dir.
                {
                    Console.WriteLine(e.Message);
                }
            }

            /// <summary>
            /// Solve the sudokus, without tracing a lot.
            /// </summary>
            /// <param name="sudos">The sudokus to trace</param>
            /// <param name="inp">The string containing the user input</param>
            void Solve(Sudoku[] sudos, string inp)
            {
                string alg = Alg;

                string[] solutions = new string[sudos.Length];
                int[] expandedtotals = new int[sudos.Length];
                Stopwatch totaltimer = new Stopwatch();
                long expanded;
                if (UseParallel)
                {
                    switch (alg)
                    {
                        case "cbt":
                            totaltimer.Start();
                            Parallel.For (0, sudos.Length, i =>
                            {
                                Sudoku sudo = sudos[i];
                                expanded = 0;
                                Stopwatch solvertimer = Stopwatch.StartNew();
                                ChronologicalBacktrackSolver.Solve(sudo, out expanded);
                                solvertimer.Stop();
                                solutions[i] = $"Sudoku {i}:\nTime to solve: {solvertimer.Elapsed.TotalMilliseconds}ms with {expanded} node expansions, solved sudoku:";
                            });
                            totaltimer.Stop();
                            break;
                        case "cbtll":
                            totaltimer.Start();
                            Parallel.For (0, sudos.Length, i =>
                            {
                                Sudoku sudo = sudos[i];
                                expanded = 0;
                                Stopwatch solvertimer = Stopwatch.StartNew();
                                ChronologicalBacktrackSolver.SolveLL(sudo, out expanded);
                                solvertimer.Stop();
                                solutions[i] = $"Sudoku {i}:\nTime to solve: {solvertimer.Elapsed.TotalMilliseconds}ms with {expanded} node expansions, solved sudoku:";
                            });
                            totaltimer.Stop();
                            break;
                        case "fc":
                            totaltimer.Start();
                            Parallel.For (0, sudos.Length, i =>
                            {
                                Sudoku sudo = sudos[i];
                                expanded = 0;
                                Stopwatch solvertimer = Stopwatch.StartNew();
                                ForwardCheckingBacktrackSolver.Solve(sudo, out expanded);
                                solvertimer.Stop();
                                solutions[i] = $"Sudoku {i}:\nTime to solve: {solvertimer.Elapsed.TotalMilliseconds}ms with {expanded} node expansions, solved sudoku:";
                            });
                            totaltimer.Stop();
                            break;
                        case "fcll":
                            totaltimer.Start();
                            Parallel.For (0, sudos.Length, i =>
                            {
                                Sudoku sudo = sudos[i];
                                expanded = 0;
                                Stopwatch solvertimer = Stopwatch.StartNew();
                                ForwardCheckingBacktrackSolver.SolveLL(sudo, out expanded);
                                solvertimer.Stop();
                                solutions[i] = $"Sudoku {i}:\nTime to solve: {solvertimer.Elapsed.TotalMilliseconds}ms with {expanded} node expansions, solved sudoku:";
                            });
                            totaltimer.Stop();
                            break;
                        case "fcmcv":
                            totaltimer.Start();
                            Parallel.For (0, sudos.Length, i =>
                            {
                                Sudoku sudo = sudos[i];
                                expanded = 0;
                                Stopwatch solvertimer = Stopwatch.StartNew();
                                ForwardCheckingMcvBacktrackSolver.Solve(sudo, out expanded);
                                solvertimer.Stop();
                                solutions[i] = $"Sudoku {i}:\nTime to solve: {solvertimer.Elapsed.TotalMilliseconds}ms with {expanded} node expansions, solved sudoku:";
                            });
                            totaltimer.Stop();
                            break;
                        case "fcmcvll":
                            totaltimer.Start();
                            Parallel.For (0, sudos.Length, i =>
                            {
                                Sudoku sudo = sudos[i];
                                expanded = 0;
                                Stopwatch solvertimer = Stopwatch.StartNew();
                                ForwardCheckingMcvBacktrackSolver.SolveLL(sudo, out expanded);
                                solvertimer.Stop();
                                solutions[i] = $"Sudoku {i}:\nTime to solve: {solvertimer.Elapsed.TotalMilliseconds}ms with {expanded} node expansions, solved sudoku:";
                            });
                            totaltimer.Stop();
                            break;
                    }
                }
                else
                {
                    Stopwatch solvertimer = new Stopwatch();
                    switch (alg)
                    {
                        case "cbt":
                            totaltimer.Start();
                            for (int i = 0; i < sudos.Length; i++)
                            {
                                Sudoku sudo = sudos[i];
                                expanded = 0;
                                solvertimer.Restart();
                                ChronologicalBacktrackSolver.Solve(sudo, out expanded);
                                solvertimer.Stop();
                                solutions[i] = $"Sudoku {i}:\nTime to solve: {solvertimer.Elapsed.TotalMilliseconds}ms with {expanded} node expansions, solved sudoku:";
                            }
                            totaltimer.Stop();
                            break;
                        case "cbtll":
                            totaltimer.Start();
                            for (int i = 0; i < sudos.Length; i++)
                            {
                                Sudoku sudo = sudos[i];
                                expanded = 0;
                                solvertimer.Restart();
                                ChronologicalBacktrackSolver.SolveLL(sudo, out expanded);
                                solvertimer.Stop();
                                solutions[i] = $"Sudoku {i}:\nTime to solve: {solvertimer.Elapsed.TotalMilliseconds}ms with {expanded} node expansions, solved sudoku:";
                            }
                            totaltimer.Stop();
                            break;
                        case "fc":
                            totaltimer.Start();
                            for (int i = 0; i < sudos.Length; i++)
                            {
                                Sudoku sudo = sudos[i];
                                expanded = 0;
                                solvertimer.Restart();
                                ForwardCheckingBacktrackSolver.Solve(sudo, out expanded);
                                solvertimer.Stop();
                                solutions[i] = $"Sudoku {i}:\nTime to solve: {solvertimer.Elapsed.TotalMilliseconds}ms with {expanded} node expansions, solved sudoku:";
                            }
                            totaltimer.Stop();
                            break;
                        case "fcll":
                            totaltimer.Start();
                            for (int i = 0; i < sudos.Length; i++)
                            {
                                Sudoku sudo = sudos[i];
                                expanded = 0;
                                solvertimer.Restart();
                                ForwardCheckingBacktrackSolver.SolveLL(sudo, out expanded);
                                solvertimer.Stop();
                                solutions[i] = $"Sudoku {i}:\nTime to solve: {solvertimer.Elapsed.TotalMilliseconds}ms with {expanded} node expansions, solved sudoku:";
                            }
                            totaltimer.Stop();
                            break;
                        case "fcmcv":
                            totaltimer.Start();
                            for (int i = 0; i < sudos.Length; i++)
                            {
                                Sudoku sudo = sudos[i];
                                expanded = 0;
                                solvertimer.Restart();
                                ForwardCheckingMcvBacktrackSolver.Solve(sudo, out expanded);
                                solvertimer.Stop();
                                solutions[i] = $"Sudoku {i}:\nTime to solve: {solvertimer.Elapsed.TotalMilliseconds}ms with {expanded} node expansions, solved sudoku:";
                            }
                            totaltimer.Stop();
                            break;
                        case "fcmcvll":
                            totaltimer.Start();
                            for (int i = 0; i < sudos.Length; i++)
                            {
                                Sudoku sudo = sudos[i];
                                expanded = 0;
                                solvertimer.Restart();
                                ForwardCheckingMcvBacktrackSolver.SolveLL(sudo, out expanded);
                                solvertimer.Stop();
                                solutions[i] = $"Sudoku {i}:\nTime to solve: {solvertimer.Elapsed.TotalMilliseconds}ms with {expanded} node expansions, solved sudoku:";
                            }
                            totaltimer.Stop();
                            break;
                    } 
                }
                for (int i = 0; i < solutions.Length; i++)
                {
                    Console.WriteLine(solutions[i]);
                    Console.WriteLine(sudos[i]);
                }
                Console.WriteLine($"-\nSolving done, total time to solve {sudos.Length} sudokus: {totaltimer.Elapsed.TotalMilliseconds} milliseconds.");
                Console.WriteLine("---\n");
            }

            /// <summary>
            /// Solve the sudokus possibly multiple times, and trace all the results into a text file.
            /// </summary>
            /// <param name="sudos">The sudokus to solve.</param>
            /// <param name="inp">The string containing the user input</param>
            void SolveTrace(Sudoku[] sudos, string inp)
            {
                StringBuilder log = new StringBuilder();    // Again read all necessary parameters form the user arguments. This time we need a little bit more
                string alg = Alg;

                //solve
                long[] expandedtotals = new long[sudos.Length];
                DateTime totalstart = DateTime.Now;
                for (int i = 0; i < sudos.Length; i++)
                {
                    Sudoku sudo = sudos[i];
                    long expanded = 0;
                    DateTime start = DateTime.Now;
                    switch (alg)
                    {
                        case "cbt": ChronologicalBacktrackSolver.SolveTrace(sudo, out expanded);             break;
                        case "cbtll": ChronologicalBacktrackSolver.SolveTraceLL(sudo, out expanded);         break;
                        case "fc": ForwardCheckingBacktrackSolver.SolveTrace(sudo, out expanded);            break;
                        case "fcll": ForwardCheckingBacktrackSolver.SolveTraceLL(sudo, out expanded);        break;
                        case "fcmcv": ForwardCheckingMcvBacktrackSolver.SolveTrace(sudo, out expanded);      break;
                        case "fcmcvll": ForwardCheckingMcvBacktrackSolver.SolveTraceLL(sudo, out expanded);  break;
                    }
                    DateTime stop = DateTime.Now;
                    expandedtotals[i] = expanded;
                    Console.WriteLine($"Sudoku {i}:\nTime to solve: {(stop - start).TotalSeconds} seconds with {expanded} node expansions, solved sudoku:");
                    Console.WriteLine(sudo);
                }
                DateTime totalstop = DateTime.Now;
                Console.WriteLine($"Total time to solve {sudos.Length} sudokus: {(totalstop - totalstart).TotalSeconds} seconds.");
            }

            /// <summary>
            /// Solve the sudokus possibly multiple times. Trace the results, the evaluation during iteration and more diagnostics each into seperate text files.
            /// </summary>
            /// <param name="sudos">The sudokus to solve.</param>
            /// <param name="inp">The string containing the user input</param>
            void SolveExtendedTrace(Sudoku[] sudos, string inp)
            {
                StringBuilder log = new StringBuilder();
                string alg = Alg;
                log.AppendLine("Sudoku Solver");
                log.AppendLine("");
                log.AppendLine(inp);
                log.AppendLine("");
                log.AppendLine($"Sudoku directory: {Dir}");
                log.AppendLine($"Using algorithm {alg}");

                //solve
                long[] expandedtotals = new long[sudos.Length];
                DateTime totalstart = DateTime.Now;
                for (int i = 0; i < sudos.Length; i++)
                {
                    Sudoku sudo = sudos[i];
                    long expanded = 0;
                    DateTime start = DateTime.Now;
                    switch (alg)
                    {
                        case "cbt":     ChronologicalBacktrackSolver.SolveExtendedTrace(sudo, out expanded, log);           break;
                        case "cbtll":   ChronologicalBacktrackSolver.SolveExtendedTraceLL(sudo, out expanded, log);         break;
                        case "fc":      ForwardCheckingBacktrackSolver.SolveExtendedTrace(sudo, out expanded, log);         break;
                        case "fcll":    ForwardCheckingBacktrackSolver.SolveExtendedTraceLL(sudo, out expanded, log);       break;
                        case "fcmcv":   ForwardCheckingMcvBacktrackSolver.SolveExtendedTrace(sudo, out expanded, log);      break;
                        case "fcmcvll": ForwardCheckingMcvBacktrackSolver.SolveExtendedTraceLL(sudo, out expanded, log);    break;
                    }
                    DateTime stop = DateTime.Now;
                    expandedtotals[i] = expanded;
                    log.AppendLine($"Sudoku {i}:\nTime to solve: {(stop - start).TotalSeconds} seconds with {expanded} node expansions, solved sudoku:");
                    log.AppendLine(sudo.ToString());
                }
                DateTime totalstop = DateTime.Now;
                Console.WriteLine($"Total time to solve {sudos.Length} sudokus: {(totalstop - totalstart).TotalSeconds} seconds.");

                // Save all three logs to separate .txt-files and store them in a directory at the specified destination
                string root = $@".\Logs";
                if (!Directory.Exists(root)) Directory.CreateDirectory(root);
                string dir;
                File.WriteAllText(dir = $@"{root}\{DateTime.Now.ToString("dd.M.yy H\\hmm\\mss\\s")} -- Solving {N} {Size}x{Size} sudokus from {FileName}.txt", log.ToString());

                try { Process.Start(dir); }
                catch (System.ComponentModel.Win32Exception) {; } // If the directory is in use by another process. In that case, we ignore it.
            }
        }
    }

    // If a required argument is missing, a MissingArgumentException is thrown. We catch it and notify the user.
    public class MissingArgumentExeption : Exception
    {
        public string CustomMessage;
        public MissingArgumentExeption(string m) { CustomMessage = m; }
    }

    // If an argument has been entered invalidly, a InvalidArgumentException is thrown. We catch it and notify the user.
    public class InvalidArgumentException : Exception
    {
        public string CustomMessage;
        public InvalidArgumentException(string m) { CustomMessage = m; }
    }

    // What will we trace?
    enum TraceMode { None, Normal, Extended }
}
