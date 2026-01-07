using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarnDroidz.Equipment
{
    public class SensorEquipment : Core.Equipment
    {
        public static SensorEquipment None()
        {
            return new SensorEquipment { Name = "None" };
        }

        public static SensorEquipment CrackedSensorSuite()
        {
            return new SensorEquipment()
            {
                Awareness = 2,
                Defense = 1,
                AttackChance = 1,
                Name = "Cracked Sensor Suite"
            };
        }

        public static SensorEquipment SalvagedTrackingUnit()
        {
            return new SensorEquipment()
            {
                Awareness = 3,
                AttackChance = 2,
                Name = "Salvaged Tracking Unit"
            };
        }

        public static SensorEquipment ThermalVisionSystem()
        {
            return new SensorEquipment()
            {
                Awareness = 5,
                Defense = 2,
                AttackChance = 3,
                Name = "Thermal Vision System"
            };
        }

        public static SensorEquipment TargetingOptics()
        {
            return new SensorEquipment()
            {
                Awareness = 4,
                AttackChance = 5,
                Defense = 1,
                Name = "Targeting Optics Module"
            };
        }

        public static SensorEquipment RadiationScannerArray()
        {
            return new SensorEquipment()
            {
                Awareness = 7,
                Defense = 3,
                AttackChance = 4,
                MaxHealth = 5,
                Name = "Radiation Scanner Array"
            };
        }

        public static SensorEquipment MilitaryGradeScanner()
        {
            return new SensorEquipment()
            {
                Awareness = 9,
                Defense = 4,
                AttackChance = 6,
                MaxHealth = 10,
                Name = "Military-Grade Scanner"
            };
        }
    }
}