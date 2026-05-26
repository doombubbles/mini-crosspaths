using Il2Cpp;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using PathsPlusPlus;

namespace MiniCrosspaths.DoubleRangaBoomers;

public class SonicBoom : UpgradePlusPlus<DoubleRangaBoomersPath>
{
    public override int Cost => 100;
    public override int Tier => 1;

    public override string Description => "Boomerangs can smash through Frozen Bloons for extra damage.";

    public override string DetailedDescription =>
        "All projectiles can pop Frozen bloons, gaining a damage bonus equal to the projectile's base damage.";

    public override void ApplyUpgrade(TowerModel towerModel, int tier)
    {
        towerModel.GetDescendants<ProjectileModel>().ForEach(proj =>
        {
            if (!proj.HasBehavior(out DamageModel damage)) return;

            damage.immuneBloonPropertiesOriginal &= ~BloonProperties.Frozen;
            damage.immuneBloonProperties &= ~BloonProperties.Frozen;

            proj.AddBehavior(DamageModifierForBloonStateModel.Create(new()
            {
                name = "Frozen",
                bloonState = "Ice",
                damageMultiplier = 1,
                damageAdditive = damage.damage,
                mustBeModified = true
            }));

            proj.hasDamageModifiers = true;
        });


        if (towerModel.HasDescendant(out AddBehaviorToBloonModel dot))
        {
            dot.parentDamageModel = null;
            dot.RemoveChildDependant(dot.parentDamageModel);
        }
    }
}