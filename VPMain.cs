using System.Collections;
using Landfall.TABS;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Landfall.TABS.UnitEditor;
using Landfall.TABS.Workshop;
using DM;
using Landfall.TABS.GameMode;
using TGCore;

namespace VanillaPlus 
{
    public class VPMain
    {
        public VPMain()
        {
            foreach (var mat in combatUpgrade.LoadAllAssets<Material>()) if (Shader.Find(mat.shader.name)) mat.shader = Shader.Find(mat.shader.name);
            
            unitList = TGMain.landfallDb.GetUnitBlueprints().ToList();
            unitsToUpgrade = new List<UnitBlueprint>(unitList.FindAll(x => x != null && x.UnitBase != null && (x.UnitBase && (x.UnitBase.name.Contains("Humanoid") || x.UnitBase.name.Contains("Stiffy") || x.UnitBase.name.Contains("Halfling") || x.UnitBase.name.Contains("Blackbeard")))));

            unitBaseSkeleton.CopyUnitBaseOntoThis(combatUpgrade.LoadAsset<GameObject>("Humanoid_2.0"));
            
            shieldWhitelist.Add("Shield_Tower_1 Weapons_VB", 2f);
            shieldWhitelist.Add("Ra_Shield", 0.5f);
            shieldWhitelist.Add("CavalryKnightShield 1_1 Weapons_VB", 0.5f);
            shieldWhitelist.Add("KnightlyShield", 2f);
            shieldWhitelist.Add("KnightShield", 2f);
            shieldWhitelist.Add("KnightShield_1 Weapons_VB", 2f);
            shieldWhitelist.Add("KnightShieldNew_1 Weapons_VB", 2f);
            shieldWhitelist.Add("EvilTankShield_Test_L_1 Weapons_VB", 0.5f);
            shieldWhitelist.Add("EvilTankShield_Test_R_1 Weapons_VB", 0.5f);
            shieldWhitelist.Add("Evil_ClericShield_1 Weapons_VB", 2f);
            shieldWhitelist.Add("Good_Cleric_Shield_1 Weapons_VB", 2f);
            shieldWhitelist.Add("Skeletonshield_Giant_1 Weapons_VB", 2f);
            shieldWhitelist.Add("Ballistic Shield_1 Weapons_VB", 0.5f);
            shieldWhitelist.Add("BallisticShield_Taser", 1f);
            shieldWhitelist.Add("BallisticShields", 0.5f);
            shieldWhitelist.Add("CrusaderShield", 1f);
            shieldWhitelist.Add("Napoleonic_Shield", 0.5f);
            shieldWhitelist.Add("Spartan_Shield", 2f);
            shieldWhitelist.Add("SmallShield_1 Weapons_VB", 2f);
            shieldWhitelist.Add("Shield_Wall_1 Weapons_VB", 0.5f);
            shieldWhitelist.Add("Legionary_Shield", 2f);
            shieldWhitelist.Add("CenturionShield_1 Weapons_VB", 1f);
            
            var sarissaSpear = TGMain.landfallDb.GetWeapons().ToList().Find(x => x.name.Contains("Spear_Greek_1"));
            if (sarissaSpear) sarissaSpear.GetComponent<Holdable>().holdableData.setRotation = false;
            var paladinHammer = TGMain.landfallDb.GetWeapons().ToList().Find(x => x.name.Contains("ClericMace_1"));
            if (paladinHammer) paladinHammer.GetComponent<MeleeWeapon>().requiredPowerToParry = 5f;
            var cultistMace = TGMain.landfallDb.GetWeapons().ToList().Find(x => x.name.Contains("ClericMaceEvil_1"));
            if (cultistMace) cultistMace.GetComponent<MeleeWeapon>().requiredPowerToParry = 5f;
            var assassinDagger = TGMain.landfallDb.GetWeapons().ToList().Find(x => x.name.Contains("Assassin_Dagger_1"));
            if (assassinDagger) assassinDagger.GetComponent<MeleeWeapon>().requiredPowerToParry = 5f;
            var warGlaive = TGMain.landfallDb.GetWeapons().ToList().Find(x => x.name.Contains("WarGlaivecurved_1"));
            if (warGlaive) warGlaive.GetComponent<MeleeWeapon>().requiredPowerToParry = 5f;
            var club = TGMain.landfallDb.GetWeapons().ToList().Find(x => x.name.Contains("Club_1") && !x.name.Contains("Aztec"));
            if (club) club.GetComponent<MeleeWeapon>().requiredPowerToParry = 5f;
            var superb = TGMain.landfallDb.GetWeapons().ToList().Find(x => x.name.Contains("Leg_SuperBoxer_W_1"));
            if (superb) superb.GetComponent<MeleeWeapon>().requiredPowerToParry = 50f;
            var superbR = TGMain.landfallDb.GetWeapons().ToList().Find(x => x.name.Contains("Leg_SuperBoxer_W_R_1"));
            if (superbR) superbR.GetComponent<MeleeWeapon>().requiredPowerToParry = 50f;
            var valk = TGMain.landfallDb.GetWeapons().ToList().Find(x => x.name.Contains("ValkyrieSword_1"));
            if (valk) valk.GetComponent<MeleeWeapon>().requiredPowerToParry = 5f;
            
            foreach (var unit in combatUpgrade.LoadAllAssets<UnitBlueprint>())
            {
                var find = TGMain.landfallDb.GetUnitBases().ToList().Find(x => x.name == unit.UnitBase.name);
                var find2 = TGMain.landfallDb.GetUnitBases().ToList().Find(x => x.name.Contains("Humanoid_1 Prefabs_VB"));
                
                if (unit.UnitBase && find) unit.UnitBase = find;
                else if (unit.UnitBase && unit.UnitBase.name.Contains("2.0") && !unit.UnitBase.name.Contains("Minotaur") && find2) unit.UnitBase = find2;
            }
            
            var upgradedUnits = combatUpgrade.LoadAllAssets<UnitBlueprint>().ToList();
            foreach (var unit in unitList)
            {
                var foundUnit = upgradedUnits.Find(x => x.name == unit.name + "_2.0");
                if (foundUnit)
                {
                    var unitSkeleton = new UnitSkeleton();
                    unitSkeleton.CopyUnitOntoThis(foundUnit);
                    unitSkeletons.Add(unit.Entity.GUID, unitSkeleton);
                    
                    var ogSkeleton = new UnitSkeleton();
                    ogSkeleton.CopyUnitOntoThis(unit);
                    originalUnitSkeletons.Add(unit.Entity.GUID, ogSkeleton);
                }
            }
            
            new Harmony("VanillaPlus").PatchAll();

            foreach (var faction in combatUpgrade.LoadAllAssets<Faction>())
            {
                var moddedUnitList = faction.Units.Where(x => x).OrderBy(x => x.GetUnitCost()).ToArray();
                faction.Units = moddedUnitList.ToArray();
                foreach (var vanillaFaction in TGMain.landfallDb.GetFactions().ToList()) 
                {
                    if (faction.Entity.Name == vanillaFaction.Entity.Name + "_NEW")
                    {
                        var vanillaUnitList = new List<UnitBlueprint>(vanillaFaction.Units);
                        vanillaUnitList.AddRange(faction.Units);
                        vanillaFaction.Units = vanillaUnitList.Where(x => x).OrderBy(x => x.GetUnitCost()).ToArray();

                        Object.DestroyImmediate(faction);
                    }
                }
            }

            foreach (var prop in combatUpgrade.LoadAllAssets<PropItem>())
            {
                var totalSubmeshes = prop.GetComponentsInChildren<MeshFilter>().Where(rend => rend.gameObject.activeSelf && rend.gameObject.activeInHierarchy && rend.mesh.subMeshCount > 0 && rend.GetComponent<MeshRenderer>() && rend.GetComponent<MeshRenderer>().enabled).Sum(rend => rend.mesh.subMeshCount) + prop.GetComponentsInChildren<SkinnedMeshRenderer>().Where(rend => rend.gameObject.activeSelf && rend.sharedMesh.subMeshCount > 0 && rend.enabled).Sum(rend => rend.sharedMesh.subMeshCount);
                if (totalSubmeshes > 0) 
                {
                    float average = 1f / totalSubmeshes;
                    var averageList = new List<float>();
                    for (var i = 0; i < totalSubmeshes - 1; i++) averageList.Add(average);
                    
                    prop.SubmeshArea = averageList.ToArray();
                }
            }
            
            foreach (var weapon in combatUpgrade.LoadAllAssets<WeaponItem>())
            {
                var totalSubmeshes = weapon.GetComponentsInChildren<MeshFilter>().Where(rend => rend.gameObject.activeSelf && rend.gameObject.activeInHierarchy && rend.mesh.subMeshCount > 0 && rend.GetComponent<MeshRenderer>() && rend.GetComponent<MeshRenderer>().enabled).Sum(rend => rend.mesh.subMeshCount) + weapon.GetComponentsInChildren<SkinnedMeshRenderer>().Where(rend => rend.gameObject.activeSelf && rend.sharedMesh.subMeshCount > 0 && rend.enabled).Sum(rend => rend.sharedMesh.subMeshCount);
                if (totalSubmeshes > 0) 
                {
                    float average = 1f / totalSubmeshes;
                    var averageList = new List<float>();
                    for (var i = 0; i < totalSubmeshes - 1; i++) averageList.Add(average);
                    
                    weapon.SubmeshArea = averageList.ToArray();
                }
            }

            foreach (var audio in combatUpgrade.LoadAllAssets<AudioSource>()) 
            {
                audio.outputAudioMixerGroup = ServiceLocator.GetService<GameModeService>().AudioSettings.AudioMixer.outputAudioMixerGroup;
            }

            TGAddons.AddItems(combatUpgrade.LoadAllAssets<UnitBlueprint>(), combatUpgrade.LoadAllAssets<Faction>(),
                combatUpgrade.LoadAllAssets<TABSCampaignAsset>(), combatUpgrade.LoadAllAssets<TABSCampaignLevelAsset>(),
                combatUpgrade.LoadAllAssets<VoiceBundle>(), combatUpgrade.LoadAllAssets<FactionIcon>(),
                combatUpgrade.LoadAllAssets<Unit>().Where(x => !x.name.Contains("2.0")), combatUpgrade.LoadAllAssets<PropItem>(),
                combatUpgrade.LoadAllAssets<SpecialAbility>(), combatUpgrade.LoadAllAssets<WeaponItem>(),
                combatUpgrade.LoadAllAssets<ProjectileEntity>());
        }
        
        public static void ToggleUpgrades_OnValueChanged(int value)
        {
            VPLauncher.ConfigUnitUpgradesEnabled.Value = value == 0;
            
            if (value == 0)
            {
                foreach (var u in unitList)
                {
                    if (u != null && unitSkeletons.ContainsKey(u.Entity.GUID))
                    {
                        unitSkeletons[u.Entity.GUID].CopyThisOntoUnit(u);
                        
                        var secret = ContentDatabase.Instance().LandfallContentDatabase.GetFactions().ToList()
                            .Find(x => x.name == "Secret");
                        if (secret)
                        {
                            secret.Units = (
                                from UnitBlueprint uj
                                    in secret.Units
                                orderby uj.GetUnitCost()
                                select uj).ToArray();
                        }
                    }
                }
            }
            else
            {
                foreach (var u in unitList)
                {
                    var unit = u;
                    if (unit != null && originalUnitSkeletons.ContainsKey(unit.Entity.GUID))
                    {
                        originalUnitSkeletons[unit.Entity.GUID].CopyThisOntoUnit(unit);
                        
                        var secret = ContentDatabase.Instance().LandfallContentDatabase.GetFactions().ToList()
                            .Find(x => x.name == "Secret");
                        if (secret)
                        {
                            secret.Units = (
                                from UnitBlueprint uj
                                    in secret.Units
                                orderby uj.GetUnitCost()
                                select uj).ToArray();
                        }
                    }
                }
            }
        }
        
        public static void TogglePhysics_OnValueChanged(int value)
        {
            VPLauncher.ConfigFloatinessEnabled.Value = value == 0;
            
            if (value == 0)
            {
                foreach (var ub in unitBaseList)
                {
                    unitBaseSkeleton.CopyThisOntoUnitBase(ub, true);
                }
            }
            else
            {
                foreach (var ub in unitBaseList)
                {
                    var guid = ub.GetComponent<Unit>().Entity.GUID;
                    if (ub != null && originalUnitBaseSkeletons.ContainsKey(guid))
                    {
                        originalUnitBaseSkeletons[guid].CopyThisOntoUnitBase(ub, false, true);
                    }
                }
            }
        }

        public static void ToggleShieldBlocking_OnValueChanged(int value)
        {
            VPLauncher.ConfigShieldsBlockEnabled.Value = value == 0;
            
            if (value == 0)
            {
                foreach (var wp in ContentDatabase.Instance().LandfallContentDatabase.GetWeapons().ToList())
                {
                    if (shieldWhitelist.ContainsKey(wp.name) && wp.GetComponent<MeleeWeapon>() && wp.GetComponent<Rigidbody>() && !wp.GetComponent<ParryRoot>())
                    {
                        wp.AddComponent<ParryRoot>().globalCooldown = shieldWhitelist[wp.name];
                    }
                }
            }
            else
            {
                foreach (var wp in ContentDatabase.Instance().LandfallContentDatabase.GetWeapons().ToList())
                {
                    if (shieldWhitelist.ContainsKey(wp.name) && wp.GetComponent<ParryRoot>())
                    {
                        Object.DestroyImmediate(wp.GetComponent<ParryRoot>());
                    }
                }
            }
        }
        
        public static void ToggleAllBlocking_OnValueChanged(int value)
        {
            VPLauncher.ConfigAllWeaponsBlockEnabled.Value = value == 1;
            
            if (value == 1)
            {
                foreach (var wp in ContentDatabase.Instance().LandfallContentDatabase.GetWeapons().ToList())
                {
                    if (!shieldWhitelist.ContainsKey(wp.name) && wp.GetComponent<MeleeWeapon>() && wp.GetComponent<Rigidbody>() && !wp.GetComponent<ParryRoot>())
                    {
                        wp.AddComponent<ParryRoot>();
                    }
                }
            }
            else
            {
                foreach (var wp in ContentDatabase.Instance().LandfallContentDatabase.GetWeapons().ToList())
                {
                    if (!shieldWhitelist.ContainsKey(wp.name) && wp.GetComponent<ParryRoot>())
                    {
                        Object.DestroyImmediate(wp.GetComponent<ParryRoot>());
                    }
                }
            }
        }

        public static AssetBundle combatUpgrade = AssetBundle.LoadFromMemory(Properties.Resources.combatupgrade);

        public static List<UnitBlueprint> unitsToUpgrade = new List<UnitBlueprint>();

        public static List<UnitBlueprint> unitList = new List<UnitBlueprint>();
        
        public static List<GameObject> unitBaseList = new List<GameObject>();
        
        public static Dictionary<string, float> shieldWhitelist = new Dictionary<string, float>();
        
        public static Dictionary<DatabaseID, UnitSkeleton> unitSkeletons = new Dictionary<DatabaseID, UnitSkeleton>();
        
        public static Dictionary<DatabaseID, UnitSkeleton> originalUnitSkeletons = new Dictionary<DatabaseID, UnitSkeleton>();

        public static UnitBaseSkeleton unitBaseSkeleton = new UnitBaseSkeleton();
        
        public static Dictionary<DatabaseID, UnitBaseSkeleton> originalUnitBaseSkeletons = new Dictionary<DatabaseID, UnitBaseSkeleton>();

        public static bool NerfsEnabled => VPLauncher.ConfigNerfsEnabled.Value;
    }
}
