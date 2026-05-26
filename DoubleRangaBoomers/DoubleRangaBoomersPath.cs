using System.Collections.Generic;
using BTD_Mod_Helper.Api.Enums;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using PathsPlusPlus;

namespace MiniCrosspaths.DoubleRangaBoomers;

public class DoubleRangaBoomersPath : PathPlusPlus
{
    public override string Tower => TowerType.BoomerangMonkey;

    public static List<AttackModel> BoomerAttacks(TowerModel towerModel)
    {
        var attacks = new List<AttackModel> {towerModel.GetAttackModel()};
        if (towerModel.appliedUpgrades.Contains(UpgradeType.MOABPress))
        {
            attacks.Add(towerModel.GetAttackModel("MOABPress"));
        }

        return attacks;
    }
}