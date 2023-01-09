using UnityEngine;
using Landfall.TABS;

namespace VanillaPlus
{
    public class SpawnFloatyPhysics : MonoBehaviour
    {
        public void Start()
        {
            var wings = transform.root.GetComponentInChildren<Wings>();
            if (wings && wings.useWingsInPlacement)
            {
                var standingHandler = transform.root.GetComponentInChildren<StandingHandler>();
                if (standingHandler) standingHandler.enabled = true;
                
                return;
            }

            if (transform.root.GetComponent<Unit>())
            {
                var unit = transform.root.GetComponent<Unit>();
                unit.data.footLeft.GetComponent<CurveAnimation>().multiplier = 2.5f;
                unit.data.footRight.GetComponent<CurveAnimation>().multiplier = 2.5f;
            }
            
            Instantiate(VPMain.combatUpgrade.LoadAsset<GameObject>("2.0Hover"),
                transform.parent);
            Destroy(this);
        }
    }
}