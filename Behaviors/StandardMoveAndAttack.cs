using DarnDroidz;
using DarnDroidz.Core;
using DarnDroidz.Interfaces;
using DarnDroidz.Systems;
using RogueSharp;
using RogueSharp.DiceNotation;
using System.Linq;
using System.Threading;

namespace DarnDroidz.Behaviors
{
    public class StandardMoveAndAttack : IBehavior
    {
        public bool Act(Droid droid, CommandSystem commandSystem)
        {
            WastelandMap dungeonMap = Game.WastelandMap;
            Player player = Game.Player;
            FieldOfView droidFov = new FieldOfView(dungeonMap);

            if (!droid.TurnsAlerted.HasValue)
            {
                droidFov.ComputeFov(droid.X, droid.Y, droid.Awareness, true);
                if (droidFov.IsInFov(player.X, player.Y))
                {
                    Game.MessageLog.Add($"{droid.Name} is eager to fight {player.Name}!");
                    droid.TurnsAlerted = 1;
                }
            }

            if (droid.TurnsAlerted.HasValue)
            {
                dungeonMap.SetIsWalkable(droid.X, droid.Y, true);
                dungeonMap.SetIsWalkable(player.X, player.Y, true);

                RogueSharp.PathFinder pathFinder = new RogueSharp.PathFinder(dungeonMap);
                RogueSharp.Path path = null;

                try
                {
                    path = pathFinder.ShortestPath(
                       dungeonMap.GetCell(droid.X, droid.Y),
                       dungeonMap.GetCell(player.X, player.Y));
                }
                catch (PathNotFoundException)
                {
                    Game.MessageLog.Add($"{droid.Name} waits for a turn...");
                }

                dungeonMap.SetIsWalkable(droid.X, droid.Y, false);
                dungeonMap.SetIsWalkable(player.X, player.Y, false);

                if (path != null)
                {
                    try
                    {
                        ICell nextStep = path.Steps.FirstOrDefault();

                        if (nextStep != null)
                        {
                            int distance = System.Math.Abs(nextStep.X - player.X) + System.Math.Abs(nextStep.Y - player.Y);

                            if (distance <= 1)
                            {
                                commandSystem.Attack(droid, player);
                            }
                            else
                            {
                                commandSystem.MoveDroid(droid, nextStep);
                            }
                        }
                    }
                    catch (System.Exception)
                    {
                        Game.MessageLog.Add($"{droid.Name} hesitates...");
                    }
                }

                droid.TurnsAlerted++;

                if (droid.TurnsAlerted > 15)
                {
                    droid.TurnsAlerted = null;
                }
            }

            return true;
        }
    }
}