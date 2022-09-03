using System.Collections;
using UnityEngine;
using Landfall.TABS;

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
            //yield return new WaitForSeconds(0.5f);
            var main = new VPMain();
            yield return new WaitForSeconds(0.25f);
            foreach (var b in LandfallUnitDatabase.GetDatabase().UnitBaseList)
            {
                if (b != null && !b.GetComponent<AttackSpeedBug>() && (b.name.Contains("Humanoid") || b.name.Contains("Stiffy")))
                {
                    b.GetComponentInChildren<StandingHandler>().enabled = false;
                    b.GetComponentInChildren<Balance>().allowedLegAngle = 125f;
                    b.GetComponentInChildren<Balance>().allowedTorsoAngle = 80f;
                    b.GetComponent<Unit>().data.legLeft.GetComponent<CurveAnimation>().animationData = VPMain.combatUpgrade
                        .LoadAsset<GameObject>("Humanoid_2.0").GetComponent<Unit>().data.legLeft
                        .GetComponent<CurveAnimation>().animationData;
                    b.GetComponent<Unit>().data.legLeft.GetComponent<CurveAnimation>().multiplier = 3f;
                    b.GetComponent<Unit>().data.legRight.GetComponent<CurveAnimation>().animationData = VPMain.combatUpgrade
                        .LoadAsset<GameObject>("Humanoid_2.0").GetComponent<Unit>().data.legRight
                        .GetComponent<CurveAnimation>().animationData;
                    b.GetComponent<Unit>().data.legRight.GetComponent<CurveAnimation>().multiplier = 3f;
                    b.GetComponent<Unit>().data.footLeft.GetComponent<CurveAnimation>().multiplier = 5f;
                    b.GetComponent<Unit>().data.footRight.GetComponent<CurveAnimation>().multiplier = 5f;
                    b.GetComponentInChildren<StepHandler>().multiplier = 1.6f;
                    b.GetComponent<Unit>().data.head.GetComponent<ConfigurableJoint>().DeepCopyOf(VPMain.combatUpgrade
                        .LoadAsset<GameObject>("Humanoid_2.0").GetComponent<Unit>().data.head
                        .GetComponent<ConfigurableJoint>());
                    b.GetComponent<Unit>().data.head.GetComponent<ConfigurableJoint>().connectedBody =
                        b.GetComponent<Unit>().data.mainRig;
                    b.GetComponent<Unit>().data.mainRig.GetComponent<ConfigurableJoint>().DeepCopyOf(VPMain.combatUpgrade
                        .LoadAsset<GameObject>("Humanoid_2.0").GetComponent<Unit>().data.mainRig
                        .GetComponent<ConfigurableJoint>());
                    b.GetComponent<Unit>().data.mainRig.GetComponent<ConfigurableJoint>().connectedBody =
                        b.GetComponent<Unit>().data.hip;
                    b.GetComponent<Unit>().data.hip.gameObject.AddComponent<SpawnFloatyPhysics>();
                }
                foreach (var u in main.unitsToUpgrade)
                {
                    var unit = (UnitBlueprint)u;
                
                    if (!main.unitsToNotUpgrade.Contains(unit) && unit.sizeMultiplier < 3f)
                    {
                        unit.animationMultiplier = 1f;
                        unit.movementSpeedMuiltiplier /= 1.5f;
                        unit.stepMultiplier = 1f;
                    }
                }
            }
            yield break;
        }

        private static VPBinder instance;
    }
}