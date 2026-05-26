#if DEBUG
using System;
using System.IO;
using System.Linq;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api.Commands;
using Il2CppAssets.Scripts.Unity;
using PathsPlusPlus;

namespace MiniCrosspaths;

public class MirrorUniverseReadmeCommand : ModCommand<GenerateCommand>
{
    public override string Command => "minicrosspathsmd";

    public override string Help => "Generates entries in the README";

    public override bool Execute(ref string resultText)
    {
        var folder = Path.Combine(ModHelper.ModSourcesDirectory, nameof(MiniCrosspaths));

        var readme = Path.Combine(folder, "README.md");

        var lines = File.ReadAllLines(readme).ToList();
        var start = lines.IndexOf("<!--Start-->") + 1;
        var end = lines.LastIndexOf("<!--End-->");

        lines.RemoveRange(start, end - start);

        var text = "\n";

        foreach (var path in mod.Content.OfType<PathPlusPlus>()
                     .OrderBy(p => Game.instance.model.towerSet.FindIndex(tdm => tdm.towerId == p.Tower)))
        {
            text += $"## {path.DisplayName}\n\n";

            foreach (var upgrade in path.Upgrades.Values)
            {
                text +=
                    $"<img src='{path.Name.Replace("Path", "")}/{upgrade.Name}.png' height=50 align='right' alt='{upgrade.DisplayName}' >\n\n";
                text += $"#### {upgrade.Tier}. {upgrade.DisplayName} (${upgrade.Cost:N0})\n\n";

                if (string.IsNullOrEmpty(upgrade.DetailedDescription))
                {
                    text += $"{upgrade.Description}\n\n";
                }
                else
                {
                    text += $"""
                             <details>
                             <summary>{upgrade.Description}</summary>
                             {upgrade.DetailedDescription}
                             </details>
                             """ + "\n\n";
                }
            }
        }

        lines.InsertRange(start, text.ReplaceLineEndings().Split(Environment.NewLine));

        File.WriteAllLines(readme, lines);

        return true;
    }
}

#endif