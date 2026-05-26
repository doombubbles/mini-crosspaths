using System;
using System.Linq;
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
using MiniCrosspaths.DoubleRangaBoomers;
using PathsPlusPlus;
using UnityEngine;

namespace MiniCrosspaths.LaserRangBoomers;

public class PlasmaRangs : UpgradePlusPlus<LaserRangBoomersPath>
{
    public override int Cost => 350;
    public override int Tier => 2;

    public override string Description => "Upgrades to Plasma-Rangs, attacking even faster.";

    public override string DetailedDescription =>
        "Can pop Frozen and Lead Bloons. Fire rate for all weapons is 50% faster, including Glaive-Lord orbital damage.";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        var mainProj = towerModel.GetAttackModel().GetDescendant<ProjectileModel>();

        var damage = mainProj.GetDamageModel();
        damage.immuneBloonProperties &= ~(BloonProperties.Lead | BloonProperties.Frozen);
        damage.immuneBloonPropertiesOriginal &= ~(BloonProperties.Lead | BloonProperties.Frozen);

        foreach (var weapon in DoubleRangaBoomersPath.BoomerAttacks(towerModel).SelectMany(attack => attack.weapons))
        {
            weapon.Rate /= 1.5f;
        }

        var display = Game.instance.model
            .GetTower(TowerType.SuperMonkey, 2, 0, towerModel.appliedUpgrades.Contains(UpgradeType.Glaives) ? 4 : 3)
            .GetAttackModel().GetDescendant<ProjectileModel>().display;

        mainProj.SetDisplay(display);
        if (towerModel.HasBehavior(out OrbitModel orbitModel))
        {
            orbitModel.projectile.ApplyDisplay<PlasmaOrbit>();
            var orbitDamage = towerModel.GetAttackModel("Orbit").GetDescendant<DamageModel>();
            orbitDamage.immuneBloonProperties &= ~(BloonProperties.Lead | BloonProperties.Frozen);
            orbitDamage.immuneBloonPropertiesOriginal &= ~(BloonProperties.Lead | BloonProperties.Frozen);
        }
    }
}

public class PlasmaOrbit : ModDisplay
{
    private static readonly int TintColor = Shader.PropertyToID("_TintColor");

    public override PrefabReference BaseDisplayReference =>
        Game.instance.model.GetTower(TowerType.BoomerangMonkey, 5, 0, 0).GetBehavior<OrbitModel>().projectile.display;

    public override void ModifyDisplayNodeAsync(UnityDisplayNode node, Action onComplete)
    {
        node.GetComponentInChildren<ParticleSystemRenderer>().material
            .SetColor(TintColor, new Color(231 / 255f, 0, 255 / 255f, .5f));

        var display = Game.instance.model
            .GetTower(TowerType.SuperMonkey, 2, 0, 4)
            .GetAttackModel().GetDescendant<ProjectileModel>().display;

        UseNode(display.AssetGUID, displayNode =>
        {
            node.GetRenderer<SpriteRenderer>().sprite = displayNode.GetRenderer<SpriteRenderer>().sprite;
            node.RecalculateGenericRenderers();
            onComplete();
        });
    }
}