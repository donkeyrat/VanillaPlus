using Landfall.TABS;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace VanillaPlus {

	public class VPMain {

        public VPMain()
        {
            var db = LandfallUnitDatabase.GetDatabase();
            AssetPool.hideFlags = HideFlags.HideAndDontSave;
            AssetPool.SetActive(false);
            var upgradedUnits = combatUpgrade.LoadAllAssets<UnitBlueprint>().ToList();
            var unitList = db.UnitList.ToList();
            unitsToUpgrade = new List<IDatabaseEntity>(unitList.FindAll(x => x != null && (UnitBlueprint)x != null && ((UnitBlueprint)x).UnitBase && (((UnitBlueprint)x).UnitBase.name.Contains("Humanoid") || ((UnitBlueprint)x).UnitBase.name.Contains("Stiffy") || ((UnitBlueprint)x).UnitBase.name.Contains("Halfling") || ((UnitBlueprint)x).UnitBase.name.Contains("Blackbeard"))));
            unitsToNotUpgrade = new List<UnitBlueprint>();
            foreach (var unit in combatUpgrade.LoadAllAssets<UnitBlueprint>())
            {
                if (!unit.name.Contains("2.0"))
                {
                    db.AddUnitWithID(unit);
                }

                var find = db.UnitBaseList.ToList().Find(x => x.name == unit.UnitBase.name);
                if (unit.UnitBase && find)
                {
                    unit.UnitBase = find;
                }
            }
            foreach (var b in db.UnitBaseList)
            {
                if (b != null && (b.name.Contains("Humanoid") || b.name.Contains("Stiffy") || b.name.Contains("Halfling") || b.name.Contains("Blackbeard")))
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
                    DeepCopyUnit(unit, foundUnit);
                    unitsToNotUpgrade.Add(unit);
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

            new GameObject()
            {
                name = "Bullshit: The Reboot",
                hideFlags = HideFlags.HideAndDontSave
            }.AddComponent<VPSecretManager>();
            var sarissaSpear = db.WeaponList.ToList().Find(x => x.name.Contains("Spear_Greek"));
            if (sarissaSpear) { sarissaSpear.GetComponent<Holdable>().holdableData.setRotation = false; }
            var paladinHammer = db.WeaponList.ToList().Find(x => x.name.Contains("ClericMace_1"));
            if (paladinHammer) { paladinHammer.GetComponent<MeleeWeapon>().requiredPowerToParry = 5f; }
            var cultistMace = db.WeaponList.ToList().Find(x => x.name.Contains("ClericMaceEvil_1"));
            if (cultistMace) { cultistMace.GetComponent<MeleeWeapon>().requiredPowerToParry = 5f; }
        }

        public void DeepCopyUnit(UnitBlueprint unit1, UnitBlueprint unit2)
        {
            unit1.health = unit2.health;
            unit1.m_props = unit2.m_props;
            unit1.animationMultiplier = unit2.animationMultiplier;
            unit1.balanceMultiplier = unit2.balanceMultiplier;
            unit1.costTweak = unit2.costTweak;
            unit1.damageMultiplier = unit2.damageMultiplier;
            unit1.dragMultiplier = unit2.dragMultiplier;
            unit1.forceCost = unit2.forceCost;
            unit1.m_propData = unit2.m_propData;
            unit1.massMultiplier = unit2.massMultiplier;
            unit1.sizeMultiplier = unit2.sizeMultiplier;
            unit1.scaleWeapons = unit2.scaleWeapons;
            unit1.stepMultiplier = unit2.stepMultiplier;
            unit1.turningData = unit2.turningData;
            unit1.turnSpeed = unit2.turnSpeed;
            unit1.balanceForceMultiplier = unit2.balanceForceMultiplier;
            unit1.movementSpeedMuiltiplier = unit2.movementSpeedMuiltiplier;
            unit1.LeftWeapon = unit2.LeftWeapon;
            unit1.RightWeapon = unit2.RightWeapon;
            unit1.objectsToSpawnAsChildren = unit2.objectsToSpawnAsChildren;
            unit1.minSizeRandom = unit2.minSizeRandom;
            unit1.maxSizeRandom = unit2.maxSizeRandom;
            unit1.UnitBase = unit2.UnitBase;
            if (unit2.Entity.Name != "") unit1.Entity.Name = unit2.Entity.Name;
        }

        public static AssetBundle combatUpgrade = AssetBundle.LoadFromMemory(Properties.Resources.combatupgrade);

        private GameObject AssetPool = new GameObject();

        public List<IDatabaseEntity> unitsToUpgrade = new List<IDatabaseEntity>();
        
        public List<UnitBlueprint> unitsToNotUpgrade = new List<UnitBlueprint>();
    }
}
