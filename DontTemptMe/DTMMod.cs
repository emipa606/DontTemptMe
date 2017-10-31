using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using UnityEngine;

namespace DontTemptMe
{
	public class DTMMod : Mod
	{
		public static DTMSettings settings;
		public override string SettingsCategory() => "Don't Tempt Me!";
		private Vector2 vecDTMBanner = new Vector2(739f, 110f);
		Vector2 vecDTMBannerSize = Vector2.zero;
		float floVertSpacer = 0f;

		public DTMMod(ModContentPack content) : base(content)
		{
			settings = GetSettings<DTMSettings>();
		}

		public override void DoSettingsWindowContents(Rect inRect)
		{
			floVertSpacer = inRect.height * .025f;
			Rect rectDTMBanner = new Rect();

			if (inRect.width > vecDTMBanner.x)
			{
				rectDTMBanner = new Rect(0f, 40f, vecDTMBanner.x, vecDTMBanner.y);
			}
			else
			{
				vecDTMBannerSize.x = inRect.width;
				vecDTMBannerSize.y = (vecDTMBannerSize.x / vecDTMBanner.x) * vecDTMBanner.y;
				rectDTMBanner = new Rect(0f, 40f, vecDTMBannerSize.x, vecDTMBannerSize.y);
			}

			Rect rectOpenDebugLogMenu = new Rect(rectDTMBanner.x, rectDTMBanner.y + rectDTMBanner.height + floVertSpacer, inRect.width * .25f, inRect.height * .05f);
			Rect rectOpenDebugLogWindow = new Rect(rectDTMBanner.x, rectOpenDebugLogMenu.y + rectOpenDebugLogMenu.height + floVertSpacer, inRect.width * .25f, inRect.height * .05f);
			Rect rectListOptions = new Rect(rectDTMBanner.x, rectOpenDebugLogWindow.y + rectOpenDebugLogWindow.height + floVertSpacer, inRect.width * .25f, inRect.height * .05f);

			GUI.DrawTexture(rectDTMBanner, DTMBin.DTMBanner);
			if (GUI.Button(rectOpenDebugLogMenu, "Debug Log Menu"))
			{
				if (!Find.WindowStack.TryRemove(typeof(Dialog_DebugLogMenu), true))
				{
					Find.WindowStack.Add(new Dialog_DebugLogMenu());
				}
			}
			if (GUI.Button(rectOpenDebugLogWindow, "Debug Log Window"))
			{
				if (!Find.WindowStack.TryRemove(typeof(EditWindow_Log), true))
				{
					Find.WindowStack.Add(new EditWindow_Log());
				}
			}

			List<ListableOption> lstOptions = new List<ListableOption>();
			ListableOption lOpt = new ListableOption_WebLink("Information and Help".Translate(), "https://www.reddit.com/r/KeenKrozzy/", DTMBin.RedditIcon);
			lstOptions.Add(lOpt);
			
			OptionListingUtility.DrawOptionListing(rectListOptions, lstOptions);
		}
	}
}
