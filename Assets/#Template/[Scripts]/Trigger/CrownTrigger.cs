using UnityEngine;

namespace DancingLineFanmade.Trigger
{
    [DisallowMultipleComponent]
    public class CrownTrigger : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                transform.GetComponentInParent<CrownCheckpoint>().EnterTrigger();
                gameObject.SetActive(false);
            }
        }
    }

}

