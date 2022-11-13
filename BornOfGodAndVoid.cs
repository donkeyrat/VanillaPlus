using System.Reflection;
using System;
using UnityEngine;
using HarmonyLib;
using Landfall.TABS;

namespace VanillaPlus
{
    [HarmonyPatch(typeof(HealthHandler), "Update")]
    class BornOfGodAndVoid
    {
        [HarmonyPrefix]
        public static bool Prefix(HealthHandler __instance, DataHandler ___data)
        {
            if (!__instance.transform.root.GetComponentInChildren<Abyssal>())
            {
                if ((bool)___data.mainRig && (___data.mainRig.position.magnitude > 2000f || float.IsNaN(___data.mainRig.position.x)))
                {
                    bool flag = false;
                    if (!___data.Dead)
                    {
                        flag = __instance.Die();
                    }
                    if (flag)
                    {
                        UnityEngine.Object.Destroy(__instance.transform.root.gameObject);
                    }
                }
                if (!___data.Dead && (bool)___data.mainRig && ___data.mainRig.position.y < MapSettings.KillHeight)
                {
                    __instance.Die();
                }
            }

            return false;
        }
    }
}