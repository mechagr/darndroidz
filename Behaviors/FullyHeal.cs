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
    public class FullyHeal : IBehavior
    {
        public bool Act(Droid droid, CommandSystem commandSystem)
        {
            if (droid.Health < droid.MaxHealth)
            {
                int healthToRecover = droid.MaxHealth - droid.Health;
                droid.Health = droid.MaxHealth;
                Game.MessageLog.Add($"{droid.Name} recovers {healthToRecover} health!");
                return true;
            }
            return false;
        }
    }
}
