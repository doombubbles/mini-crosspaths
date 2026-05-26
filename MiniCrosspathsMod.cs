global using BTD_Mod_Helper.Extensions;
global using HarmonyLib;
using MelonLoader;
using BTD_Mod_Helper;
using MiniCrosspaths;

[assembly: MelonInfo(typeof(MiniCrosspathsMod), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace MiniCrosspaths;

public class MiniCrosspathsMod : BloonsTD6Mod;