using System.Linq;
using Il2Cpp;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using PathsPlusPlus;

namespace MiniCrosspaths.SplodeyDartMonkeys;

public class HeatTippedDarts : UpgradePlusPlus<SplodeyDartMonkeysPath>
{
    public override int Cost => 125;
    public override int Tier => 1;

    public override string Description => "Heat tipped darts allow the Dart Monkey to pop Frozen and Lead Bloons.";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        towerModel.GetDescendants<DamageModel>().ForEach(damage =>
        {
            damage.immuneBloonProperties &= ~BloonProperties.Frozen;
            damage.immuneBloonProperties &= ~BloonProperties.Lead;
            damage.immuneBloonPropertiesOriginal &= ~BloonProperties.Frozen;
            damage.immuneBloonPropertiesOriginal &= ~BloonProperties.Lead;
        });


        var dart = towerModel.GetDescendant<ProjectileModel>();

        foreach (var heatTippedDisplay in GetContent<HeatTippedDisplay>()
                     .Where(heatTippedDisplay => dart.display == heatTippedDisplay.BaseDisplayReference))
        {
            heatTippedDisplay.Apply(dart);
        }
    }
}