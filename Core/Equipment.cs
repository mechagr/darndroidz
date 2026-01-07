using RLNET;
using RogueSharp;
using DarnDroidz.Equipment;
using DarnDroidz.Interfaces;

namespace DarnDroidz.Core
{
    public class Equipment : IEquipment, ITreasure, IDrawable
    {
        public Equipment()
        {
            Symbol = ']';
            Color = RLColor.Yellow;
        }

        public int Attack { get; set; }
        public int AttackChance { get; set; }
        public int Awareness { get; set; }
        public int Defense { get; set; }
        public int DefenseChance { get; set; }
        public int Gold { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public string Name { get; set; }
        public int Speed { get; set; }

        protected bool Equals(Equipment other)
        {
            return Attack == other.Attack && AttackChance == other.AttackChance && Awareness == other.Awareness && Defense == other.Defense && DefenseChance == other.DefenseChance && Gold == other.Gold && Health == other.Health && MaxHealth == other.MaxHealth && string.Equals(Name, other.Name) && Speed == other.Speed;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((Equipment)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Attack;
                hashCode = (hashCode * 397) ^ AttackChance;
                hashCode = (hashCode * 397) ^ Awareness;
                hashCode = (hashCode * 397) ^ Defense;
                hashCode = (hashCode * 397) ^ DefenseChance;
                hashCode = (hashCode * 397) ^ Gold;
                hashCode = (hashCode * 397) ^ Health;
                hashCode = (hashCode * 397) ^ MaxHealth;
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Speed;
                return hashCode;
            }
        }

        public static bool operator ==(Equipment left, Equipment right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Equipment left, Equipment right)
        {
            return !Equals(left, right);
        }

        public bool PickUp(IActor actor)
        {
            if (this is SensorEquipment)
            {
                actor.Sensor = this as SensorEquipment;
                Game.MessageLog.Add($"{actor.Name} installed a {Name}!");
                return true;
            }

            if (this is ChassisEquipment)
            {
                actor.Chassis = this as ChassisEquipment;
                Game.MessageLog.Add($"{actor.Name} installed {Name}!");
                return true;
            }

            if (this is WeaponMount)
            {
                actor.Mount = this as WeaponMount;
                Game.MessageLog.Add($"{actor.Name} mounted a {Name}!");
                return true;
            }

            if (this is LocomotionUpgrade)
            {
                actor.Locomotion = this as LocomotionUpgrade;
                Game.MessageLog.Add($"{actor.Name} installed {Name}!");
                return true;
            }

            return false;
        }

        public RLColor Color { get; set; }
        public char Symbol { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public void Draw(RLConsole console, IMap map)
        {
            if (!map.IsExplored(X, Y))
            {
                return;
            }

            if (map.IsInFov(X, Y))
            {
                console.Set(X, Y, Color, Colors.FloorBackgroundFov, Symbol);
            }
            else
            {
                console.Set(X, Y, RLColor.Blend(Color, RLColor.Gray, 0.5f), Colors.FloorBackground, Symbol);
            }
        }
    }
}