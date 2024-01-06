using HercAndHippoLibCs;
using static HercAndHippoConsole.Constants;

namespace HercAndHippoConsole;

internal class DisplayPlan
{
    private readonly IConsoleDisplayable[,] planArray;
    private readonly BufferStats bufferStats;
    public static Location GetScreenCenter(int bufferWidth, int bufferHeight)
        => ((bufferWidth - VIEW_MARGIN) / 2, (bufferHeight - VIEW_MARGIN) / 2);

    public DisplayPlan(Level state, ScrollStatus scrollStatus, BufferStats bufferStats)
    {
        IConsoleDisplayable[,] planArray = new IConsoleDisplayable[bufferStats.BufferWidth, bufferStats.BufferHeight];           
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

    /// <summary>
    /// Assuming this instance of DisplayPlan is represented in the console display,
    /// update the console by comparing this DisplayPlan to the given newDisplayPlan
    /// and changing anything that needs to be changed.
    /// If this is done successfully, return true. Return false
    /// if an exception is encountered. eg if the Console size changed.
    /// </summary>
    public bool RefreshDisplay(DisplayPlan newDisplayPlan)
    {
        
        bool forceRefresh = bufferStats.BufferSizeChanged;

        IConsoleDisplayable[,] oldDisplay = this.planArray;
        IConsoleDisplayable[,] newDisplay = newDisplayPlan.planArray;

        int maxCol = bufferStats.BufferWidth - VIEW_MARGIN;
        int maxRow = bufferStats.BufferHeight - VIEW_MARGIN;

        if (forceRefresh)
        {
            DisplayUtilities.ResetConsoleColors();
            Console.Clear();
        }
        try
        {
            for (int row = 0; row < maxRow; row++)
            {
                for (int col = 0; col < maxCol; col++)
                {
                    IConsoleDisplayable? oldDisp = forceRefresh ? null : oldDisplay[col, row];
                    IConsoleDisplayable newDisp = newDisplay[col, row];
                    if (newDisp != default && (forceRefresh || oldDisp != newDisp))
                    {
                        // Something is here that wasn't here before, so show it
                        Console.SetCursorPosition(col, row);
                        Console.BackgroundColor = newDisp.BackgroundColor.ToConsoleColor();
                        Console.ForegroundColor = newDisp.Color.ToConsoleColor();
                        Console.Write(newDisp.ConsoleDisplayString);
                    }
                    if (newDisp == default && oldDisp != default)
                    {
                        // Something used to be here, but now nothing is here, so clear the spot
                        Console.SetCursorPosition(col, row);
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write(" ");
                    }
                } // end for (columns)
            } // end for (rows)
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