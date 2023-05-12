using HercAndHippoLibCs;
using static HercAndHippoConsole.Constants;

namespace HercAndHippoConsole
{

    internal record BufferStats(bool BufferSizeChanged, int BufferWidth, int BufferHeight)
    {
        public BufferStats Update()
        {
            bool changed = BufferHeight != Console.BufferHeight || BufferWidth != Console.BufferWidth;
            return new(changed, BufferWidth: Console.BufferWidth, BufferHeight: Console.BufferHeight);
        } 
    }

    internal static class DisplayLogic
    {
        public static Location GetScreenCenter(int bufferWidth, int bufferHeight)
            => ((bufferWidth - VIEW_MARGIN) / 2, (bufferHeight - VIEW_MARGIN) / 2);

        public static IDisplayable[,] DisplayData(Level state, ScrollStatus transitionStatus, BufferStats bufferStats)
        {
            IDisplayable[,] ToShow = new IDisplayable[bufferStats.BufferWidth, bufferStats.BufferHeight];

            Location screenCenter = GetScreenCenter(bufferStats.BufferWidth, bufferStats.BufferHeight);
            Location logicalCenter = transitionStatus.LogicalCenter;

            foreach (IDisplayable toShow in state.LevelObjects)
            {
                // Note that these are ints, not instances of the Column/Row type.
                // If writeCol and writeRow were column/row respectively,
                // we would use the "max out" addition logic, which
                int writeCol = screenCenter.Col - logicalCenter.Col + toShow.Location.Col; 
                int writeRow = screenCenter.Row - logicalCenter.Row + toShow.Location.Row;
                if (writeCol >= MIN_COL && writeCol < bufferStats.BufferWidth - VIEW_MARGIN && 
                    writeRow >= MIN_ROW && writeRow < bufferStats.BufferHeight - VIEW_MARGIN)
                {
                    ToShow[writeCol, writeRow] = toShow;
                }
            }
            return ToShow;
        }
    }
}
