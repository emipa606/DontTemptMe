using System.Reflection;
using HarmonyLib;
using Verse;

namespace DontTemptMe;

[StaticConstructorOnStartup]
internal static class DTMPatching
{
    static DTMPatching()
    {
        new Harmony("com.DontTemptMe").PatchAll(Assembly.GetExecutingAssembly());
    }
}