using System;
using HarmonyLib;
using Verse;

namespace DontTemptMe;

[HarmonyPatch(typeof(GameDataSaveLoader), nameof(GameDataSaveLoader.SaveGame))]
public static class GameDataSaveLoader_SaveGame
{
    public static bool Prefix(string fileName)
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
}