using Landfall.TABS;
using UnityEngine;

namespace VanillaPlus
{
    public class UnitBaseSkeleton
    {
        public bool standingHandlerEnabled;

        public float stepMultiplier;

        public float allowedLegAngle;
        public float allowedTorsoAngle;

        public CurveAnimationData legLeftData;
        public float legLeftMultiplier;
        
        public CurveAnimationData legRightData;
        public float legRightMultiplier;
        
        public float footLeftMultiplier;
        public float footRightMultiplier;

        public void CopyThisOntoUnitBase(GameObject ub1, bool isAddingPhysics = false, bool isRemovingPhysics = false)
        {
            ub1.GetComponentInChildren<StandingHandler>().enabled = standingHandlerEnabled;
            ub1.GetComponentInChildren<Balance>().allowedLegAngle = allowedLegAngle;
            ub1.GetComponentInChildren<Balance>().allowedTorsoAngle = allowedTorsoAngle;
            ub1.GetComponentInChildren<StepHandler>().multiplier = stepMultiplier;

            var unit = ub1.GetComponent<Unit>();

            var legLeftCurve = unit.data.legLeft.GetComponent<CurveAnimation>();
            legLeftCurve.animationData = legLeftData;
            legLeftCurve.multiplier = legLeftMultiplier;
            
            var legRightCurve = unit.data.legRight.GetComponent<CurveAnimation>();
            legRightCurve.animationData = legRightData;
            legRightCurve.multiplier = legRightMultiplier;

            if (isAddingPhysics && !unit.data.hip.GetComponent<SpawnFloatyPhysics>()) unit.data.hip.gameObject.AddComponent<SpawnFloatyPhysics>();
            if (isRemovingPhysics && unit.data.hip.GetComponent<SpawnFloatyPhysics>()) Object.Destroy(unit.data.hip.GetComponent<SpawnFloatyPhysics>());
        }

        public void CopyUnitBaseOntoThis(GameObject ub1)
        {
            standingHandlerEnabled = ub1.GetComponentInChildren<StandingHandler>().enabled;
            allowedLegAngle = ub1.GetComponentInChildren<Balance>().allowedLegAngle;
            allowedTorsoAngle = ub1.GetComponentInChildren<Balance>().allowedTorsoAngle;
            stepMultiplier = ub1.GetComponentInChildren<StepHandler>().multiplier;

            var unit = ub1.GetComponent<Unit>();

            var legLeftCurve = unit.data.legLeft.GetComponent<CurveAnimation>();
            legLeftData = legLeftCurve.animationData;
            legLeftMultiplier = legLeftCurve.multiplier;

            var legRightCurve = unit.data.legRight.GetComponent<CurveAnimation>();
            legRightData = legRightCurve.animationData;
            legRightMultiplier = legRightCurve.multiplier;

            //footLeftMultiplier = unit.data.footLeft.GetComponent<CurveAnimation>().multiplier;
            //footRightMultiplier = unit.data.footRight.GetComponent<CurveAnimation>().multiplier;
        }
    }
}