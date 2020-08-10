namespace Sudoku2
{
    /// <summary>
    /// Used to parse sudokus from text files.
    /// </summary>
    static class Parser
    {
        /// <summary>
        /// Parses 9x9 Sudokus from a text file
        /// </summary>
        /// <param name="dir">The directory of the text file</param>
        /// <param name="numSudos">The number of sudokus to parse</param>
        /// <returns>An array of parsed Sudokus</returns>
        public static Sudoku[] Parse9(string dir, int numSudos)
        {
            string text = System.IO.File.ReadAllText(dir);
            Sudoku[] sudos = new Sudoku[numSudos];
            string[] lines = text.Split('\n');
            for (int i = 0; i < numSudos; i++)
            {
                int start = i * 10;
                int[,] sudo = new int[9, 9];
                for (int y = 0; y < 9; y++)
                {
                    string line = lines[start + y + 1];
                    for (int x = 0; x < 9; x++)
                    {
                        int curr = (int)char.GetNumericValue(line[x]);
                        sudo[x, y] = curr;
                    }
                }
                sudos[i] = new Sudoku(sudo);
            }
            return sudos;
        }

        /// <summary>
        /// Parses 16x16 Sudokus from a text file
        /// </summary>
        /// <param name="dir">The directory of the text file</param>
        /// <param name="numSudos">The number of sudokus to parse</param>
        /// <returns>An array of parsed Sudokus</returns>
        public static Sudoku[] Parse16(string dir, int numSudos)
        {
            string text = System.IO.File.ReadAllText(dir);
            Sudoku[] sudos = new Sudoku[numSudos];
            string[] lines = text.Split('\n');
            for (int i = 0; i < numSudos; i++)
            {
                int start = i * 17;
                int[,] sudo = new int[16, 16];
                for (int y = 0; y < 16; y++)
                {
                    string line = lines[start + y];
                    string[] lineSplit = line.Split(' ');
                    for (int x = 0; x < 16; x++)
                    {
                        int curr = int.Parse(lineSplit[x]);
                        sudo[x, y] = curr;
                    }
                }
                sudos[i] = new Sudoku(sudo);
            }
            return sudos;
        }
    }
}
