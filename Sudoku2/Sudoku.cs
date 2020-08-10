using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Sudoku2
{
    class Sudoku
    {
        #region Variables
        public readonly int N;                          // The size of the sudoku, so an instance of this class represents an N x N sudoku
        public readonly int SqrtN;                      // The square root of N, which is (amongst others) the size of the sudoku in blocks (so a sudoku has SqrtN x SqrtN = N blocks)
        public readonly int VarCount;                   // The amount of variables

        private readonly Tile[,] data;                  // Contains all values of all Tiles of the sudoku. (data[x,y].Value: the value of Tile x,y of the sudoku)
        private readonly List<Point> variables;         // During construction of the class, this List gets filled with the coordinates of all variables (ie Tiles with initial value 0)
        private readonly HashSet<int>[] rows;           // This HashSet represents the values already assigned in each row, such that Rows[i].Contains(a) returns true if some Tile in row i has value a
        private readonly HashSet<int>[] columns;        // This HashSet represents the values already assigned in each column, such that Columns[i].Contains(a) returns true if some Tile in column i has value a
        private readonly HashSet<int>[,] blocks;        // This HashSet represents the values already assigned in each block, such that Blocks[x,y].Contains(a) returns true if some Tile in block (x,y) has value a
        private readonly DomainLinkedList domains;      // This data structure holds (pointers to) all domains, and sorts them on domain size
                                                        // For more info on this custom data structure, see our report and the comments within the class

        private SudokuVisualizer visualizer;            // The visualizer
        #endregion

        #region Constructors
        /// <summary>
        /// Generate a new Sudoku
        /// </summary>
        /// <param name="input">The input values for the sudoku</param>
        public Sudoku(int[,] input)
        {
            N = input.GetLength(0);                     // Thankfully a Sudoku is always square
            SqrtN = (int)Math.Sqrt(N);                  // By calculating this only once, we save a lot of redundant calculations
            VarCount = 0;

            data = new Tile[N, N];                      // Initializing all data structures, to be filled up later
            variables = new List<Point>();              
            rows = new HashSet<int>[N];
            columns = new HashSet<int>[N];
            blocks = new HashSet<int>[SqrtN, SqrtN];
            domains = new DomainLinkedList(N);

            for (int i = 0; i < N; i++)
            {
                rows[i] = new HashSet<int>();
                columns[i] = new HashSet<int>();
            }

            for (int bx = 0; bx < SqrtN; bx++)
            {
                for(int by = 0; by < SqrtN; by++)
                {
                    blocks[bx, by] = new HashSet<int>();
                }
            }

            for (int y = 0; y < N; y++)                                         
            {
                for (int x = 0; x < N; x++)
                {
                    XyToBxBy(x, y, out int bx, out int by);                     // This nice helper method converts a (x,y) point of a Tile to a (bx,by) point of the block the Tiles resides in
                    int val = input[x, y];                                      // Extract the value of the right Tile
                    data[x, y] = new Tile(x, y, bx, by, val, domains, N);       // Generate the Tile and put it in our data array

                    rows[y].Add(val);                                           // Let's not forget the hashsets, we'll need them
                    columns[x].Add(val);
                    blocks[bx, by].Add(val);
                }
            }
            for (int y = 0; y < N; y++)                                         // We need this second nested for-loop because we can only calculate the domain of a Tile when all Tiles are filled
            {
                for (int x = 0; x < N; x++)
                {
                    CalculateAndStoreDomain(x, y);                              // Calculate the domain of the variable at (x,y), and immediately store it at the right places
                    if (data[x,y].Value == 0)                                   // If a Tile is not yet filled in, we've found a variable!
                    {
                        VarCount++;                                             
                        variables.Add(new Point(x, y));                         // So add it to the List
                    }
                }
            }
        }
        #endregion

        #region Helper Methodes
        /// <summary>
        /// Convert a (x,y) point of a Tile to a (bx,by) point of a block the Tile resides in.
        /// </summary>
        /// <param name="x">The x-coordinate of the Tile</param>
        /// <param name="y">The y-coordinate of the Tile</param>
        /// <param name="bx">the x-coordinate of the block</param>
        /// <param name="by">the y-coordinate of the block</param>
        private void XyToBxBy(int x, int y, out int bx, out int by)             
        {
            bx = x / SqrtN;             // We can use the behaviour of the /(int, int)-operator to our advantage here, since a Tile with X-coordinate 2 still has a block X-coordinate of 0
            by = y / SqrtN;             // Plus, it's super speedy!
        }

        /// <summary>
        /// Gets the variables of the Sudoku.
        /// </summary>
        /// <returns>A List containing the coordinates of the variables</returns>
        public List<Point> GetVariables()
        {
            return variables;
        }
        #endregion

        #region Old-school Domain
        /// <summary>
        /// Sets a tile of the sudoku to the specified value.
        /// </summary>
        /// <param name="p">The coordinates of the tile that gets the new value</param>
        /// <param name="value">The value to which the tile needs it value changed</param>
        public void SetTile(Point p, int value)
        {
            if (useVis) SetVisualizerPoint(p, value);   // Set the corresponding label of the visualizer to the same value (if we are using the visualizer)

            int x = p.X;                    // Get the coordinates from the point
            int y = p.Y;
            XyToBxBy(x, y, out int bx, out int by);

            int temp = data[x, y].Value;    // Get the current value

            rows[y].Remove(temp);           // Remove the value from the row, column and block
            columns[x].Remove(temp);
            blocks[bx, by].Remove(temp);

            data[x, y].Value = value;       // Add the value to the tile, row, column and block
            rows[y].Add(value);
            columns[x].Add(value);
            blocks[bx, by].Add(value);
        }

        /// <summary>
        /// Returns the domain of the tile on the provided coordinate.
        /// </summary>
        /// <param name="t">The coordinates of the tile</param>
        /// <returns>The domain of tile on coordinates t</returns>
        public List<int> CalculateDomainList(Point t) { return CalculateDomainList(t.X, t.Y); }

        /// <summary>
        /// Returns the domain of the tile on the provided coordinate.
        /// </summary>
        /// <param name="x">The x coordinate of the tile</param>
        /// <param name="y">The y coordiante of the tile</param>
        /// <returns>The domain of tile on coordinates x, y</returns>
        public List<int> CalculateDomainList(int x, int y)
        {
            List<int> res = new List<int>();                    // Initialize domainlist
            if (!data[x,y].IsGiven)                             // Only create domain if the tile is not a fixed value
            {
                for (int i = 1; i <= N; i++)                    // Check for every number 1 to N if it occurs in the...
                {
                    XyToBxBy(x, y, out int bx, out int by);
                    if (!(
                        rows[y].Contains(i) ||                  // ...row...
                        columns[x].Contains(i) ||               // ...column....
                        blocks[bx, by].Contains(i)              // ... or block the tile is located in
                    ))
                    {
                        res.Add(i);                             // Than add it to the domainlist
                    }
                }
            }
            return res;                                         // Return the domain
        }

        /// <summary>
        /// Checks if there are empty domains in the row, column or block a tile is located in.
        /// </summary>
        /// <param name="p">The coordinates of the tile</param>
        /// <returns>True if there is atleast one empty domain, false otherwise</returns>
        public bool CheckIfEmptyDomains(Point p)
        {
            int x = p.X;        // Get coordinates of p
            int y = p.Y;
            int bx = x / SqrtN;
            int by = y / SqrtN;
            for (int i = 0; i < N; i++)
            {
                Point row = new Point(i, y);    // There are always N-1 other tiles in the row, column and block
                Point col = new Point(x, i);    // So we create a point for each of those points...
                Point bl = new Point(bx * SqrtN + (i % SqrtN), by * SqrtN + (i / SqrtN));
                if (data[row.X, row.Y].Value == 0 && CalculateDomainList(row).Count == 0) return true;  // ...and we calculate the domain for each of those points
                if (data[col.X, col.Y].Value == 0 && CalculateDomainList(col).Count == 0) return true;  // When we encounter a domain which is empty...
                if (data[bl.X, bl.Y].Value == 0 && CalculateDomainList(bl).Count == 0) return true;     // ...we immediatelly return true for finding an empty domain
            }
            return false;                       // Otherwise we return false
        }

        /// <summary>
        /// For a list of points returns the index to the point which gives the coordinates to the tile with the smallest domain.
        /// </summary>
        /// <param name="var">The list of points out of which we need the point that represents the tile with the smalles domain</param>
        /// <returns>The index of the point with the smallest domain</returns>
        public int GetSmallestDomainIndex(List<Point> var)
        {
            int i = 0;
            while (data[var[i].X, var[i].Y].Value != 0)             // Skip variables that already have a value assigned
            {
                i++;
            }
            int best = i;                                           // Get the index of the first unassigned variable
            int bestdomain = CalculateDomainList(var[i]).Count;     // Get the domain of the first unassigned variable
            while (i < var.Count)
            {
                Point v = var[i];                                   // Get the next variable
                if (data[v.X, v.Y].Value == 0)                      // Check if it is unassigned
                {
                    int domaincount = CalculateDomainList(v).Count; // Calculate the domain for of the variable
                    if (domaincount < bestdomain)                   // If the domain size is smaller than the current best domain size...
                    {
                        best = i;                   // ...set the best index to current index
                        bestdomain = domaincount;   // ...set the best domain size to current domain size
                    }
                }
                i++;
            }
            return best;    // Return the index of the variable with the smalles domain size
        }
        #endregion

        #region Linked List Domain
        /// <summary>
        /// Set the value of the Tile at point p to value, and update the domains accordingly.
        /// </summary>
        /// <param name="p">The Point of the Tile to change the value of</param>
        /// <param name="value">The value to give the Tile</param>
        public void SetAndUpdate(Point p, int value) 
        {
            if (useVis) SetVisualizerPoint(p, value);                                        // If we use the visualizer, tell it to update the value of this point

            if (value == 0) throw new InvalidOperationException();                           // If you want to set the value to a Tile to 0, you need to use ResetAndUpdate
                                                                                             // as this needs some specific operations
            int x = p.X;
            int y = p.Y;
            XyToBxBy(x, y, out int bx, out int by);                                          // Get the coordinates of the according block

            data[x, y].Value = value;                                                        // Set the value in our data array
            rows[y].Add(value);                                                              // Update the HashSets
            columns[x].Add(value);      
            blocks[bx, by].Add(value);

            data[x, y].Domain.Hide();                                                        // If a Tile has a value (ie not 0) assigned to it, we don't want this Tile 
                                                                                             // to be picked based on its domain size. For more info on this operation, 
                                                                                             // read our report and look at the comments within DomainHashSet
            for (int i = 0; i < N; i++)                                                                             // If a Tile at (x,y) in block (bx,by) gets its value (v) assigned, the domains of each
            {                                                                                                       // Tile in row y, column x and block (bx,by) need to not include v anymore as it is "taken".
                int xInBlock = bx * SqrtN + (i % SqrtN);                                                            // For i in (0,N), (xInBlock_i, yInBlock_i) iterates over all points within block (bx,by)
                int yInBlock = by * SqrtN + (i / SqrtN);                                                            // so that we can change the domains of each Tile in that block. Together with (x, i) and
                                                                                                                    // (i, y), we can change the domains of all necessary Tiles in one single for-loop. Note 
                if (!data[i,y].IsGiven) data[i, y].Domain.Remove(value);                                            // that we only update the domains of a Tile if it is not IsGiven (ie it is a variable),
                if (!data[x,i].IsGiven) data[x, i].Domain.Remove(value);                                            // because constant (given) Tiles have their Domain property set to null.
                if (!data[xInBlock, yInBlock].IsGiven) data[xInBlock, yInBlock].Domain.Remove(value);
            }
        }

        /// <summary>
        /// Reset the value of the Tile at point p (i.e. set the value back to 0 or unassigned).
        /// </summary>
        /// <param name="p">The Point of the Tile to reset the value of</param>
        public void ResetAndUpdate(Point p)
        {                                   
            if (useVis) visualizer.SetDataPoint(p, 0);                                      // If we use the visualizer, tell it to update the value of this point

            int x = p.X;
            int y = p.Y;
            XyToBxBy(x, y, out int bx, out int by);                                         // Get the coordinates of the according block
            int value = data[x, y].Value;                                                   // Get the previous value of the Tile

            data[x, y].Value = 0;                                                           // Reset the value in our data array
            rows[y].Remove(value);                                                          // Update the HashSets
            columns[x].Remove(value);
            blocks[bx, by].Remove(value);

            for (int i = 0; i < N; i++)                                                     // Again update the Tiles in column x, row y and block (bx,by)
            {
                int xInBlock = bx * SqrtN + (i % SqrtN);
                int yInBlock = by * SqrtN + (i / SqrtN);
                                                                                            // Only this time, we may have to add the previous value to the domains. of all Tiles row y, column x and block (bx,by)
                //row y                                                                     // We may however only add this value to the domain of a Tile at (x',y') if that value isn't yet assigned somewhere
                XyToBxBy(i, y, out int cbx, out int cby);                                   // in *its* row, column or block (so row y', column x' and block (bx', by'))
                if (!(columns[i].Contains(value) || blocks[cbx, cby].Contains(value)) && !data[i, y].IsGiven) data[i, y].Domain.Add(value);

                //column x
                XyToBxBy(x, i, out cbx, out cby);
                if (!(rows[i].Contains(value) || blocks[cbx, cby].Contains(value)) && !data[x, i].IsGiven) data[x, i].Domain.Add(value);

                //block (bx,by)
                if (!(rows[yInBlock].Contains(value) || columns[xInBlock].Contains(value)) && !data[xInBlock, yInBlock].IsGiven) data[xInBlock, yInBlock].Domain.Add(value);
            }

            data[x, y].Domain.Show();                                                       // The Tile is again unassigned, so we need to be able to pick it based on its domain size.
        }
        
        /// <summary>
        /// Calculate the domain of the Tile at point (x,y) from scratch, and save it.
        /// </summary>
        /// <param name="x">The x-coordinate of the Tile</param>
        /// <param name="y">The y-coordinate of the Tile</param>
        private void CalculateAndStoreDomain(int x, int y)
        {
            Tile t = data[x, y];                                                            // Get the current Tile
            if (!t.IsGiven)                                                                 // If the Tile is given, we don't need to store a domain
            {
                for (int i = 1; i <= N; i++)                                                // We're doing a in in (1,N] for-loop since we're looking at the value of i specificly
                {   
                    XyToBxBy(x, y, out int bx, out int by);                                 
                    if (!(                                                                  // For a value i in {1,...,N}, we can add i to the domain if
                        rows[y].Contains(i) ||                                                      // i doesn't yet exist in row y, and
                        columns[x].Contains(i) ||                                                   // i doesn't yet exist in column x, and
                        blocks[bx, by].Contains(i)                                                  // i doesn't yet exist in block (bx, by)
                    ))  
                    {
                        t.Domain.Add(i);
                    }
                }
            }
        }

        /// <summary>
        /// Get the domain of the Tile at Point p.
        /// </summary>
        /// <param name="p">The location of the Tile</param>
        /// <returns>the DomainHashSet of the domain of the Tile at Point p</returns>
        public DomainHashSet GetDomain(Point p)
        {
            return data[p.X, p.Y].Domain;
        }

        /// <summary>
        /// Get the domain of the Tile that has the smallest domain. 
        /// </summary>
        /// <param name="point">The point</param>
        /// <returns></returns>
        public DomainHashSet GetSmallestDomain(out Point point)
        {
            DllNode node = domains.GetNodeWithSmallestDomain();
            point = node.Coord;
            return data[point.X, point.Y].Domain;
        }

        /// <summary>
        /// Checks whether there exists some Tile that has an empty domain.
        /// </summary>
        /// <returns>true if there exists some Tile that has an empty domain, false if not</returns>
        public bool ExistsEmptyDomain()
        {
            return domains.ExistsEmptyDomain();
        }

        /// <summary>
        /// This HashSet-derived class is made to both keep track of the domain of a Tile, and to pass the operations done on the HashSet along to a DomainHashSet instance.
        /// </summary>
        public class DomainHashSet : HashSet<int>
        {
            private readonly DomainLinkedList Dll;          // Internal reference to the DomainLinkedList
            private readonly DllNode Node;                  // Internal reference to the DllNode that this DomainHashSet corresponds to
            internal DomainHashSet(int size, DomainLinkedList dll, DllNode node) : base(size)
            {
                Dll = dll;
                Node = node;
            }
            public new void Remove(int rem)
            {
                if (base.Remove(rem))                       // base.Remove(val) returns true if a val existed in the HashSet. Only then do we need 
                {                                           // to inform the DomainLinkedList   
                    Dll.MoveUp(Node);
                }
            }

            public new void Add(int add)
            {
                if (base.Add(add))                         // In a similar manner, base.Add(val) return true if val didn't yet exist in the HashSet. 
                {                                          // Only then do we need to inform the DomainLinkedList
                    Dll.MoveDown(Node);
                }
            }

            public void Hide()                              // Letting the Hide() and Show() go through the DomainHashSet makes thinks cleaner within the Tile,
            {                                               // since we don't need to keep references to DllNodes therein
                Dll.Hide(Node);
            }

            public void Show()
            {
                Dll.Show(Node);
            }
        }

        /// <summary>
        /// This data structure keeps book of the size of the domains of each Tile, and sorts them in an online manner.
        /// </summary>
        internal class DomainLinkedList
        {   
            /* The DomainLinkedList is in fact a collection of N + 1 small bidirectional sublinkedlists. Each of those linked lists (entries) keeps references to all the Tiles
             * with domains of the size of i, with i being the entry number. We can enter an entry from the front or the back, using FrontEntries or BackEntries respectively.
             * The linked lists are comprised of DllNodes, which contain a reference (Point) to its corresponding Tile, so that that DllNode has an entry number equal to the 
             * size of the domain of that Tile.
             */ 
            public DllNode[] FrontEntries { get; private set; }     // We can enter the linked list containing references to all the Tiles with domain size i with FrontEntries[i]
            public DllNode[] BackEntries { get; private set; }      // Idem for BackEntries[i]. So FrontEntries[i] and BackEntries[i] are linked together, initially directly. 
                                                                    // If a DllNode gets inserted in entry i, it will be placed between FrontEntries[i] and BackEntries[i]
            public DllNode FrontHiddenEntry { get; private set; }   // Between FrontHiddenEntry and BackHiddenEntry are placed all Hidden DllNodes, referencing to Tiles that
            public DllNode BackHiddenEntry { get; private set; }    // are variable but have a value assigned to them. This way we wont count them in the smallest domain search.

            private readonly int N;                                 // The number of entries. Equal to Sudoku.N + 1

            /// <summary>
            /// Generate a new DomainLinkedList.
            /// </summary>
            /// <param name="n">The maximum size of the domain</param>
            public DomainLinkedList(int n)
            {
                N = (n + 1);
                FrontEntries = new DllNode[N];
                BackEntries = new DllNode[N];
                FrontHiddenEntry = new DllNode(true);
                BackHiddenEntry = new DllNode(true);
                FrontHiddenEntry.Next = BackHiddenEntry;            // Initially the FrontHiddenEntry and BackHiddenEntry are linked together
                BackHiddenEntry.Prev = FrontHiddenEntry;

                for (int i = 0; i < N; i++)
                {
                    FrontEntries[i] = new DllNode(true);
                    BackEntries[i] = new DllNode(true);
                    FrontEntries[i].Next = BackEntries[i];          // Idem for FrontEntries[i] and BackEntries[i], for all i
                    BackEntries[i].Prev = FrontEntries[i];
                }
            }

            /// <summary>
            /// Insert a new node in the LinkedList.
            /// </summary>
            /// <param name="coord">The coordinates of the Tile this node will correspond to</param>
            /// <param name="c">The size of the domain of the Tile at coord</param>
            /// <returns></returns>
            public DllNode Insert(Point coord, int c)
            {
                DllNode ins = new DllNode()
                {
                    Coord = coord,
                    Entry = c
                };
                ins.Next = FrontEntries[c].Next;                    // We insert the generated DllNode at the front of the sublinkedlist, 
                ins.Prev = FrontEntries[c];                         // using the standard linkedlist-insert method
                ins.Next.Prev = ins;
                ins.Prev.Next = ins;
                return ins;
            }

            /// <summary>
            /// Checks whether there exists and empty domain.
            /// </summary>
            /// <returns>true if there exists and empty domain, false if not</returns>
            public bool ExistsEmptyDomain()                     
            {
                return !FrontEntries[0].Next.IsSentry;              // There exists an empty domain in the entire Sudoku iff there is a (non-Sentry) DllNode
                                                                    // in entry 0.
            }

            /// <summary>
            /// Gets the node referencing the Tile with the smallest domain in the sudoku.
            /// </summary>
            /// <returns>The DllNode referencing the Tile with the smallest domain in the sudoku</returns>
            public DllNode GetNodeWithSmallestDomain()
            {
                int i = 1;                                              // i represents the entry we're currently looking at. We don't want to look at empty domains as they are irrelevant here
                DllNode curr = FrontEntries[i].Next;                    // We want the next DllNode of FrontEntries[i] as FrontEntries[i] itself is a Sentry node
                while (curr.IsSentry) curr = FrontEntries[++i].Next;    // If the current node is also a Sentry, we know that entry i is currently empty and we need to look at the next entry.
                                                                        // We repeat this until we find a nonempty entry. 
                return curr;
            }

            /// <summary>
            /// Moves the DllNode node up one entry.
            /// </summary>
            /// <param name="node">The DllNode to move up</param>
            public void MoveUp(DllNode node)
            {
                node.Entry--;                                           // For bookkeeping
                if (node.IsHidden) return;                              // If the node is hidden, we don't want to move it from the HiddenEntries-chain
                Remove(node);                                           // Remove the node
                node.Next = BackEntries[node.Entry];                    // And readd it to the right entry
                node.Prev = BackEntries[node.Entry].Prev;
                node.Prev.Next = node;
                node.Next.Prev = node;
            }

            public void MoveDown(DllNode node)
            {
                node.Entry++;                                           // For bookeeping
                if (node.IsHidden) return;                              // If the node is hidden, we don't want to move it from the HiddenEntries-chain
                Remove(node);                                           // Remove the node
                node.Next = BackEntries[node.Entry];                    // And readd it to the right entry
                node.Prev = BackEntries[node.Entry].Prev;
                node.Prev.Next = node;
                node.Next.Prev = node;
            }

            /// <summary>
            /// Hides the node from the domain-size sorting process.
            /// </summary>
            /// <param name="node">The DllNode to hide</param>
            public void Hide(DllNode node)
            {
                Remove(node);                                           // We remove the node from its current entry
                node.Next = FrontHiddenEntry.Next;                      // And add it to the HiddenEntries-chain
                node.Prev = FrontHiddenEntry;
                node.Next.Prev = node;
                node.Prev.Next = node;
                node.IsHidden = true;                                   // So that we don't accidentally move it up or down above
            }

            /// <summary>
            /// Makes the node visible again for the domain-size sorting process.
            /// </summary>
            /// <param name="node">The DllNode to solve</param>
            public void Show(DllNode node)
            {
                Remove(node);                                           // We remove the node from the HiddenEntries-chain
                node.Next = FrontEntries[node.Entry].Next;              // And add it to its corresponding entry. Note that 
                node.Prev = FrontEntries[node.Entry];                   // we add it to the front (like a Stack) instead of the back (like a Queue)
                node.Prev.Next = node;                                  
                node.Next.Prev = node;
                node.IsHidden = false;
            }

            /// <summary>
            /// Removes the node from its current (sub)linkedlist.
            /// </summary>
            /// <param name="node">The node to remove</param>
            void Remove(DllNode node)
            {
                node.Next.Prev = node.Prev;
                node.Prev.Next = node.Next;
                node.Prev = null;
                node.Next = null;
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

        /// <summary>
        /// Items for within a DomainLinkedList
        /// </summary>
        internal class DllNode
        {
            public DllNode Prev;                        // References to the 'previous' DllNode in the DomainLinkedList
            public DllNode Next;                        // References to the 'next' DllNode in the DomainLinkedList
            public Point Coord;                         // The coordinates of the Tile this Node corresponds to
            public int Entry;                           // The entry this node is inserted. Is also equal to the size of the domain of the Tile at Coord
            public bool IsHidden;                       // True if the DllNode is hidden and thus inserted in the HiddenEntries in the DomainLinkedList

            public readonly bool IsSentry;              // True if the DllNode is a sentry, so a first or last element in an entry LinkedList.

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
        private bool useVis = false;        // Set to true when we use a solver with trace
        private bool visStarted = false;    // Used to determine whether we need to Setup the visualizer before setting a point

        /// <summary>
        /// Intitializes a new visualizer and sets it up with the current data.
        /// </summary>
        public void StartVisualizer()
        {
            visualizer = new SudokuVisualizer();
            visualizer.FormClosed += Visualizer_FormClosed;
            visualizer.Setup(N, data);
            Application.DoEvents();         // DoEvents() since this is a console application and the console needs to keep on running, but we still want to update the UI thread
            useVis = true;
            visStarted = true;
        }

        /// <summary>
        /// Sets the value of a label of the visualizer to the value specified.
        /// </summary>
        /// <param name="t">The coordinates of the label that needs its value changed</param>
        /// <param name="value">The value to which the label needs to be changed</param>
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
            Application.DoEvents();         // Update UI
        }

        /// <summary>
        /// Closes the visualizer
        /// </summary>
        public void CloseVisualizer() { visualizer.Close(); }

        /// <summary>
        /// Resets the variables when the visualizer is closed
        /// </summary>
        private void Visualizer_FormClosed(object sender, FormClosedEventArgs e)
        {
            useVis = false;
            visStarted = false;
        }
        #endregion

        #region ToString
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

        #region Tile
        /// <summary>
        /// Represents a tile within a Sudoku.
        /// </summary>
        internal struct Tile
        {
            public Point Coord;                 // The location in the Sudoku
            public Point BlockCoord;            // The location of the block the Tile is in

            public int Value;                   // The value of the Tile
            public DomainHashSet Domain;        // The domain of the Tile
            public bool IsGiven;                // Is true, the value of the Tile is constant

            /// <summary>
            /// Generates an instance of a Tile.
            /// </summary>
            /// <param name="x">The x-coordinate within the Sudoku</param>
            /// <param name="y">The y-coordinate within the Sudoku</param>
            /// <param name="bx">The x-coordinate within the block the Tile is in</param>
            /// <param name="by">The y-coordinate within the block the Tile is in</param>
            /// <param name="v">The initial value of the Tile. Note: if not 0, the Tile will be marked as IsGiven</param>
            /// <param name="dll">The DomainLinkedList, for creating a DomainHashSet</param>
            /// <param name="n">The size of the Sudoku. Will be passed to the DomainHashSet</param>
            public Tile(int x, int y, int bx, int by, int v, DomainLinkedList dll, int n)
            {
                Coord = new Point(x, y);
                BlockCoord = new Point(bx, by);

                Value = v;
                IsGiven = v != 0;
                Domain = (!IsGiven) ? new DomainHashSet(n, dll, dll.Insert(Coord, 0)) : null;
            }

            public override string ToString()
            {
                return $"{Coord.ToString()}, Block {BlockCoord.ToString()}: Value {Value}";
            }
        }
        #endregion
    }
}

