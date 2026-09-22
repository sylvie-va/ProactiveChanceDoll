using BepInEx;
using R2API.Utils;

[assembly: HG.Reflection.SearchableAttribute.OptIn]

namespace ProactiveChanceDoll
{
    [BepInPlugin("com.sylvie.proactivechancedoll", "Proactive Chance Doll", "1.0.0")]
    [NetworkCompatibility(CompatibilityLevel.NoNeedForSync, VersionStrictness.DifferentModVersionsAreOk)]
    public sealed class ProactiveChanceDoll : BaseUnityPlugin
    {
        private void Awake()
        {
            ChanceDollSharingHooks.Hook();
        }

        private void OnDestroy()
        {
            ChanceDollSharingHooks.UnHook();
        }
    }
}
