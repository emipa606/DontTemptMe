using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace DontTemptMe;

[HarmonyPatch]
public static class Listing_Standard_CheckboxLabeled
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        return AccessTools.GetDeclaredMethods(typeof(Listing_Standard))
            .Where(methodInfo => methodInfo.Name == "CheckboxLabeled");
    }

    public static bool Prefix(ref string label)
    {
        return label != "DevelopmentMode".Translate();
    }
}