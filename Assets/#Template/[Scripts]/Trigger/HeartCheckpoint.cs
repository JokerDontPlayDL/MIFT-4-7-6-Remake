using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DancingLineFanmade.Trigger
{
    public class HeartCheckpoint : Checkpoint
    {
        [Title("爱心存档点配置")] [SerializeField] private Transform rotator;
        [SerializeField] private Transform frame;
        [SerializeField] private Transform heart;

        private new void Start()
        {
            base.Start();
            rotator.DOMoveY(rotator.position.y + 0.5f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        }

        private void Update()
        {
            frame.Rotate(Vector3.up, Time.deltaTime * -45f);
            heart.Rotate(Vector3.up, Time.deltaTime * 45f);
        }

        public new void EnterTrigger()
        {
            base.EnterTrigger();
        }
    }
}