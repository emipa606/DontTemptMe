using System;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace DontTemptMe;

[StaticConstructorOnStartup]
internal static class DTMPatching
{
    static DTMPatching()
    {
        var harmonyInstance = new Harmony("com.DontTemptMe");
        var original = AccessTools.Method(typeof(Listing_Standard), "CheckboxLabeled");
        var original2 = AccessTools.Method(typeof(Listing_Standard), "ButtonText");
        var original3 = AccessTools.Method(typeof(GameDataSaveLoader), "SaveGame");
        var original4 = AccessTools.Method(typeof(SavedGameLoaderNow), "LoadGameFromSaveFileNow");
        var prefix = new HarmonyMethod(typeof(DTMPatching).GetMethod("PreFCheckboxLabeled"));
        var prefix2 = new HarmonyMethod(typeof(DTMPatching).GetMethod("PreFButtonText"));
        var prefix3 = new HarmonyMethod(typeof(DTMPatching).GetMethod("PreFSaveGame"));
        var prefix4 = new HarmonyMethod(typeof(DTMPatching).GetMethod("PreFLoadGameFromSaveFile"));
        harmonyInstance.Patch(original, prefix);
        harmonyInstance.Patch(original2, prefix2);
        harmonyInstance.Patch(original3, prefix3);
        harmonyInstance.Patch(original4, prefix4);
        Log.Message("DTMPatch initialized");
        if (!Prefs.DevMode)
        {
            return;
        }

        Prefs.DevMode = false;
        Log.Message("DevMode has been turned off.");
    }

    public static bool PreFCheckboxLabeled(ref string label)
    {
        return label != "DevelopmentMode".Translate();
    }

    public static bool PreFButtonText(ref string label)
    {
        return label != "ChangeStoryteller".Translate();
    }

    public static bool PreFSaveGame(string fileName)
    {
        try
        {
            var traverse = Traverse.CreateWithType("GameDataSaveLoader").Field("lastSaveTick");
            SafeSaver.Save(GenFilePaths.FilePathForSavedGame(fileName), "savegame", delegate
            {
                ScribeMetaHeaderUtility.WriteMetaHeader();
                var target = Current.Game;
                Scribe_Deep.Look(ref target, "Κgame");
            });
            traverse.SetValue(Find.TickManager.TicksGame);
        }
        catch (Exception ex)
        {
            Log.Message(
                $"EXCEPTION! {ex.TargetSite.ReflectedType?.Name}.{ex.TargetSite.Name} \n\tMESSAGE: {ex.Message} \n\tException occurred calling GameDataSaveLoader method");
        }

        return false;
    }

    public static bool PreFLoadGameFromSaveFile(string fileName)
    {
        var text = LoadedModManager.RunningMods.Select(mod => mod.ToString()).ToCommaList(true);
        Log.Message($"Loading game from file {fileName} with mods {text}");
        DeepProfiler.Start($"Loading game from file {fileName}");
        Current.Game = new Game();
        DeepProfiler.Start("InitLoading (read file)");
        Scribe.loader.InitLoading(GenFilePaths.FilePathForSavedGame(fileName));
        DeepProfiler.End();
        ScribeMetaHeaderUtility.LoadGameDataHeader(ScribeMetaHeaderUtility.ScribeHeaderMode.Map, true);
        if (!Scribe.EnterNode("Κgame"))
        {
            Log.Error("Could not find DTMG XML node.");
            Scribe.ForceStop();
            GenScene.GoToMainMenu();
            Messages.Message(
                "Game MUST be created with 'Don't Tempt Me!' loaded. Please select a 'Don't Tempt Me!' save game file.",
                MessageTypeDefOf.RejectInput);
            return false;
        }

        Current.Game = new Game();
        Current.Game.LoadGame();
        PermadeathModeUtility.CheckUpdatePermadeathModeUniqueNameOnGameLoad(fileName);
        DeepProfiler.End();
        return false;
    }
}