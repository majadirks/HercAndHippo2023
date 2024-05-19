using HercAndHippoLibCs;
using static BlazingHippo.Constants;

namespace BlazingHippo;

public record PlannedHho(IConsoleDisplayable Hho, int Col, int Row);

public class DisplayPlan
{
    public List<PlannedHho> Planned {get; init;}
    public static readonly Location ScreenCenter = ((GAME_WIDTH - VIEW_MARGIN) / 2, (GAME_HEIGHT - VIEW_MARGIN) / 2);
    public Level State { get; init; }
    public DisplayPlan(Level state, ScrollStatus scrollStatus)
    {
        State = state;
        Planned = new List<PlannedHho>(capacity: GAME_HEIGHT * GAME_WIDTH);
        Location logicalCenter = scrollStatus.LogicalCenter;
        
        foreach (IConsoleDisplayable levelObj in state.LevelObjects.Where(obj => obj is IConsoleDisplayable).Cast<IConsoleDisplayable>())
        {
            int col = ScreenCenter.Col - logicalCenter.Col + levelObj.Location.Col; 
            int row = ScreenCenter.Row - logicalCenter.Row + levelObj.Location.Row;
            if (col >= MIN_DISPLAY_COL && col < GAME_WIDTH - VIEW_MARGIN && 
                row >= MIN_DISPLAY_ROW && row < GAME_HEIGHT - VIEW_MARGIN)
            {
                var phho = new PlannedHho(Hho: levelObj, Col: col, Row: row);
                Planned.Add(phho);
            }
        }
    }
}