//此脚本可用于输出当前时间，方便调试触发器的时间点，可用于像皇冠、存档点等。
using DancingLineFanmade.Level;
using UnityEngine;

namespace DancingLineFanmade.Trigger
{
    [DisallowMultipleComponent]
    public class TriggerDebugTime : MonoBehaviour
    {
        private void OnTriggerEnter(Collider c)
        {
            if (c.CompareTag("Player"))
            {
                Debug.Log("触发时间： " + AudioManager.Time);
            }
        }
    }
}

