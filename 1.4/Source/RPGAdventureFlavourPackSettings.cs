using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using Verse;

namespace MrSamuelStreamer.RPGAdventureFlavourPack;

public class RPGAdventureFlavourPackSettings : ModSettings
{
    public bool DragonsInRelicSites = true;
    public bool ShowCaravanLoot = true;
    public bool AddExtraRimQuests = true;
    public bool AllowExtraMedievalItems = true;
    public int ElderAgeThreshold = 60;
    public float ElderAgeMultiplier = 1f;
    public float GlobalHungerFactor = -1f;
    public SimpleCurve ChronoFieldAgeCurve = new(DefaultChronoAgeCurve.Points);

    private Vector2 _chronoFieldAgeCurveScrollPosition;
    private List<Action> _settingsUpdatedActions = [];
    public void RegisterSettingsUpdatedAction(Action action) => _settingsUpdatedActions.Add(action);

    public float GetGlobalHungerFactor() => GlobalHungerFactor < 0 ? 1 : GlobalHungerFactor;

    public SimpleCurve GetChronoFieldAgeCurve() => ChronoFieldAgeCurve ?? DefaultChronoAgeCurve;

    /**
     * A special setting that if you've downloaded the configs off the workshop will already be true
     * If false a message box will pop up at game start warning you you should get the configs
     * People not playing along can just tick it in settings.
     */
    public bool ConfigsApplied = false;

    private HashSet<string> _extraRimQuestsMatching = [];
    private HashSet<string> _extraRimQuestGivers = [];
    private HashSet<string> _extraMedievalItems = [];

    private readonly bool _rimQuestActive = ModLister.GetActiveModWithIdentifier("mlie.rimquest", true) != null;
    private readonly bool _rimMedievalActive = ModLister.GetActiveModWithIdentifier("Ogam.Rimedieval", true) != null;
    private string _rimQuestTextEntry = "Some_RimQuestGiver or Filter";
    private string _rimMedievalTextEntry = "DefName of extra item to allow e.g. Genepack";
    private string _elderAgeBuffer;
    private string _elderAgeMultiplierBuffer;

    private const float RowHeight = 32f;
    private const float Indent = 9f;
    private static Vector2 _scrollPosition;
    private readonly Listing_Standard _options = new();

    public HashSet<string> ExtraRimQuestGivers()
    {
        if (!AddExtraRimQuests || (_extraRimQuestGivers?.Count ?? 0) != 0) return _extraRimQuestGivers;
        Log.Message("RPGAdventureFlavourPack Extra RimQuests is enabled but no quest-givers chosen, defaulting");
        _extraRimQuestGivers = [..DefaultExtraRimQuestGivers];
        NotifySettingsUpdate();
        return _extraRimQuestGivers;
    }

    public HashSet<string> ExtraRimQuestsMatching()
    {
        if (!AddExtraRimQuests || (_extraRimQuestsMatching?.Count ?? 0) != 0) return _extraRimQuestsMatching;
        Log.Message("RPGAdventureFlavourPack Extra RimQuests is enabled but inclusion list was empty, defaulting");
        _extraRimQuestsMatching = [..DefaultExtraRimQuestsMatching];
        NotifySettingsUpdate(true);
        return _extraRimQuestsMatching;
    }

    public HashSet<string> ExtraMedievalDefs()
    {
        if (!AllowExtraMedievalItems || (_extraMedievalItems?.Count ?? 0) != 0) return _extraMedievalItems;
        Log.Message("RPGAdventureFlavourPack Extra Medieval Items is enabled but inclusion list was empty, defaulting");
        _extraMedievalItems = [..DefaultExtraMedievalItems];
        NotifySettingsUpdate(true);
        return _extraMedievalItems;
    }

    private void NotifySettingsUpdate(bool forceWrite = false)
    {
        foreach (Action action in _settingsUpdatedActions)
        {
            action();
        }

        if (forceWrite) Write();
    }

    private enum Tab
    {
        Core,
        RimQuest,
        RimMedieval
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
            case Tab.RimMedieval:
                DrawRimMedievalSettings(viewPort);
                break;
            default:
                throw new ArgumentException($"Unknown tab selected: {_tab.ToString()}");
        }

        _options.End();
    }

    private void DrawCoreSettings(Rect viewPort)
    {
        _options.CheckboxLabeled("RPGAdventureFlavourPackSettings_Core_DragonsInRelicSites".Translate(),
            ref DragonsInRelicSites);

        _options.CheckboxLabeled("RPGAdventureFlavourPackSettings_Core_ConfigsApplied".Translate(),
            ref ConfigsApplied);

        _options.CheckboxLabeled("RPGAdventureFlavourPackSettings_Core_ShowCaravanLoot".Translate(),
            ref ShowCaravanLoot);

        if (GlobalHungerFactor < 0) GlobalHungerFactor = GetGlobalHungerFactor();
        GlobalHungerFactor = _options.SliderLabeled("RPGAdventureFlavourPackSettings_Core_GlobalHungerFactor".Translate(GlobalHungerFactor.ToStringPercent()), GlobalHungerFactor,
            0.0f, 5f);



        if (ModsConfig.IsActive("vanillaexpanded.vpsycastse"))
        {
            // Technically doesn't require VPE but we're reusing the chronopath patch for efficiency
            _elderAgeBuffer = ElderAgeThreshold.ToString();
            _options.Label("RPGAdventureFlavourPackSettings_Core_ElderAgeThreshold".Translate());
            _options.IntEntry(ref ElderAgeThreshold, ref _elderAgeBuffer);
            _elderAgeMultiplierBuffer ??= ElderAgeMultiplier.ToString(CultureInfo.InvariantCulture);
            _options.TextFieldNumericLabeled("RPGAdventureFlavourPackSettings_Core_ElderAgeMultiplier".Translate(), ref ElderAgeMultiplier, ref _elderAgeMultiplierBuffer);

            ChronoFieldAgeCurve ??= DefaultChronoAgeCurve;
            _options.Label("RPGAdventureFlavourPackSettings_Core_ChronomancerAgeFactor".Translate());
            DrawCurve(_options, ref ChronoFieldAgeCurve, ref _chronoFieldAgeCurveScrollPosition);
            if (ChronoFieldAgeCurve.Points.Count == 0)
            {
                ChronoFieldAgeCurve.SetPoints([new CurvePoint(0, 1)]);
                Log.Error("Set points to 0");
            }
        }
    }

    private void DrawRimQuestSettings(Rect viewPort)
    {
        _options.CheckboxLabeled("RPGAdventureFlavourPackSettings_RimQuest_AddExtraRimQuests".Translate(),
            ref AddExtraRimQuests);
        _rimQuestTextEntry = _options.TextEntry(_rimQuestTextEntry);
        if (_options.ButtonText("RPGAdventureFlavourPackSettings_RimQuest_AddExtraQuestGiver".Translate()))
        {
            _extraRimQuestGivers.Add(_rimQuestTextEntry);
        }

        if (_options.ButtonText("RPGAdventureFlavourPackSettings_RimQuest_AddExtraQuestsMatching".Translate()))
        {
            _extraRimQuestsMatching.Add(_rimQuestTextEntry);
        }

        Listing_Standard scrollableListing = MakeScrollableSubListing(viewPort, ref _scrollPosition, _options.CurHeight);
        scrollableListing.Label("RPGAdventureFlavourPackSettings_RimQuest_ExtraQuestGivers".Translate());
        scrollableListing.Indent(Indent);
        foreach (string rimQuestExtraQuestGiver in _extraRimQuestGivers.Where(rimQuestExtraQuestGiver =>
                     scrollableListing.ButtonTextLabeled(rimQuestExtraQuestGiver,
                         "RPGAdventureFlavourPackSettings_Remove".Translate())))
        {
            _extraRimQuestGivers.Remove(rimQuestExtraQuestGiver);
        }

        scrollableListing.Outdent(Indent);
        scrollableListing.GapLine();
        scrollableListing.Label("RPGAdventureFlavourPackSettings_RimQuest_ExtraQuestsMatching".Translate());
        scrollableListing.Indent(Indent);
        foreach (string rimQuestMatcher in _extraRimQuestsMatching.Where(rimQuestMatcher =>
                     scrollableListing.ButtonTextLabeled(rimQuestMatcher,
                         "RPGAdventureFlavourPackSettings_Remove".Translate())))
        {
            _extraRimQuestsMatching.Remove(rimQuestMatcher);
        }

        EndScrollableSubListing(scrollableListing);
    }

    private void DrawRimMedievalSettings(Rect viewPort)
    {
        _options.CheckboxLabeled("RPGAdventureFlavourPackSettings_RimMedieval_AllowExtraMedievalItems".Translate(),
            ref AllowExtraMedievalItems);
        _rimMedievalTextEntry = _options.TextEntry(_rimMedievalTextEntry);
        if (_options.ButtonText("RPGAdventureFlavourPackSettings_RimMedieval_AllowExtraMedievalItem".Translate()))
        {
            _extraMedievalItems.Add(_rimMedievalTextEntry);
        }

        Listing_Standard scrollableListing = MakeScrollableSubListing(viewPort, ref _scrollPosition, _options.CurHeight);
        scrollableListing.Label("RPGAdventureFlavourPackSettings_RimMedieval_AllowedExtraMedievalItems".Translate());
        scrollableListing.Indent(Indent);
        foreach (var extraMedievalItem in _extraMedievalItems.Where(extraMedievalItem =>
                     scrollableListing.ButtonTextLabeled(extraMedievalItem,
                         "RPGAdventureFlavourPackSettings_Remove".Translate())))
        {
            _extraMedievalItems.Remove(extraMedievalItem);
        }

        scrollableListing.Outdent(Indent);
        scrollableListing.GapLine();

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

        if (_rimMedievalActive)
        {
            tabsList.Add(new TabRecord("RPGAdventureFlavourPackSettings_Tab_RimMedieval".Translate(),
                () => _tab = Tab.RimMedieval,
                _tab == Tab.RimMedieval));
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

    private Listing_Standard MakeScrollableSubListing(Rect viewPort, ref Vector2 scrollPosition, float topOffset)
    {
        Rect scrollRect = viewPort.BottomPartPixels(viewPort.yMax - topOffset - RowHeight);
        Rect dataRect = scrollRect.ContractedBy(30, 10);
        Widgets.BeginScrollView(dataRect, ref scrollPosition, scrollRect);
        Listing_Standard scrollableListing = new();
        scrollableListing.Begin(dataRect);
        return scrollableListing;
    }

    public void DrawCurve(Listing_Standard listing, ref SimpleCurve curve, ref Vector2 scrollPosition)
    {
        for (int i = 0; i < curve.PointsCount; i++)
        {
            CurvePoint point = curve[i];

            Rect pointRect = listing.GetRect(Text.LineHeight + 3).ContractedBy(listing.ColumnWidth / 5f, 0f);

            Widgets.Label(pointRect.LeftHalf().LeftHalf(), "RPGAdventureFlavourPackSettings_Generic_CurvePoint".Translate(i + 1, point.x, point.y));

            string xBuffer = point.x.ToString(CultureInfo.InvariantCulture);
            float x = point.x;
            Widgets.TextFieldNumeric(pointRect.LeftHalf().RightHalf(), ref x, ref xBuffer);

            string yBuffer = point.y.ToString(CultureInfo.InvariantCulture);
            float y = point.y;
            Widgets.TextFieldNumeric(pointRect.RightHalf().LeftHalf(), ref y, ref yBuffer);
            curve[i] = new CurvePoint(x, y);

            if (Widgets.ButtonText(pointRect.RightHalf().RightHalf(), "Remove".Translate()))
            {
                curve.Points.Remove(point);
            }

            listing.GapLine();
        }

        if (listing.ButtonText("Add".Translate()))
        {
            CurvePoint p = curve.MaxBy(e => e.x);
            Log.Message($"Adding point {p.x + 1}, {p.y + 1}");
            curve.Add(p.x + 1, p.y + 1);
        }
    }

    public override void ExposeData()
    {
        if (Scribe.mode == LoadSaveMode.Saving) ChronoFieldAgeCurve ??= DefaultChronoAgeCurve;
        base.ExposeData();
        Scribe_Values.Look(ref DragonsInRelicSites, "ExtraFunRelics", true);
        Scribe_Values.Look(ref ConfigsApplied, "ConfigsApplied", false);
        Scribe_Values.Look(ref ShowCaravanLoot, "ShowCaravanLoot", true);
        Scribe_Values.Look(ref AddExtraRimQuests, "AddExtraRimQuests", true);
        Scribe_Values.Look(ref AllowExtraMedievalItems, "AllowExtraMedievalItems", true);
        Scribe_Values.Look(ref GlobalHungerFactor, "GlobalHungerFactor", 1f);
        Scribe_Values.Look(ref ElderAgeThreshold, "ElderAgeThreshold", 60);
        Scribe_Values.Look(ref ElderAgeMultiplier, "ElderAgeMultiplier", 1f);
        Scribe_Collections.Look(ref _extraRimQuestGivers, "ExtraRimQuestGivers", LookMode.Value);
        Scribe_Collections.Look(ref _extraRimQuestsMatching, "ExtraRimQuestsMatching", LookMode.Value);
        Scribe_Collections.Look(ref _extraMedievalItems, "ExtraMedievalItems", LookMode.Value);

        List<CurvePoint> curvePoints = null;

        if (Scribe.mode == LoadSaveMode.Saving && ChronoFieldAgeCurve != null)
        {
            curvePoints = ChronoFieldAgeCurve.Points;
        }

        Scribe_Collections.Look(ref curvePoints, "ChronoFieldAgeCurve", LookMode.Value);

        if ((Scribe.mode == LoadSaveMode.LoadingVars || Scribe.mode == LoadSaveMode.PostLoadInit) && curvePoints != null)
        {
            ChronoFieldAgeCurve = new SimpleCurve(curvePoints);
        }

        GlobalHungerFactor = GetGlobalHungerFactor();

        switch (Scribe.mode)
        {
            case LoadSaveMode.PostLoadInit:
                ChronoFieldAgeCurve ??= DefaultChronoAgeCurve;
                break;
            case LoadSaveMode.Saving:
                NotifySettingsUpdate();
                break;
        }
    }

    private static SimpleCurve DefaultChronoAgeCurve = new(new[]
    {
        new CurvePoint(0f, 1f),
        new CurvePoint(1f, 1.5f),
        new CurvePoint(2f, 4f),
        new CurvePoint(4f, 8f),
        new CurvePoint(8f, 10f),
    });

    private static readonly HashSet<string> DefaultExtraMedievalItems =
        ["Genepack"];

    private static readonly HashSet<string> DefaultExtraRimQuestGivers =
        ["RQ_MedievalQuestGiver", "RQ_TribalQuestGiver"];

    private static readonly HashSet<string> DefaultExtraRimQuestsMatching =
        ["VPE_EltexMeteor", "_MonsterEncounterQuest", "Hunt"];
}
