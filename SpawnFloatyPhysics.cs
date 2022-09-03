using UnityEngine;
using Landfall.TABS;

namespace VanillaPlus
{
    public class SpawnFloatyPhysics : MonoBehaviour
    {
        public void Awake()
        {
            Instantiate(VPMain.combatUpgrade.LoadAsset<GameObject>("2.0Hover"),
                transform.parent);
            Destroy(this);
        }
    }
}