using Il2CppAssets.Scripts.Models.Towers;
using PathsPlusPlus;

namespace MiniCrosspaths.HoverPlaneAces;

public class Pursuit : UpgradePlusPlus<HoverPlaneAcesPath>
{
    public override int Cost => 500;
    public override int Tier => 2;

    public override string Description =>
        "A new targeting option enables Monkey Ace to seek and pursue the Bloons automatically.";

    public override string DetailedDescription =>
        "Gains the Pursuit targeting option like Heli Pilots.";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        // Covered by HoverPlane tier check already
    }
}