using System;
using System.Linq;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Simulation.Towers.Weapons;
using PathsPlusPlus;

namespace MiniCrosspaths.InstantBombShooters;

public class PrecisionMunition : UpgradePlusPlus<InstantBombShootersPath>
{
    public override int Cost => 500;
    public override int Tier => 2;

    public override string Description =>
        "Can hit Bloons it can see anywhere on screen. Attacks 30% slower for very far away Bloons.";

    public override string DetailedDescription =>
        "Gains global effective range. If Bloons are further than twice the original displayed range, attacks 30% slower.";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        towerModel.isGlobalRange = true;
    }

    [HarmonyPatch(typeof(Weapon), nameof(Weapon.CalcRateFrames))]
    internal static class Weapon_CalcRateFrames
    {
        [HarmonyPostfix]
        internal static void Postfix(Weapon __instance)
        {
            if (!__instance.weaponModel.name.Contains(nameof(PrecisionMunition)) ||
                __instance.newProjectiles.FirstOrDefault() is not { } proj ||
                proj.target == null) return;

            var distance = __instance.GetEjectPosition().Distance(proj.target.Position);

            if (distance / __instance.attack.tower.towerModel.range > 2)
            {
                __instance.rateFrames = (int) (__instance.rateFrames * 1.3f);
            }
        }
    }
}