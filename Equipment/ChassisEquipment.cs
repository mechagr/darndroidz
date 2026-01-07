using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarnDroidz.Equipment
{
    public class ChassisEquipment : Core.Equipment
    {
        public static ChassisEquipment None()
        {
            return new ChassisEquipment { Name = "None" };
        }

        public static ChassisEquipment ScrapMetalPlating()
        {
            return new ChassisEquipment()
            {
                Defense = 1,
                DefenseChance = 10,
                Name = "Scrap Metal Plating"
            };
        }

        public static ChassisEquipment CompositeShielding()
        {
            return new ChassisEquipment()
            {
                Defense = 2,
                DefenseChance = 5,
                Name = "Composite Shielding"
            };
        }

        public static ChassisEquipment RustProofChassis()
        {
            return new ChassisEquipment()
            {
                Defense = 2,
                DefenseChance = 10,
                Name = "Rust-Proof Chassis"
            };
        }
    }
}
