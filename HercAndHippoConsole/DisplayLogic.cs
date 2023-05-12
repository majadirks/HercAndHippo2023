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
                // Note that these are ints, not instances of the Column/Row type.
                // If writeCol and writeRow were column/row respectively,
                // we would use the "max out" addition logic, which
                int writeCol = screenCenter.Col - logicalCenter.Col + toShow.Location.Col; 
                int writeRow = screenCenter.Row - logicalCenter.Row + toShow.Location.Row;
                if (writeCol >= MIN_COL && writeCol < bufferWidth - VIEW_MARGIN && 
                    writeRow >= MIN_ROW && writeRow < bufferHeight - VIEW_MARGIN)
                {
                    ToShow[writeCol, writeRow] = toShow;
                }
            }
            return ToShow;
        }
    }
}
