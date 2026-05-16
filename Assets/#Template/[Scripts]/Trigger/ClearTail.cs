using UnityEngine;
using DancingLineFanmade.Level;

namespace DancingLineFanmade.Trigger
{
    [DisallowMultipleComponent]
    public class ClearTail : MonoBehaviour
    {
        void OnTriggerEnter(Collider target)
        {
            if (target.CompareTag("Player"))
            {
                Player player = target.GetComponent<Player>();
                player.ClearPool();
            }
            if (target.CompareTag("FakePlayer"))
            {
                FakePlayer fp = target.GetComponent<FakePlayer>();
                fp.ClearPool();
            }
        }
    }
}
