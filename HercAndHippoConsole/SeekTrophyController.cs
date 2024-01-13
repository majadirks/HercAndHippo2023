using HercAndHippoLibCs;
using System.Text;

namespace HercAndHippoConsole
{
    internal class SeekTrophyController : GameController
    {
        private readonly ILocatable trophy;
        private readonly System.Collections.IEnumerator enumerator;
        private ActionInput lastAction;
        private readonly int depth;
        private readonly Random random;
        public SeekTrophyController(Level state, int depth, int iterations)
        {
            ILocatable trophy = state
                .LevelObjects
                .Where(obj => obj is Trophy)
                .Cast<Trophy>()
                .Single();
            this.trophy = trophy;
            this.depth = depth;
            this.random = new();

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
            if (state.WinState != WinState.InProgress)
                return ActionInput.NoAction;
            if (!state.Player.Health.HasHealth)
                return ActionInput.NoAction;
            Player player = state.Player;
            Dictionary<ActionInput, int> metrics = new();
            Hippo? hippo = state.Hippo;
            if (hippo == null)
                return ActionInput.NoAction;
            ILocatable toSeek = hippo.LockedToPlayer ? trophy : hippo;
            Task<Level> moveNorth = Task.Run(() => state.RefreshCyclables(ActionInput.MoveNorth));
            Task<Level> moveEast = Task.Run(() => state.RefreshCyclables(ActionInput.MoveEast));
            Task<Level> moveWest = Task.Run(() => state.RefreshCyclables(ActionInput.MoveWest));
            Task<Level> idle = Task.Run(() => state.RefreshCyclables(ActionInput.NoAction));
            Task<Level> shootWest = Task.Run (() => state.RefreshCyclables(ActionInput.ShootWest));
            Task<Level> shootSouth = Task.Run(() => state.RefreshCyclables(ActionInput.ShootSouth));
            Task.WaitAll(moveNorth, moveEast, moveWest, idle, shootWest, shootSouth);

            metrics[ActionInput.MoveNorth] = moveNorth.Result.Player.DeepSeek(moveNorth.Result, toSeek, depth - 1, state.Player.Location).Metric;
            metrics[ActionInput.MoveEast] = moveEast.Result.Player.DeepSeek(moveEast.Result, toSeek, depth - 1, state.Player.Location).Metric;
            metrics[ActionInput.MoveWest] = moveWest.Result.Player.DeepSeek(moveWest.Result, toSeek, depth - 1, state.Player.Location).Metric;
            metrics[ActionInput.NoAction] = idle.Result.Player.DeepSeek(idle.Result, toSeek, depth - 1, state.Player.Location).Metric;
            metrics[ActionInput.ShootWest] = shootWest.Result.Player.DeepSeek(shootWest.Result, toSeek, depth- 1, state.Player.Location).Metric;
            metrics[ActionInput.ShootSouth] = shootSouth.Result.Player.DeepSeek(shootSouth.Result, toSeek, depth - 1, state.Player.Location).Metric;

            int min = metrics.Values.Min();
            var minInputs = metrics.Keys.Where(ai => metrics[ai] == min);
            var newInputs = minInputs.Where(ai => ai != lastAction);
            ActionInput best = newInputs.Any() ? Random(newInputs) : Random(minInputs);
            lastAction = best;
            return best;
        }

        private ActionInput Random(IEnumerable<ActionInput> items)
        {
            ActionInput[] itemsArr = items.ToArray();
            int ct = itemsArr.Length;
            int i = random.Next(0, ct);
            return itemsArr[i];
        }
        public override ActionInput NextAction(Level state)
        {
            bool read = enumerator.MoveNext();
            return read ? (ActionInput)enumerator.Current : ActionInput.NoAction;
        }
    }
}