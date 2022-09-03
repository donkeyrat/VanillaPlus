using UnityEngine;
using UnityEngine.SceneManagement;
using Pathfinding;
using Landfall.TABS;
using System.Collections;

namespace VanillaPlus
{
    public class VPSecretManager : MonoBehaviour
    {
        public VPSecretManager()
        {
            SceneManager.sceneLoaded += SceneLoaded;
        }

        public void SceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.name == "02_Lvl1_Farmer_VC")
            {
                var secrets = new GameObject()
                {
                    name = "Secrets"
                };
                Instantiate(VPMain.combatUpgrade.LoadAsset<GameObject>("SkewerThrower_Unlock"), secrets.transform, true);
            }
            if (scene.name == "05_Lvl2_Medieval_VC")
            {
                var secrets = new GameObject()
                {
                    name = "Secrets"
                };
                Instantiate(VPMain.combatUpgrade.LoadAsset<GameObject>("Marauder_Unlock"), secrets.transform, true);
            }
            if (scene.name == "05_AsiaTemple_VC")
            {
                var secrets = new GameObject()
                {
                    name = "Secrets"
                };
                Instantiate(VPMain.combatUpgrade.LoadAsset<GameObject>("Nunchaku_Unlock"), secrets.transform, true);
            }
        }
    }
}
