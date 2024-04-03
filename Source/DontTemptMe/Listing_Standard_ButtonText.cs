using HarmonyLib;
using Verse;

namespace DontTemptMe;

[HarmonyPatch(typeof(Listing_Standard), nameof(Listing_Standard.ButtonText))]
public static class Listing_Standard_ButtonText
{
    public static bool Prefix(ref string label)
    {
        return label != "ChangeStoryteller".Translate();
    }
}