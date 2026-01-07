using DarnDroidz;
using DarnDroidz.Core;
using DarnDroidz.Interfaces;
using DarnDroidz.Systems;
using RogueSharp;
using RogueSharp.DiceNotation;

namespace DarnDroidz.Behaviors
{
    public class RunAway : IBehavior
    {
        public bool Act(Droid droid, CommandSystem commandSystem)
        {
            WastelandMap wastelandMap = Game.WastelandMap;
            Player player = Game.Player;

            wastelandMap.SetIsWalkable(droid.X, droid.Y, true);
            wastelandMap.SetIsWalkable(player.X, player.Y, true);

            GoalMap goalMap = new GoalMap(wastelandMap);
            goalMap.AddGoal(player.X, player.Y, 0);

            RogueSharp.Path path = null;

            try
            {
                path = goalMap.FindPathAvoidingGoals(droid.X, droid.Y);
            }
            catch (PathNotFoundException)
            {
                Game.MessageLog.Add($"{droid.Name} cowers in fear...");
            }

            wastelandMap.SetIsWalkable(droid.X, droid.Y, false);
            wastelandMap.SetIsWalkable(player.X, player.Y, false);

            if (path != null)
            {
                try
                {
                    ICell step = path.StepForward();
                    commandSystem.MoveDroid(droid, step);
                }
                catch (NoMoreStepsException)
                {
                    Game.MessageLog.Add($"{droid.Name} cowers in fear...");
                }
            }

            return true;
        }
    }
}
