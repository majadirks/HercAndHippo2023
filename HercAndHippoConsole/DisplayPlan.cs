using HercAndHippoLibCs;
using static HercAndHippoConsole.Constants;

namespace HercAndHippoConsole;

internal record DisplayDiff(int Col, int Row, IConsoleDisplayable? OldDisplayable, IConsoleDisplayable? NewDisplayable)
{
    public bool AppearanceChanged => NewDisplayable?.Color != OldDisplayable?.Color ||
        NewDisplayable?.BackgroundColor != OldDisplayable?.BackgroundColor ||
        NewDisplayable?.ConsoleDisplayString != OldDisplayable?.ConsoleDisplayString;
}

internal class DisplayPlan
{
    private readonly IConsoleDisplayable[,] planArray;
    private readonly BufferStats bufferStats;
    private IEnumerable<DisplayDiff>? diffs;
    private static Dictionary<DisplayDiff, IEnumerable<DisplayDiff>> diffCache;

    public static Location GetScreenCenter(int bufferWidth, int bufferHeight)
        => ((bufferWidth - VIEW_MARGIN) / 2, (bufferHeight - VIEW_MARGIN) / 2);

    public DisplayPlan(Level state, ScrollStatus scrollStatus, BufferStats bufferStats)
    {
        IConsoleDisplayable[,] planArray = new IConsoleDisplayable[bufferStats.BufferWidth, bufferStats.BufferHeight];
        diffs = null;
        diffCache = new();
        Location screenCenter = GetScreenCenter(bufferStats.BufferWidth, bufferStats.BufferHeight);
        Location logicalCenter = scrollStatus.LogicalCenter;
        
        foreach (IConsoleDisplayable levelObj in state.LevelObjects.Where(obj => obj is IConsoleDisplayable).Cast<IConsoleDisplayable>())
        {
            // Note that these are ints, not instances of the Column/Row type.
            // If writeCol and writeRow were column/row respectively,
            // they would "bottom out" at 1. This would cause a bug where
            // things that should disappear off the left edge of the screen
            // would just hang out in column 1.
            int writeCol = screenCenter.Col - logicalCenter.Col + levelObj.Location.Col; 
            int writeRow = screenCenter.Row - logicalCenter.Row + levelObj.Location.Row;
            if (writeCol >= MIN_DISPLAY_COL && writeCol < bufferStats.BufferWidth - VIEW_MARGIN && 
                writeRow >= MIN_DISPLAY_ROW && writeRow < bufferStats.BufferHeight - VIEW_MARGIN)
            {
                planArray[writeCol, writeRow] = levelObj;
            }
        }

        this.planArray = planArray;
        this.bufferStats = bufferStats;
    }

    public IEnumerable<DisplayDiff> GetDiffs(DisplayPlan newDisplayPlan)
    {
        bool calculated = false;

        while (!calculated)
        {
            try
            {
                // ToDo: tune capacity with some sort of moving average
                List<DisplayDiff> diffs = new();
                bool forceRefresh = bufferStats.BufferSizeChanged;

                IConsoleDisplayable[,] oldDisplay = this.planArray;
                IConsoleDisplayable[,] newDisplay = newDisplayPlan.planArray;

                int maxCol = bufferStats.BufferWidth - VIEW_MARGIN;
                int maxRow = bufferStats.BufferHeight - VIEW_MARGIN;
                for (int row = 0; row < maxRow; row++)
                {
                    for (int col = 0; col < maxCol; col++)
                    {
                        IConsoleDisplayable? oldDisp = forceRefresh ? null : oldDisplay[col, row];
                        IConsoleDisplayable newDisp = newDisplay[col, row];
                        if (newDisp != default && (forceRefresh || oldDisp != newDisp))
                        {
                            var diff = new DisplayDiff(Col: col, Row: row, OldDisplayable: oldDisp, NewDisplayable: newDisp);
                            // Something is here that wasn't here before, so plan to show it
                            if (diff.AppearanceChanged)
                                diffs.Add(diff);
                        }
                        if (newDisp == default && oldDisp != default)
                        {
                            // Something used to be here, but now nothing is here,
                            // so plan to clear the spot
                            diffs.Add(new DisplayDiff(Col: col, Row: row, OldDisplayable: oldDisp, NewDisplayable: null));
                        }
                    } // end for (columns)
                } // end for (rows)
                calculated = true;
                return diffs;
            }
            catch
            {
                calculated = false;
                bufferStats.ForceRefresh();
            } 
        }
        if (this.diffs == null)
            throw new Exception("Unexpected exception.");
        return diffs;
    }

    /// <summary>
    /// Assuming this instance of DisplayPlan is represented in the console display,
    /// update the console by comparing this DisplayPlan to the given newDisplayPlan
    /// and changing anything that needs to be changed.
    /// If this is done successfully, return true. Return false
    /// if an exception is encountered. eg if the Console size changed.
    /// </summary>
    public bool RefreshDisplay(IEnumerable<DisplayDiff> diffs)
    {
        bool forceRefresh = bufferStats.BufferSizeChanged;

        if (forceRefresh)
        {
            DisplayUtilities.ResetConsoleColors();
            Console.Clear();
        }
        try
        {
            var enumerable = diffs ?? Array.Empty<DisplayDiff>();
            foreach (var diff in enumerable)
            {
                int row = diff.Row;
                int col = diff.Col;
                IConsoleDisplayable? oldDisp = diff.OldDisplayable;
                IConsoleDisplayable? newDisp = diff.NewDisplayable;
                if (newDisp != default && (forceRefresh || oldDisp != newDisp))
                {
                    // Something is here that wasn't here before, so show it
                    Console.SetCursorPosition(col, row);
                    Console.BackgroundColor = newDisp.BackgroundColor.ToConsoleColor();
                    Console.ForegroundColor = newDisp.Color.ToConsoleColor();
                    Console.Write(newDisp.ConsoleDisplayString);
                }
                if ((newDisp == null || newDisp == default) && oldDisp != default)
                {
                    // Something used to be here, but now nothing is here, so clear the spot
                    Console.SetCursorPosition(col, row);
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write(" ");
                }
            }
            return true;
        }
        catch
        {
            DisplayUtilities.ResetConsoleColors();
            Console.Clear();
            return false;
        }
        finally
        {
            Console.SetCursorPosition(1, Console.BufferHeight - 1); // Move the cursor so it doesn't always appear next to the player
        }
    } // end method RefreshDisplay()
} // end struct DisplayPlan