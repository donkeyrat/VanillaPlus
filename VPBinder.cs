using System.Collections;
using UnityEngine;
using Landfall.TABS;
using System.Collections.Generic;
using System.Linq;
using DM;

namespace VanillaPlus {

    public class VPBinder : MonoBehaviour {

        public static void UnitGlad() {

            if (!instance) {

                instance = new GameObject {
                    hideFlags = HideFlags.HideAndDontSave
                }.AddComponent<VPBinder>();
            }
            instance.StartCoroutine(StartUnitgradLate());
        }

        private static IEnumerator StartUnitgradLate() {

            yield return new WaitUntil(() => FindObjectOfType<ServiceLocator>() != null);
            yield return new WaitUntil(() => ServiceLocator.GetService<ISaveLoaderService>() != null);

            new VPMain();
            
            yield return new WaitForSeconds(0.4f);

            var db = ContentDatabase.Instance().LandfallContentDatabase;

            VPMain.unitBaseList = new List<GameObject>(db.GetUnitBases().ToList().FindAll(b =>
                b != null && !b.name.Contains("_2.") && (b.name.Contains("Humanoid") || b.name.Contains("Stiffy") ||
                                                         b.name.Contains("Halfling") ||
                                                         b.name.Contains("Blackbeard"))));
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
            VPMain.ToggleNerfs_OnValueChanged(0);
        }

        private static VPBinder instance;
    }
}