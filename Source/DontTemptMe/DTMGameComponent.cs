using Verse;

namespace DontTemptMe;

public class DTMGameComponent : GameComponent
{
    public DTMGameComponent(Game game)
    {
    }

    public override void FinalizeInit()
    {
        base.FinalizeInit();
        if (!Prefs.DevMode)
        {
            return;
        }

        Prefs.DevMode = false;
        Log.Message("DevMode has been turned off.");
    }
}