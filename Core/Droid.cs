using DarnDroidz.Behaviors;
using DarnDroidz.Systems;
using RLNET;
using System;

namespace DarnDroidz.Core
{
    public class Droid : Actor
    {
        public int EnergyCell { get; set; }
        public int? TurnsAlerted { get; set; }

        public void DrawStats(RLConsole statConsole, int position)
        {
            int yPosition = 13 + (position * 2);

            statConsole.Print(1, yPosition, Symbol.ToString(), Color);

            int width = (int)Math.Floor((double)Health / MaxHealth * 16.0);
            int remainingWidth = 16 - width;

            statConsole.SetBackColor(3, yPosition, width, 1, Swatch.Primary);
            statConsole.SetBackColor(3 + width, yPosition, remainingWidth, 1, Swatch.PrimaryDarker);

            statConsole.Print(2, yPosition, $": {Name}", Swatch.DbLight);
        }

        public virtual void PerformAction(CommandSystem commandSystem)
        {
            var behavior = new StandardMoveAndAttack();
            behavior.Act(this, commandSystem);
        }

        public static Droid Clone(Droid anotherDroid)
        {
            return new Droid
            {
                Attack = anotherDroid.Attack,
                AttackChance = anotherDroid.AttackChance,
                Awareness = anotherDroid.Awareness,
                Color = anotherDroid.Color,
                Defense = anotherDroid.Defense,
                DefenseChance = anotherDroid.DefenseChance,
                Health = anotherDroid.Health,
                MaxHealth = anotherDroid.MaxHealth,
                Name = anotherDroid.Name,
                Speed = anotherDroid.Speed,
                Symbol = anotherDroid.Symbol,
                EnergyCell = anotherDroid.EnergyCell
            };
        }
    }
}
