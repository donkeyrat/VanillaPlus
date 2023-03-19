using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using Landfall.TABS;
using TGCore;
using UnityEngine;

namespace VanillaPlus 
{
    [BepInPlugin("teamgrad.vanillaplus", "VanillaPlus", "2.1.0")]
    [BepInDependency("teamgrad.core")]
    public class VPLauncher : TGMod 
    {
        public override void EarlyLaunch()
        {
            new VPMain();
        }

        public override void LateLaunch()
        {
            VPMain.unitBaseList = new List<GameObject>(TGMain.landfallDb.GetUnitBases().ToList().FindAll(b => b != null && !b.name.Contains("_2.") && (b.name.Contains("Humanoid") || b.name.Contains("Stiffy") || b.name.Contains("Halfling") || b.name.Contains("Blackbeard"))));
            foreach (var ub in VPMain.unitBaseList)
            {
                var ogSkeleton = new UnitBaseSkeleton();
                ogSkeleton.CopyUnitBaseOntoThis(ub);
                VPMain.originalUnitBaseSkeletons.Add(ub.GetComponent<Unit>().Entity.GUID, ogSkeleton);
            }
            
            VPMain.unitsToUpgrade = new List<UnitBlueprint>(TGMain.landfallDb.GetUnitBlueprints().ToList().FindAll(x => x != null && x.UnitBase && (x.UnitBase.name.Contains("Humanoid") || x.UnitBase.name.Contains("Stiffy") || x.UnitBase.name.Contains("Halfling") || x.UnitBase.name.Contains("Blackbeard"))));
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

        public override void AddSettings()
        {
            ConfigUnitUpgradesEnabled = Config.Bind("Gameplay", "UnitUpgradesEnabled", true, "Enables/disables unit modifications.");
            ConfigFloatinessEnabled = Config.Bind("Gameplay", "FloatinessEnabled", true, "Enables/disables unit modifications.");
            ConfigShieldsBlockEnabled = Config.Bind("Gameplay", "ShieldsBlockEnabled", true, "Enables/disables shied blocking.");
            ConfigAllWeaponsBlockEnabled = Config.Bind("Bug", "AllWeaponsBlockEnabled", false, "Enables/disables all weapons block on contact.");
            ConfigNerfsEnabled = Config.Bind("Bug", "NerfsEnabled", true, "Enables/disables a reduction to all unit health by 30% and a reduction to invulnerability time by 50%.");
            
            var toggleUpgrades = TGAddons.CreateSetting(SettingsInstance.SettingsType.Options, "Toggle unit upgrades", "Enables/disables unit modifications.", "GAMEPLAY", 0f, VPLauncher.ConfigUnitUpgradesEnabled.Value ? 0 : 1, new[] { "Enable unit modifications", "Disable unit modifications" });
            toggleUpgrades.OnValueChanged += VPMain.ToggleUpgrades_OnValueChanged;
            
            var togglePhysics = TGAddons.CreateSetting(SettingsInstance.SettingsType.Options, "Toggle floaty physics", "Enables/disables floatier unit movement.", "GAMEPLAY", 0f, VPLauncher.ConfigFloatinessEnabled.Value ? 0 : 1, new[] { "Enable floaty physics", "Disable floaty physics" });
            togglePhysics.OnValueChanged += VPMain.TogglePhysics_OnValueChanged;
            
            var toggleShieldBlocking = TGAddons.CreateSetting(SettingsInstance.SettingsType.Options, "Toggle shield blocking", "Enables/disables shied blocking.", "GAMEPLAY", 0f, VPLauncher.ConfigShieldsBlockEnabled.Value ? 0 : 1, new[] { "Enable shield blocking", "Disable shield blocking" });
            toggleShieldBlocking.OnValueChanged += VPMain.ToggleShieldBlocking_OnValueChanged;
            
            var toggleAllBlocking = TGAddons.CreateSetting(SettingsInstance.SettingsType.Options, "Toggle all weapon blocking", "Enables/disables all weapons block on contact.", "BUG", 0f, VPLauncher.ConfigAllWeaponsBlockEnabled.Value ? 0 : 1, new[] { "Disable weapon blocking", "Enable weapon blocking" });
            toggleAllBlocking.OnValueChanged += VPMain.ToggleAllBlocking_OnValueChanged;
            
            var toggleNerfs = TGAddons.CreateSetting(SettingsInstance.SettingsType.Options, "Toggle nerfs", "Enables/disables a reduction to all unit health by 30% and a reduction to invulnerability time by 50%.", "BUG", 0f, VPLauncher.ConfigNerfsEnabled.Value ? 0 : 1, new[] { "Enable nerfs", "Disables nerfs" });
            toggleNerfs.OnValueChanged += delegate(int value)
            {
                ConfigNerfsEnabled.Value = value == 0;
            };
        }

        public static ConfigEntry<bool> ConfigUnitUpgradesEnabled;
        
        public static ConfigEntry<bool> ConfigFloatinessEnabled;

        public static ConfigEntry<bool> ConfigShieldsBlockEnabled;
        
        public static ConfigEntry<bool> ConfigAllWeaponsBlockEnabled;
        
        public static ConfigEntry<bool> ConfigNerfsEnabled;
    }
}