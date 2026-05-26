using System.Linq;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.ModOptions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Unity;
using PathsPlusPlus;

namespace MiniCrosspaths.HoverPlaneAces;

public class HoverPlane : UpgradePlusPlus<HoverPlaneAcesPath>
{
    public override int Cost => 600;
    public override int Tier => 1;

    public override string Description => "Now longer flies in paths, instead hovering wherever you direct it.";

    public override string DetailedDescription => "Replaces all default movement with Heli Pilot style movement.";

    public override void ApplyUpgrade(TowerModel towerModel, int tier)
    {
        if (towerModel.HasDescendant<HeliMovementModel>()) return;

        var heli = Game.instance.model.GetTower(TowerType.HeliPilot);
        var pursuitHeli = Game.instance.model.GetTower(TowerType.HeliPilot, 2, 0, 0);

        var airUnit = towerModel.GetBehavior<AirUnitModel>();
        var attacks = towerModel.GetBehaviors<AttackModel>();

        var heliMovement = heli.GetDescendant<HeliMovementModel>().Duplicate();
        heliMovement.maxSpeed = airUnit.GetBehavior<PathMovementModel>().speed;
        heliMovement.tiltAngle = 0;

        airUnit.RemoveBehavior<PathMovementModel>();
        airUnit.AddBehavior(heliMovement);

        var firstAttack = true;
        var shouldHavePursuit = tier > 1;

        foreach (var attack in attacks.OrderBy(model => model.name.Length))
        {
            attack.fireWithoutTarget = false;
            attack.range = 99999;

            foreach (var weaponModel in attack.weapons)
            {
                weaponModel.fireWithoutTarget = false;
            }

            attack.RemoveBehaviors<CirclePatternModel>();
            attack.RemoveBehaviors<FigureEightPatternModel>();
            attack.RemoveBehaviors<CenterElipsePatternModel>();
            attack.RemoveBehaviors<WingmonkeyPatternModel>();

            var copyFromAttack = (shouldHavePursuit ? pursuitHeli : heli).GetAttackModel();

            attack.AddBehavior(copyFromAttack.GetBehavior<RotateToTargetAirUnitModel>().Duplicate());

            attack.GetDescendants<ArcEmissionModel>().ForEach(arc => arc.useAirUnitRotation = true);

            if (!firstAttack) continue;
            firstAttack = false;

            foreach (var targetSupplierModel in copyFromAttack.GetBehaviors<TargetSupplierModel>())
            {
                attack.AddBehavior(targetSupplierModel.Duplicate(Name));
            }

            if (attack.HasBehavior(out PursuitSettingModel pursuit))
            {
                pursuit.pursuitDistance = towerModel.appliedUpgrades.Contains(UpgradeType.Spectre) ? 45 : 15;
            }
        }

        towerModel.UpdateTargetProviders();

        if (ModHelper.HasMod("TacticalTweaks", out var tacticalTweaks) &&
            tacticalTweaks.ModSettings.TryGetValue("PursuitPathPrioritization", out var setting)
            && setting is ModSettingBool enabled && enabled)
        {
            towerModel.towerSelectionMenuThemeId = "TacticalTweaks-PursuitTsmTheme";
        }
        else
        {
            towerModel.towerSelectionMenuThemeId = "Default";
        }
    }
}