using HercAndHippoLibCs;
using static HercAndHippoConsole.Constants;
using static System.Math;

namespace HercAndHippoConsole
{
    internal static class DisplayLogic
    {
        public static Location GetScreenCenter(int bufferWidth, int bufferHeight)
            => ((bufferWidth - VIEW_MARGIN) / 2, (bufferHeight - VIEW_MARGIN) / 2);

        public static IDisplayable[,] DisplayData(Level state, TransitionStatus transitionStatus, int bufferWidth, int bufferHeight)
        {
            IDisplayable[,] ToShow = new IDisplayable[bufferWidth, bufferHeight];

            Location screenCenter = GetScreenCenter(bufferWidth, bufferHeight);
            Location logicalCenter = transitionStatus.LogicalCenter;

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
