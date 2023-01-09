using HarmonyLib;

namespace VanillaPlus.HarmonyPatches
{
    [HarmonyPatch(typeof(DataHandler), "Start")]
    class HealthNerfPatch
    {
        [HarmonyPrefix]
        public static void Postfix(DataHandler __instance)
        {
            if (VPMain.ToggleNerfs == 0) __instance.health *= 0.7f;
            __instance.maxHealth = __instance.health;
        }
    }
}