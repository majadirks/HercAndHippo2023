using HercAndHippoLibCs;
namespace BlazingHippo;

public class StatusBar
{
    private readonly int margin;
    public StatusBar(int margin)
    {
        this.margin = margin;
    }

    public void ShowStatus(Level state, string? msg = null)
    {          
        // ToDo
        Player player = state.Player;
        Hippo? hippo = state.Hippo;
        OverwriteLine($"{player.Health}, {player.AmmoCount}, Velocity: {Math.Round(player.Velocity.CurrentVelocity,1)}, Energy: {player.KineticEnergy}");
        OverwriteLine(player.Inventory.ToString());
        string hippoStr = hippo == null ?
            "No hippo present." :
            hippo.LockedToPlayer ? 
            $"Hippo locked, health = {hippo.Health}." : 
            $"Hippo loose, health = {hippo.Health}.";
        string levelStr = $"Level: gravity {state.Gravity}, cycle count {state.Cycles}.";
        OverwriteLine(hippoStr + " " + levelStr);
        if (msg == null && state.GetMessage() is Message message)
            OverwriteLine(message.Text);
        else if (msg != null)
            OverwriteLine(msg);
        else
            OverwriteLine(" ");
    }

    private static void OverwriteLine(string str)
    {
        Console.WriteLine(str + new string(' ', Constants.GAME_WIDTH - str.Length));
    }
}
