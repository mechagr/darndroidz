using DarnDroidz;
using DarnDroidz.Core;
using DarnDroidz.Droids;
using DarnDroidz.Interfaces;
using DarnDroidz.Systems;
using RogueSharp;
using System;

namespace DarnDroidz.Behaviors
{
    public sealed class SplitJunkRoller : IBehavior
    {
        public bool Act(Droid droid, CommandSystem commandSystem)
        {
            if (droid == null)
                return false;

            if (droid.Health >= droid.MaxHealth)
                return false;

            int halfHealth = droid.MaxHealth / 2;
            if (halfHealth <= 0)
                return false;

            WastelandMap map = Game.WastelandMap;
            if (map == null)
                return false;

            Cell cell = FindClosestUnoccupiedCell(map, droid.X, droid.Y);
            if (cell == null)
                return false;

            JunkRoller clone = Droid.Clone(droid) as JunkRoller;
            if (clone == null)
                return false;

            clone.X = cell.X;
            clone.Y = cell.Y;
            clone.MaxHealth = halfHealth;
            clone.Health = halfHealth;
            clone.TurnsAlerted = 1;

            map.AddDroid(clone);

            droid.MaxHealth = halfHealth;
            droid.Health = halfHealth;

            Game.MessageLog.Add($"{droid.Name} splits itself in two...");

            return true;
        }

        private static Cell FindClosestUnoccupiedCell(WastelandMap map, int originX, int originY)
        {
            const int maxRadius = 4;

            for (int radius = 1; radius <= maxRadius; radius++)
            {
                for (int x = originX - radius; x <= originX + radius; x++)
                {
                    for (int y = originY - radius; y <= originY + radius; y++)
                    {
                        if (Math.Abs(x - originX) != radius &&
                            Math.Abs(y - originY) != radius)
                            continue;

                        Cell cell = map.GetCell(x, y) as Cell;
                        if (cell == null)
                            continue;

                        if (!cell.IsWalkable)
                            continue;

                        if (map.GetDroidAt(x, y) != null)
                            continue;

                        return cell;
                    }
                }
            }

            return null;
        }
    }
}
