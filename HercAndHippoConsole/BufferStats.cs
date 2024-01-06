namespace HercAndHippoConsole;

internal class BufferStats
{
    public bool BufferSizeChanged { get; private set; }
    public int BufferWidth { get; private set; }
    public int BufferHeight { get; private set; }
    public BufferStats(bool bufferSizeChanged, int bufferWidth, int bufferHeight)
    {
        BufferSizeChanged = bufferSizeChanged;
        BufferWidth = bufferWidth;
        BufferHeight = bufferHeight;
    }
    public void Refresh()
    {
        int newBH = Console.BufferHeight;
        int newBW = Console.BufferWidth;

        BufferSizeChanged = BufferHeight != newBH || BufferWidth != newBW;
        BufferWidth = newBW;
        BufferHeight = newBH;
   
    }

    public void ForceRefresh()
    {
        Refresh();
        BufferSizeChanged = true;
    }
    
}
