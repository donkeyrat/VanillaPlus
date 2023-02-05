using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using DM;
using Landfall.TABS;
using UnityEngine;

namespace VanillaPlus {

    [BepInPlugin("teamgrad.vanillaplus", "VanillaPlus", "2.1.0")]
    public class VPLauncher : BaseUnityPlugin 
    {
        public void Awake()
        {
            DoConfig();
            StartCoroutine(LaunchMod());
        }
		
        private static IEnumerator LaunchMod() 
        {
            yield return new WaitUntil(() => FindObjectOfType<ServiceLocator>() != null);

            new VPMain();
            
            yield return new WaitForSeconds(0.4f);

            var db = ContentDatabase.Instance().LandfallContentDatabase;

            VPMain.unitBaseList = new List<GameObject>(db.GetUnitBases().ToList().FindAll(b => b != null && !b.name.Contains("_2.") && (b.name.Contains("Humanoid") || b.name.Contains("Stiffy") || b.name.Contains("Halfling") || b.name.Contains("Blackbeard"))));
            foreach (var ub in VPMain.unitBaseList)
            {
                var ogSkeleton = new UnitBaseSkeleton();
                ogSkeleton.CopyUnitBaseOntoThis(ub);
                VPMain.originalUnitBaseSkeletons.Add(ub.GetComponent<Unit>().Entity.GUID, ogSkeleton);
            }
            
            VPMain.unitsToUpgrade = new List<UnitBlueprint>(db.GetUnitBlueprints().ToList().FindAll(x => x != null && x != null && x.UnitBase && (x.UnitBase.name.Contains("Humanoid") || x.UnitBase.name.Contains("Stiffy") || x.UnitBase.name.Contains("Halfling"))));
            foreach (var unit in VPMain.unitsToUpgrade)
            {
                if (unit.sizeMultiplier < 3f)
                {
                    unit.animationMultiplier = Mathf.Lerp(unit.animationMultiplier, 1f, 0.5f);
                    unit.movementSpeedMuiltiplier = Mathf.Lerp(unit.movementSpeedMuiltiplier, 0.75f, 0.5f);
                    unit.stepMultiplier = Mathf.Lerp(unit.stepMultiplier, 0.75f, 0.5f);
                }
            }
            
            VPMain.ToggleUpgrades_OnValueChanged(0);
            VPMain.TogglePhysics_OnValueChanged(0);
            VPMain.ToggleShieldBlocking_OnValueChanged(0);
            VPMain.ToggleAllBlocking_OnValueChanged(0);
        }

        private void DoConfig()
        {
            ConfigUnitUpgradesEnabled = Config.Bind("Gameplay", "UnitUpgradesEnabled", true, "Enables/disables unit modifications.");
            ConfigFloatinessEnabled = Config.Bind("Gameplay", "FloatinessEnabled", true, "Enables/disables unit modifications.");
            ConfigShieldsBlockEnabled = Config.Bind("Gameplay", "ShieldsBlockEnabled", true, "Enables/disables shied blocking.");
            ConfigAllWeaponsBlockEnabled = Config.Bind("Bug", "AllWeaponsBlockEnabled", false, "Enables/disables all weapons block on contact.");
            ConfigNerfsEnabled = Config.Bind("Bug", "NerfsEnabled", true, "Enables/disables a reduction to all unit health by 30% and a reduction to invulnerability time by 50%.");
        }

        public static ConfigEntry<bool> ConfigUnitUpgradesEnabled;
        
        public static ConfigEntry<bool> ConfigFloatinessEnabled;

        public static ConfigEntry<bool> ConfigShieldsBlockEnabled;
        
        public static ConfigEntry<bool> ConfigAllWeaponsBlockEnabled;
        
        public static ConfigEntry<bool> ConfigNerfsEnabled;
    }
}