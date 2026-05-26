using System;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Api.Enums;
using Il2Cpp;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Display;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using Il2CppSystem.IO;
using PathsPlusPlus;
using UnityEngine;

namespace MiniCrosspaths.LaserRangBoomers;

public class LaserRangs : UpgradePlusPlus<LaserRangBoomersPath>
{
    public override int Cost => 250;
    public override int Tier => 1;

    public override string Description => "Now throws Laser-Rangs, gaining increased pierce and projectile speed.";

    public override string DetailedDescription =>
        "Projectiles can longer pop Purple Bloons, but travel 25% faster and have pierce increased by +4 or x1.5, whichever is higher, with MOAB press getting a flat +60.";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        var mainProj = towerModel.GetAttackModel().GetDescendant<ProjectileModel>();

        foreach (var proj in towerModel.GetDescendants<ProjectileModel>().ToArray())
        {
            if (!proj.HasBehavior<DamageModel>()) continue;

            proj.pierce = Math.Max(proj.pierce + 4, (int) (proj.pierce * 1.5f));
        }

        var damage = mainProj.GetDamageModel();
        if (towerModel.tier < 5)
        {
            damage.immuneBloonProperties |= BloonProperties.Purple;
            damage.immuneBloonPropertiesOriginal |= BloonProperties.Purple;
        }

        if (mainProj.HasBehavior(out FollowPathModel followPathModel))
        {
            followPathModel.Speed *= 1.25f;
        }
        else if (mainProj.HasBehavior(out TravelStraitModel travelStraitModel))
        {
            travelStraitModel.Speed *= 1.25f;
        }

        if (towerModel.appliedUpgrades.Contains(UpgradeType.MOABPress))
        {
            var proj = towerModel.GetAttackModel("MOABPress").GetDescendant<ProjectileModel>();
            proj.pierce += 60;
        }

        var display = Game.instance.model
            .GetTower(TowerType.SuperMonkey, 1, 0, towerModel.appliedUpgrades.Contains(UpgradeType.Glaives) ? 4 : 3)
            .GetAttackModel().GetDescendant<ProjectileModel>().display;

        mainProj.SetDisplay(display);
        if (towerModel.HasBehavior(out OrbitModel orbitModel))
        {
            orbitModel.projectile.ApplyDisplay<LaserOrbit>();
        }

        if (towerModel.HasDescendant(out AddBehaviorToBloonModel dot))
        {
            dot.parentDamageModel = null;
            dot.RemoveChildDependant(dot.parentDamageModel);
        }
    }
}

public class LaserOrbit : ModDisplay
{
    private static readonly int TintColor = Shader.PropertyToID("_TintColor");

    public override PrefabReference BaseDisplayReference =>
        Game.instance.model.GetTower(TowerType.BoomerangMonkey, 5, 0, 0).GetBehavior<OrbitModel>().projectile.display;

    public override void ModifyDisplayNodeAsync(UnityDisplayNode node, Action onComplete)
    {
        node.GetComponentInChildren<ParticleSystemRenderer>().material
            .SetColor(TintColor, new Color(255 / 255f, 28 / 255f, 0, .5f));

        var display = Game.instance.model
            .GetTower(TowerType.SuperMonkey, 1, 0, 4)
            .GetAttackModel().GetDescendant<ProjectileModel>().display;

        UseNode(display.AssetGUID, displayNode =>
        {
            node.GetRenderer<SpriteRenderer>().sprite = displayNode.GetRenderer<SpriteRenderer>().sprite;
            node.RecalculateGenericRenderers();
            onComplete();
        });
    }
}