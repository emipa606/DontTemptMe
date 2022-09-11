using UnityEngine;
using Verse;

namespace DontTemptMe;

[StaticConstructorOnStartup]
public static class DTMBin
{
    public static Texture2D DTMBanner;

    public static Texture2D RedditIcon;

    static DTMBin()
    {
        DTMBanner = ContentFinder<Texture2D>.Get("DTM/Banner");
        RedditIcon = ContentFinder<Texture2D>.Get("DTM/Reddit_Icon");
        DTMBanner.wrapMode = TextureWrapMode.Clamp;
        RedditIcon.wrapMode = TextureWrapMode.Clamp;
    }
}