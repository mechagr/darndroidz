using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RogueSharp.Random;

namespace DarnDroidz.Systems
{
    public class EquipmentGenerator
    {
        private readonly Pool<Core.Equipment> _equipmentPool;

        public EquipmentGenerator(int level)
        {
            _equipmentPool = new Pool<Core.Equipment>();

            if (level <= 3)
            {
                _equipmentPool.Add(Equipment.ChassisEquipment.ScrapMetalPlating(), 20);
                _equipmentPool.Add(Equipment.SensorEquipment.CrackedSensorSuite(), 20);
                _equipmentPool.Add(Equipment.SensorEquipment.SalvagedTrackingUnit(), 20);
                _equipmentPool.Add(Equipment.LocomotionUpgrade.RustyLegServos(), 20);
                _equipmentPool.Add(Equipment.WeaponMount.MakeshiftBoltThrower(), 25);
                _equipmentPool.Add(Equipment.WeaponMount.ScrapSpikeLauncher(), 5);
                _equipmentPool.Add(Equipment.ChassisEquipment.CompositeShielding(), 5);
                _equipmentPool.Add(Equipment.SensorEquipment.ThermalVisionSystem(), 5);
            }
            else if (level <= 6)
            {
                _equipmentPool.Add(Equipment.ChassisEquipment.CompositeShielding(), 20);
                _equipmentPool.Add(Equipment.ChassisEquipment.RustProofChassis(), 20);
                _equipmentPool.Add(Equipment.SensorEquipment.ThermalVisionSystem(), 20);
                _equipmentPool.Add(Equipment.SensorEquipment.TargetingOptics(), 20);
                _equipmentPool.Add(Equipment.LocomotionUpgrade.AllTerrainPawGrips(), 20);
                _equipmentPool.Add(Equipment.LocomotionUpgrade.ReinforcedJointAssembly(), 20);
                _equipmentPool.Add(Equipment.LocomotionUpgrade.SandWalkerTreads(), 15);
                _equipmentPool.Add(Equipment.WeaponMount.BiteForceAmplifier(), 15);
                _equipmentPool.Add(Equipment.WeaponMount.IntegratedPulseRifle(), 15);
                _equipmentPool.Add(Equipment.SensorEquipment.RadiationScannerArray(), 5);
                _equipmentPool.Add(Equipment.LocomotionUpgrade.SprintActuators(), 5);
            }
            else
            {
                _equipmentPool.Add(Equipment.SensorEquipment.RadiationScannerArray(), 25);
                _equipmentPool.Add(Equipment.SensorEquipment.MilitaryGradeScanner(), 25);
                _equipmentPool.Add(Equipment.LocomotionUpgrade.SprintActuators(), 25);
                _equipmentPool.Add(Equipment.LocomotionUpgrade.ShockAbsorbingLimbs(), 25);
                _equipmentPool.Add(Equipment.WeaponMount.PlasmaJawCannon(), 25);
                _equipmentPool.Add(Equipment.WeaponMount.ShoulderMountedLaser(), 25);
            }
        }

        public Core.Equipment CreateEquipment()
        {
            return _equipmentPool.Get();
        }
    }
}
