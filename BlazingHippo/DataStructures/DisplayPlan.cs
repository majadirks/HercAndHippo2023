using HercAndHippoLibCs;
using static BlazingHippo.Constants;

namespace BlazingHippo;

public record PlannedHho(IConsoleDisplayable Hho, int Col, int Row);

public class DisplayPlan
{
    //private readonly IConsoleDisplayable[,] planArray;
    public PlannedHho[] Planned {get; init;}
    //private const int MAX_COL = GAME_WIDTH - VIEW_MARGIN;
    //private const int MAX_ROW = GAME_HEIGHT - VIEW_MARGIN;
    public static readonly Location ScreenCenter = ((GAME_WIDTH - VIEW_MARGIN) / 2, (GAME_HEIGHT - VIEW_MARGIN) / 2);
    public Level State { get; init; }
    public DisplayPlan(Level state, ScrollStatus scrollStatus)
    {
        State = state;
        //IConsoleDisplayable[,] planArray = new IConsoleDisplayable[GAME_WIDTH, GAME_HEIGHT];
        Planned = new PlannedHho[GAME_WIDTH * GAME_HEIGHT];
        Location logicalCenter = scrollStatus.LogicalCenter;
        
        foreach (IConsoleDisplayable levelObj in state.LevelObjects.Where(obj => obj is IConsoleDisplayable).Cast<IConsoleDisplayable>())
        {
            int col = ScreenCenter.Col - logicalCenter.Col + levelObj.Location.Col; 
            int row = ScreenCenter.Row - logicalCenter.Row + levelObj.Location.Row;
            if (col >= MIN_DISPLAY_COL && col < GAME_WIDTH - VIEW_MARGIN && 
                row >= MIN_DISPLAY_ROW && row < GAME_HEIGHT - VIEW_MARGIN)
            {
                //planArray[col, row] = levelObj;
                Planned[col + col * row] = new(Hho: levelObj, Col: col, Row: row);
            }
        }

        //this.planArray = planArray;
    }


    //public IEnumerable<DisplayDiff> GetDiffs(DisplayPlan newDisplayPlan)
    //{
    //    bool calculated = false;

    //    while (!calculated)
    //    {
    //        try
    //        {
    //            List<DisplayDiff> diffs = [];
    //            IConsoleDisplayable[,] oldDisplay = this.planArray;
    //            IConsoleDisplayable[,] newDisplay = newDisplayPlan.planArray;
                
    //            for (int row = 0; row < MAX_ROW; row++)
    //            {
    //                for (int col = 0; col < MAX_COL; col++)
    //                {
    //                    IConsoleDisplayable? oldDisp = oldDisplay[col, row];
    //                    IConsoleDisplayable newDisp = newDisplay[col, row];
    //                    if (newDisp != default && oldDisp != newDisp)
    //                    {
    //                        var diff = new DisplayDiff(Col: col, Row: row, OldDisplayable: oldDisp, NewDisplayable: newDisp);
    //                        // Something is here that wasn't here before, so plan to show it
    //                        if (diff.AppearanceChanged)
    //                            diffs.Add(diff);
    //                    }
    //                    if (newDisp == default && oldDisp != default)
    //                    {
    //                        // Something used to be here, but now nothing is here,
    //                        // so plan to clear the spot
    //                        diffs.Add(new DisplayDiff(Col: col, Row: row, OldDisplayable: oldDisp, NewDisplayable: null));
    //                    }
    //                } // end for (columns)
    //            } // end for (rows)
    //            calculated = true;
    //            return diffs;
    //        }
    //        catch
    //        {
    //            calculated = false;
    //        } 
    //    }
    //    if (this.diffs == null)
    //        throw new Exception("Unexpected exception.");
    //    return diffs;
    //}
}