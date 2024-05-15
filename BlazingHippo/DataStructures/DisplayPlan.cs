using HercAndHippoLibCs;
using static BlazingHippo.Constants;

namespace BlazingHippo;

internal record DisplayDiff(int Col, int Row, IConsoleDisplayable? OldDisplayable, IConsoleDisplayable? NewDisplayable)
{
    public bool AppearanceChanged => NewDisplayable?.Color != OldDisplayable?.Color ||
        NewDisplayable?.BackgroundColor != OldDisplayable?.BackgroundColor ||
        NewDisplayable?.ConsoleDisplayString != OldDisplayable?.ConsoleDisplayString;
}

internal class DisplayPlan
{
    private readonly IConsoleDisplayable[,] planArray;
    private readonly IEnumerable<DisplayDiff>? diffs;
    private const int MAX_COL = GAME_WIDTH - VIEW_MARGIN;
    private const int MAX_ROW = GAME_HEIGHT - VIEW_MARGIN;
    public static readonly Location ScreenCenter = ((GAME_WIDTH - VIEW_MARGIN) / 2, (GAME_HEIGHT - VIEW_MARGIN) / 2);

    public DisplayPlan(Level state, ScrollStatus scrollStatus)
    {
        IConsoleDisplayable[,] planArray = new IConsoleDisplayable[GAME_WIDTH, GAME_HEIGHT];
        diffs = null;
        Location logicalCenter = scrollStatus.LogicalCenter;
        
        foreach (IConsoleDisplayable levelObj in state.LevelObjects.Where(obj => obj is IConsoleDisplayable).Cast<IConsoleDisplayable>())
        {
            int writeCol = ScreenCenter.Col - logicalCenter.Col + levelObj.Location.Col; 
            int writeRow = ScreenCenter.Row - logicalCenter.Row + levelObj.Location.Row;
            if (writeCol >= MIN_DISPLAY_COL && writeCol < GAME_WIDTH - VIEW_MARGIN && 
                writeRow >= MIN_DISPLAY_ROW && writeRow < GAME_HEIGHT - VIEW_MARGIN)
            {
                planArray[writeCol, writeRow] = levelObj;
            }
        }

        this.planArray = planArray;
    }

    public IEnumerable<DisplayDiff> GetDiffs(DisplayPlan newDisplayPlan)
    {
        bool calculated = false;

        while (!calculated)
        {
            try
            {
                List<DisplayDiff> diffs = [];
                IConsoleDisplayable[,] oldDisplay = this.planArray;
                IConsoleDisplayable[,] newDisplay = newDisplayPlan.planArray;
                
                for (int row = 0; row < MAX_ROW; row++)
                {
                    for (int col = 0; col < MAX_COL; col++)
                    {
                        IConsoleDisplayable? oldDisp = oldDisplay[col, row];
                        IConsoleDisplayable newDisp = newDisplay[col, row];
                        if (newDisp != default && oldDisp != newDisp)
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
        try
        {
            var enumerable = diffs ?? Array.Empty<DisplayDiff>();
            foreach (var diff in enumerable)
            {
                int row = diff.Row;
                int col = diff.Col;
                IConsoleDisplayable? oldDisp = diff.OldDisplayable;
                IConsoleDisplayable? newDisp = diff.NewDisplayable;
                if (newDisp != default && oldDisp != newDisp)
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
            return false;
        }
        finally
        {
            Console.SetCursorPosition(1, Console.BufferHeight - 1); // Move the cursor so it doesn't always appear next to the player
        }
    } // end method RefreshDisplay()
} // end struct DisplayPlan