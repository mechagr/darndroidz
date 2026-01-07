using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarnDroidz.Equipment
{
    public class LocomotionUpgrade : Core.Equipment
    {
        public static LocomotionUpgrade None()
        {
            return new LocomotionUpgrade { Name = "None" };
        }

        public static LocomotionUpgrade RustyLegServos()
        {
            return new LocomotionUpgrade()
            {
                Speed = 2,
                Defense = 1,
                Name = "Rusty Leg Servos"
            };
        }

        public static LocomotionUpgrade AllTerrainPawGrips()
        {
            return new LocomotionUpgrade()
            {
                Speed = 3,
                DefenseChance = 2,
                Name = "All-Terrain Paw Grips"
            };
        }

        public static LocomotionUpgrade ReinforcedJointAssembly()
        {
            return new LocomotionUpgrade()
            {
                Speed = 4,
                Defense = 3,
                MaxHealth = 10,
                Name = "Reinforced Joint Assembly"
            };
        }

        public static LocomotionUpgrade SandWalkerTreads()
        {
            return new LocomotionUpgrade()
            {
                Speed = 5,
                Defense = 2,
                DefenseChance = 3,
                Name = "Sand-Walker Treads"
            };
        }

        public static LocomotionUpgrade SprintActuators()
        {
            return new LocomotionUpgrade()
            {
                Speed = 7,
                Defense = 3,
                AttackChance = 2,
                Name = "Sprint Actuators"
            };
        }

        public static LocomotionUpgrade ShockAbsorbingLimbs()
        {
            return new LocomotionUpgrade()
            {
                Speed = 6,
                Defense = 5,
                DefenseChance = 4,
                MaxHealth = 15,
                Name = "Shock-Absorbing Limbs"
            };
        }
    }
}