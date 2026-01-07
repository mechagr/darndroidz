using DarnDroidz.Core;
using RogueSharp.DiceNotation;
using System.Drawing;

namespace DarnDroidz.Droids
{
    public class ScrapBot : Droid
    {
        public static ScrapBot Create(int level)
        {
            int health = Dice.Roll("2D5");

            return new ScrapBot
            {
                Attack = Dice.Roll("1D3") + level / 3,
                AttackChance = Dice.Roll("25D3"),
                Awareness = 10,
                Color = Colors.ScrapBotColor,
                Defense = Dice.Roll("1D3") + level / 3,
                DefenseChance = Dice.Roll("10D4"),
                EnergyCell = Dice.Roll("5D5"), 
                Health = health,
                MaxHealth = health,
                Name = "ScrapBot",
                Speed = 14,
                Symbol = 'S'
            };
        }
    }
}
