using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace DontTemptMe;

[HarmonyPatch(typeof(SavedGameLoaderNow), nameof(SavedGameLoaderNow.LoadGameFromSaveFileNow))]
public static class SavedGameLoaderNow_LoadGameFromSaveFileNow
{
    public static bool Prefix(string fileName)
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