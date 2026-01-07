using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarnDroidz.Equipment
{
    public class WeaponMount : Core.Equipment
    {
        public static WeaponMount None()
        {
            return new WeaponMount { Name = "None" };
        }

        public static WeaponMount MakeshiftBoltThrower()
        {
            return new WeaponMount()
            {
                Attack = 4,
                AttackChance = 3,
                Name = "Makeshift Bolt Thrower"
            };
        }

        public static WeaponMount ScrapSpikeLauncher()
        {
            return new WeaponMount()
            {
                Attack = 5,
                AttackChance = 2,
                Defense = 1,
                Name = "Scrap Spike Launcher"
            };
        }

        public static WeaponMount BiteForceAmplifier()
        {
            return new WeaponMount()
            {
                Attack = 8,
                AttackChance = 5,
                Speed = 1,
                Name = "Bite-Force Amplifier"
            };
        }

        public static WeaponMount IntegratedPulseRifle()
        {
            return new WeaponMount()
            {
                Attack = 9,
                AttackChance = 6,
                Awareness = 1,
                Name = "Integrated Pulse Rifle"
            };
        }

        public static WeaponMount PlasmaJawCannon()
        {
            return new WeaponMount()
            {
                Attack = 12,
                AttackChance = 8,
                Defense = 2,
                Name = "Plasma Jaw Cannon"
            };
        }

        public static WeaponMount ShoulderMountedLaser()
        {
            return new WeaponMount()
            {
                Attack = 15,
                AttackChance = 10,
                Awareness = 2,
                MaxHealth = 5,
                Name = "Shoulder-Mounted Laser"
            };
        }
    }
}