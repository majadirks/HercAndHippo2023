using HercAndHippoLibCs;
using static HercAndHippoConsole.Constants;

namespace HercAndHippoConsole
{
    internal static class DisplayLogic
    {
        public static Location GetScreenCenter(int bufferWidth, int bufferHeight)
            => ((bufferWidth - VIEW_MARGIN) / 2, (bufferHeight - VIEW_MARGIN) / 2);

        public static IDisplayable[,] DisplayData(Level state, ScrollStatus transitionStatus, int bufferWidth, int bufferHeight)
        {
            IDisplayable[,] ToShow = new IDisplayable[bufferWidth, bufferHeight];

            Location screenCenter = GetScreenCenter(bufferWidth, bufferHeight);
            Location logicalCenter = transitionStatus.LogicalCenter;

            foreach (IDisplayable toShow in state.LevelObjects)
            {
                Location writeLocation = screenCenter - logicalCenter + toShow.Location;
                Column writeCol = writeLocation.Col;
                Row writeRow = writeLocation.Row;
                if (writeCol >= MIN_COL && writeCol < bufferWidth - VIEW_MARGIN && writeRow >= MIN_ROW && writeRow < bufferHeight - VIEW_MARGIN)
                {
                    ToShow[writeCol, writeRow] = toShow;
                    if (toShow is Bullet b)
                    {
                        ;
                    }
                }
            }
            return ToShow;
        }
    }
}
