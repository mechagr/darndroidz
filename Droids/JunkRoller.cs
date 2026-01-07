using DarnDroidz.Behaviors;
using DarnDroidz.Core;
using DarnDroidz.Systems;
using RogueSharp.DiceNotation;

namespace DarnDroidz.Droids
{
    public class JunkRoller : Droid
    {
        public static JunkRoller Create(int level)
        {
            int health = Dice.Roll("4D5");
            return new JunkRoller
            {
                Attack = Dice.Roll("1D2") + level / 3,
                AttackChance = Dice.Roll("10D5"),
                Awareness = 10,
                Color = Colors.JunkRollerColor,
                Defense = Dice.Roll("1D2") + level / 3,
                DefenseChance = Dice.Roll("10D4"),
                EnergyCell = Dice.Roll("1D20"),
                Health = health,
                MaxHealth = health,
                Name = "JunkRoller",
                Speed = 14,
                Symbol = 'o'
            };
        }

        public override void PerformAction(CommandSystem commandSystem)
        {
            var splitJunkRollerBehavior = new SplitJunkRoller();
            if (!splitJunkRollerBehavior.Act(this, commandSystem))
            {
                base.PerformAction(commandSystem);
            }
        }
    }
}