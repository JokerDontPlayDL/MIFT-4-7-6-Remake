using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DancingLineFanmade;

namespace DancingLineFanmade.Trigger
{
    [DisallowMultipleComponent, RequireComponent(typeof(CheckpointTrigger))]
    public class CrownTrigger : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                transform.GetComponentInParent<Crown>().TriggerEnter();
                gameObject.SetActive(false);
            }
        }
    }

}

