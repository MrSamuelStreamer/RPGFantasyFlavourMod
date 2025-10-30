using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LudeonTK;
using RimWorld;
using Verse;

namespace MrSamuelStreamer.RPGAdventureFlavourPack;

public class DebugTools
{
    [DebugAction("Pawns", "Print Xenotype", requiresRoyalty = false, requiresIdeology = false, requiresBiotech = true, requiresAnomaly = false, displayPriority = 0,
        hideInSubMenu = false, actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    private static void PrintXenotypeForPawn(Pawn p)
    {
        if (!p.RaceProps.Humanlike)
            return;

        string blob = $"""
                       <?xml version="1.0" encoding="utf-8" ?>
                       <Defs>
                       
                           <XenotypeDef>
                               <defName>{p.genes.xenotypeName}</defName>
                               <label>{p.genes.xenotypeName}</label>
                               <description>{p.genes.XenotypeDescShort}</description>
                               <descriptionShort>{p.genes.XenotypeDescShort}</descriptionShort>
                               <iconPath>{p.genes.Xenotype.iconPath}</iconPath>
                               <inheritable>{p.genes.CustomXenotype?.inheritable ?? p.genes.Xenotype.inheritable}</inheritable>
                               <genes>
                               <!-- Xenogenes -->
                               {p.genes.Xenogenes.Select(g => geneLine(g.def)).ToLineList()}
                               <!-- Endogenes -->
                               {p.genes.Endogenes.Select(g => geneLine(g.def)).ToLineList()}
                               </genes>
                           </XenotypeDef>

                       </Defs>
                       """;

        Log.Message(blob);

        DebugActionsUtility.DustPuffFrom(p);
    }

    [DebugAction("Pawns", "Print All Xenotypes", requiresRoyalty = false, requiresIdeology = false, requiresBiotech = true, requiresAnomaly = false, displayPriority = 0,
        hideInSubMenu = false, actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    private static void PrintXenotypes()
    {
        HashSet<CustomXenotype> allXenotypes = [];
        HashSet<string> allXenotypeFiles = [];
        allXenotypes.AddRange(Current.Game?.customXenotypeDatabase?.customXenotypes ?? []);
        allXenotypeFiles.AddRange(Current.Game?.customXenotypeDatabase?.customXenotypes?.Select(x => x.fileName) ?? []);

        HashSet<string> extraXenosToLoad = CharacterCardUtility.CustomXenotypesForReading?.Select(x => x.fileName).Where(cx => !allXenotypeFiles.Contains(cx)).ToHashSet() ?? [];
        allXenotypes.AddRange(CharacterCardUtility.CustomXenotypesForReading?.Where(x => extraXenosToLoad.Contains(x.fileName)) ?? []);
        allXenotypeFiles.AddRange(extraXenosToLoad);

        foreach (FileInfo fileInfo in GenFilePaths.AllCustomXenotypeFiles.OrderBy(f => f.LastWriteTime))
        {
            string filePath = GenFilePaths.AbsFilePathForXenotype(Path.GetFileNameWithoutExtension(fileInfo.Name));
            CustomXenotype xenotype;
            if (allXenotypeFiles.Contains(filePath) || !GameDataSaveLoader.TryLoadXenotype(filePath, out xenotype)) continue;
            allXenotypes.Add(xenotype);
            allXenotypeFiles.Add(filePath);
        }

        string blob = $"""
                       <?xml version="1.0" encoding="utf-8" ?>
                       <Defs>

                       {allXenotypes.Select(XenotypeBlob).ToLineList()}

                       </Defs>
                       """;

        Log.Message(blob);
    }

    private static string XenotypeBlob(CustomXenotype xenotype) =>
        $"""
         <XenotypeDef>
            <defName>ExportedCustomXeno_{xenotype.name.Replace(' ', '_').Replace(',', '_').Replace('.', '_').Replace('-', '_')}</defName>
            <label>{xenotype.name}</label>
            <description>{xenotype.name}</description>
            <descriptionShort>{xenotype.name}</descriptionShort>
            <iconPath>{xenotype.IconDef.texPath}</iconPath>
            <inheritable>{xenotype.inheritable}</inheritable>
            <genes>
            {xenotype.genes.Select(geneLine).ToLineList()}
            </genes>
         </XenotypeDef>

         """;

    private static string geneLine(GeneDef geneDef)
    {
        StringBuilder sb = new();
        sb.Append("            <li");
        if (!geneDef.modContentPack.IsCoreMod) sb.Append($" MayRequire=\"{geneDef.modContentPack.PackageId}\"");
        sb.Append(">");
        sb.Append(geneDef.defName);
        sb.Append("</li>");
        return sb.ToString();
    }


    public static void PrintXenotype(CustomXenotype xenotype, string absFilePath)
    {
        try
        {
            xenotype.fileName = Path.GetFileNameWithoutExtension(absFilePath);
            SafeSaver.Save(absFilePath, "savedXenotype", () =>
            {
                ScribeMetaHeaderUtility.WriteMetaHeader();
                Scribe_Deep.Look(ref xenotype, nameof(xenotype));
            });
        }
        catch (Exception ex)
        {
            Log.Error("Exception while saving xenotype: " + ex);
        }
    }
}
