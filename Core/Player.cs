using RLNET;

namespace DarnDroidz.Core
{
    public class Player : Actor
    {
        public Player()
        {
            Attack = 2;
            AttackChance = 50;
            Awareness = 15;
            Color = Colors.Player;
            Defense = 2;
            DefenseChance = 40;
            EnergyCells = 0;     
            Health = 100;
            MaxHealth = 100;
            Name = "Tucker";      
            Speed = 10;
            Symbol = '@';
            X = 10;
            Y = 10;
        }

        public void Draw(RLConsole console, WastelandMap map)
        {
            if (!map.IsInFov(X, Y))
                return;

            console.Set(X, Y, Color, Colors.FloorBackgroundFov, Symbol);
        }

        public void DrawStats(RLConsole statConsole)
        {
            statConsole.Print(1, 1, $"Name:    {Name}", Colors.Text);
            statConsole.Print(1, 3, $"Health:  {Health}/{MaxHealth}", Colors.Text);
            statConsole.Print(1, 5, $"Attack:  {Attack} ({AttackChance}%)", Colors.Text);
            statConsole.Print(1, 7, $"Defense: {Defense} ({DefenseChance}%)", Colors.Text);
            statConsole.Print(1, 9, $"Energy:  {EnergyCells}", Colors.EnergyCells);
        }
        public int EnergyCells { get; set; }
    }
}
