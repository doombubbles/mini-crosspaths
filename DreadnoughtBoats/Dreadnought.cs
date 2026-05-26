using System.Linq;
using BTD_Mod_Helper.Api.Display;
using Il2Cpp;
using Il2CppAssets.Scripts.Models.GenericBehaviors;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Display;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using PathsPlusPlus;
using UnityEngine;
using Math = System.Math;

namespace MiniCrosspaths.DreadnoughtBoats;

public class Dreadnought : UpgradePlusPlus<DreadnoughtBoatsPath>
{
    public override int Tier => 1;
    public override int Cost => 600;

    public override string Description =>
        "Shoots molten cannonballs instead of darts/grapes that deal extra damage and can pop all Bloon types.";

    public override string DetailedDescription =>
        "Projectiles can hit all Bloon types, and their base damage and additive bonuses are all increased by +1 or x1.3, whichever is higher.";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        var display = Game.instance.model.GetTower(TowerType.WizardMonkey, 0, 1, 0)
            .GetAttackModel("Fireball")
            .GetDescendant<DisplayModel>()
            .display;

        var altDisplay = Game.instance.model.GetTower(TowerType.TackShooter, 5, 0, 0)
            .GetAttackModel("Meteor")
            .GetDescendant<DisplayModel>()
            .display;

        var bombDisplay = Game.instance.model.GetTower(TowerType.BombShooter, 3, 0, 0)
            .GetDescendant<ProjectileModel>()
            .display;

        var towers = towerModel.GetDescendants<TowerModel>().AsIEnumerable().Prepend(towerModel);
        var projectiles = towers.SelectMany(t => t.GetDescendants<ProjectileModel>().AsIEnumerable());

        foreach (var proj in projectiles)
        {
            if (proj.HasBehavior(out DamageModel damage))
            {
                damage.immuneBloonProperties = damage.immuneBloonPropertiesOriginal = BloonProperties.None;

                damage.damage = Math.Max(damage.damage + 1, damage.damage * 1.3f);
                foreach (var damageModifierModel in proj.GetBehaviors<DamageModifierForTagModel>())
                {
                    damageModifierModel.damageAddative = Math.Max(damageModifierModel.damageAddative + 1,
                        damageModifierModel.damageAddative * 1.3f);
                }
            }

            if (!proj.HasBehavior<TravelStraitModel>()) return;

            proj.SetDisplay<MoltenCannonBall>();

            if (proj.GetBehaviors<CreateProjectileOnContactModel>().Count < 1)
            {
                proj.scale = proj.radius > 3 ? .7f : .5f;
            }
        }
    }
}

public class MoltenCannonBall : ModDisplay
{
    public override PrefabReference BaseDisplayReference => Game.instance.model.GetTower(TowerType.TackShooter, 5, 0, 0)
        .GetAttackModel("Meteor")
        .GetDescendant<DisplayModel>()
        .display;

    public override void ModifyDisplayNode(UnityDisplayNode node)
    {
        node.GetRenderer<ParticleSystemRenderer>("Head").gameObject.SetActive(false);
        node.GetRenderer<ParticleSystemRenderer>("Tail").gameObject.SetActive(false);
    }

}