using UnityEngine;
using Landfall.TABS;

namespace VanillaPlus
{
    public class UnitSkeleton
    {
        public float health;

        public GameObject[] m_props;

        public float animationMultiplier;

        public float balanceMultiplier;

        public float costTweak;

        public float damageMultiplier;

        public uint forceCost;

        public PropItemData[] m_propData;

        public float massMultiplier;

        public float sizeMultiplier;

        public float stepMultiplier;

        public TurningData turningData;

        public float turnSpeed;

        public float balanceForceMultiplier;

        public float movementSpeedMuiltiplier;

        public GameObject LeftWeapon;

        public GameObject RightWeapon;

        public GameObject[] objectsToSpawnAsChildren;

        public float minSizeRandom;

        public float maxSizeRandom;

        public GameObject UnitBase;

        public string Name;
        
        public void CopyThisOntoUnit(UnitBlueprint unit1)
        {
            var unit2 = this;
            unit1.health = unit2.health;
            unit1.m_props = unit2.m_props;
            unit1.animationMultiplier = unit2.animationMultiplier;
            unit1.balanceMultiplier = unit2.balanceMultiplier;
            unit1.costTweak = unit2.costTweak;
            unit1.damageMultiplier = unit2.damageMultiplier;
            unit1.forceCost = unit2.forceCost;
            unit1.m_propData = unit2.m_propData;
            unit1.massMultiplier = unit2.massMultiplier;
            unit1.sizeMultiplier = unit2.sizeMultiplier;
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
            if (unit2.Name != "") unit1.Entity.Name = unit2.Name;
        }

        public void CopyUnitOntoThis(UnitBlueprint unit2)
        {
            var unit1 = this;
            unit1.health = unit2.health;
            unit1.m_props = unit2.m_props;
            unit1.animationMultiplier = unit2.animationMultiplier;
            unit1.balanceMultiplier = unit2.balanceMultiplier;
            unit1.costTweak = unit2.costTweak;
            unit1.damageMultiplier = unit2.damageMultiplier;
            unit1.forceCost = unit2.forceCost;
            unit1.m_propData = unit2.m_propData;
            unit1.massMultiplier = unit2.massMultiplier;
            unit1.sizeMultiplier = unit2.sizeMultiplier;
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
            if (unit2.Entity.Name != "") unit1.Name = unit2.Entity.Name;
        }
    }
}