using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using System.Reflection;
using Verse;
using RimWorld;

namespace DontTemptMe
{
	[StaticConstructorOnStartup]
	internal static class DTMPatching
	{
		static DTMPatching()
		{
			HarmonyInstance Listing_StandardPatch = HarmonyInstance.Create("com.DontTemptMe.Listing_Standard.CheckboxLabeled");
			MethodInfo methInfCheckboxLabeled = AccessTools.Method(typeof(Listing_Standard), "CheckboxLabeled", null, null);
			HarmonyMethod harmonyMethodPreFCheckboxLabeled = new HarmonyMethod(typeof(DTMPatching).GetMethod("PreFCheckboxLabeled"));
			Listing_StandardPatch.Patch(methInfCheckboxLabeled, harmonyMethodPreFCheckboxLabeled, null, null);
			Log.Message("Listing_StandardPatch initialized");
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
	}
}
