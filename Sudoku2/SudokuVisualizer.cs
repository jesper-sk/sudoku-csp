using System.Drawing;
using System.Windows.Forms;

namespace Sudoku2
{
    /// <summary>
    /// Can be used to visualize a sudoku, has built in animations that enhance perception of changes made by an algorithm.
    /// </summary>
    public partial class SudokuVisualizer : Form
    {
        private int size;           // the width and height of the sudoku, thus how many labels wide and high
        private Label[,] labelData; // 2-dimensional label array, representing the tiles of the sudoku
        private Point lastUpdate;   // What tile was updated last time

        public SudokuVisualizer()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets up the visualizer: Creating the labels that represent the tiles and giving them initial values.
        /// </summary>
        /// <param name="s">The size of the sudoku that needs to be visualized</param>
        /// <param name="data">The initial data of the sudoku used to give initial values to labels</param>
        internal void Setup(int s, Sudoku.Tile[,] data)
        {
            size = s;                               // Set the size of the sudoku, thus the amount of labels we need as width and height
            int stepsize = Width / size;            // Determine the amount of pixels each label has a width and height
            ClientSize = new Size(Width, Height);   // Set the Clientsize as the size provided in the designer

            int x = 0;
            int y = 0;
            labelData = new Label[size, size];      // Create the 2-dimensional array in which we will put the labels
            for (int i = 0; i < size; i++)          // Start the row creation
            {
                y = stepsize * i;                   // Get y coordinate corresponding to the row we are creating
                for (int j = 0; j < size; j++)      // Create each item in the row
                {
                    x = stepsize * j;                                           // Get the x
                    int value = data[j, i].Value;                               // Get the value corresponding to the coordinate from the sudoku
                    Label tile = new Label                                      // Create the label
                    {
                        Location = new System.Drawing.Point(x, y),
                        Size = new Size(stepsize, stepsize),
                        TextAlign = ContentAlignment.MiddleCenter,
                        Text = value == 0 ? "" : value.ToString(),              // Initially we are only going to show the fixed values
                        ForeColor = value == 0 ? Color.Green : Color.Red,       // Variables get green text, fixed values red
                        Font = new Font("Arial", Font.Size, FontStyle.Bold)
                    };
                    Controls.Add(tile);
                    labelData[j, i] = tile;
                }
            }
            Show(); // Show the visualizer
        }

        /// <summary>
        /// Changes the value of the label that is on the coordinates of the provided point to the value specified.
        /// Will change the background of the label to cyan to make it stand out.
        /// </summary>
        /// <param name="data">The coordinates of the label that needs his value changed</param>
        /// <param name="value">The value to which the label needs to be changed</param>
        internal void SetDataPoint(Point data, int value)
        {
            labelData[data.X, data.Y].BackColor = Color.Cyan;                                               // Change background for visualization
            labelData[data.X, data.Y].Text = value == 0 ? "" : value.ToString();                            // Remove text if value is zero
            if (lastUpdate != null) labelData[lastUpdate.X, lastUpdate.Y].BackColor = SystemColors.Control; // Reset the background of the last label updated
            lastUpdate = data;                                                                              // Set current label as last updated label
        }
    }
}
