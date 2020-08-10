using System;
using System.Collections.Generic;
using System.Text;

namespace LatexFormatting
{
    /// <summary>
    /// A class that generates a Latex tabular.
    /// </summary>
    class LatexTabularMaker
    {
        private readonly StringBuilder sb;              // Will contain the tabular in string format

        public readonly int NumColumns; 
        public bool IsClosed { get; private set; }      // If the table is closed, we can't add any more rows
        public int NumRows { get; private set; }        

        /// <summary>
        /// Generates a LatexTabularMaker
        /// </summary>
        /// <param name="numColumns">The number of columns</param>
        public LatexTabularMaker(int numColumns)
        {
            sb = new StringBuilder();
            NumColumns = numColumns;
            NumRows = 0;
            IsClosed = false;

            sb.Append($@"\begin{{tabular}}{{*{{{NumColumns}}}{{|c}}|}} \hline ");
        }

        /// <summary>
        /// Add a row of entries to the tabular. Should be less than or equal to the NumColumns.
        /// </summary>
        /// <param name="entries">The array containing the entries</param>
        /// <param name="isHeader">If true, the tabular inserts a double hline below this row</param>
        public void AddRow(string[] entries, bool isHeader = false)
        {
            if (IsClosed) throw new InvalidOperationException("Table is closed");

            int numEntries = entries.Length;                                                                    // We first check whether we have more entries than columns
            if (numEntries > NumColumns) throw new InvalidOperationException("Too many entries");

            int columnsPerEntry = NumColumns / numEntries;                                                      // We support an entry size smaller than the column size

            if (columnsPerEntry == 1) for (int i = 0; i < (numEntries - 1); i++) sb.Append($"{entries[i]} & ");
            else for (int i = 0; i < (numEntries - 1); i++) sb.Append($@"\multicolumn{{{columnsPerEntry}}}{{|c}}{{{entries[i]}}} & ");

            int rest = NumColumns - (columnsPerEntry * (numEntries - 1));
            if (rest == 1) sb.Append($"{entries[numEntries - 1]}");
            else sb.Append($@"\multicolumn{{{rest}}}{{|c|}}{{{entries[numEntries - 1]}}}");

            sb.Append(@"\\ \hline ");
            if (isHeader) sb.Append(@"\hline ");
            NumRows++;
        }

        /// <summary>
        /// Add a row of entries to the tabular, with a set number of columns per entry. The total number of columns should sum up to less than or equal to the NumColumns.
        /// </summary>
        /// <param name="entries">The array containing the entries</param>
        /// <param name="columns">The array containg the number of columns per entry, such that the number of columns of entries[i] is equal to columns[i] for all i</param>
        /// <param name="isHeader">If true, the tabular inserts a double hline below this row</param>
        public void AddMultiColumnRow(string[] entries, int[] columns, bool isHeader = false)
        {
            if (IsClosed) throw new InvalidOperationException("Table is closed");
            int totColumns = 0;
            foreach (int c in columns) totColumns += c;
            if (totColumns > NumColumns) throw new InvalidOperationException("Invalid number of columns");

            int numEntries = entries.Length;
            if (columns.Length != entries.Length) throw new InvalidOperationException("Invalid number of columns"); 
            for (int i = 0; i < numEntries - 1; i++) sb.Append($@"\multicolumn{{{columns[i]}}}{{|c}}{{{entries[i]}}} & ");
            sb.Append($@"\multicolumn{{{columns[numEntries - 1]}}}{{|c|}}{{{entries[numEntries - 1]}}}");

            sb.Append(@"\\ \hline ");
            if (isHeader) sb.Append(@"\hline ");
            NumRows++;
        }

        /// <summary>
        /// Closes the table.
        /// </summary>
        public void CloseTable()
        {
            if (!IsClosed) sb.Append("\\end{tabular}");
            IsClosed = true;
        }

        /// <summary>
        /// Closes the tables, and returns the final product as a string.
        /// </summary>
        /// <returns>The tabular as a string</returns>
        public string ReturnTable()
        {
            CloseTable();
            return ToString();
        }

        public override string ToString()
        {
            return sb.ToString();
        }
    }
}

namespace Sudoku2
{
    /// <summary>
    /// A static class containing all kings of utilities.
    /// </summary>
    static class Util
    {
        public static string ListToString(List<int> l)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            if (l.Count != 0)
            {
                sb.Append(l[0]);
                for (int j = 1; j < l.Count; j++)
                {
                    int i = l[j];
                    sb.Append($", {i}");
                }
            }
            sb.Append("}");
            return sb.ToString();
        }

        public static string ArrToString(int[] l)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            if (l.Length != 0)
            {
                sb.Append(l[0]);
                for (int j = 1; j < l.Length; j++)
                {
                    int i = l[j];
                    sb.Append($", {i}");
                }
            }
            sb.Append("}");
            return sb.ToString();
        }

        public static string ArrToString(string[] l)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            if (l.Length != 0)
            {
                sb.Append(l[0]);
                for (int j = 1; j < l.Length; j++)
                {
                    string i = l[j];
                    sb.Append($", {i}");
                }
            }
            sb.Append("}");
            return sb.ToString();
        }


        public static string DHSToSTring(Sudoku.DomainHashSet dhs, int n)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            for(int i = 1; i < n; i++)
            {
                if (dhs.Contains(i)) sb.Append($"{i}, ");
            }
            if (dhs.Contains(n)) sb.Append($"{n}");
            sb.Append("}");
            return sb.ToString();
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
