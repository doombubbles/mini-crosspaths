using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Towers;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Unity.Bridge;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.TowerSelectionMenu.TowerSelectionMenuThemes;
using Newtonsoft.Json.Linq;
using PathsPlusPlus;

namespace MiniCrosspaths.CrystalBallWizards;

public class CrystalBall : UpgradePlusPlus<CrystalBallWizardsPath>
{
    public override int Cost => 1000;
    public override int Tier => 2;

    public override string Description =>
        "Gains a toggle to allow long range targeting of Bloons in radius of your other towers. Loses Guided Magic's obstacle ignoring behavior while active.";

    public override string DetailedDescription =>
        "Toggle Advanced Intel style targeting at the cost of giving up Guided Magic ignoring blockers, if you have it. " +
        "Projectiles will always use Guided Magic homing to hit targets.";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        towerModel.towerSelectionMenuThemeId = GetId<CrystalBallTSM>();

        foreach (var attackModel in towerModel.GetAttackModels())
        {
            if (attackModel.HasBehavior<TargetFirstPrioCamoModel>())
            {
                attackModel.RemoveBehavior<TargetFirstPrioCamoModel>();
                attackModel.AddBehavior(TargetFirstSharedRangeModel.Create(new()
                {
                    isSelectable = true,
                    isSharedRangeEnabled = false
                }));
            }

            if (attackModel.HasBehavior<TargetLastPrioCamoModel>())
            {
                attackModel.RemoveBehavior<TargetLastPrioCamoModel>();
                attackModel.AddBehavior(TargetLastSharedRangeModel.Create(new()
                {
                    isSelectable = true,
                    isSharedRangeEnabled = false
                }));
            }

            if (attackModel.HasBehavior<TargetClosePrioCamoModel>())
            {
                attackModel.RemoveBehavior<TargetClosePrioCamoModel>();
                attackModel.AddBehavior(TargetCloseSharedRangeModel.Create(new()
                {
                    isSelectable = true,
                    isSharedRangeEnabled = false
                }));
            }

            if (attackModel.HasBehavior<TargetStrongPrioCamoModel>())
            {
                attackModel.RemoveBehavior<TargetStrongPrioCamoModel>();
                attackModel.AddBehavior(TargetStrongSharedRangeModel.Create(new()
                {
                    isSelectable = true,
                    isSharedRangeEnabled = false
                }));
            }

            attackModel.attackThroughWalls = false;
        }

        foreach (var weaponModel in towerModel.GetWeapons())
        {
            weaponModel.emission.AddBehavior(EmissionCamoIfTargetIsCamoModel.Create());
        }
    }

    public override void OnUpgraded(Tower tower)
    {
        tower.AddMutator<CrystalBallMutator>();
    }
}

public class CrystalBallMutator : ModMutator
{
    public override bool CantBeAbsorbed => true;
    public override bool Saved => true;
    public override int Priority => 5;

    public override bool Mutate(Model baseModel, Model model, JToken data)
    {
        if (!model.Is(out TowerModel towerModel)) return false;

        towerModel.ignoreBlockers = false;

        foreach (var attackModel in towerModel.GetAttackModels())
        {
            if (attackModel.HasBehavior(out TargetFirstSharedRangeModel first))
            {
                first.isSharedRangeEnabled = true;
            }
            if (attackModel.HasBehavior(out TargetLastSharedRangeModel last))
            {
                last.isSharedRangeEnabled = true;
            }
            if (attackModel.HasBehavior(out TargetCloseSharedRangeModel close))
            {
                close.isSharedRangeEnabled = true;
            }
            if (attackModel.HasBehavior(out TargetStrongSharedRangeModel strong))
            {
                strong.isSharedRangeEnabled = true;
            }

            attackModel.attackThroughWalls = false;
        }

        foreach (var projectileModel in model.GetDescendants<ProjectileModel>().AsIEnumerable())
        {
            var travelStraitModel = projectileModel.GetBehavior<TravelStraitModel>();
            if (travelStraitModel == null) continue;

            if (projectileModel.GetBehavior<TrackTargetModel>() == null)
            {
                projectileModel.AddBehavior(TrackTargetModel.Create(new()
                {
                    distance = 80,
                    trackNewTargets = true,
                    maxSeekAngle = 360,
                    turnRate = 360
                }));
            }

            projectileModel.ignoreBlockers = false;
        }

        return true;
    }
}

public class CrystalBallTSM : ModTsmTheme
{
    public override string BaseTheme => "Default";

    public TSMButton ToggleCrystalBall { get; private set; } = null!;

    public ModHelperImage Icon { get; private set; } = null!;

    public override void SetupTheme(BaseTSMTheme theme)
    {
        ToggleCrystalBall = theme.gameObject.AddTSMButton(
            new Info(nameof(ToggleCrystalBall), RightArrowX, AboveArrowsY, DefaultButtonSize),
            VanillaSprites.TwitchBtnSquare, nameof(ToggleCrystalBall));

        Icon = ToggleCrystalBall.gameObject.AddImage(new Info(nameof(Icon), DefaultIconSize), "");
    }

    public override void TowerChanged(BaseTSMTheme theme, TowerToSimulation tower)
    {
        Icon.Image.SetSprite(tower.tower.IsMutatedBy<CrystalBallMutator>()
            ? GetSpriteReference(nameof(CrystalBall)).AssetGUID
            : VanillaSprites.GuildedMagicUpgradeIcon);

        ToggleCrystalBall.gameObject.SetActive(tower.Def.appliedUpgrades.Contains(UpgradeType.GuidedMagic));
    }

    public override void OnButtonPressed(BaseTSMTheme theme, TowerToSimulation tower, string buttonId)
    {
        if (buttonId != nameof(ToggleCrystalBall)) return;

        if (tower.tower.IsMutatedBy<CrystalBallMutator>())
        {
            tower.tower.RemoveMutator<CrystalBallMutator>();
        }
        else
        {
            tower.tower.AddMutator<CrystalBallMutator>();
        }
    }
}