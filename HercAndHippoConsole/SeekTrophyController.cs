using HercAndHippoLibCs;
using System.Text;

namespace HercAndHippoConsole
{
    internal class SeekTrophyController : GameController
    {
        private readonly ILocatable trophy;
        private readonly System.Collections.IEnumerator enumerator;
        private ActionInput lastAction;
        public SeekTrophyController(Level state, int iterations)
        {
            ILocatable trophy = state
                .LevelObjects
                .Where(obj => obj is Trophy)
                .Cast<Trophy>()
                .Single();
            this.trophy = trophy;

            lastAction = ActionInput.NoAction;

            Console.WriteLine($"Planning {iterations} actions...");
            ActionInput[] actions = new ActionInput[iterations];
            for (int i = 0; i < iterations; i++)
            {
                double pct = Math.Round(i * 100.0 / iterations, 2);
                Console.WriteLine($"\t{pct}%...");
                actions[i] = FindNextFrom(state);
                Console.WriteLine(actions[i]);
                state = state.RefreshCyclables(actions[i]);
            }

            string outputPath = "plan.txt";
            Console.WriteLine($"Writing plan to {outputPath}...");
            StringBuilder plan = new();
            for (int i = 0; i < iterations; i++)
            {
                plan.AppendLine(actions[i].ToString() + ",");
            }
            Console.Write(plan.ToString());
            File.WriteAllText(outputPath, plan.ToString());
            Console.Clear();

            this.enumerator = actions.GetEnumerator();
        }
        private ActionInput FindNextFrom(Level state)
        {
            Player player = state.Player;
            Dictionary<ActionInput, int> metrics = new();

            Task<Level> moveNorth = Task.Run(() => state.RefreshCyclables(ActionInput.MoveNorth));
            Task<Level> moveEast = Task.Run(() => state.RefreshCyclables(ActionInput.MoveEast));
            Task<Level> moveWest = Task.Run(() => state.RefreshCyclables(ActionInput.MoveWest));
            Task<Level> idle = Task.Run(() => state.RefreshCyclables(ActionInput.NoAction));
            Task<Level> shootWest = Task.Run (() => state.RefreshCyclables(ActionInput.ShootWest));
            Task<Level> shootSouth = Task.Run(() => state.RefreshCyclables(ActionInput.ShootSouth));
            Task.WaitAll(moveNorth, moveEast, moveWest, idle, shootWest, shootSouth);

            metrics[ActionInput.MoveNorth] = Location.ManhattanDistance(moveNorth.Result.Player.Location, trophy.Location);
            metrics[ActionInput.MoveEast] = Location.ManhattanDistance(moveEast.Result.Player.Location, trophy.Location);
            metrics[ActionInput.MoveWest] = Location.ManhattanDistance(moveWest.Result.Player.Location, trophy.Location);
            metrics[ActionInput.NoAction] = Location.ManhattanDistance(idle.Result.Player.Location, trophy.Location);
            metrics[ActionInput.ShootWest] = Location.ManhattanDistance(shootWest.Result.Player.Location, trophy.Location);
            metrics[ActionInput.ShootSouth] = Location.ManhattanDistance(shootSouth.Result.Player.Location, trophy.Location);
            int curDist = Location.ManhattanDistance(player.Location, trophy.Location);
            if (metrics.Values.Where(v => v != curDist).Any())
            {
                int min = metrics.Values.Where(v => v != curDist).Min();
                var newInputs = metrics.Keys.Where(ai => ai != lastAction && metrics[ai] == min);
                ActionInput best = newInputs.Any() ? newInputs.First() : metrics.Keys.Where(ai => metrics[ai]== min).First();
                lastAction = best;
                return best;
            }
            else
            {
                int min = metrics.Values.Min();
                ActionInput best = metrics.Keys.Where(ai => metrics[ai] == min).First();
                lastAction = best;
                return best;
            }          
        }
        public override ActionInput NextAction(Level state)
        {
            bool read = enumerator.MoveNext();
            return read ? (ActionInput)enumerator.Current : ActionInput.NoAction;
        }
    }
}
