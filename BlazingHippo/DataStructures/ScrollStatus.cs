using HercAndHippoLibCs;
using static BlazingHippo.Constants;
using static System.Math;

namespace BlazingHippo;

public record ScrollStatus(Direction Horizontal, Direction Vertical, Location LogicalCenter)
{
    public const double HORIZONTAL_RADIUS_RATIO = 0.1;  // Fairly small; small motion around center triggers left/right scroll
    public const double VERTICAL_RADIUS_RATIO = 0.3; // Larger; can get close to top or bottom of screen before up/down scroll kicks in
    public static readonly int HorizRadius = (int)(GAME_WIDTH * HORIZONTAL_RADIUS_RATIO);
    public static readonly int VertRadius = (int)(GAME_HEIGHT * VERTICAL_RADIUS_RATIO) - 1;
    public readonly bool InVerticalTransition = Vertical != Direction.Idle;
    public readonly bool InHorizontalTransition = Horizontal != Direction.Idle;
    public static ScrollStatus Default(Location playerLocation) => new(Direction.Idle, Direction.Idle, playerLocation);
    
    public ScrollStatus Update(Location playerLocation)
        =>this.DoScroll(playerLocation);
    
    private Location NextLogicalCenter()
    {
        Column logicalCenterCol = LogicalCenter.Col;
        Row logicalCenterRow = LogicalCenter.Row;
        if (Horizontal == Direction.East) logicalCenterCol++;
        if (Horizontal == Direction.West) logicalCenterCol--;
        if (Vertical == Direction.North) logicalCenterRow--;
        if (Vertical == Direction.South) logicalCenterRow++;
        return (logicalCenterCol, logicalCenterRow);
    }

    private ScrollStatus DoScroll(Location playerLocation)
    {
        Location logicalCenter = NextLogicalCenter();
        Direction newVert;
        Direction newHoriz;


        int verticalDist = Abs(playerLocation.Row - logicalCenter.Row);
        if (verticalDist > VertRadius || logicalCenter.Row < MIN_DISPLAY_ROW || logicalCenter.Row > GAME_HEIGHT)
        {
            if (playerLocation.Row > logicalCenter.Row)
            {
                newVert = Direction.South;
                logicalCenter = logicalCenter with { Row = logicalCenter.Row + 1 };
            }
            else
            {
                newVert = Direction.North;
                logicalCenter = logicalCenter with { Row = logicalCenter.Row -1 };
            }         
        }
        else
        {
            newVert = Direction.Idle;
        }

        int horizDist = Abs(playerLocation.Col - logicalCenter.Col);
        if (horizDist > HorizRadius || logicalCenter.Col < MIN_DISPLAY_COL || logicalCenter.Col > GAME_WIDTH)
        {
            if (playerLocation.Col > logicalCenter.Col)
            {
                newHoriz = Direction.East;
                logicalCenter = logicalCenter with { Col = logicalCenter.Col + 1 };
            }
            else
            {
                newHoriz = Direction.West;
                logicalCenter = logicalCenter with { Col = logicalCenter.Col - 1 };
            }
        }
        else
        {
            newHoriz = Direction.Idle;
        }
        return this with { Horizontal = newHoriz, Vertical = newVert, LogicalCenter = logicalCenter };
    }
}
