using HercAndHippoLibCs;
using static HercAndHippoConsole.Constants;
using static HercAndHippoConsole.DisplayLogic;
using static System.Math;

namespace HercAndHippoConsole
{
    internal record TransitionStatus(Direction Horizontal, Direction Vertical, int HorizRadius, int VertRadius)
    {
        public const double RADIUS_RATIO = 0.375;
        public readonly bool InVerticalTransition = Vertical != Direction.Idle;
        public readonly bool InHorizontalTransition = Horizontal != Direction.Idle;
        public static readonly TransitionStatus Default = new(Direction.Idle, Direction.Idle, 100, 100);
        public TransitionStatus UpdateTriggerRadius(int bufferWidth, int bufferHeight)
        {
            int newHorizRadius = Convert.ToInt32(bufferWidth * RADIUS_RATIO);
            int newVertRadius = Convert.ToInt32(bufferHeight * RADIUS_RATIO);
            return this with { HorizRadius = newHorizRadius, VertRadius = newVertRadius };
        }

        public (TransitionStatus next, Location newLogicalCenter) UpdateDirection(Location playerLocation, Location previousLogicalCenter)
        {
            Location logicalCenter = GetLogicalCenter(previousLogicalCenter, Default);
            Direction newVert;
            Direction newHoriz;


            int verticalDist = Abs(playerLocation.Row - logicalCenter.Row);
            if (verticalDist > VertRadius || previousLogicalCenter.Row < MIN_ROW)
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
            if (horizDist > HorizRadius || previousLogicalCenter.Col < MIN_COL)
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
            return (this with { Horizontal = newHoriz, Vertical = newVert }, logicalCenter);
        }
    }
}
