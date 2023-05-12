using HercAndHippoLibCs;
using static HercAndHippoConsole.Constants;
using static System.Math;

namespace HercAndHippoConsole
{
    internal record ScrollStatus(Direction Horizontal, Direction Vertical, int HorizRadius, int VertRadius, Location LogicalCenter)
    {
        public const double HORIZONTAL_RADIUS_RATIO = 0.1;  // Fairly small; small motion around center triggers left/right scroll
        public const double VERTICAL_RADIUS_RATIO = 0.3; // Larger; can get close to top or bottom of screen before up/down scroll kicks in
        public readonly bool InVerticalTransition = Vertical != Direction.Idle;
        public readonly bool InHorizontalTransition = Horizontal != Direction.Idle;
        public static readonly ScrollStatus Default = new(Direction.Idle, Direction.Idle, 100, 100, (1,1));
        public ScrollStatus UpdateTriggerRadius(int bufferWidth, int bufferHeight)
        {
            // Round these down (versus Convert.ToInt32, which uses banker's rounding).
            // This makes scrolling more aggressive on smaller displays.
            int newHorizRadius = (int) (bufferWidth * HORIZONTAL_RADIUS_RATIO);
            int newVertRadius = (int) (bufferHeight * VERTICAL_RADIUS_RATIO);
            return this with { HorizRadius = newHorizRadius, VertRadius = newVertRadius };
        }

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

        public ScrollStatus DoScroll(Location playerLocation, Location previousLogicalCenter, int bufferWidth, int bufferHeight)
        {
            Location logicalCenter = NextLogicalCenter();
            Direction newVert;
            Direction newHoriz;


            int verticalDist = Abs(playerLocation.Row - logicalCenter.Row);
            if (verticalDist > VertRadius || previousLogicalCenter.Row < MIN_ROW || previousLogicalCenter.Row > bufferHeight)
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
            if (horizDist > HorizRadius || previousLogicalCenter.Col < MIN_COL || previousLogicalCenter.Col > bufferWidth)
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
}
