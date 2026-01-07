using DarnDroidz.Interfaces;
using DarnDroidz.Equipment;
using RLNET;
using RogueSharp;

namespace DarnDroidz.Core
{
    public class Actor : IActor, IDrawable, IScheduleable
    {
        private int _attack;
        private int _attackChance;
        private int _awareness;
        private int _defense;
        private int _defenseChance;
        private int _energyCells;
        private int _gold;
        private int _health;
        private int _maxHealth;
        private string _name;
        private int _speed;
        private RLColor _color;

        public int Attack
        {
            get
            {
                int equipmentBonus = 0;
                if (Sensor != null) equipmentBonus += Sensor.Attack;
                if (Chassis != null) equipmentBonus += Chassis.Attack;
                if (Mount != null) equipmentBonus += Mount.Attack;
                if (Locomotion != null) equipmentBonus += Locomotion.Attack;
                return _attack + equipmentBonus;
            }
            set { _attack = value; }
        }

        public int AttackChance
        {
            get
            {
                int equipmentBonus = 0;
                if (Sensor != null) equipmentBonus += Sensor.AttackChance;
                if (Chassis != null) equipmentBonus += Chassis.AttackChance;
                if (Mount != null) equipmentBonus += Mount.AttackChance;
                if (Locomotion != null) equipmentBonus += Locomotion.AttackChance;
                return _attackChance + equipmentBonus;
            }
            set { _attackChance = value; }
        }

        public int Awareness
        {
            get
            {
                int equipmentBonus = 0;
                if (Sensor != null) equipmentBonus += Sensor.Awareness;
                if (Chassis != null) equipmentBonus += Chassis.Awareness;
                if (Mount != null) equipmentBonus += Mount.Awareness;
                if (Locomotion != null) equipmentBonus += Locomotion.Awareness;
                return _awareness + equipmentBonus;
            }
            set { _awareness = value; }
        }

        public int Defense
        {
            get
            {
                int equipmentBonus = 0;
                if (Sensor != null) equipmentBonus += Sensor.Defense;
                if (Chassis != null) equipmentBonus += Chassis.Defense;
                if (Mount != null) equipmentBonus += Mount.Defense;
                if (Locomotion != null) equipmentBonus += Locomotion.Defense;
                return _defense + equipmentBonus;
            }
            set { _defense = value; }
        }

        public int DefenseChance
        {
            get
            {
                int equipmentBonus = 0;
                if (Sensor != null) equipmentBonus += Sensor.DefenseChance;
                if (Chassis != null) equipmentBonus += Chassis.DefenseChance;
                if (Mount != null) equipmentBonus += Mount.DefenseChance;
                if (Locomotion != null) equipmentBonus += Locomotion.DefenseChance;
                return _defenseChance + equipmentBonus;
            }
            set { _defenseChance = value; }
        }

        public int EnergyCells
        {
            get { return _energyCells; }
            set { _energyCells = value; }
        }

        public int Gold
        {
            get { return _gold; }
            set { _gold = value; }
        }

        public int Health
        {
            get { return _health; }
            set { _health = value; }
        }

        public int MaxHealth
        {
            get
            {
                int equipmentBonus = 0;
                if (Sensor != null) equipmentBonus += Sensor.MaxHealth;
                if (Chassis != null) equipmentBonus += Chassis.MaxHealth;
                if (Mount != null) equipmentBonus += Mount.MaxHealth;
                if (Locomotion != null) equipmentBonus += Locomotion.MaxHealth;
                return _maxHealth + equipmentBonus;
            }
            set { _maxHealth = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public int Speed
        {
            get
            {
                int equipmentBonus = 0;
                if (Sensor != null) equipmentBonus += Sensor.Speed;
                if (Chassis != null) equipmentBonus += Chassis.Speed;
                if (Mount != null) equipmentBonus += Mount.Speed;
                if (Locomotion != null) equipmentBonus += Locomotion.Speed;
                return _speed + equipmentBonus;
            }
            set { _speed = value; }
        }

        public RLColor Color
        {
            get { return _color; }
            set { _color = value; }
        }

        public char Symbol { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public SensorEquipment Sensor { get; set; }
        public ChassisEquipment Chassis { get; set; }
        public WeaponMount Mount { get; set; }
        public LocomotionUpgrade Locomotion { get; set; }

        public int Time
        {
            get { return Speed; }
        }

        public void Draw(RLConsole console, IMap map)
        {
            if (!map.GetCell(X, Y).IsExplored)
                return;

            if (map.IsInFov(X, Y))
            {
                console.Set(X, Y, Color, Colors.FloorBackgroundFov, Symbol);
            }
            else
            {
                console.Set(X, Y, Colors.Floor, Colors.FloorBackground, '.');
            }
        }
    }
}