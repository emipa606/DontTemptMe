using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Text;
using Harmony;
using System.Reflection;
using Verse;
using RimWorld;
using UnityEngine;

namespace DontTemptMe
{
	[StaticConstructorOnStartup]
	internal static class DTMPatching
	{
		static DTMPatching()
		{
			HarmonyInstance DTMPatch = HarmonyInstance.Create("com.DontTemptMe");

			MethodInfo methInfCheckboxLabeled = AccessTools.Method(typeof(Listing_Standard), "CheckboxLabeled", null, null);
			MethodInfo methInfButtonText = AccessTools.Method(typeof(Listing_Standard), "ButtonText", null, null);
			MethodInfo methInfSaveGame = AccessTools.Method(typeof(GameDataSaveLoader), "SaveGame", null, null);
			MethodInfo methInfLoadGameFromSaveFile = AccessTools.Method(typeof(SavedGameLoader), "LoadGameFromSaveFile", null, null);

			HarmonyMethod harmonyMethodPreFCheckboxLabeled = new HarmonyMethod(typeof(DTMPatching).GetMethod("PreFCheckboxLabeled"));
			HarmonyMethod harmonyMethodPreFButtonText = new HarmonyMethod(typeof(DTMPatching).GetMethod("PreFButtonText"));
			HarmonyMethod harmonyMethodPreFSaveGame = new HarmonyMethod(typeof(DTMPatching).GetMethod("PreFSaveGame"));
			HarmonyMethod harmonyMethodPreFLoadGameFromSaveFile = new HarmonyMethod(typeof(DTMPatching).GetMethod("PreFLoadGameFromSaveFile"));

			DTMPatch.Patch(methInfCheckboxLabeled, harmonyMethodPreFCheckboxLabeled, null, null);
			DTMPatch.Patch(methInfButtonText, harmonyMethodPreFButtonText, null, null);
			DTMPatch.Patch(methInfSaveGame, harmonyMethodPreFSaveGame, null, null);
			DTMPatch.Patch(methInfLoadGameFromSaveFile, harmonyMethodPreFLoadGameFromSaveFile, null, null);

			Log.Message("DTMPatch initialized");

			if (Prefs.DevMode == true)
			{
				Prefs.DevMode = false;
				Log.Message("DevMode has been turned off.");
			}

		}

		public static bool PreFCheckboxLabeled(ref string label)
		{
			if (label == Translator.Translate("DevelopmentMode"))
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public static bool PreFButtonText(ref string label)
		{
			if (label == Translator.Translate("ChangeStoryteller"))
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public static bool PreFSaveGame(string fileName)
		{
			try
			{


				Traverse traverse = Traverse.CreateWithType("GameDataSaveLoader").Field("lastSaveTick");
				SafeSaver.Save(GenFilePaths.FilePathForSavedGame(fileName), "savegame", delegate
				{
					ScribeMetaHeaderUtility.WriteMetaHeader();
					Game game = Current.Game;
					Scribe_Deep.Look<Game>(ref game, "Κgame", new object[0]);
				});
				traverse.SetValue(Find.TickManager.TicksGame);
			}
			catch (Exception e)
			{
				Log.Message(string.Format("EXCEPTION! {0}.{1} \n\tMESSAGE: {2} \n\tException occurred calling {3} method", e.TargetSite.ReflectedType.Name,
					e.TargetSite.Name, e.Message));
			}

			return false;
		}


		public static bool PreFLoadGameFromSaveFile(string fileName)
		{
			string str = GenText.ToCommaList(from mod in LoadedModManager.RunningMods
											 select mod.ToString(), true);
			Log.Message("Loading game from file " + fileName + " with mods " + str);
			DeepProfiler.Start("Loading game from file " + fileName);
			Current.Game = new Game();
			DeepProfiler.Start("InitLoading (read file)");
			Scribe.loader.InitLoading(GenFilePaths.FilePathForSavedGame(fileName));
			DeepProfiler.End();
			ScribeMetaHeaderUtility.LoadGameDataHeader(ScribeMetaHeaderUtility.ScribeHeaderMode.Map, true);
			bool flag = !Scribe.EnterNode("Κgame");
			bool result;
			if (flag)
			{
				Log.Error("Could not find DTMG XML node.");
				Scribe.ForceStop();
				GenScene.GoToMainMenu();
				Messages.Message("Game MUST be created with 'Don't Tempt Me!' loaded. Please select a 'Don't Tempt Me!' save game file.", MessageTypeDefOf.RejectInput);
				result = false;
			}
			else
			{
				Current.Game = new Game();
				Current.Game.LoadGame();
				PermadeathModeUtility.CheckUpdatePermadeathModeUniqueNameOnGameLoad(fileName);
				DeepProfiler.End();
				result = false;
			}
			return result;
		}
	}
}
