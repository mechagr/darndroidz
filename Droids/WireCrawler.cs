using DarnDroidz.Core;
using DarnDroidz.Systems;
using DarnDroidz.Behaviors;
using RogueSharp.DiceNotation;

namespace DarnDroidz.Droids
{
    public class WireCrawler : Droid
    {
        private int? _turnsSpentRunning = null;
        private bool _shoutedForHelp = false;

        public static WireCrawler Create(int level)
        {
            int health = Dice.Roll("1D5");

            return new WireCrawler
            {
                Attack = Dice.Roll("1D2") + level / 3,
                AttackChance = Dice.Roll("10D5"),
                Awareness = 10,
                Color = Colors.WireCrawlerColor,
                Defense = Dice.Roll("1D2") + level / 3,
                DefenseChance = Dice.Roll("10D4"),
                EnergyCell = Dice.Roll("1D20"),
                Health = health,
                MaxHealth = health,
                Name = "WireCrawler",
                Speed = 12,
                Symbol = 'w'
            };
        }

        public override void PerformAction(CommandSystem commandSystem)
        {
            var fullyHealBehavior = new FullyHeal();
            var runAwayBehavior = new RunAway();
            var shoutForHelpBehavior = new ShoutForHelp();

            if (_turnsSpentRunning.HasValue && _turnsSpentRunning.Value > 15)
            {
                fullyHealBehavior.Act(this, commandSystem);
                _turnsSpentRunning = null;
            }
            else if (Health < MaxHealth)
            {
                runAwayBehavior.Act(this, commandSystem);

                _turnsSpentRunning = _turnsSpentRunning.HasValue
                    ? _turnsSpentRunning + 1
                    : 1;

                if (!_shoutedForHelp)
                {
                    _shoutedForHelp = shoutForHelpBehavior.Act(this, commandSystem);
                }
            }
            else
            {
                base.PerformAction(commandSystem);
            }
        }
    }
}
