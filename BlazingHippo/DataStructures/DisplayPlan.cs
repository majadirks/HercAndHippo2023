using HercAndHippoLibCs;
using static BlazingHippo.Constants;

namespace BlazingHippo;

public record DisplayDiff(int Col, int Row, IConsoleDisplayable? OldDisplayable, IConsoleDisplayable? NewDisplayable)
{
    public bool AppearanceChanged => NewDisplayable?.Color != OldDisplayable?.Color ||
        NewDisplayable?.BackgroundColor != OldDisplayable?.BackgroundColor ||
        NewDisplayable?.ConsoleDisplayString != OldDisplayable?.ConsoleDisplayString;
}

public class DisplayPlan
{
    private readonly IConsoleDisplayable[,] planArray;
    private readonly IEnumerable<DisplayDiff>? diffs;
    private const int MAX_COL = GAME_WIDTH - VIEW_MARGIN;
    private const int MAX_ROW = GAME_HEIGHT - VIEW_MARGIN;
    public static readonly Location ScreenCenter = ((GAME_WIDTH - VIEW_MARGIN) / 2, (GAME_HEIGHT - VIEW_MARGIN) / 2);
    public Level State { get; init; }
    public DisplayPlan(Level state, ScrollStatus scrollStatus)
    {
        State = state;
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
}