using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using PathsPlusPlus;

namespace MiniCrosspaths.DreadnoughtBoats;

public class Privateer : UpgradePlusPlus<DreadnoughtBoatsPath>
{
    public override int Cost => 800;
    public override int Tier => 2;

    public override string Description =>
        "Earns 50% more cash from Bloons popped / hooked. Merchantman income is increased by 10%.";

    public override string DetailedDescription =>
        "Cash from Bloons is 1.5x, Merchantman income is +10% after other additive bonuses like Trade Empire.";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        towerModel.AddBehavior(CashIncreaseModel.Create(new()
        {
            name = Name,
            multiplier = 1.5f
        }));

        if (towerModel.HasDescendant(out PerRoundCashBonusTowerModel cashPerRound))
        {
            cashPerRound.cashRoundBonusMultiplier += .1f;
        }
    }
}