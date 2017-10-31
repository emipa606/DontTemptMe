using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;
using Harmony;

namespace DontTemptMe
{
	[StaticConstructorOnStartup]
	public static class DTMBin
	{
		public static Texture2D DTMBanner;
		public static Texture2D RedditIcon;

		static DTMBin()
		{
			DTMBanner = ContentFinder<Texture2D>.Get("DTM/Banner", true);
			RedditIcon = ContentFinder<Texture2D>.Get("DTM/Reddit_Icon", true);
			DTMBanner.wrapMode = TextureWrapMode.Clamp;
			RedditIcon.wrapMode = TextureWrapMode.Clamp;
		}
	}
}
