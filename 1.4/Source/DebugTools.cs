using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace MrSamuelStreamer.RPGAdventureFlavourPack;

public class DebugTools
{
    // foreach (CustomXenotype customXenotype in Current.Game.customXenotypeDatabase.customXenotypes)
    // <nameMaker>${p.genes.CustomXenotype?.}</nameMaker>
    //<chanceToUseNameMaker>1</chanceToUseNameMaker>
    [DebugAction("Pawns", "Print Xenotype", false, false, true, 0, false, actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
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
                              {p.genes.Xenogenes.Select(g => geneLine(g.def)).ToLineList()}
                              </genes>
                          </XenotypeDef>
                      
                      </Defs>
                      """;

        Log.Message(blob);

        DebugActionsUtility.DustPuffFrom(p);
    }

    [DebugAction("Pawns", "Print All Xenotypes", false, false, true, 0, false, actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    private static void PrintXenotypes()
    {
        string blob = $"""
                      <?xml version="1.0" encoding="utf-8" ?>
                      <Defs>
                      
                      {Current.Game.customXenotypeDatabase.customXenotypes.Select(XenotypeBlob).ToLineList()}
                      
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
            SafeSaver.Save(absFilePath, "savedXenotype", (Action) (() =>
            {
                ScribeMetaHeaderUtility.WriteMetaHeader();
                Scribe_Deep.Look<CustomXenotype>(ref xenotype, nameof (xenotype));
            }));
        }
        catch (Exception ex)
        {
            Log.Error("Exception while saving xenotype: " + ex.ToString());
        }
    }
}

