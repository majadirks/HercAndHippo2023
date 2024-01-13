using HercAndHippoLibCs;
using System.Text;

namespace HercAndHippoConsole
{
    internal class DeepSeekTrophyController : GameController
    {
        private readonly ILocatable trophy;
        private readonly int depth;
        private readonly System.Collections.IEnumerator enumerator;
        public DeepSeekTrophyController(Level state, int depth, int iterations)
        {
            ILocatable trophy = state
                .LevelObjects
                .Where(obj => obj is Trophy)
                .Cast<Trophy>()
                .Single();
            this.trophy = trophy;
            this.depth = depth;

            Console.WriteLine($"Planning {iterations} actions with depth {depth}...");
            ActionInput[] actions = new ActionInput[iterations];
            for (int i = 0; i < iterations; i++)
            {
                double pct = Math.Round(i * 100.0 / iterations, 2);
                Console.WriteLine($"\t{pct}%...");
                actions[i] = FindNextFrom(state);
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
            DeepSeekResults deepSeek = player.DeepSeek(state, trophy, depth: depth, cameFrom: player.Location);
            Direction dir = deepSeek.Direction;
            return dir switch
            {
                Direction.North => ActionInput.MoveNorth,
                Direction.East => ActionInput.MoveEast,
                Direction.South => ActionInput.MoveSouth,
                Direction.West => ActionInput.MoveWest,
                Direction.Idle => ActionInput.NoAction,
                _ => throw new NotSupportedException()
            };
        }
        public override ActionInput NextAction(Level state)
        {
            bool read = enumerator.MoveNext();
            return read ? (ActionInput)enumerator.Current : ActionInput.NoAction;
        }
    }
}
