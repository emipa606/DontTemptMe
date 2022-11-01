using System.Reflection;
using HarmonyLib;
using Verse;

namespace DontTemptMe;

[StaticConstructorOnStartup]
internal static class DTMPatching
{
    static DTMPatching()
    {
        var harmonyInstance = new Harmony("com.DontTemptMe");
        harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        Log.Message("DTMPatch initialized");
        if (!Prefs.DevMode)
        {
            return;
        }

        Prefs.DevMode = false;
        Log.Message("DevMode has been turned off.");
    }
}