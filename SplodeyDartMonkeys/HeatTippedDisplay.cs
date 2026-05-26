using BTD_Mod_Helper.Api.Display;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Display;
using Il2CppNinjaKiwi.Common.ResourceUtils;

namespace MiniCrosspaths.SplodeyDartMonkeys;

public abstract class HeatTippedDisplay : ModDisplay
{
    public abstract int Tier { get; }

    public override PrefabReference BaseDisplayReference => Game.instance.model
        .GetTower(TowerType.DartMonkey, 0, 0, Tier)
        .GetDescendant<ProjectileModel>().display;

    public override void ModifyDisplayNode(UnityDisplayNode node)
    {
        node.GetSpriteRenderer().sprite = GetSprite(Name);
    }

}