using System.Collections.Generic;
using System.Reflection;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Helpers;
using Il2Cpp;
using Il2CppAssets.Scripts.Models.Audio;
using Il2CppAssets.Scripts.Models.Effects;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Simulation.Towers.Projectiles.Behaviors;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using PathsPlusPlus;

namespace MiniCrosspaths.SplodeyDartMonkeys;

public class SplodeyDarts : UpgradePlusPlus<SplodeyDartMonkeysPath>
{
    public override int Cost => 400;
    public override int Tier => 2;

    public override string Description =>
        "Projectiles will create a small explosion the first time they collide with a Bloon.";

    public override string DetailedDescription =>
        "Explosions have the same pierce as the projectile, and the same base damage (but unaffected by damage modifiers).";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        base.ApplyUpgrade(towerModel);

        foreach (var projectile in towerModel.GetDescendants<ProjectileModel>().AsIEnumerable())
        {
            if (!projectile.HasBehavior<TravelStraitModel>() ||
                !projectile.HasBehavior(out DamageModel damageModel)) continue;

            projectile.AddBehavior(CreateProjectileOnContactModel.Create(new()
            {
                name = nameof(SplodeyDarts),
                emission = SingleEmissionModel.Create(),
                projectile = ProjectileModel.Create(new()
                {
                    name = "Splosion",
                    pierce = projectile.pierce,
                    radius = 10,
                    ignoreBlockers = true,
                    behaviors =
                    [
                        DamageModel.Create(new()
                        {
                            damage = damageModel.damage,
                            maxDamage = damageModel.damage,
                            immuneBloons = towerModel.tier == 5 ? BloonProperties.None : BloonProperties.Black
                        }),
                        AgeModel.Create(new()
                        {
                            lifespan = 0.1f
                        })
                    ],
                    CanHitCamo = towerModel.appliedUpgrades.Contains(UpgradeType.EnhancedEyesight),
                })
            }));

            projectile.AddBehavior(CreateEffectOnContactModel.Create(new()
            {
                name = nameof(SplodeyDarts),
                effectModel = EffectModel.Create(new()
                {
                    assetId = new PrefabReference("6d84b13b7622d2744b8e8369565bc058"),
                    lifespan = 2
                })
            }));

            projectile.AddBehavior(CreateSoundOnProjectileCollisionModel.Create(new()
            {
                name = nameof(SplodeyDarts),
                sound1 = SoundModel.Create(new()
                {
                    assetId = new AudioClipReference("ec9a5a7531a750e4c82bf8429e0812e4")
                }),
                sound2 = SoundModel.Create(new()
                {
                    assetId = new AudioClipReference("57fac571daa668140b0674eb515f9586")
                }),
                sound3 = SoundModel.Create(new()
                {
                    assetId = new AudioClipReference("5b1ee796433122141a0db034af251f15")
                }),
                sound4 = SoundModel.Create(new()
                {
                    assetId = new AudioClipReference("f5f616027b9887c4db5b1341a614b352")
                }),
                sound5 = SoundModel.Create(new()
                {
                    assetId = new AudioClipReference("9a36e64ca09a7ef42bd4af49c1cf4ef6")
                })
            }));
        }
    }
}

[HarmonyPatch]
internal static class CreateProjectileOnContact_Collide
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(CreateProjectileOnContact), nameof(CreateProjectileOnContact.Collide));
        yield return AccessTools.Method(typeof(CreateProjectileOnContact),
            nameof(CreateProjectileOnContact.CollideMap));
    }

    [HarmonyPrefix]
    internal static bool Prefix(CreateProjectileOnContact __instance)
    {
        return !(__instance.projectile != null &&
                 __instance.createProjectileOnContactModel.name.Contains(nameof(SplodeyDarts)) &&
                 __instance.projectile.collidedWith.Count > 1);
    }
}

[HarmonyPatch]
internal static class CreateEffectOnContact_Collide
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(CreateEffectOnContact), nameof(CreateEffectOnContact.Collide));
        yield return AccessTools.Method(typeof(CreateEffectOnContact),
            nameof(CreateEffectOnContact.CollideMap));
    }

    [HarmonyPrefix]
    internal static bool Prefix(CreateEffectOnContact __instance)
    {
        return !(__instance.projectile != null &&
                 __instance.createEffectOnContactModel.name.Contains(nameof(SplodeyDarts)) &&
                 __instance.projectile.collidedWith.Count > 1);
    }
}

[HarmonyPatch]
internal static class CreateSoundOnProjectileCollision_
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(CreateSoundOnProjectileCollision),
            nameof(CreateSoundOnProjectileCollision.Collide));
        yield return AccessTools.Method(typeof(CreateSoundOnProjectileCollision),
            nameof(CreateSoundOnProjectileCollision.CollideMap));
    }

    [HarmonyPrefix]
    internal static bool Prefix(CreateSoundOnProjectileCollision __instance)
    {
        return !(__instance.projectile != null &&
                 __instance.createSoundOnProjectileCollisionModel.name.Contains(nameof(SplodeyDarts)) &&
                 __instance.projectile.collidedWith.Count > 1);
    }
}