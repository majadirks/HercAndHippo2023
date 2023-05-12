using HercAndHippoLibCs;
using static HercAndHippoConsole.Constants;

namespace HercAndHippoConsole
{

    internal record BufferStats(bool BufferSizeChanged, int BufferWidth, int BufferHeight)
    {
        public BufferStats Refresh()
        {
            bool changed = BufferHeight != Console.BufferHeight || BufferWidth != Console.BufferWidth;
            return new(changed, BufferWidth: Console.BufferWidth, BufferHeight: Console.BufferHeight);
        } 
    }

    internal record DisplayPlan(IDisplayable[,] PlanArray)
    {    
        public static Location GetScreenCenter(int bufferWidth, int bufferHeight)
            => ((bufferWidth - VIEW_MARGIN) / 2, (bufferHeight - VIEW_MARGIN) / 2);

        public static DisplayPlan CreateDisplayPlan(Level state, ScrollStatus scrollStatus, BufferStats bufferStats)
        {
            IDisplayable[,] ToShow = new IDisplayable[bufferStats.BufferWidth, bufferStats.BufferHeight];

            Location screenCenter = GetScreenCenter(bufferStats.BufferWidth, bufferStats.BufferHeight);
            Location logicalCenter = scrollStatus.LogicalCenter;

            foreach (IDisplayable toShow in state.LevelObjects)
            {
                // Note that these are ints, not instances of the Column/Row type.
                // If writeCol and writeRow were column/row respectively,
                // we would use the "max out" addition logic, which isn't what we want.
                int writeCol = screenCenter.Col - logicalCenter.Col + toShow.Location.Col; 
                int writeRow = screenCenter.Row - logicalCenter.Row + toShow.Location.Row;
                if (writeCol >= MIN_COL && writeCol < bufferStats.BufferWidth - VIEW_MARGIN && 
                    writeRow >= MIN_ROW && writeRow < bufferStats.BufferHeight - VIEW_MARGIN)
                {
                    ToShow[writeCol, writeRow] = toShow;
                }
            }
            return new DisplayPlan(PlanArray: ToShow);
        }

        public void RefreshDisplay(Level state, ScrollStatus scrollStatus, BufferStats bufferStats)
        {
            DisplayPlan newDisplayPlan = CreateDisplayPlan(state, scrollStatus, bufferStats);
            bool forceRefresh = bufferStats.BufferSizeChanged;

            var oldDisplay = this.PlanArray;
            var newDisplay = newDisplayPlan.PlanArray;

            int maxCol = (forceRefresh ? Console.BufferWidth : bufferStats.BufferWidth) - VIEW_MARGIN;
            int maxRow = (forceRefresh ? Console.BufferHeight : bufferStats.BufferHeight) - VIEW_MARGIN;

            // Rather than using the cached maxCol and maxRow values calculated above,
            // the following method recalculates the buffer width and height when it is needed
            // to prevent attempting to set the cursor position to an offscreen location (which throws an exception).
            bool InView(int col, int row) => col < Console.BufferWidth - VIEW_MARGIN && row < Console.BufferHeight - VIEW_MARGIN;

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
                        Console.ForegroundColor = newDisp.Color;
                        Console.Write(newDisp.ConsoleDisplayString);
                    }
                    if ((newDisp == default && (forceRefresh || oldDisp != default)) &&
                        InView(col, row))
                    {
                        // Something used to be here, but now nothing is here, so clear the spot
                        Console.SetCursorPosition(col, row);
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write(" ");
                    }
                }
            }
        }
    }
}
