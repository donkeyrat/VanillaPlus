using HarmonyLib;

namespace VanillaPlus.HarmonyPatches
{
    [HarmonyPatch(typeof(HealthHandler), "Update")]
    class AbyssalPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(HealthHandler __instance)
        {
            if (__instance.transform.root.GetComponentInChildren<Abyssal>()) return false;
            
            return true;
        }
    }
}