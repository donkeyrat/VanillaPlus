using UnityEngine;
using Landfall.TABS;
using Landfall.TABS.AI;
using System.Collections;
using BitCode.Debug.Commands;

namespace VanillaPlus 
{
    public class Superimage : MonoBehaviour 
    {
        public void Start() { GetComponent<UnitSpawner>().unitBlueprint = transform.root.GetComponent<Unit>().unitBlueprint; }

        public void SpawnAfterimage() { StartCoroutine(Spawn()); }

        public IEnumerator Spawn() 
        {

            transform.position = transform.root.GetComponent<Unit>().data.mainRig.position;
            
            var u = GetComponent<UnitSpawner>().Spawn();
            u.name = "SUPERIMAGE";
            u.transform.position = new Vector3(transform.root.GetComponent<Unit>().data.mainRig.position.x, transform.root.GetComponent<Unit>().data.mainRig.position.y - 1.353508f, transform.root.GetComponent<Unit>().data.mainRig.position.z);
            u.data.GetComponent<UnitColorHandler>().SetMaterial(imgMaterial);
            u.GetComponent<UnitAPI>().forceSupressFromWinCondition = true;
            u.targetingPriorityMultiplier = 0.1f;
            foreach (var move in u.GetComponentsInChildren<ConditionalEvent>()) Destroy(move.gameObject);

            Instantiate(poofEffect, u.data.mainRig.position, poofEffect.transform.rotation);

            yield return new WaitForSeconds(fadeTime);
            
            Instantiate(despawnEffect, u.data.mainRig.position, poofEffect.transform.rotation);

            yield return new WaitForSeconds(destroyDelay);

            foreach (var trail in u.GetComponentsInChildren<TrailRenderer>())
            {
                trail.transform.SetParent(null);
                trail.emitting = false;
                trail.gameObject.AddComponent<RemoveAfterSeconds>().seconds = trail.time * 1.5f;
            }
            
            u.DestroyUnit();
        }

        public Material imgMaterial;

        public GameObject poofEffect;

        public GameObject despawnEffect;

        public float fadeTime = 0.5f;

        public float destroyDelay = 0.05f;
    }
}