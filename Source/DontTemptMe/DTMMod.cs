using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace DontTemptMe;

public class DTMMod : Mod
{
    private readonly Vector2 vecDTMBanner = new Vector2(739f, 110f);
    private float floVertSpacer;

    private Vector2 vecDTMBannerSize = Vector2.zero;

    public DTMMod(ModContentPack content)
        : base(content)
    {
        GetSettings<DTMSettings>();
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
        if (inRect.width > vecDTMBanner.x)
        {
            rect = new Rect(0f, 40f, vecDTMBanner.x, vecDTMBanner.y);
        }
        else
        {
            vecDTMBannerSize.x = inRect.width;
            vecDTMBannerSize.y = vecDTMBannerSize.x / vecDTMBanner.x * vecDTMBanner.y;
            rect = new Rect(0f, 40f, vecDTMBannerSize.x, vecDTMBannerSize.y);
        }

        var position = new Rect(rect.x, rect.y + rect.height + floVertSpacer, inRect.width * 0.25f,
            inRect.height * 0.05f);
        var position2 = new Rect(rect.x, position.y + position.height + floVertSpacer, inRect.width * 0.25f,
            inRect.height * 0.05f);
        var rect2 = new Rect(rect.x, position2.y + position2.height + floVertSpacer, inRect.width * 0.25f,
            inRect.height * 0.05f);
        GUI.DrawTexture(rect, DTMBin.DTMBanner);
        if (GUI.Button(position, "Debug Log Menu") && !Find.WindowStack.TryRemove(typeof(Dialog_DebugOutputMenu)))
        {
            Find.WindowStack.Add(new Dialog_DebugOutputMenu());
        }

        if (GUI.Button(position2, "Debug Log Window") && !Find.WindowStack.TryRemove(typeof(EditWindow_Log)))
        {
            Find.WindowStack.Add(new EditWindow_Log());
        }

        var list = new List<ListableOption>();
        ListableOption item = new ListableOption_WebLink("Information and Help".Translate(),
            "https://www.reddit.com/r/KeenKrozzy/", DTMBin.RedditIcon);
        list.Add(item);
        OptionListingUtility.DrawOptionListing(rect2, list);
    }
}