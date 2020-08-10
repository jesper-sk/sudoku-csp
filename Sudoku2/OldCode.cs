/*

    static class old
    {
        public static void SolveLL(Sudoku sudo, out int expanded)
        {
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
                if (domain.Count > 0)
                {
                    bool valueFound = false;
                    int varTrial = varTrials[v.X, v.Y];
                    int t = 0;
                    for (int n = 1; n <= sudo.N; n++)
                    {
                        if (domain.Contains(n))
                        {
                            t++;
                            if (t > varTrial)
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
                }
                else
                {
                    varTrials[v.X, v.Y] = 0;
                    backtrack = true;
                    totaldone--;
                }
                expanded++;
            }
        }
    }
    */
    /* LL Algs
     
    // CBT
            public static void SolveLL(Sudoku sudo, out int expanded)
        {
            int varCount = sudo.VarCount;
            List<Point> variables = sudo.GetVariables();
            List<int> varTrials = new List<int>(varCount);
            for (int j = 0; j < varCount; j++)
            {
                varTrials.Add(0);
            }

            int i = 0;
            bool backtrack = false;
            expanded = 0;
            while (i >= 0 && i < varCount)
            {
                Point v = variables[i];

                if (backtrack)
                {
                    sudo.SetAndUpdate(v, 0);
                    backtrack = false;
                }

                sudo.GetDomain(v, out List<int> domain);
                if (domain.Count > 0 && varTrials[i] < domain.Count)
                {
                    sudo.SetAndUpdate(v, domain[varTrials[i]]);
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
        }
                public static void SolveTraceLL(Sudoku sudo, out int expanded)
        {
            sudo.StartVisualizer();
            int varCount = sudo.VarCount;
            List<Point> variables = sudo.GetVariables();
            List<int> varTrials = new List<int>(varCount);
            for (int j = 0; j < varCount; j++)
            {
                varTrials.Add(0);
            }

            int i = 0;
            bool backtrack = false;
            expanded = 0;
            while (i >= 0 && i < varCount)
            {
                Point v = variables[i];

                if (backtrack)
                {
                    sudo.SetAndUpdate(v, 0);
                    backtrack = false;
                }

                sudo.GetDomain(v, out List<int> domain);
                if (domain.Count > 0 && varTrials[i] < domain.Count)
                {
                    sudo.SetAndUpdate(v, domain[varTrials[i]]);
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
            if (i == -1) Console.WriteLine("No solution found, returning...");
            else Console.WriteLine("Solution found!");
            sudo.CloseVisualizer();
        }
                public static void SolveExtendedTraceLL(Sudoku sudo, out int expanded)
        {
            sudo.StartVisualizer();
            int varCount = sudo.VarCount;
            List<Point> variables = sudo.GetVariables();
            List<int> varTrials = new List<int>(varCount);
            for (int j = 0; j < varCount; j++)
            {
                varTrials.Add(0);
            }

            int i = 0;
            bool backtrack = false;
            expanded = 0;
            while (i >= 0 && i < varCount)
            {
                //Console.WriteLine($"\ni={i}");
                string res = "i= ";
                for (int j = 0; j < i; j++)
                {
                    res += "|";
                }
                res += " " + i;
                Console.WriteLine(res);

                Point v = variables[i];

                Console.WriteLine($"Checking point {v.ToString()}");
                if (backtrack)
                {
                    Console.WriteLine($"Backtrack, resetting point");
                    sudo.SetAndUpdate(v, 0);
                    backtrack = false;
                }

                sudo.GetDomain(v, out List<int> domain);
                //Console.WriteLine($"Domain {ListToString(domain)}");
                if (domain.Count > 0 && varTrials[i] < domain.Count)
                {
                    Console.WriteLine($"-->  Assigning {domain[varTrials[i]]}");
                    sudo.SetAndUpdate(v, domain[varTrials[i]]);
                    varTrials[i]++;
                    i++;
                    //Console.WriteLine("|  v.Trials: " + varTrials[i]);
                }
                else
                {
                    Console.WriteLine($"Resetting trials of tile ({v.X},{v.Y})");
                    varTrials[i] = 0;
                    backtrack = true;
                    i--;
                }
                expanded++;
                //Console.WriteLine(sudo);
            }
            if (i == -1) Console.WriteLine("No solution found, returning...");
            else Console.WriteLine("Solution found!");
            sudo.CloseVisualizer();
        }

    // FC

            public static void SolveLL(Sudoku sudo, out int expanded)
        {
            int varCount = sudo.VarCount;
            List<Point> variables = sudo.GetVariables();
            List<int> varTrials = new List<int>(varCount);
            for (int j = 0; j < varCount; j++)
            {
                varTrials.Add(0);
            }
            int i = 0;
            bool backtrack = false;
            expanded = 0;
            while (i >= 0 && i < varCount)
            {
                Point v = variables[i];

                if (backtrack)
                {
                    sudo.SetAndUpdate(v, 0);
                    backtrack = false;
                }

                sudo.GetDomain(v, out List<int> domain);
                if (domain.Count > 0)
                {
                    bool valueFound = false;
                    while (varTrials[i] < domain.Count)
                    {
                        sudo.SetAndUpdate(v, domain[varTrials[i]]);
                        varTrials[i]++;
                        if (!sudo.ExistsEmptyDomain())
                        {
                            valueFound = true;
                            break;
                        }
                        else
                        {
                            sudo.SetAndUpdate(v, 0);
                        }
                    }
                    if (valueFound)
                    {
                        i++;
                    }
                    else
                    {
                        varTrials[i] = 0;
                        backtrack = true;
                        i--;
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
        }

            public static void SolveTraceLL(Sudoku sudo, out int expanded)
        {
            sudo.StartVisualizer();
            int varCount = sudo.VarCount;
            List<Point> variables = sudo.GetVariables();
            List<int> varTrials = new List<int>(varCount);
            for (int j = 0; j < varCount; j++)
            {
                varTrials.Add(0);
            }
            int i = 0;
            bool backtrack = false;
            expanded = 0;
            while (i >= 0 && i < varCount)
            {
                Point v = variables[i];

                if (backtrack)
                {
                    sudo.SetAndUpdate(v, 0);
                    backtrack = false;
                }

                sudo.GetDomain(v, out List<int> domain);
                if (domain.Count > 0)
                {
                    bool valueFound = false;
                    while (varTrials[i] < domain.Count)
                    {
                        sudo.SetAndUpdate(v, domain[varTrials[i]]);
                        varTrials[i]++;
                        if (!sudo.ExistsEmptyDomain())
                        {
                            valueFound = true;
                            break;
                        }
                        else
                        {
                            sudo.SetAndUpdate(v, 0);
                        }
                    }
                    if (valueFound)
                    {
                        i++;
                    }
                    else
                    {
                        varTrials[i] = 0;
                        backtrack = true;
                        i--;
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
            if (i == -1) Console.WriteLine("No solution found, returning...");
            else Console.WriteLine("Solution found!");
            sudo.CloseVisualizer();
        }

            public static void SolveExtendedTraceLL(Sudoku sudo, out int expanded)
        {
            sudo.StartVisualizer();
            int varCount = sudo.VarCount;
            List<Point> variables = sudo.GetVariables();
            List<int> varTrials = new List<int>(varCount);
            for (int j = 0; j < varCount; j++)
            {
                varTrials.Add(0);
            }
            //Console.ReadKey();
            int i = 0;
            bool backtrack = false;
            expanded = 0;
            while (i >= 0 && i < varCount)
            {
                string res = "i= ";
                for (int j = 0; j < i; j++)
                {
                    res += "|";
                }
                res += " " + i;
                Console.WriteLine(res);

                Point v = variables[i];
                Console.WriteLine($"Tile {v.ToString()}");

                if (backtrack)
                {
                    Console.WriteLine("Backtrack, resetting Tile");
                    sudo.SetAndUpdate(v, 0);
                    backtrack = false;
                }

                sudo.GetDomain(v, out List<int> domain);
                //Console.WriteLine($"domain {ListToString(domain)}");
                //Console.WriteLine($"varTrials: {varTrials[v.X, v.Y]}");
                if (domain.Count > 0)
                {
                    bool valueFound = false;
                    while (varTrials[i] < domain.Count)
                    {
                        Console.WriteLine($"|  Trying {domain[varTrials[i]]}");
                        sudo.SetAndUpdate(v, domain[varTrials[i]]);
                        varTrials[i]++;
                        if (!sudo.ExistsEmptyDomain())
                        {
                            Console.WriteLine("|  Success");
                            valueFound = true;
                            break;
                        }
                        else
                        {
                            Console.WriteLine("|  Results in empty domain...");
                            sudo.SetAndUpdate(v, 0);
                        }
                    }
                    if (valueFound)
                    {
                        Console.WriteLine("Value found, proceeding");
                        i++;
                    }
                    else
                    {
                        Console.WriteLine("No value found, tracking back");
                        varTrials[i] = 0;
                        backtrack = true;
                        i--;
                    }
                }
                else
                {
                    Console.WriteLine("Empty domain, tracking back");
                    varTrials[i] = 0;
                    backtrack = true;
                    i--;
                }
                expanded++;
                //Console.ReadKey();
            }
            if (i == -1) Console.WriteLine("No solution found, returning...");
            else Console.WriteLine("Solution found!");
            sudo.CloseVisualizer();
        }

    // FC MCV

            public static void SolveLL(Sudoku sudo, out int expanded)
        {
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
                    sudo.SetAndUpdate(b, 0);
                    backtrack = false;
                }
                sudo.GetSmallestDomain(out List<int> domain, out Point v);
                if (domain.Count > 0)
                {
                    bool valueFound = false;
                    while (varTrials[v.X, v.Y] < domain.Count)
                    {
                        sudo.SetAndUpdate(v, domain[varTrials[v.X, v.Y]]);
                        varTrials[v.X, v.Y]++;
                        if (!sudo.ExistsEmptyDomain())
                        {
                            valueFound = true;
                            break;
                        }
                        else
                        {
                            sudo.SetAndUpdate(v, 0);
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
                }
                else
                {
                    varTrials[v.X, v.Y] = 0;
                    backtrack = true;
                    totaldone--;
                }
                expanded++;
            }
        }

            public static void SolveTraceLL(Sudoku sudo, out int expanded)
        {
            sudo.StartVisualizer();
            int[,] varTrials = new int[sudo.N, sudo.N];
            for (int x = 0; x < sudo.N; x++)
            {
                for (int y = 0; y < sudo.N; y++)
                {
                    varTrials[x, y] = 0;
                }
            }
            Stack<Point> prevs = new Stack<Point>();
            //Console.ReadKey();
            int totaldone = 0;
            bool backtrack = false;
            expanded = 0;
            while (totaldone >= 0 && totaldone < sudo.VarCount)
            {
                if (backtrack)
                {
                    Point b = prevs.Pop();
                    Console.WriteLine($"\nBacktrack, resetting Tile {b.ToString()}");
                    sudo.SetAndUpdate(b, 0);
                    backtrack = false;
                }

                Console.WriteLine($"\nTotaldone {totaldone}");
                sudo.GetSmallestDomain(out List<int> domain, out Point v);
                Console.WriteLine($"Var: {v.ToString()}");
                Console.WriteLine($"domain {Util.ListToString(domain)}");
                Console.WriteLine($"varTrials: {varTrials[v.X, v.Y]}");
                if (domain.Count > 0)
                {
                    bool valueFound = false;
                    while (varTrials[v.X, v.Y] < domain.Count)
                    {
                        Console.WriteLine($"|  Trying {domain[varTrials[v.X, v.Y]]}");
                        sudo.SetAndUpdate(v, domain[varTrials[v.X, v.Y]]);
                        varTrials[v.X, v.Y]++;
                        if (!sudo.ExistsEmptyDomain())
                        {
                            Console.WriteLine("|  Success");
                            valueFound = true;
                            break;
                        }
                        else
                        {
                            Console.WriteLine("|  Results in empty domain...");
                            sudo.SetAndUpdate(v, 0);
                        }
                    }
                    if (valueFound)
                    {
                        totaldone++;
                        Console.WriteLine("Value found, proceeding");
                        prevs.Push(v);
                    }
                    else
                    {
                        Console.WriteLine("No value found, tracking back");
                        varTrials[v.X, v.Y] = 0;
                        backtrack = true;
                        totaldone--;
                    }
                }
                else
                {
                    //Console.WriteLine("Empty domain, tracking back");
                    varTrials[v.X, v.Y] = 0;
                    backtrack = true;
                    totaldone--;
                }
                expanded++;
            }
            if (totaldone == -1) Console.WriteLine("No solution found, returning...");
            else Console.WriteLine("Solution found!");
            sudo.CloseVisualizer();
        }

            public static void SolveExtendedTraceLL(Sudoku sudo, out int expanded)
        {
            sudo.StartVisualizer();
            int[,] varTrials = new int[sudo.N, sudo.N];
            for(int x = 0; x < sudo.N; x++)
            {
                for(int y = 0; y < sudo.N; y++)
                {
                    varTrials[x, y] = 0;
                }
            }
            Stack<Point> prevs = new Stack<Point>();
            //Console.ReadKey();
            int totaldone = 0;
            bool backtrack = false;
            expanded = 0;
            while (totaldone >= 0 && totaldone < sudo.VarCount)
            {
                if (backtrack)
                {
                    Point b = prevs.Pop();
                    Console.WriteLine($"\nBacktrack, resetting Tile {b.ToString()}");
                    sudo.SetAndUpdate(b, 0);
                    backtrack = false;
                }

                Console.WriteLine($"\nTotaldone {totaldone}");
                sudo.GetSmallestDomain(out List<int> domain, out Point v);
                Console.WriteLine($"Var: {v.ToString()}");
                Console.WriteLine($"domain {Util.ListToString(domain)}");
                Console.WriteLine($"varTrials: {varTrials[v.X, v.Y]}");
                if (domain.Count > 0)
                {
                    bool valueFound = false;
                    while (varTrials[v.X, v.Y] < domain.Count)
                    {
                        Console.WriteLine($"|  Trying {domain[varTrials[v.X, v.Y]]}");
                        sudo.SetAndUpdate(v, domain[varTrials[v.X, v.Y]]);
                        varTrials[v.X, v.Y]++;
                        if (!sudo.ExistsEmptyDomain())
                        {
                            Console.WriteLine("|  Success");
                            valueFound = true;
                            break;
                        }
                        else
                        {
                            Console.WriteLine("|  Results in empty domain...");
                            sudo.SetAndUpdate(v, 0);
                        }
                    }
                    if (valueFound)
                    {
                        totaldone++;
                        Console.WriteLine("Value found, proceeding");
                        prevs.Push(v);
                    }
                    else
                    {
                        Console.WriteLine("No value found, tracking back");
                        varTrials[v.X, v.Y] = 0;
                        backtrack = true;
                        totaldone--;
                    }
                }
                else
                {
                    //Console.WriteLine("Empty domain, tracking back");
                    varTrials[v.X, v.Y] = 0;
                    backtrack = true;
                    totaldone--;
                }
                expanded++;
            }
            if (totaldone == -1) Console.WriteLine("No solution found, returning...");
            else Console.WriteLine("Solution found!");
            sudo.CloseVisualizer();
        }

    */

    /* Sudoku

    using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Sudoku2
{
    class Sudoku
    {
        #region Variables
        public readonly int N;
        public readonly int SqrtN;

        private Tile[,] data;              // Contains all values of all tiles of the sudoku. (data[x,y].Value: the value of tile x,y of the sudoku)

        public int VarCount;
        private List<Point> variables;
  
        private HashSet<int>[] Rows;
        private HashSet<int>[] Columns;
        private HashSet<int>[] Blocks;

        private SudokuVisualizer visualizer;

        private DomainLinkedList Domains;
        #endregion

        #region Constructors
        /// <summary>
        /// Generate a new Sudoku
        /// </summary>
        /// <param name="input">The input values for the sudoku</param>
        public Sudoku(int[,] input)
        {
            // Initialize readonly variables
            N = input.GetLength(0);
            SqrtN = (int)Math.Sqrt(N);

            VarCount = 0;
            variables = new List<Point>();

            data = new Tile[N, N];
            Rows = new HashSet<int>[N];
            Columns = new HashSet<int>[N];
            Blocks = new HashSet<int>[N];

            for (int i = 0; i < N; i++)
            {
                Rows[i] = new HashSet<int>();
                Columns[i] = new HashSet<int>();
                Blocks[i] = new HashSet<int>();
            }

            for(int bx = 0; bx < SqrtN; bx++)
            {
                for(int by = 0; by < SqrtN; by++)
                {
                    for (int x = 0; x < SqrtN; x++)
                    {
                        for (int y = 0; y < SqrtN; y++)
                        {
                            int val = input[
                                    (bx * SqrtN) + x,
                                    (by * SqrtN) + y
                                    ];

                            data[(bx * SqrtN) + x, (by * SqrtN) + y] = new Tile(
                                (bx * SqrtN) + x, 
                                (by * SqrtN) + y, 
                                (by * SqrtN) + bx,
                                val
                                );

                            Rows[(by * SqrtN) + y].Add(val);
                            Columns[(bx * SqrtN) + x].Add(val);
                            Blocks[(by * SqrtN) + bx].Add(val);

                            if (val == 0)
                            {
                                VarCount++;
                                variables.Add(new Point((bx * SqrtN) + x, (by * SqrtN) + y));
                            }
                        }
                    }
                }
            }
            InitDomains();
        }
        #endregion

        #region Helper Methodes
        private int PtoB(int x, int y)
        {
            return ((y / SqrtN) * SqrtN) + x / SqrtN;
        }

        private int PtoB(Point p)
        {
            return PtoB(p.X, p.Y);
        }

        public List<Point> GetVariables()
        {
            return variables;
        }
#endregion

#region Old-school Domain
public void SetTile(Point p, int value)
{
    if (useVis)
        SetVisualizerPoint(p, value);

    int x = p.X;
    int y = p.Y;
    int b = PtoB(x, y);

    int temp = data[x, y].Value;
    //Console.WriteLine($"  From {p.ToString()}, removing {temp}");
    Rows[y].Remove(temp);
    Columns[x].Remove(temp);
    Blocks[b].Remove(temp);

    //Console.WriteLine($"   Adding {value}");
    data[x, y].Value = value;
    Rows[y].Add(value);
    Columns[x].Add(value);
    Blocks[b].Add(value);
}

public bool CheckIfEmptyDomains(Point p)
{
    int x = p.X;
    int y = p.Y;
    int bx = x / SqrtN;
    int by = y / SqrtN;
    for (int i = 0; i < N; i++)
    {
        Point row = new Point(i, y);
        Point col = new Point(x, i);
        Point bl = new Point(bx * SqrtN + (i % SqrtN), by * SqrtN + (i / SqrtN));
        if (data[row.X, row.Y].Value == 0 && CalculateDomain(row).Count == 0) return true;
        if (data[col.X, col.Y].Value == 0 && CalculateDomain(col).Count == 0) return true;
        if (data[bl.X, bl.Y].Value == 0 && CalculateDomain(bl).Count == 0) return true;
    }
    return false;
}

public List<int> CalculateDomain(Point t)
{
    return CalculateDomain(t.X, t.Y);
}
public List<int> CalculateDomain(int x, int y)
{
    List<int> res = new List<int>();
    for (int i = 1; i <= N; i++)
    {
        //Console.WriteLine($"Trying {i} on block ({x},{y})");
        //if (Rows[y].Contains(i)) Console.WriteLine(i + " in row");
        //if (Columns[x].Contains(i)) Console.WriteLine(i + " in column");
        //if (Blocks[PtoB(x,y)].Contains(i)) Console.WriteLine(i + " in block");
        if (!(
            Rows[y].Contains(i) ||
            Columns[x].Contains(i) ||
            Blocks[PtoB(x, y)].Contains(i)
        ))
        {
            //Console.WriteLine(i + " not in relevant tiles, adding...");
            res.Add(i);
        }
    }
    return res;
}

public int GetSmallestDomainIndex(List<Point> var)
{
    int i = 0;
    while (data[var[i].X, var[i].Y].Value != 0)
    {
        i++;
    }
    int best = i;
    int bestdomain = CalculateDomain(var[i]).Count;
    while (i < var.Count)
    {
        Point v = var[i];
        if (data[v.X, v.Y].Value == 0)
        {
            int domaincount = CalculateDomain(v).Count;
            if (domaincount < bestdomain)
            {
                best = i;
                bestdomain = domaincount;
            }
        }
        i++;
    }
    return best;
}
#endregion

#region Linked List Domain
void InitDomains()
{
    Domains = new DomainLinkedList(N);
    for (int y = 0; y < N; y++)
    {
        for (int x = 0; x < N; x++)
        {
            if (data[x, y].Value == 0)
            {
                List<int> domain = CalculateDomain(x, y);
                data[x, y].DomainNode = Domains.InsertDomain(this, domain, new Point(x, y));
            }
        }
    }
}

public void SetAndUpdate(Point p, int value)
{
    if (useVis) SetVisualizerPoint(p, value);

    //Console.WriteLine($"{Domains.ToString()}");
    //Console.WriteLine("\t----");

    int x = p.X;
    int y = p.Y;
    int bx = x / SqrtN;
    int by = y / SqrtN;
    int b = PtoB(x, y);

    int temp = data[x, y].Value;
    //Console.WriteLine($"  From {p.ToString()}, removing {temp}");
    Rows[y].Remove(temp);
    Columns[x].Remove(temp);
    Blocks[b].Remove(temp);

    //Console.WriteLine($"   Adding {value}");
    data[x, y].Value = value;
    Rows[y].Add(value);
    Columns[x].Add(value);
    Blocks[b].Add(value);


    for (int i = 0; i < N; i++)
    {
        Point row = new Point(i, y);
        Point col = new Point(x, i);
        Point bl = new Point(bx * SqrtN + (i % SqrtN), by * SqrtN + (i / SqrtN));
        if (data[row.X, row.Y].DomainNode != null && i != x) Domains.UpdateDomain(data[row.X, row.Y].DomainNode, CalculateDomain(row), data[row.X, row.Y].Value == 0);
        if (data[col.X, col.Y].DomainNode != null && i != y) Domains.UpdateDomain(data[col.X, col.Y].DomainNode, CalculateDomain(col), data[col.X, col.Y].Value == 0);
        if (data[bl.X, bl.Y].DomainNode != null && (bl.X != x || bl.Y != y)) Domains.UpdateDomain(data[bl.X, bl.Y].DomainNode, CalculateDomain(bl), data[bl.X, bl.Y].Value == 0);
    }
    Domains.UpdateDomainFront(data[x, y].DomainNode, CalculateDomain(x, y), data[x, y].Value == 0);
    //Console.WriteLine($"{Domains.ToString()}");
}

public void GetDomain(Point point, out List<int> domain)
{
    domain = data[point.X, point.Y].DomainNode.Domain;
}

public void GetSmallestDomain(out List<int> domain, out Point point)
{
    DllNode node = Domains.GetNodeWithSmallestDomain();
    domain = node.Domain;
    point = new Point(node.Tile.X, node.Tile.Y);
}

public bool ExistsEmptyDomain()
{
    return Domains.EmptyDomainExists();
}

internal class DomainLinkedList
{
    public DllNode[] FrontEntries { get; private set; }
    public DllNode[] BackEntries { get; private set; }
    public int N;
    public DomainLinkedList(int n)
    {
        N = (n + 1);
        FrontEntries = new DllNode[N];
        BackEntries = new DllNode[N];
        for (int i = 0; i < N; i++)
        {
            FrontEntries[i] = new DllNode(true);
            BackEntries[i] = new DllNode(true);
            FrontEntries[i].Next = BackEntries[i];
            BackEntries[i].Prev = FrontEntries[i];
        }
    }

    public DllNode InsertDomain(Sudoku sudo, List<int> domain, Point coord)
    {
        int c = domain.Count;
        DllNode ins = new DllNode()
        {
            Domain = domain,
            Tile = sudo.data[coord.X, coord.Y],
            Entry = c
        };
        ins.Next = FrontEntries[c].Next;
        ins.Prev = FrontEntries[c];
        ins.Next.Prev = ins;
        ins.Prev.Next = ins;
        return ins;
    }

    public bool EmptyDomainExists()
    {
        return !FrontEntries[0].Next.IsSentry;
    }

    public DllNode GetNodeWithSmallestDomain()
    {
        int i = 1;
        DllNode curr = FrontEntries[i].Next;
        while (true)
        {
            if (curr.IsSentry) curr = FrontEntries[++i].Next;
            else if (curr.Tile.Value == 0) break;
            else curr = curr.Next;
        }
        return curr;
    }

    public void UpdateDomain(DllNode node, List<int> domain, bool readd = true)
    {
        node.Domain = domain;
        if (!(node.Entry == domain.Count))
        {
            //Console.WriteLine($"Removing node: ({node.Tile.X},{node.Tile.Y}), readd: {readd}");
            Remove(node);
            if (readd)
            {
                int c = domain.Count;
                node.Entry = c;
                node.Next = BackEntries[c];
                node.Prev = BackEntries[c].Prev;
                node.Prev.Next = node;
                node.Next.Prev = node;
            }
        }
        //else Console.WriteLine($"Removing ({node.Tile.X},{node.Tile.Y}) not neccessary (or is it)");
    }

    public void UpdateDomainFront(DllNode node, List<int> domain, bool readd = true)
    {
        node.Domain = domain;
        if (!(node.Entry == domain.Count))
        {
            //Console.WriteLine($"Removing node: ({node.Tile.X},{node.Tile.Y}), readd: {readd}");
            Remove(node);
            if (readd)
            {
                int c = domain.Count;
                node.Entry = c;
                node.Prev = FrontEntries[c];
                node.Next = FrontEntries[c].Next;
                node.Prev.Next = node;
                node.Next.Prev = node;
            }
        }
        //else Console.WriteLine($"Removing ({node.Tile.X},{node.Tile.Y}) not neccessary (or is it)");
    }

    void Remove(DllNode rem)
    {
        if (rem.Next != null) rem.Next.Prev = rem.Prev;
        if (rem.Prev != null) rem.Prev.Next = rem.Next;
        rem.Prev = null;
        rem.Next = null;
        rem.Entry = -1;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < N; i++)
        {
            sb.Append($"\tEntry {i}:");
            DllNode curr = FrontEntries[i].Next;
            while (!curr.Next.IsSentry)
            {
                sb.Append(curr.ToString());
                sb.Append(", ");
                curr = curr.Next;
            }
            sb.Append("\n");
        }
        return sb.ToString();
    }

}
public class DllNode
{
    public DllNode Prev;
    public DllNode Next;
    public List<int> Domain;
    public int Entry;
    internal Tile Tile;
    public readonly bool IsSentry;

    public DllNode(bool sentry = false)
    {
        IsSentry = sentry;
    }
    public override string ToString()
    {
        return Tile.ToString();
    }
}
#endregion

#region Visualizer
private bool useVis = false;
private bool visStarted = false;

public void StartVisualizer()
{
    visualizer = new SudokuVisualizer();
    visualizer.FormClosed += Visualizer_FormClosed;
    visualizer.Setup(N, data);
    Application.DoEvents();
    useVis = true;
    visStarted = true;
}

public void SetVisualizerPoint(Point t, int value)
{
    if (visStarted)
    {
        visualizer.SetDataPoint(t, value);
    }
    else
    {
        StartVisualizer();
        visualizer.SetDataPoint(t, value);
    }
    Application.DoEvents();
}

public void CloseVisualizer()
{
    visualizer.Close();
}

private void Visualizer_FormClosed(object sender, FormClosedEventArgs e)
{
    useVis = false;
    visStarted = false;
}
#endregion

#region ToString
/// <summary>
/// Converts the internal representation of the sudoku to a printable string version.
/// </summary>
/// <returns>The sudoku represented in a string.</returns>
public string ToString1()
{
    StringBuilder res = new StringBuilder();
    if (N < 10)
    {
        for (int i = 0; i < 2 * N + 1; i++)
        {
            res.Append("-");
        }
        res.Append("\n");
        for (int y = 0; y < N; y++)
        {
            res.Append("|");
            for (int x = 0; x < N; x++)
            {
                res.Append(data[x, y].Value);
                res.Append("|");
            }
            res.Append("\n");
        }
        for (int i = 0; i < 2 * N + 1; i++)
        {
            res.Append("-");
        }
    }
    else
    {
        for (int i = 0; i < 3 * N + 1; i++)
        {
            res.Append("-");
        }
        res.Append("\n");
        for (int y = 0; y < N; y++)
        {
            res.Append("|");
            for (int x = 0; x < N; x++)
            {
                if (data[x, y].Value < 10) res.Append(" ");
                res.Append(data[x, y].Value);
                res.Append("|");
            }
            res.Append("\n");
        }
        for (int i = 0; i < 3 * N + 1; i++)
        {
            res.Append("-");
        }
    }

    return res.ToString();
}

public override string ToString()
{
    StringBuilder res = new StringBuilder();
    for (int i = 0; i < (N * 4 + 1); i++)
    {
        res.Append("-");
    }
    res.Append("\n");
    for (int y = 0; y < N; y++)
    {
        res.Append("|");
        for (int x = 0; x < N; x++)
        {
            int val = data[x, y].Value;
            if (val < 10) res.Append(" ");
            res.Append(val);
            if (val < 100) res.Append(" ");
            res.Append("|");
        }
        res.Append("\n");
        for (int i = 0; i < (N * 4 + 1); i++)
        {
            res.Append("-");
        }
        res.Append("\n");
    }
    return res.ToString();
}
        #endregion
    }
    struct Tile
{
    public int X;
    public int Y;
    public int B;

    public int Value;
    public Sudoku.DllNode DomainNode;

    public Tile(int x, int y, int b, int v)
    {
        X = x;
        Y = y;
        B = b;

        Value = v;
        DomainNode = null;
    }

    public override string ToString()
    {
        return $"({X},{Y}), B{B}: Value {Value}";
    }
}

public class Point
{
    public int X;
    public int Y;

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override string ToString()
    {
        return $"({X},{Y})";
    }
}
}

    */

    /* NSudoku
     
    using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Sudoku2
{
    class NSudoku
    {
        #region Variables
        public readonly int N;
        public readonly int SqrtN;

        private NTile[,] data;              // Contains all values of all NTiles of the sudoku. (data[x,y].Value: the value of NTile x,y of the sudoku)

        public int VarCount;
        private List<Point> variables;

        private HashSet<int>[] Rows;
        private HashSet<int>[] Columns;
        private HashSet<int>[,] Blocks;

        private SudokuVisualizer visualizer;

        private DomainLinkedList Domains;
        #endregion

        #region Constructors
        /// <summary>
        /// Generate a new Sudoku
        /// </summary>
        /// <param name="input">The input values for the sudoku</param>
        public NSudoku(int[,] input)
        {
            // Initialize readonly variables
            N = input.GetLength(0);
            SqrtN = (int)Math.Sqrt(N);

            VarCount = 0;
            variables = new List<Point>();

            Domains = new DomainLinkedList(N);

            data = new NTile[N, N];
            Rows = new HashSet<int>[N];
            Columns = new HashSet<int>[N];
            Blocks = new HashSet<int>[SqrtN, SqrtN];

            for (int i = 0; i < N; i++)
            {
                Rows[i] = new HashSet<int>();
                Columns[i] = new HashSet<int>();
            }

            for(int bx = 0; bx < SqrtN; bx++)
            {
                for(int by = 0; by < SqrtN; by++)
                {
                    Blocks[bx, by] = new HashSet<int>();
                }
            }

            for (int bx = 0; bx < SqrtN; bx++)
            {
                for (int by = 0; by < SqrtN; by++)
                {
                    for (int x = 0; x < SqrtN; x++)
                    {
                        for (int y = 0; y < SqrtN; y++)
                        {
                            int val = input[
                                    (bx * SqrtN) + x,
                                    (by * SqrtN) + y
                                    ];

                            data[(bx * SqrtN) + x, (by * SqrtN) + y] = new NTile(
                                (bx * SqrtN) + x,
                                (by * SqrtN) + y,
                                (by * SqrtN) + bx,
                                val,
                                Domains,
                                N
                                );

                            Rows[(by * SqrtN) + y].Add(val);
                            Columns[(bx * SqrtN) + x].Add(val);
                            Blocks[bx, by].Add(val);
}
                    }
                }
            }
            for (int y = 0; y<N; y++)
            {
                for (int x = 0; x<N; x++)
                {
                    CalculateDomain(x, y);
                    if (data[x, y].Value == 0)
                    {
                        VarCount++;
                        variables.Add(new Point(x, y));
                    }
                }
            }
            //Console.WriteLine(Domains.ToString());
        }
        #endregion

        #region Helper Methodes
        private int PtoB(int x, int y)
{
    return ((y / SqrtN) * SqrtN) + x / SqrtN;
}

private void XyToBxBy(int x, int y, out int bx, out int by)
{
    bx = x / SqrtN;
    by = y / SqrtN;
}

private int PtoB(Point p)
{
    return PtoB(p.X, p.Y);
}

public List<Point> GetVariables()
{

    return variables;
}

public void Reset()
{
    for (int x = 0; x < N; x++)
    {
        for (int y = 0; y < N; y++)
        {
            if (!data[x, y].IsGiven) SetAndUpdate(new Point(x, y), 0);
        }
    }
}
#endregion

#region Old-school Domain
public void SetNTile(Point p, int value)
{
    if (useVis)
        SetVisualizerPoint(p, value);

    int x = p.X;
    int y = p.Y;
    XyToBxBy(x, y, out int bx, out int by);

    int temp = data[x, y].Value;
    //Console.WriteLine($"  From {p.ToString()}, removing {temp}");
    Rows[y].Remove(temp);
    Columns[x].Remove(temp);
    Blocks[bx, by].Remove(temp);

    //Console.WriteLine($"   Adding {value}");
    data[x, y].Value = value;
    Rows[y].Add(value);
    Columns[x].Add(value);
    Blocks[bx, by].Add(value);
}

public void CalculateDomain(Point t)
{
    CalculateDomain(t.X, t.Y);
}
public void CalculateDomain(int x, int y)
{
    NTile t = data[x, y];
    if (!t.IsGiven)
    {
        for (int i = 1; i <= N; i++)
        {
            //Console.WriteLine($"Trying {i} on block ({x},{y})");
            //if (Rows[y].Contains(i)) Console.WriteLine(i + " in row");
            //if (Columns[x].Contains(i)) Console.WriteLine(i + " in column");
            //if (Blocks[PtoB(x,y)].Contains(i)) Console.WriteLine(i + " in block");
            XyToBxBy(x, y, out int bx, out int by);
            if (!(
                Rows[y].Contains(i) ||
                Columns[x].Contains(i) ||
                Blocks[bx, by].Contains(i)
            ))
            {
                //Console.WriteLine(i + " not in relevant NTiles, adding...");
                t.Domain.Add(i);
            }
        }
    }
}
#endregion

#region Linked List Domain


public void SetAndUpdate(Point p, int value)
{
    if (useVis) SetVisualizerPoint(p, value);

    if (value == 0) throw new InvalidOperationException();
    //Console.WriteLine($"{Domains.ToString()}");
    //Console.WriteLine("\t----");

    int x = p.X;
    int y = p.Y;
    XyToBxBy(x, y, out int bx, out int by);

    //Console.WriteLine($"   Adding {value}");
    data[x, y].Value = value;
    Rows[y].Add(value);
    Columns[x].Add(value);
    Blocks[bx, by].Add(value);

    data[x, y].Domain.Hide();

    for (int i = 0; i < N; i++)
    {
        int xInBlock = bx * SqrtN + (i % SqrtN);
        int yInBlock = by * SqrtN + (i / SqrtN);

        if (!data[i, y].IsGiven) data[i, y].Domain.Remove(value);
        if (!data[x, i].IsGiven) data[x, i].Domain.Remove(value);
        if (!data[xInBlock, yInBlock].IsGiven) data[xInBlock, yInBlock].Domain.Remove(value);
    }

    //Console.WriteLine($"{Domains.ToString()}");
}

public void ResetAndUpdate(Point p)
{
    if (useVis) visualizer.SetDataPoint(p, 0);

    int x = p.X;
    int y = p.Y;
    XyToBxBy(x, y, out int bx, out int by);
    int value = data[x, y].Value;

    //Console.WriteLine($"{Domains.ToString()}");
    //Console.WriteLine("\t----");

    data[x, y].Value = 0;
    Rows[y].Remove(value);
    Columns[x].Remove(value);
    Blocks[bx, by].Remove(value);

    for (int i = 0; i < N; i++)
    {
        int xInBlock = bx * SqrtN + (i % SqrtN);
        int yInBlock = by * SqrtN + (i / SqrtN);

        //row y
        XyToBxBy(i, y, out int cbx, out int cby);
        if (!(Columns[i].Contains(value) || Blocks[cbx, cby].Contains(value)) && !data[i, y].IsGiven) data[i, y].Domain.Add(value);

        //column x
        XyToBxBy(x, i, out cbx, out cby);
        if (!(Rows[i].Contains(value) || Blocks[cbx, cby].Contains(value)) && !data[x, i].IsGiven) data[x, i].Domain.Add(value);

        //block (bx,by)
        if (!(Rows[yInBlock].Contains(value) || Columns[xInBlock].Contains(value)) && !data[xInBlock, yInBlock].IsGiven) data[xInBlock, yInBlock].Domain.Add(value);
    }

    data[x, y].Domain.Show();

    //Console.WriteLine($"{Domains.ToString()}");
}

public DomainHashSet GetDomain(Point p)
{
    return data[p.X, p.Y].Domain;
}

public DomainHashSet GetSmallestDomain(out Point point)
{
    DllNode node = Domains.GetNodeWithSmallestDomain();
    point = node.Coord;
    return data[point.X, point.Y].Domain;
}

public bool ExistsEmptyDomain()
{
    return Domains.EmptyDomainExists();
}
internal class DomainLinkedList
{
    public DllNode[] FrontEntries { get; private set; }
    public DllNode[] BackEntries { get; private set; }
    public DllNode FrontHiddenEntry { get; private set; }
    public DllNode BackHiddenEntry { get; private set; }
    public int N;
    public DomainLinkedList(int n)
    {
        N = (n + 1);
        FrontEntries = new DllNode[N];
        BackEntries = new DllNode[N];
        FrontHiddenEntry = new DllNode(true);
        BackHiddenEntry = new DllNode(true);
        FrontHiddenEntry.Next = BackHiddenEntry;
        BackHiddenEntry.Prev = FrontHiddenEntry;

        for (int i = 0; i < N; i++)
        {
            FrontEntries[i] = new DllNode(true);
            BackEntries[i] = new DllNode(true);
            FrontEntries[i].Next = BackEntries[i];
            BackEntries[i].Prev = FrontEntries[i];
        }
    }

    public DllNode Insert(Point coord, int c)
    {
        DllNode ins = new DllNode()
        {
            Coord = coord,
            Entry = c
        };
        ins.Next = FrontEntries[c].Next;
        ins.Prev = FrontEntries[c];
        ins.Next.Prev = ins;
        ins.Prev.Next = ins;
        return ins;
    }

    public bool EmptyDomainExists()
    {
        return !FrontEntries[0].Next.IsSentry;
    }

    public DllNode GetNodeWithSmallestDomain()
    {
        int i = 1;
        DllNode curr = FrontEntries[i].Next;
        while (true)
        {
            if (curr.IsSentry) curr = FrontEntries[++i].Next;
            else break;
        }
        return curr;
    }

    public void MoveUp(DllNode node)
    {
        node.Entry--;
        if (node.IsHidden) return;
        Remove(node);
        node.Next = BackEntries[node.Entry];
        node.Prev = BackEntries[node.Entry].Prev;
        node.Prev.Next = node;
        node.Next.Prev = node;
    }

    public void MoveDown(DllNode node)
    {
        node.Entry++;
        if (node.IsHidden) return;
        Remove(node);
        node.Next = BackEntries[node.Entry];
        node.Prev = BackEntries[node.Entry].Prev;
        node.Prev.Next = node;
        node.Next.Prev = node;
    }

    public void Hide(DllNode node)
    {
        Remove(node);
        node.Next = FrontHiddenEntry.Next;
        node.Prev = FrontHiddenEntry;
        node.Next.Prev = node;
        node.Prev.Next = node;
        node.IsHidden = true;
    }

    public void Show(DllNode node)
    {
        Remove(node);
        node.Next = FrontEntries[node.Entry].Next;
        node.Prev = FrontEntries[node.Entry];
        node.Prev.Next = node;
        node.Next.Prev = node;
        node.IsHidden = false;
    }

    void Remove(DllNode rem)
    {
        if (rem.Next != null) rem.Next.Prev = rem.Prev;
        if (rem.Prev != null) rem.Prev.Next = rem.Next;
        rem.Prev = null;
        rem.Next = null;
    }


    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < N; i++)
        {
            sb.Append($"\tEntry {i}:");
            DllNode curr = FrontEntries[i].Next;
            while (!curr.IsSentry)
            {
                sb.Append(curr.ToString());
                sb.Append(", ");
                curr = curr.Next;
            }
            sb.Append("\n");
        }
        return sb.ToString();
    }

}
internal class DllNode
{
    public DllNode Prev;
    public DllNode Next;
    public int Entry;
    public Point Coord;
    public readonly bool IsSentry;
    public bool IsHidden;

    public DllNode(bool sentry = false)
    {
        IsSentry = sentry;
    }

    public override string ToString()
    {
        return Coord.ToString();
    }
}
#endregion

#region Visualizer
private bool useVis = false;
private bool visStarted = false;

public void StartVisualizer()
{
    visualizer = new SudokuVisualizer();
    visualizer.FormClosed += Visualizer_FormClosed;
    visualizer.NSetup(N, data);
    Application.DoEvents();
    useVis = true;
    visStarted = true;
}

public void SetVisualizerPoint(Point t, int value)
{
    if (visStarted)
    {
        visualizer.SetDataPoint(t, value);
    }
    else
    {
        StartVisualizer();
        visualizer.SetDataPoint(t, value);
    }
    Application.DoEvents();
}

public void CloseVisualizer()
{
    visualizer.Close();
}

private void Visualizer_FormClosed(object sender, FormClosedEventArgs e)
{
    useVis = false;
    visStarted = false;
}
#endregion

#region ToString
/// <summary>
/// Converts the internal representation of the sudoku to a printable string version.
/// </summary>
/// <returns>The sudoku represented in a string.</returns>
public string ToString1()
{
    StringBuilder res = new StringBuilder();
    if (N < 10)
    {
        for (int i = 0; i < 2 * N + 1; i++)
        {
            res.Append("-");
        }
        res.Append("\n");
        for (int y = 0; y < N; y++)
        {
            res.Append("|");
            for (int x = 0; x < N; x++)
            {
                res.Append(data[x, y].Value);
                res.Append("|");
            }
            res.Append("\n");
        }
        for (int i = 0; i < 2 * N + 1; i++)
        {
            res.Append("-");
        }
    }
    else
    {
        for (int i = 0; i < 3 * N + 1; i++)
        {
            res.Append("-");
        }
        res.Append("\n");
        for (int y = 0; y < N; y++)
        {
            res.Append("|");
            for (int x = 0; x < N; x++)
            {
                if (data[x, y].Value < 10) res.Append(" ");
                res.Append(data[x, y].Value);
                res.Append("|");
            }
            res.Append("\n");
        }
        for (int i = 0; i < 3 * N + 1; i++)
        {
            res.Append("-");
        }
    }

    return res.ToString();
}

public override string ToString()
{
    StringBuilder res = new StringBuilder();
    for (int i = 0; i < (N * 4 + 1); i++)
    {
        res.Append("-");
    }
    res.Append("\n");
    for (int y = 0; y < N; y++)
    {
        res.Append("|");
        for (int x = 0; x < N; x++)
        {
            int val = data[x, y].Value;
            if (val < 10) res.Append(" ");
            res.Append(val);
            if (val < 100) res.Append(" ");
            res.Append("|");
        }
        res.Append("\n");
        for (int i = 0; i < (N * 4 + 1); i++)
        {
            res.Append("-");
        }
        res.Append("\n");
    }
    return res.ToString();
}
#endregion
public class DomainHashSet : HashSet<int>
{
    private DomainLinkedList Dll;
    private DllNode Node;
    internal DomainHashSet(int size, DomainLinkedList dll, DllNode node) : base(size)
    {
        Dll = dll;
        Node = node;
    }
    public new void Remove(int rem)
    {
        if (base.Remove(rem))
        {
            Dll.MoveUp(Node);
        }
    }

    public new void Add(int add)
    {
        if (base.Add(add))
        {
            Dll.MoveDown(Node);
        }
    }

    public void Hide()
    {
        Dll.Hide(Node);
    }

    public void Show()
    {
        Dll.Show(Node);
    }
}

internal struct NTile
{
    public Point Coord;
    public int B;

    public int Value;
    //public DllNode DomainNode;
    public DomainHashSet Domain;
    public bool IsGiven;

    public NTile(int x, int y, int b, int v, DomainLinkedList dll, int n)
    {
        Coord = new Point(x, y);
        B = b;

        Value = v;
        IsGiven = v != 0;
        Domain = (!IsGiven) ? new DomainHashSet(n, dll, dll.Insert(Coord, 0)) : null;
    }

    public override string ToString()
    {
        return $"({Coord.ToString()}), B{B}: Value {Value}";
    }
}
    }
}

     */
