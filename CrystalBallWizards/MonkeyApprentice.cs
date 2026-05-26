using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using PathsPlusPlus;

namespace MiniCrosspaths.CrystalBallWizards;

public class MonkeyApprentice : UpgradePlusPlus<CrystalBallWizardsPath>
{
    public override int Cost => 75;
    public override int Tier => 1;

    public override string Description => "Further study makes all Wizard projectiles travel much further.";

    public override string DetailedDescription =>
        "All projectiles that travel in a straight line have doubled lifespan.";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        foreach (var travelStraitModel in towerModel.GetDescendants<TravelStraitModel>().AsIEnumerable())
        {
            travelStraitModel.Lifespan *= 2;
        }
    }
}