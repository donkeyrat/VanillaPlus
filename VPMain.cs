using Landfall.TABS;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BitCode.Debug.Commands;
using HarmonyLib;
using Landfall.TABS.UnitEditor;

namespace VanillaPlus {

	public class VPMain {

        public VPMain()
        {
            var db = LandfallUnitDatabase.GetDatabase();
            AssetPool.hideFlags = HideFlags.HideAndDontSave;
            AssetPool.SetActive(false);
            var upgradedUnits = combatUpgrade.LoadAllAssets<UnitBlueprint>().ToList();
            unitList = db.UnitList.ToList();
            unitsToUpgrade = new List<IDatabaseEntity>(unitList.FindAll(x => x != null && (UnitBlueprint)x != null && ((UnitBlueprint)x).UnitBase && (((UnitBlueprint)x).UnitBase.name.Contains("Humanoid") || ((UnitBlueprint)x).UnitBase.name.Contains("Stiffy") || ((UnitBlueprint)x).UnitBase.name.Contains("Halfling") || ((UnitBlueprint)x).UnitBase.name.Contains("Blackbeard"))));
            unitsToNotUpgrade = new List<UnitBlueprint>();
            
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
            shieldWhitelist.Add("CookieShield", 1f);
            shieldWhitelist.Add("CenturionShield_1 Weapons_VB", 1f);
            
            foreach (var unit in combatUpgrade.LoadAllAssets<UnitBlueprint>())
            {
                if (!unit.name.Contains("2.0"))
                {
                    db.AddUnitWithID(unit);
                }

                var find = db.UnitBaseList.ToList().Find(x => x.name == unit.UnitBase.name);
                var find2 = db.UnitBaseList.ToList().Find(x => x.name.Contains("Humanoid_1 Prefabs_VB"));
                if (unit.UnitBase && find)
                {
                    unit.UnitBase = find;
                }
                else if (unit.UnitBase && unit.UnitBase.name.Contains("2.0") && !unit.UnitBase.name.Contains("Minotaur") && find2)
                {
                    unit.UnitBase = find2;
                }
            }

            foreach (var b in db.UnitBaseList)
            {
                if (b != null && !b.name.Contains("_2.") && (b.name.Contains("Humanoid") || b.name.Contains("Stiffy") || b.name.Contains("Halfling") || b.name.Contains("Blackbeard")))
                {
                    b.GetComponentInChildren<StandingHandler>().enabled = false;
                    b.GetComponentInChildren<Balance>().allowedLegAngle = 125f;
                    b.GetComponentInChildren<Balance>().allowedTorsoAngle = 80f;
                    b.GetComponent<Unit>().data.legLeft.GetComponent<CurveAnimation>().animationData = combatUpgrade
                        .LoadAsset<GameObject>("Humanoid_2.0").GetComponent<Unit>().data.legLeft
                        .GetComponent<CurveAnimation>().animationData;
                    b.GetComponent<Unit>().data.legLeft.GetComponent<CurveAnimation>().multiplier = 3f;
                    b.GetComponent<Unit>().data.legRight.GetComponent<CurveAnimation>().animationData = combatUpgrade
                        .LoadAsset<GameObject>("Humanoid_2.0").GetComponent<Unit>().data.legRight
                        .GetComponent<CurveAnimation>().animationData;
                    b.GetComponent<Unit>().data.legRight.GetComponent<CurveAnimation>().multiplier = 3f;
                    b.GetComponent<Unit>().data.footLeft.GetComponent<CurveAnimation>().multiplier = 5f;
                    b.GetComponent<Unit>().data.footRight.GetComponent<CurveAnimation>().multiplier = 5f;
                    b.GetComponentInChildren<StepHandler>().multiplier = 1.6f;
                    b.GetComponent<Unit>().data.head.GetComponent<ConfigurableJoint>().DeepCopyOf(combatUpgrade
                        .LoadAsset<GameObject>("Humanoid_2.0").GetComponent<Unit>().data.head
                        .GetComponent<ConfigurableJoint>());
                    b.GetComponent<Unit>().data.head.GetComponent<ConfigurableJoint>().connectedBody =
                        b.GetComponent<Unit>().data.mainRig;
                    b.GetComponent<Unit>().data.mainRig.GetComponent<ConfigurableJoint>().DeepCopyOf(combatUpgrade
                        .LoadAsset<GameObject>("Humanoid_2.0").GetComponent<Unit>().data.mainRig
                        .GetComponent<ConfigurableJoint>());
                    b.GetComponent<Unit>().data.mainRig.GetComponent<ConfigurableJoint>().connectedBody =
                        b.GetComponent<Unit>().data.hip;
                    b.GetComponent<Unit>().data.hip.gameObject.AddComponent<SpawnFloatyPhysics>();
                    b.AddComponent<AttackSpeedBug>();
                }
            }
            foreach (var u in unitList)
            {
                var unit = (UnitBlueprint)u;
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

            foreach (var u in unitsToUpgrade)
            {
                var unit = (UnitBlueprint)u;
                
                if (!unitsToNotUpgrade.Contains(unit) && unit.sizeMultiplier < 3f)
                {
                    unit.animationMultiplier = Mathf.Lerp(unit.animationMultiplier, 1f, 0.5f);
                    unit.movementSpeedMuiltiplier = Mathf.Lerp(unit.movementSpeedMuiltiplier, 0.75f, 0.5f);
                    unit.stepMultiplier = Mathf.Lerp(unit.stepMultiplier, 0.75f, 0.5f);
                    unitsToNotUpgrade.Add(unit);
                }
            }

            new GameObject()
            {
                name = "Bullshit: The Reboot",
                hideFlags = HideFlags.HideAndDontSave
            }.AddComponent<VPSecretManager>();
            
            var sarissaSpear = db.WeaponList.ToList().Find(x => x.name.Contains("Spear_Greek_1"));
            if (sarissaSpear) sarissaSpear.GetComponent<Holdable>().holdableData.setRotation = false;
            var paladinHammer = db.WeaponList.ToList().Find(x => x.name.Contains("ClericMace_1"));
            if (paladinHammer) paladinHammer.GetComponent<MeleeWeapon>().requiredPowerToParry = 5f;
            var cultistMace = db.WeaponList.ToList().Find(x => x.name.Contains("ClericMaceEvil_1"));
            if (cultistMace) cultistMace.GetComponent<MeleeWeapon>().requiredPowerToParry = 5f;
            var assassinDagger = db.WeaponList.ToList().Find(x => x.name.Contains("Assassin_Dagger_1"));
            if (assassinDagger) assassinDagger.GetComponent<MeleeWeapon>().requiredPowerToParry = 5f;
            var warGlaive = db.WeaponList.ToList().Find(x => x.name.Contains("WarGlaivecurved_1"));
            if (warGlaive) warGlaive.GetComponent<MeleeWeapon>().requiredPowerToParry = 5f;
            var club = db.WeaponList.ToList().Find(x => x.name.Contains("Club_1") && !x.name.Contains("Aztec"));
            if (club) club.GetComponent<MeleeWeapon>().requiredPowerToParry = 5f;
            var superb = db.WeaponList.ToList().Find(x => x.name.Contains("Leg_SuperBoxer_W_1 Weapons_VB"));
            if (superb) superb.GetComponent<MeleeWeapon>().requiredPowerToParry = 50f;
            var superbR = db.WeaponList.ToList().Find(x => x.name.Contains("Leg_SuperBoxer_W_R_1 Weapons_VB"));
            if (superbR) superbR.GetComponent<MeleeWeapon>().requiredPowerToParry = 50f;
            var valk = db.WeaponList.ToList().Find(x => x.name.Contains("ValkyrieSword_1 Weapons_VB"));
            if (valk) valk.GetComponent<MeleeWeapon>().requiredPowerToParry = 5f;

            var toggleUpgrades = CreateSetting(SettingsInstance.SettingsType.Options, "Toggle unit upgrades",
                "Enables/disables unit modifications.", "GAMEPLAY",
                new string[] { "Enable unit modifications", "Disable unit modifications" });
            toggleUpgrades.OnValueChanged += ToggleUpgrades;
            ToggleUpgrades(toggleUpgrades.defaultValue);
            
            var toggleShieldBlocking = CreateSetting(SettingsInstance.SettingsType.Options, "Toggle shield blocking",
                "Enables/disables shied blocking.", "GAMEPLAY",
                new string[] { "Enable shield blocking", "Disable shield blocking" });
            toggleShieldBlocking.OnValueChanged += ToggleShieldBlocking;
            ToggleShieldBlocking(toggleShieldBlocking.defaultValue);
            
            var toggleAllBlocking = CreateSetting(SettingsInstance.SettingsType.Options, "Make all weapons block",
                "Makes all weapons block on contact", "BUG",
                new string[] { "Disable weapon blocking", "Enable weapon blocking" });
            toggleAllBlocking.OnValueChanged += ToggleAllBlocking;
            ToggleAllBlocking(toggleAllBlocking.defaultValue);
            
            List<Faction> factions = (List<Faction>)typeof(LandfallUnitDatabase).GetField("Factions", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
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

            List<GameObject> stuff = (List<GameObject>)typeof(LandfallUnitDatabase).GetField("UnitBases", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            List<GameObject> stuff2 = (List<GameObject>)typeof(LandfallUnitDatabase).GetField("Weapons", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            List<GameObject> stuff3 = (List<GameObject>)typeof(LandfallUnitDatabase).GetField("Projectiles", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            List<GameObject> stuff4 = (List<GameObject>)typeof(LandfallUnitDatabase).GetField("CombatMoves", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            List<GameObject> stuff5 = (List<GameObject>)typeof(LandfallUnitDatabase).GetField("CharacterProps", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var objecting in combatUpgrade.LoadAllAssets<GameObject>()) {

                if (objecting != null) {

                    if (objecting.GetComponent<Unit>()) {
                        stuff.Add(objecting);
                    }
                    else if (objecting.GetComponent<WeaponItem>()) {
                        stuff2.Add(objecting);
                    }
                    else if (objecting.GetComponent<ProjectileEntity>()) {
                        stuff3.Add(objecting);
                    }
                    else if (objecting.GetComponent<SpecialAbility>()) {
                        stuff4.Add(objecting);
                    }
                    else if (objecting.GetComponent<PropItem>() && objecting.GetComponent<PropItem>().ShowInEditor) {
                        stuff5.Add(objecting);
                    }
                }
            }
            typeof(LandfallUnitDatabase).GetField("UnitBases", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, stuff);
            typeof(LandfallUnitDatabase).GetField("Weapons", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, stuff2);
            typeof(LandfallUnitDatabase).GetField("Projectiles", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, stuff3);
            typeof(LandfallUnitDatabase).GetField("CombatMoves", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, stuff4);
            typeof(LandfallUnitDatabase).GetField("CharacterProps", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, stuff5);
        }

        public void ToggleUpgrades(int value)
        {
            if (value == 0)
            {
                foreach (var u in unitList)
                {
                    var unit = (UnitBlueprint)u;
                    if (unit != null && unitSkeletons.ContainsKey(unit.Entity.GUID))
                    {
                        unitSkeletons[unit.Entity.GUID].CopyThisOntoUnit(unit);
                        var secret = (Faction)LandfallUnitDatabase.GetDatabase().FactionList.ToList()
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
                    var unit = (UnitBlueprint)u;
                    if (unit != null && originalUnitSkeletons.ContainsKey(unit.Entity.GUID))
                    {
                        originalUnitSkeletons[unit.Entity.GUID].CopyThisOntoUnit(unit);
                        var secret = (Faction)LandfallUnitDatabase.GetDatabase().FactionList.ToList()
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
        }

        public void ToggleShieldBlocking(int value)
        {
            if (value == 0)
            {
                foreach (var wp in LandfallUnitDatabase.GetDatabase().WeaponList)
                {
                    if (shieldWhitelist.ContainsKey(wp.name) && wp.GetComponent<MeleeWeapon>() && wp.GetComponent<Rigidbody>() && !wp.GetComponent<ParryRoot>())
                    {
                        wp.AddComponent<ParryRoot>().globalCooldown = shieldWhitelist[wp.name];
                    }
                }
            }
            else
            {
                foreach (var wp in LandfallUnitDatabase.GetDatabase().WeaponList)
                {
                    if (shieldWhitelist.ContainsKey(wp.name) && wp.GetComponent<ParryRoot>())
                    {
                        Object.DestroyImmediate(wp.GetComponent<ParryRoot>());
                    }
                }
            }
        }
        
        public void ToggleAllBlocking(int value)
        {
            if (value == 1)
            {
                foreach (var wp in LandfallUnitDatabase.GetDatabase().WeaponList)
                {
                    if (!shieldWhitelist.ContainsKey(wp.name) && wp.GetComponent<MeleeWeapon>() && wp.GetComponent<Rigidbody>() && !wp.GetComponent<ParryRoot>())
                    {
                        wp.AddComponent<ParryRoot>();
                    }
                }
            }
            else
            {
                foreach (var wp in LandfallUnitDatabase.GetDatabase().WeaponList)
                {
                    if (!shieldWhitelist.ContainsKey(wp.name) && wp.GetComponent<ParryRoot>())
                    {
                        Object.DestroyImmediate(wp.GetComponent<ParryRoot>());
                    }
                }
            }
        }
        
        public SettingsInstance CreateSetting(SettingsInstance.SettingsType settingsType, string settingName, string toolTip, string settingListToAddTo, string[] options = null, float min = 0f, float max = 1f) {

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
            if (settingListToAddTo == "BUG") { listToAdd = global.BugsSettings; }
            else if (settingListToAddTo == "VIDEO") { listToAdd = global.VideoSettings; }
            else if (settingListToAddTo == "AUDIO") { listToAdd = global.AudioSettings; }
            else if (settingListToAddTo == "CONTROLS") { listToAdd = global.ControlSettings; }
            else { listToAdd = global.GameplaySettings; }

            var list = listToAdd.ToList();
            list.Add(setting);

            if (settingListToAddTo == "BUG") { typeof(GlobalSettingsHandler).GetField("m_bugsSettings", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(global, list.ToArray()); }
            else if (settingListToAddTo == "VIDEO") { typeof(GlobalSettingsHandler).GetField("m_videoSettings", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(global, list.ToArray()); }
            else if (settingListToAddTo == "AUDIO") { typeof(GlobalSettingsHandler).GetField("m_audioSettings", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(global, list.ToArray()); }
            else if (settingListToAddTo == "CONTROLS") { typeof(GlobalSettingsHandler).GetField("m_controlSettings", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(global, list.ToArray()); }
            else { typeof(GlobalSettingsHandler).GetField("m_gameplaySettings", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(global, list.ToArray()); }

            return setting;
        }

        public static AssetBundle combatUpgrade = AssetBundle.LoadFromMemory(Properties.Resources.combatupgrade);

        private GameObject AssetPool = new GameObject();

        public List<IDatabaseEntity> unitsToUpgrade = new List<IDatabaseEntity>();
        
        public List<UnitBlueprint> unitsToNotUpgrade = new List<UnitBlueprint>();

        public List<IDatabaseEntity> unitList = new List<IDatabaseEntity>();
        
        public Dictionary<string, float> shieldWhitelist = new Dictionary<string, float>();
        
        public Dictionary<DatabaseID, UnitSkeleton> unitSkeletons = new Dictionary<DatabaseID, UnitSkeleton>();
        
        public Dictionary<DatabaseID, UnitSkeleton> originalUnitSkeletons = new Dictionary<DatabaseID, UnitSkeleton>();
    }
}
