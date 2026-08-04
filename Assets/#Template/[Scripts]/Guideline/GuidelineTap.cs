using System.Collections;
using System.Collections.Generic;
using DancingLineFanmade.Level;
using DG.Tweening;
using UnityEngine;

namespace DancingLineFanmade.Guideline
{
    [DisallowMultipleComponent]
    public class GuidelineTap : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Material material;
        [SerializeField] private Sprite sprite;
        [SerializeField] internal float displayTime = -100f;
        [SerializeField] internal float triggerTime;
        [SerializeField] internal float triggerDistance = 1f;
        [SerializeField] internal int colorIndex;
        [SerializeField] internal bool haveLine = true;
        [SerializeField] internal bool triggered;

        private GameObject triggerEffect;
        private GameObject autoEffect;
        private BoxCollider autoplayCollider;
        private readonly List<SpriteRenderer> sprites = new();
        private const float TimeOffset = 0.25f;

        internal bool Autoplay;
        internal bool NoEffect;
        internal bool UseEffect;
        internal bool VisibleGuideLine = true;

        private float Distance => (transform.position - Player.Instance.transform.position).sqrMagnitude;

        public void SetColor(List<Color> colors)
        {
            spriteRenderer.color = colors[colorIndex];
        }

        private void Update()
        {
            if (AudioManager.Time > displayTime && !spriteRenderer.enabled && !triggered &&
                LevelManager.GameState == GameStatus.Playing)
                SetDisplay(true);
            if (Autoplay)
            {
                SetDisplay(VisibleGuideLine);
            }

            if (AudioManager.Time > triggerTime)
            {
                AnimateLine();
            }
        }

        public void InitBox(bool auto, bool visible)
        {
            if (!Autoplay)
            {
                Player.Instance.OnTurn.AddListener(Trigger);//线转向时触发
            }
            sprites.AddRange(GetComponentsInChildren<SpriteRenderer>());
            triggerEffect = Resources.Load<GameObject>("Prefabs/GuidelineTapEffect");
            autoEffect = Resources.Load<GameObject>("Prefabs/AutoEffect");
            spriteRenderer.material = material;
            spriteRenderer.sprite = sprite;
            Autoplay = auto;
            NoEffect = auto;

            if (displayTime <= 0)
            {
                displayTime = 0f;
                SetDisplay(true);
            }
            else SetDisplay(false);
            if (triggered) spriteRenderer.enabled = false;
        }

        public void AddBoxCollider(Vector3 size)
        {
            autoplayCollider = gameObject.AddComponent<BoxCollider>();
            autoplayCollider.isTrigger = true;
            autoplayCollider.size = size;
        }

        private void Trigger()
        {
            if (!(Distance <= triggerDistance) || !(Mathf.Abs(AudioManager.Time - triggerTime) <= TimeOffset) ||
                triggered)
                return;
            triggered = true;
            if (NoEffect)
                return;
            SetDisplay(false);
            StartCoroutine(DisplayEffect());
            Player.Instance.OnTurn.RemoveListener(Trigger);
        }

        public void SetDisplay(bool active)
        {
            foreach (var variable in sprites)
            {
                variable.enabled = active;
            }
        }

        public IEnumerator DisplayEffect()
        {
            // var color = Color.white;
            // var scale = transform.localScale;
            // var scaleVector = Vector3.one * 1.05f;
            var effect = Instantiate(triggerEffect, transform.position, Quaternion.Euler(-90, 0, 0)).transform;
            var component = effect.GetComponent<SpriteRenderer>();
            component.DOFade(0f, 0.75f).WaitForCompletion();
            effect.DOScale(7.5f, 0.75f).WaitForCompletion();//点击效果消散动画
            yield return new WaitForSeconds(0.75f);
            Destroy(effect.gameObject);
            yield return null;
        }

        public IEnumerator AutoTurnMark()
        {
            var mark = Instantiate(autoEffect, transform.position, Quaternion.Euler(-90, 0, 0)).transform;
            var sprite = mark.GetComponent<SpriteRenderer>();
            sprite.DOFade(0, 0.75f).WaitForCompletion();
            mark.DOScale(7.5f, 0.75f).WaitForCompletion();
            yield return new WaitForSeconds(0.75f);
            Destroy(mark.gameObject);
            yield return null;
        }

        private void OnTriggerEnter(Collider other)//这里主要是自动播放转向
        {
            if (!Autoplay || autoplayCollider == null || !other.CompareTag("Player") || triggered ||
                Player.Instance.Falling)//判断条件
                return;
            var playerPosition = Player.Instance.transform.position;//线的位置
            var position = transform.position;//引导框的位置
            var normalizedPlayerPosition = new Vector3(playerPosition.x, 0, playerPosition.z);//线的二维方位
            var normalizedPosition = new Vector3(position.x, 0, position.z);//自身二维方位
            var time = Mathf.Abs((normalizedPlayerPosition - normalizedPosition).magnitude) / Player.Instance.Speed;//计算从线进入引导框触发器到触发的时间
            if (time > 0)
                Invoke(nameof(TurnPlayer), time);
            else TurnPlayer();//等待转向
        }

        private void TurnPlayer()
        {
            Player.Instance.Turn();
            if (UseEffect)
            {
                StartCoroutine(AutoTurnMark());
                SetDisplay(false);
            }

        }

        private void AnimateLine()
        {
            var line = transform.GetChild(0);
        }
    }
}