using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace MrSamuelStreamer.RPGAdventureFlavourPack;

public class RPGAdventureFlavourPackSettings : ModSettings
{
    public bool ShowCaravanLoot = true;
    public bool AddExtraRimQuests = true;

    public HashSet<string> ExtraRimQuestsMatching = new();
    public HashSet<string> ExtraRimQuestGivers = new();

    private readonly bool _rimQuestActive = ModLister.GetActiveModWithIdentifier("mlie.rimquest", true) != null;
    private string _rimQuestTextEntry = "Some_RimQuestGiver or Filter";

    private const float RowHeight = 32f;
    private const float Indent = 9f;
    private static Vector2 _scrollPosition;
    private readonly Listing_Standard _options = new();

    private enum Tab
    {
        Core,
        RimQuest
    }

    private static Tab _tab = Tab.Core;

    public void DoWindowContents(Rect wrect)
    {
        Rect viewPort = DrawTabs(wrect);
        _options.Begin(viewPort);

        switch (_tab)
        {
            case Tab.Core:
                DrawCoreSettings(viewPort);
                break;
            case Tab.RimQuest:
                DrawRimQuestSettings(viewPort);
                break;
            default:
                throw new ArgumentException($"Unknown tab selected: {_tab.ToString()}");
        }

        _options.End();
    }

    private void DrawCoreSettings(Rect viewPort)
    {
        _options.CheckboxLabeled("RPGAdventureFlavourPackSettings_Core_ShowCaravanLoot".Translate(),
            ref ShowCaravanLoot);
    }

    private void DrawRimQuestSettings(Rect viewPort)
    {
        _options.CheckboxLabeled("RPGAdventureFlavourPackSettings_RimQuest_AddExtraRimQuests".Translate(),
            ref AddExtraRimQuests);
        _rimQuestTextEntry = _options.TextEntry(_rimQuestTextEntry);
        if (_options.ButtonText("RPGAdventureFlavourPackSettings_RimQuest_AddExtraQuestGiver".Translate()))
        {
            ExtraRimQuestGivers.Add(_rimQuestTextEntry);
        }

        if (_options.ButtonText("RPGAdventureFlavourPackSettings_RimQuest_AddExtraQuestsMatching".Translate()))
        {
            ExtraRimQuestsMatching.Add(_rimQuestTextEntry);
        }

        Listing_Standard scrollableListing = MakeScrollableSubListing(viewPort);
        scrollableListing.Label("RPGAdventureFlavourPackSettings_RimQuest_ExtraQuestGivers".Translate());
        scrollableListing.Indent(Indent);
        foreach (var rimQuestExtraQuestGiver in ExtraRimQuestGivers.Where(rimQuestExtraQuestGiver =>
                     scrollableListing.ButtonTextLabeled(rimQuestExtraQuestGiver,
                         "RPGAdventureFlavourPackSettings_Remove".Translate())))
        {
            ExtraRimQuestGivers.Remove(rimQuestExtraQuestGiver);
        }

        scrollableListing.Outdent(Indent);
        scrollableListing.GapLine();
        scrollableListing.Label("RPGAdventureFlavourPackSettings_RimQuest_ExtraQuestsMatching".Translate());
        scrollableListing.Indent(Indent);
        foreach (var rimQuestMatcher in ExtraRimQuestsMatching.Where(rimQuestMatcher =>
                     scrollableListing.ButtonTextLabeled(rimQuestMatcher,
                         "RPGAdventureFlavourPackSettings_Remove".Translate())))
        {
            ExtraRimQuestsMatching.Remove(rimQuestMatcher);
        }

        EndScrollableSubListing(scrollableListing);
    }


    private Rect DrawTabs(Rect rect)
    {
        List<TabRecord> tabsList = new()
        {
            new TabRecord("RPGAdventureFlavourPackSettings_Tab_Core".Translate(), () => _tab = Tab.Core,
                _tab == Tab.Core)
        };

        if (_rimQuestActive)
        {
            tabsList.Add(new TabRecord("RPGAdventureFlavourPackSettings_Tab_RimQuest".Translate(),
                () => _tab = Tab.RimQuest,
                _tab == Tab.RimQuest));
        }

        Rect tabRect = rect.ContractedBy(0, RowHeight);
        TabDrawer.DrawTabs(tabRect, tabsList);

        return tabRect.GetInnerRect();
    }

    private void EndScrollableSubListing(Listing scrollableListing)
    {
        scrollableListing.Outdent(Indent);
        scrollableListing.GapLine();
        scrollableListing.End();
        Widgets.EndScrollView();
    }

    private Listing_Standard MakeScrollableSubListing(Rect viewPort)
    {
        Rect scrollRect = viewPort.BottomPartPixels(viewPort.yMax - _options.CurHeight - RowHeight);
        Rect dataRect = scrollRect.ContractedBy(30, 10);
        Widgets.BeginScrollView(dataRect, ref _scrollPosition, scrollRect);
        Listing_Standard scrollableListing = new();
        scrollableListing.Begin(dataRect);
        return scrollableListing;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref ShowCaravanLoot, "ShowCaravanLoot", true);
        Scribe_Values.Look(ref AddExtraRimQuests, "AddExtraRimQuests", true);
        Scribe_Collections.Look(ref ExtraRimQuestGivers, "ExtraRimQuestGivers", LookMode.Value);
        Scribe_Collections.Look(ref ExtraRimQuestsMatching, "ExtraRimQuestsMatching", LookMode.Value);
        
        if (AddExtraRimQuests && (ExtraRimQuestGivers?.Count ?? 0) == 0)
            ExtraRimQuestGivers = new HashSet<string>(DefaultExtraRimQuestGivers);
        if (AddExtraRimQuests && (ExtraRimQuestsMatching?.Count ?? 0) == 0)
            ExtraRimQuestsMatching = new HashSet<string>(DefaultExtraRimQuestsMatching);
    }

    private static readonly HashSet<string> DefaultExtraRimQuestGivers =
        new() { "RQ_MedievalQuestGiver", "RQ_TribalQuestGiver" };

    private static readonly HashSet<string> DefaultExtraRimQuestsMatching =
        new() { "VPE_EltexMeteor", "_MonsterEncounterQuest", "Hunt" };
}
