using HercAndHippoLibCs;
using static HercAndHippoConsole.Constants;

namespace HercAndHippoConsole
{
    internal class BufferStats
    {
        public bool BufferSizeChanged { get; private set; }
        public int BufferWidth { get; private set; }
        public int BufferHeight { get; private set; }
        public BufferStats(bool bufferSizeChanged, int bufferWidth, int bufferHeight)
        {
            BufferSizeChanged = bufferSizeChanged;
            BufferWidth = bufferWidth;
            BufferHeight = bufferHeight;
        }
        public void Refresh()
        {
            int newBH = Console.BufferHeight;
            int newBW = Console.BufferWidth;

            BufferSizeChanged = BufferHeight != newBH || BufferWidth != newBW;
            BufferWidth = newBW;
            BufferHeight = newBH;
       
        } 
    }

    internal readonly struct DisplayPlan
    {
        private readonly IDisplayable[,] planArray;
        private readonly BufferStats bufferStats;
        public static Location GetScreenCenter(int bufferWidth, int bufferHeight)
            => ((bufferWidth - VIEW_MARGIN) / 2, (bufferHeight - VIEW_MARGIN) / 2);

        public DisplayPlan(Level state, ScrollStatus scrollStatus, BufferStats bufferStats)
        {
            IDisplayable[,] planArray = new IDisplayable[bufferStats.BufferWidth, bufferStats.BufferHeight];           
            Location screenCenter = GetScreenCenter(bufferStats.BufferWidth, bufferStats.BufferHeight);
            Location logicalCenter = scrollStatus.LogicalCenter;

            foreach (IDisplayable toShow in state.LevelObjects)
            {
                // Note that these are ints, not instances of the Column/Row type.
                // If writeCol and writeRow were column/row respectively,
                // they would "bottom out" at 1. This would cause a bug where
                // things that should disappear off the left edge of the screen
                // would just hang out in column 1.
                int writeCol = screenCenter.Col - logicalCenter.Col + toShow.Location.Col; 
                int writeRow = screenCenter.Row - logicalCenter.Row + toShow.Location.Row;
                if (writeCol >= MIN_DISPLAY_COL && writeCol < bufferStats.BufferWidth - VIEW_MARGIN && 
                    writeRow >= MIN_DISPLAY_ROW && writeRow < bufferStats.BufferHeight - VIEW_MARGIN)
                {
                    planArray[writeCol, writeRow] = toShow;
                }
            }

            this.planArray = planArray;
            this.bufferStats = bufferStats;
        }

        public void RefreshDisplay(Level newState, ScrollStatus newScrollStatus)
        {
            DisplayPlan newDisplayPlan = new(newState, newScrollStatus, bufferStats);
            bool forceRefresh = bufferStats.BufferSizeChanged;
            
            var oldDisplay = this.planArray;
            var newDisplay = newDisplayPlan.planArray;

            int maxCol = bufferStats.BufferWidth - VIEW_MARGIN;
            int maxRow = bufferStats.BufferHeight - VIEW_MARGIN;

            // Rather than using the cached maxCol and maxRow values calculated above,
            // the following method recalculates the buffer width and height when it is needed
            // to prevent attempting to set the cursor position to an offscreen location (which throws an exception).
            static bool InView(int col, int row) 
                => col < Console.BufferWidth - VIEW_MARGIN && row < Console.BufferHeight - VIEW_MARGIN;

            if (forceRefresh) Console.Clear();
            for (int row = 0; row < maxRow; row++)
            {
                for (int col = 0; col < maxCol; col++)
                {
                    IDisplayable oldDisp = oldDisplay[col, row];
                    IDisplayable newDisp = newDisplay[col, row];
                    if ((newDisp != default && (forceRefresh || (oldDisp != newDisp))) &&
                        InView(col, row))
                    {
                        // Something is here that wasn't here before, so show it
                        Console.SetCursorPosition(col, row);
                        Console.BackgroundColor = newDisp.BackgroundColor;
                        Console.ForegroundColor = newDisp.Color;
                        Console.Write(newDisp.ConsoleDisplayString);
                    }
                    if ((newDisp == default && oldDisp != default) &&
                        InView(col, row))
                    {
                        // Something used to be here, but now nothing is here, so clear the spot
                        Console.SetCursorPosition(col, row);
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write(" ");
                    }
                } // end for (columns)
            } // end for (rows)
        } // end method RefreshDisplay()
    } // end struct DisplayPlan
} // end namespace
