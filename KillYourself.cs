using UnityEngine;
using System;
using Landfall.TABS;

namespace VanillaPlus
{
    public class KillYourself : MonoBehaviour
    {
        public void Kill()
        {
            transform.root.GetComponent<Unit>().data.healthHandler.Die();
        }
    }
}