using DarnDroidz.Core;
using DarnDroidz.Interfaces;
using DarnDroidz.Systems;
using RogueSharp;

namespace DarnDroidz.Behaviors
{
    public class ShoutForHelp : IBehavior
    {
        public bool Act(Droid droid, CommandSystem commandSystem)
        {
            bool didShoutForHelp = false;

            WastelandMap wastelandMap = Game.WastelandMap;
            FieldOfView fov = new FieldOfView(wastelandMap);

            fov.ComputeFov(droid.X, droid.Y, droid.Awareness, true);

            foreach (var location in wastelandMap.GetDroidLocations())
            {
                if (!fov.IsInFov(location.X, location.Y))
                    continue;

                Droid alertedDroid = wastelandMap.GetDroidAt(location.X, location.Y);
                if (alertedDroid == null)
                    continue;

                if (!alertedDroid.TurnsAlerted.HasValue)
                {
                    alertedDroid.TurnsAlerted = 1;
                    didShoutForHelp = true;
                }
            }

            if (didShoutForHelp)
            {
                Game.MessageLog.Add($"{droid.Name} shouts for help!");
            }

            return didShoutForHelp;
        }
    }
}
