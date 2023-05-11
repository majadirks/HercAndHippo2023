using HercAndHippoLibCs;
using static HercAndHippoConsole.Constants;
using static System.Math;

namespace HercAndHippoConsole
{
    internal static class DisplayLogic
    {
        ///<summary>Return the Location within the level around which to center the view</summary>
        public static Location GetLogicalCenter(Location previous, TransitionStatus transition)
        {
            Column logicalCenterCol = previous.Col;
            Row logicalCenterRow = previous.Row;
            if (transition.Horizontal == Direction.East) logicalCenterCol++;
            if (transition.Horizontal == Direction.West) logicalCenterCol--;
            if (transition.Vertical == Direction.North) logicalCenterRow--;
            if (transition.Vertical == Direction.South) logicalCenterRow++;

            return (logicalCenterCol, logicalCenterRow);
        }

        public static Location GetScreenCenter(int bufferWidth, int bufferHeight)
            => ((bufferWidth - VIEW_MARGIN) / 2, (bufferHeight - VIEW_MARGIN) / 2);

        public static IDisplayable[,] DisplayData(Level state, Location previousLogicalCenter, TransitionStatus transitionStatus, int bufferWidth, int bufferHeight)
        {
            IDisplayable[,] ToShow = new IDisplayable[bufferWidth, bufferHeight];

            Location screenCenter = GetScreenCenter(bufferWidth, bufferHeight);
            Location logicalCenter
                = GetLogicalCenter(previousLogicalCenter, transitionStatus);

            foreach (IDisplayable toShow in state.LevelObjects)
            {
                Column writeCol = screenCenter.Col + toShow.Location.Col - logicalCenter.Col;
                Row writeRow = screenCenter.Row + toShow.Location.Row - logicalCenter.Row;
                if (writeCol >= MIN_COL && writeCol < bufferWidth && writeRow >= MIN_ROW && writeRow < bufferHeight)
                {
                    ToShow[writeCol, writeRow] = toShow;
                }
            }
            return ToShow;
        }
    }
}
