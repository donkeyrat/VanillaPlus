using UnityEngine;
using Landfall.TABS;

namespace VanillaPlus
{
    public class ParryRoot : MonoBehaviour
    {
        public void Update()
        {
            globalCounter += Time.deltaTime;
            if (globalCounter > globalCooldown)
            {
                canParry = true;
            }
        }
        
        public void OnCollisionEnter(Collision col)
        {
            if (!col.rigidbody || !col.transform.GetComponent<MeleeWeapon>() || !col.transform.root.GetComponent<Unit>()) return;
            if (col.transform.root.GetComponent<Unit>().Team == transform.root.GetComponent<Unit>().Team || col.transform.root.GetComponent<Unit>().data.Dead || !GetComponent<MeleeWeapon>().isSwinging) return;
            if (canParry && GetComponent<MeleeWeapon>().requiredPowerToParry * 1.5f >= col.transform.GetComponent<MeleeWeapon>().requiredPowerToParry)
            {
                globalCounter = 0f;
                canParry = false;
                var force = 30f;
                
                Debug.Log("parry");
                
                col.transform.GetComponent<MeleeWeapon>().StopSwing();
                GetComponent<MeleeWeapon>().internalCounter = Mathf.Lerp(GetComponent<MeleeWeapon>().internalCounter,
                    GetComponent<MeleeWeapon>().internalCooldown, 0.8f);
                if (col.transform.GetComponent<ParryRoot>())
                {
                    col.transform.GetComponent<ParryRoot>().canParry = false;
                    col.transform.GetComponent<ParryRoot>().globalCounter /= 2f;
                }
                
                col.rigidbody.AddForce((col.transform.position - col.contacts[0].point).normalized * force, ForceMode.VelocityChange);
                
                GetComponent<Rigidbody>().AddForce((transform.position - col.contacts[0].point).normalized * force * -0.5f, ForceMode.VelocityChange);
                
                if (GetComponentInChildren<CollisionSound>())
                {
                    ServiceLocator.GetService<SoundPlayer>().PlaySoundEffect(GetComponentInChildren<CollisionSound>().SoundEffectRef, 1f, col.contacts[0].point, SoundEffectVariations.MaterialType.Metal);
                }
                var parry = Instantiate(VPMain.combatUpgrade.LoadAsset<GameObject>("E_Parry"), col.contacts[0].point, Quaternion.identity);
                parry.GetComponent<ParticleSystem>().Play();
            }
        }

        public bool canParry;
        
        public float globalCooldown = 2f;
    
        public float globalCounter = 1f;
    }
}