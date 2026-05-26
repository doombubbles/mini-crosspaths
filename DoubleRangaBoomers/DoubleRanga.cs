using System.Linq;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using PathsPlusPlus;

namespace MiniCrosspaths.DoubleRangaBoomers;

public class DoubleRanga : UpgradePlusPlus<DoubleRangaBoomersPath>
{
    public override int Cost => 400;
    public override int Tier => 2;

    public override string Description => "Throws 2 Boomerangs at a time, but not as frequently.";

    public override string DetailedDescription =>
        "Main boomerangs, MOAB Press Boomerangs, and orbiting glaives have double projectile count but two thirds attack speed.";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        foreach (var weapon in DoubleRangaBoomersPath.BoomerAttacks(towerModel).SelectMany(attack => attack.weapons))
        {
            weapon.Rate *= 1.5f;
            weapon.SetEmission(ArcEmissionModel.Create(new()
            {
                angle = 30,
                count = 2
            }));
        }

        if (towerModel.HasBehavior(out OrbitModel orbitModel))
        {
            orbitModel.count *= 2;
            towerModel.GetAttackModel("Orbit").weapons[0]!.Rate *= .75f;
        }
    }
}