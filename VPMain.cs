using Landfall.TABS;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Landfall.TABS.UnitEditor;
using Landfall.TABS.Workshop;
using DM;
using UnityEngine.UI;

namespace VanillaPlus 
{
    public class VPMain 
    {
        public VPMain()
        {
            var db = ContentDatabase.Instance();
            
            unitList = db.LandfallContentDatabase.GetUnitBlueprints().ToList();
            unitsToUpgrade = new List<UnitBlueprint>(unitList.FindAll(x => x != null && x.UnitBase != null && (x.UnitBase && (x.UnitBase.name.Contains("Humanoid") || x.UnitBase.name.Contains("Stiffy") || x.UnitBase.name.Contains("Halfling") || x.UnitBase.name.Contains("Blackbeard")))));

            unitBaseSkeleton.CopyUnitBaseOntoThis(combatUpgrade
                .LoadAsset<GameObject>("Humanoid_2.0"));
            
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
            shieldWhitelist.Add("Wargod_Shield", 0.5f);
            shieldWhitelist.Add("SmallShield_1 Weapons_VB", 2f);
            shieldWhitelist.Add("Shield_Wall_1 Weapons_VB", 0.5f);
            shieldWhitelist.Add("Legionary_Shield", 2f);
            shieldWhitelist.Add("CenturionShield_1 Weapons_VB", 1f);
            
            var sarissaSpear = db.GetAllWeapons().ToList().Find(x => x.name.Contains("Spear_Greek_1"));
            if (sarissaSpear) sarissaSpear.GetComponent<Holdable>().holdableData.setRotation = false;
            var paladinHammer = db.GetAllWeapons().ToList().Find(x => x.name.Contains("ClericMace_1"));
            if (paladinHammer) paladinHammer.GetComponent<MeleeWeapon>().requiredPowerToParry = 5f;
            var cultistMace = db.GetAllWeapons().ToList().Find(x => x.name.Contains("ClericMaceEvil_1"));
            if (cultistMace) cultistMace.GetComponent<MeleeWeapon>().requiredPowerToParry = 5f;
            var assassinDagger = db.GetAllWeapons().ToList().Find(x => x.name.Contains("Assassin_Dagger_1"));
            if (assassinDagger) assassinDagger.GetComponent<MeleeWeapon>().requiredPowerToParry = 5f;
            var warGlaive = db.GetAllWeapons().ToList().Find(x => x.name.Contains("WarGlaivecurved_1"));
            if (warGlaive) warGlaive.GetComponent<MeleeWeapon>().requiredPowerToParry = 5f;
            var club = db.GetAllWeapons().ToList().Find(x => x.name.Contains("Club_1") && !x.name.Contains("Aztec"));
            if (club) club.GetComponent<MeleeWeapon>().requiredPowerToParry = 5f;
            var superb = db.GetAllWeapons().ToList().Find(x => x.name.Contains("Leg_SuperBoxer_W_1"));
            if (superb) superb.GetComponent<MeleeWeapon>().requiredPowerToParry = 50f;
            var superbR = db.GetAllWeapons().ToList().Find(x => x.name.Contains("Leg_SuperBoxer_W_R_1"));
            if (superbR) superbR.GetComponent<MeleeWeapon>().requiredPowerToParry = 50f;
            var valk = db.GetAllWeapons().ToList().Find(x => x.name.Contains("ValkyrieSword_1"));
            if (valk) valk.GetComponent<MeleeWeapon>().requiredPowerToParry = 5f;
            
            foreach (var unit in combatUpgrade.LoadAllAssets<UnitBlueprint>())
            {
                if (!unit.name.Contains("2.0"))
                {
                    newUnits.Add(unit);
                }

                var find = db.GetAllUnitBases().ToList().Find(x => x.name == unit.UnitBase.name);
                var find2 = db.GetAllUnitBases().ToList().Find(x => x.name.Contains("Humanoid_1 Prefabs_VB"));
                if (unit.UnitBase && find)
                {
                    unit.UnitBase = find;
                }
                else if (unit.UnitBase && unit.UnitBase.name.Contains("2.0") && !unit.UnitBase.name.Contains("Minotaur") && find2)
                {
                    unit.UnitBase = find2;
                }
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

            var toggleUpgrades = CreateSetting(SettingsInstance.SettingsType.Options, "Toggle unit upgrades",
                "Enables/disables unit modifications.", "GAMEPLAY",
                new string[] { "Enable unit modifications", "Disable unit modifications" });
            toggleUpgrades.OnValueChanged += ToggleUpgrades_OnValueChanged;
            
            var togglePhysics = CreateSetting(SettingsInstance.SettingsType.Options, "Toggle floaty physics",
                "Enables/disables unit modifications.", "GAMEPLAY",
                new string[] { "Enable floaty physics", "Disable floaty physics" });
            togglePhysics.OnValueChanged += TogglePhysics_OnValueChanged;
            
            var toggleShieldBlocking = CreateSetting(SettingsInstance.SettingsType.Options, "Toggle shield blocking",
                "Enables/disables shied blocking.", "GAMEPLAY",
                new string[] { "Enable shield blocking", "Disable shield blocking" });
            toggleShieldBlocking.OnValueChanged += ToggleShieldBlocking_OnValueChanged;
            
            var toggleAllBlocking = CreateSetting(SettingsInstance.SettingsType.Options, "Make all weapons block",
                "Makes all weapons block on contact", "BUG",
                new string[] { "Disable weapon blocking", "Enable weapon blocking" });
            toggleAllBlocking.OnValueChanged += ToggleAllBlocking_OnValueChanged;
            
            var toggleNerfs = CreateSetting(SettingsInstance.SettingsType.Options, "Enables/disables nerfs.",
                "Enables/disables a reduction to all unit health by 30% and a reduction to invulnerability time by 50%.", "BUG",
                new string[] { "Enable nerfs", "Disables nerfs" });
            toggleNerfs.OnValueChanged += ToggleNerfs_OnValueChanged;
            
            new GameObject
            {
                name = "Bullshit: The Reboot",
                hideFlags = HideFlags.HideAndDontSave
            }.AddComponent<VPSecretManager>();
            
            new Harmony("Boongus").PatchAll();
            
            var factions = db.LandfallContentDatabase.GetFactions().ToList();
            foreach (var fac in combatUpgrade.LoadAllAssets<Faction>()) {

                var theNew = new List<UnitBlueprint>(fac.Units);
                var veryNewUnits = (
                    from UnitBlueprint unit
                        in fac.Units
                    orderby unit.GetUnitCost()
                    select unit).ToList();
                fac.Units = veryNewUnits.ToArray();
                foreach (var vFac in factions) {

                    if (fac.Entity.Name == vFac.Entity.Name + "_UPGRADE") {

                        var vFacUnits = new List<UnitBlueprint>(vFac.Units);
                        vFacUnits.AddRange(fac.Units);
                        var newUnits = (
                            from UnitBlueprint unit
                                in vFacUnits
                            orderby unit.GetUnitCost()
                            select unit).ToList();
                        vFac.Units = newUnits.ToArray();
                        Object.DestroyImmediate(fac);
                    }
                }
            }

            foreach (var objecting in combatUpgrade.LoadAllAssets<GameObject>()) 
            {
                if (objecting != null) {

                    if (objecting.GetComponent<Unit>()) newBases.Add(objecting);
                    else if (objecting.GetComponent<WeaponItem>()) {
                        newWeapons.Add(objecting);
                        int totalSubmeshes = 0;
                        foreach (var rend in objecting.GetComponentsInChildren<MeshFilter>()) {
                            if (rend.gameObject.activeSelf && rend.gameObject.activeInHierarchy && rend.mesh.subMeshCount > 0 && rend.GetComponent<MeshRenderer>() && rend.GetComponent<MeshRenderer>().enabled == true) {

                                totalSubmeshes += rend.mesh.subMeshCount;
                            }
                        }
                        foreach (var rend in objecting.GetComponentsInChildren<SkinnedMeshRenderer>()) {
                            if (rend.gameObject.activeSelf && rend.sharedMesh.subMeshCount > 0 && rend.enabled) {

                                totalSubmeshes += rend.sharedMesh.subMeshCount;
                            }
                        }
                        if (totalSubmeshes != 0) {
                            float average = 1f / totalSubmeshes;
                            var averageList = new List<float>();
                            for (int i = 0; i < totalSubmeshes; i++) { averageList.Add(average); }
                            objecting.GetComponent<WeaponItem>().SubmeshArea = null;
                            objecting.GetComponent<WeaponItem>().SubmeshArea = averageList.ToArray();
                        }
                    }
                    else if (objecting.GetComponent<ProjectileEntity>()) newProjectiles.Add(objecting);
                    else if (objecting.GetComponent<SpecialAbility>()) newAbilities.Add(objecting);
                    else if (objecting.GetComponent<PropItem>() && objecting.GetComponent<PropItem>().ShowInEditor) {
                        newProps.Add(objecting);
                        int totalSubmeshes = 0;
                        foreach (var rend in objecting.GetComponentsInChildren<MeshFilter>()) {
                            if (rend.gameObject.activeSelf && rend.gameObject.activeInHierarchy && rend.mesh.subMeshCount > 0 && rend.GetComponent<MeshRenderer>() && rend.GetComponent<MeshRenderer>().enabled == true) {

                                totalSubmeshes += rend.mesh.subMeshCount;
                            }
                        }
                        foreach (var rend in objecting.GetComponentsInChildren<SkinnedMeshRenderer>()) {
                            if (rend.gameObject.activeSelf && rend.sharedMesh.subMeshCount > 0 && rend.enabled) {

                                totalSubmeshes += rend.sharedMesh.subMeshCount;
                            }
                        }
                        if (totalSubmeshes != 0) {
                            float average = 1f / totalSubmeshes;
                            var averageList = new List<float>();
                            for (int i = 0; i < totalSubmeshes; i++) { averageList.Add(average); }
                            objecting.GetComponent<PropItem>().SubmeshArea = null;
                            objecting.GetComponent<PropItem>().SubmeshArea = averageList.ToArray();
                        }
                    }
                }
            }
            
            AddContentToDatabase();
        }

        public static void ToggleUpgrades_OnValueChanged(int value)
        {
            if (value == 0)
            {
                foreach (var u in unitList)
                {
                    var unit = (UnitBlueprint)u;
                    if (unit != null && unitSkeletons.ContainsKey(unit.Entity.GUID))
                    {
                        unitSkeletons[unit.Entity.GUID].CopyThisOntoUnit(unit);
                        
                        var secret = ContentDatabase.Instance().LandfallContentDatabase.GetFactions().ToList()
                            .Find(x => ((Faction)x).name == "Secret");
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
        
        public static void ToggleNerfs_OnValueChanged(int value)
        {
            ToggleNerfs = value;
        }
        
        public SettingsInstance CreateSetting(SettingsInstance.SettingsType settingsType, string settingName, string toolTip, string settingListToAddTo, string[] options = null, float min = 0f, float max = 1f) 
        {
            var setting = new SettingsInstance();

            setting.settingName = settingName;
            setting.toolTip = toolTip;
            setting.m_settingsKey = settingName;

            setting.settingsType = settingsType;
            setting.options = options;
            setting.min = min;
            setting.max = max;

            var global = ServiceLocator.GetService<GlobalSettingsHandler>();
            SettingsInstance[] listToAdd;
            if (settingListToAddTo == "BUG") listToAdd = global.BugsSettings;
            else if (settingListToAddTo == "VIDEO") listToAdd = global.VideoSettings;
            else if (settingListToAddTo == "AUDIO") listToAdd = global.AudioSettings;
            else if (settingListToAddTo == "CONTROLS") listToAdd = global.ControlSettings;
            else { listToAdd = global.GameplaySettings; }

            var list = listToAdd.ToList();
            list.Add(setting);

            if (settingListToAddTo == "BUG") typeof(GlobalSettingsHandler).GetField("m_bugsSettings", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(global, list.ToArray());
            else if (settingListToAddTo == "VIDEO") typeof(GlobalSettingsHandler).GetField("m_videoSettings", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(global, list.ToArray());
            else if (settingListToAddTo == "AUDIO") typeof(GlobalSettingsHandler).GetField("m_audioSettings", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(global, list.ToArray());
            else if (settingListToAddTo == "CONTROLS") typeof(GlobalSettingsHandler).GetField("m_controlSettings", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(global, list.ToArray());
            else typeof(GlobalSettingsHandler).GetField("m_gameplaySettings", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(global, list.ToArray());

            return setting;
        }
        
        public void AddContentToDatabase()
        {
	        Dictionary<DatabaseID, UnityEngine.Object> nonStreamableAssets = (Dictionary<DatabaseID, UnityEngine.Object>)typeof(AssetLoader).GetField("m_nonStreamableAssets", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(ContentDatabase.Instance().AssetLoader);
	        
            var db = ContentDatabase.Instance().LandfallContentDatabase;
            
            Dictionary<DatabaseID, UnitBlueprint> units = (Dictionary<DatabaseID, UnitBlueprint>)typeof(LandfallContentDatabase).GetField("m_unitBlueprints", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var unit in newUnits)
            {
	            if (!units.ContainsKey(unit.Entity.GUID))
	            {
		            units.Add(unit.Entity.GUID, unit);
		            nonStreamableAssets.Add(unit.Entity.GUID, unit);
	            }
            }
            typeof(LandfallContentDatabase).GetField("m_unitBlueprints", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, units);
            
            Dictionary<DatabaseID, Faction> factions = (Dictionary<DatabaseID, Faction>)typeof(LandfallContentDatabase).GetField("m_factions", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            List<DatabaseID> defaultHotbarFactions = (List<DatabaseID>)typeof(LandfallContentDatabase).GetField("m_defaultHotbarFactionIds", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var faction in newFactions)
            {
	            if (!factions.ContainsKey(faction.Entity.GUID))
	            {
		            factions.Add(faction.Entity.GUID, faction);
		            nonStreamableAssets.Add(faction.Entity.GUID, faction);
		            defaultHotbarFactions.Add(faction.Entity.GUID);
	            }
            }
            typeof(LandfallContentDatabase).GetField("m_factions", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, factions);
            typeof(LandfallContentDatabase).GetField("m_defaultHotbarFactionIds", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, defaultHotbarFactions.OrderBy(x => factions[x].index).ToList());

            Dictionary<DatabaseID, TABSCampaignAsset> campaigns = (Dictionary<DatabaseID, TABSCampaignAsset>)typeof(LandfallContentDatabase).GetField("m_campaigns", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var campaign in newCampaigns)
            {
	            if (!campaigns.ContainsKey(campaign.Entity.GUID))
	            {
		            campaigns.Add(campaign.Entity.GUID, campaign);
		            nonStreamableAssets.Add(campaign.Entity.GUID, campaign);
	            }
            }
            typeof(LandfallContentDatabase).GetField("m_campaigns", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, campaigns);
            
            Dictionary<DatabaseID, TABSCampaignLevelAsset> campaignLevels = (Dictionary<DatabaseID, TABSCampaignLevelAsset>)typeof(LandfallContentDatabase).GetField("m_campaignLevels", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var campaignLevel in newCampaignLevels)
            {
	            if (!campaignLevels.ContainsKey(campaignLevel.Entity.GUID))
	            {
		            campaignLevels.Add(campaignLevel.Entity.GUID, campaignLevel);
		            nonStreamableAssets.Add(campaignLevel.Entity.GUID, campaignLevel);
	            }
            }
            typeof(LandfallContentDatabase).GetField("m_campaignLevels", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, campaignLevels);
            
            Dictionary<DatabaseID, VoiceBundle> voiceBundles = (Dictionary<DatabaseID, VoiceBundle>)typeof(LandfallContentDatabase).GetField("m_voiceBundles", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var voiceBundle in newVoiceBundles)
            {
	            if (!voiceBundles.ContainsKey(voiceBundle.Entity.GUID))
	            {
		            voiceBundles.Add(voiceBundle.Entity.GUID, voiceBundle);
		            nonStreamableAssets.Add(voiceBundle.Entity.GUID, voiceBundle);
	            }
            }
            typeof(LandfallContentDatabase).GetField("m_voiceBundles", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, voiceBundles);
            
            List<DatabaseID> factionIcons = (List<DatabaseID>)typeof(LandfallContentDatabase).GetField("m_factionIconIds", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var factionIcon in newFactionIcons)
            {
	            if (!factionIcons.Contains(factionIcon.Entity.GUID))
	            {
		            factionIcons.Add(factionIcon.Entity.GUID);
		            nonStreamableAssets.Add(factionIcon.Entity.GUID, factionIcon);
	            }
            }
            typeof(LandfallContentDatabase).GetField("m_factionIconIds", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, factionIcons);
            
            Dictionary<DatabaseID, GameObject> unitBases = (Dictionary<DatabaseID, GameObject>)typeof(LandfallContentDatabase).GetField("m_unitBases", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var unitBase in newBases)
            {
	            if (!unitBases.ContainsKey(unitBase.GetComponent<Unit>().Entity.GUID))
	            {
		            unitBases.Add(unitBase.GetComponent<Unit>().Entity.GUID, unitBase);
		            nonStreamableAssets.Add(unitBase.GetComponent<Unit>().Entity.GUID, unitBase);
	            }
            }
            typeof(LandfallContentDatabase).GetField("m_unitBases", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, unitBases);
            
            Dictionary<DatabaseID, GameObject> props = (Dictionary<DatabaseID, GameObject>)typeof(LandfallContentDatabase).GetField("m_characterProps", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var prop in newProps)
            {
	            if (!props.ContainsKey(prop.GetComponent<PropItem>().Entity.GUID))
	            {
		            props.Add(prop.GetComponent<PropItem>().Entity.GUID, prop);
		            nonStreamableAssets.Add(prop.GetComponent<PropItem>().Entity.GUID, prop);
	            }
            }
            typeof(LandfallContentDatabase).GetField("m_characterProps", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, props);
            
            Dictionary<DatabaseID, GameObject> abilities = (Dictionary<DatabaseID, GameObject>)typeof(LandfallContentDatabase).GetField("m_combatMoves", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var ability in newAbilities)
            {
	            if (!abilities.ContainsKey(ability.GetComponent<SpecialAbility>().Entity.GUID))
	            {
		            abilities.Add(ability.GetComponent<SpecialAbility>().Entity.GUID, ability);
		            nonStreamableAssets.Add(ability.GetComponent<SpecialAbility>().Entity.GUID, ability);
	            }
            }
            typeof(LandfallContentDatabase).GetField("m_combatMoves", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, abilities);
            
            Dictionary<DatabaseID, GameObject> weapons = (Dictionary<DatabaseID, GameObject>)typeof(LandfallContentDatabase).GetField("m_weapons", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var weapon in newWeapons)
            {
	            if (!weapons.ContainsKey(weapon.GetComponent<WeaponItem>().Entity.GUID))
	            {
		            weapons.Add(weapon.GetComponent<WeaponItem>().Entity.GUID, weapon);
		            nonStreamableAssets.Add(weapon.GetComponent<WeaponItem>().Entity.GUID, weapon);
	            }
            }
            typeof(LandfallContentDatabase).GetField("m_weapons", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, weapons);
            
            Dictionary<DatabaseID, GameObject> projectiles = (Dictionary<DatabaseID, GameObject>)typeof(LandfallContentDatabase).GetField("m_projectiles", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var proj in newProjectiles)
            {
	            if (!projectiles.ContainsKey(proj.GetComponent<ProjectileEntity>().Entity.GUID))
	            {
		            projectiles.Add(proj.GetComponent<ProjectileEntity>().Entity.GUID, proj);
		            nonStreamableAssets.Add(proj.GetComponent<ProjectileEntity>().Entity.GUID, proj);
	            }
            }
            typeof(LandfallContentDatabase).GetField("m_projectiles", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, projectiles);


            
            ServiceLocator.GetService<CustomContentLoaderModIO>().QuickRefresh(WorkshopContentType.Unit, null);
        }
        
        public List<UnitBlueprint> newUnits = new List<UnitBlueprint>();

        public List<Faction> newFactions = new List<Faction>();
        
        public List<TABSCampaignAsset> newCampaigns = new List<TABSCampaignAsset>();
        
        public List<TABSCampaignLevelAsset> newCampaignLevels = new List<TABSCampaignLevelAsset>();
        
        public List<VoiceBundle> newVoiceBundles = new List<VoiceBundle>();
        
        public List<FactionIcon> newFactionIcons = new List<FactionIcon>();
        
        public List<GameObject> newBases = new List<GameObject>();
        
        public List<GameObject> newProps = new List<GameObject>();
        
        public List<GameObject> newAbilities = new List<GameObject>();

        public List<GameObject> newWeapons = new List<GameObject>();
        
        public List<GameObject> newProjectiles = new List<GameObject>();

        public static AssetBundle combatUpgrade = AssetBundle.LoadFromMemory(Properties.Resources.combatupgrade);

        public static List<UnitBlueprint> unitsToUpgrade = new List<UnitBlueprint>();

        public static List<UnitBlueprint> unitList = new List<UnitBlueprint>();
        
        public static List<GameObject> unitBaseList = new List<GameObject>();
        
        public static Dictionary<string, float> shieldWhitelist = new Dictionary<string, float>();
        
        public static Dictionary<DatabaseID, UnitSkeleton> unitSkeletons = new Dictionary<DatabaseID, UnitSkeleton>();
        
        public static Dictionary<DatabaseID, UnitSkeleton> originalUnitSkeletons = new Dictionary<DatabaseID, UnitSkeleton>();

        public static UnitBaseSkeleton unitBaseSkeleton = new UnitBaseSkeleton();
        
        public static Dictionary<DatabaseID, UnitBaseSkeleton> originalUnitBaseSkeletons = new Dictionary<DatabaseID, UnitBaseSkeleton>();

        public static int ToggleNerfs = 0;
    }
}
