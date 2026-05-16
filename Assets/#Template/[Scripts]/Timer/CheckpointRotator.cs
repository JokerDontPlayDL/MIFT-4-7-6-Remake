using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DancingLineFanmade;

namespace DancingLineFanmade.Timer
{
    public class CheckpointRotator : MonoBehaviour
    {
        private Transform frame;
        private Transform heart;

        void Start()
        {
            frame = transform.Find("Frame");
            heart = transform.Find("Heart");
        }

        void Update()
        {
            frame.Rotate(Vector3.up, Time.deltaTime * -45f);
            heart.Rotate(Vector3.up, Time.deltaTime * 45f);
        }
    }

}
