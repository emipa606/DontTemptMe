using System.Collections.Generic;
using LudeonTK;
using Mlie;
using UnityEngine;
using Verse;

namespace DontTemptMe;

public class DTMMod : Mod
{
    private static string currentVersion;
    private readonly Vector2 vecDtmBanner = new(739f, 110f);
    private float floVertSpacer;

    private Vector2 vecDtmBannerSize = Vector2.zero;

    public DTMMod(ModContentPack content)
        : base(content)
    {
        GetSettings<DTMSettings>();
        currentVersion = VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);
    }

    public override string SettingsCategory()
    {
        return "Don't Tempt Me!";
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        //IL_01c6: Unknown result type (might be due to invalid IL or missing references)
        //IL_01d0: Expected O, but got Unknown
        floVertSpacer = inRect.height * 0.025f;
        Rect rect;
        if (inRect.width > vecDtmBanner.x)
        {
            rect = new Rect(0f, 40f, vecDtmBanner.x, vecDtmBanner.y);
        }
        else
        {
            vecDtmBannerSize.x = inRect.width;
            vecDtmBannerSize.y = vecDtmBannerSize.x / vecDtmBanner.x * vecDtmBanner.y;
            rect = new Rect(0f, 40f, vecDtmBannerSize.x, vecDtmBannerSize.y);
        }

        var position = new Rect(rect.x, rect.y + rect.height + floVertSpacer, inRect.width * 0.25f,
            inRect.height * 0.05f);
        var position2 = new Rect(rect.x, position.y + position.height + floVertSpacer, inRect.width * 0.25f,
            inRect.height * 0.05f);
        var rect2 = new Rect(rect.x, position2.y + position2.height + floVertSpacer, inRect.width * 0.25f,
            inRect.height * 0.05f);
        GUI.DrawTexture(rect, DTMBin.DTMBanner);
        if (GUI.Button(position, "DTM.logmenu".Translate()) && !Find.WindowStack.TryRemove(typeof(Dialog_Debug)))
        {
            Find.WindowStack.Add(new Dialog_Debug());
        }

        if (GUI.Button(position2, "DTM.logwindow".Translate()) && !Find.WindowStack.TryRemove(typeof(EditWindow_Log)))
        {
            Find.WindowStack.Add(new EditWindow_Log());
        }

        var list = new List<ListableOption>();
        ListableOption item = new ListableOption_WebLink("DTM.information".Translate(),
            "https://www.reddit.com/r/KeenKrozzy/", DTMBin.RedditIcon);
        list.Add(item);
        if (currentVersion != null)
        {
            list.Add(new ListableOption("DTM.modVersion".Translate(currentVersion), null));
        }

        OptionListingUtility.DrawOptionListing(rect2, list);
    }
}