using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
using PathsPlusPlus;
using UnityEngine;

namespace MiniCrosspaths.InstantBombShooters;

public class QuantumPayload : UpgradePlusPlus<InstantBombShootersPath>
{
    public override int Cost => 150;

    public override int Tier => 1;

    public override string Description => "Bombs explode instantly at the target's location with no travel time.";

    public override void ApplyUpgrade(TowerModel towerModel, int tier)
    {
        foreach (var attack in towerModel.GetDescendants<AttackModel>().AsIEnumerable())
        {
            var weapon = attack.weapons[0];
            var projectile = weapon?.projectile;

            if (weapon == null ||
                projectile == null ||
                !weapon.emission.Is<SingleEmissionModel>() ||
                !projectile.HasBehavior<TravelStraitModel>() ||
                !Mathf.Approximately(projectile.maxPierce, 1)) continue;

            if (tier > 1)
            {
                attack.range = 999999;
                weapon.SetName(weapon.name + "_" + nameof(PrecisionMunition));
            }

            weapon.SetEmission(InstantDamageEmissionModel.Create());
            projectile.RemoveBehavior<TravelStraitModel>();
            projectile.RemoveBehavior<TrackTargetModel>();
            projectile.AddBehavior(InstantModel.Create());
            projectile.AddBehavior(AgeModel.Create(new() { lifespan = .1f }));

            if (weapon.GetBehavior<AlternateProjectileModel>().Is(out var alternate))
            {
                alternate.RemoveChildDependant(alternate.emissionModel);
                alternate.emissionModel = InstantDamageEmissionModel.Create();
                alternate.AddChildDependant(alternate.emissionModel);
                alternate.projectile.RemoveBehavior<TravelStraitModel>();
                alternate.projectile.RemoveBehavior<TrackTargetModel>();
                projectile.AddBehavior(InstantModel.Create());
                projectile.AddBehavior(AgeModel.Create(new() { lifespan = .1f }));
            }
        }
    }
}