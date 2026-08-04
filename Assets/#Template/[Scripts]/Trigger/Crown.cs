using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DancingLineFanmade.Level;
using DG.Tweening;

namespace DancingLineFanmade.Trigger
{
    [DisallowMultipleComponent, RequireComponent(typeof(Checkpoint))]
    public class Crown : MonoBehaviour
    {
        public GameObject crownObject;
        public GameObject crownEffect;
        public GameObject crownIcon;

        private MeshRenderer crownRenderer;
        private SpriteRenderer iconRenderer;
        private GameObject effectObject;
        private Vector3 iconPos;
        private Vector3 targetPos;
        private Color crownColor;
        private Color iconColor;
        private bool revived = false;

        private Player player;

        private ParticleSystem crownAura;
        private readonly List<Tween> auraTweens = new List<Tween>();
        private Tween crownTween;
        private const float auraTweenDuration = 1.25f;
        private bool taken;
        private bool usedParticalDisappear;

        void Start()
        {
            crownRenderer = crownObject.GetComponent<MeshRenderer>();
            iconRenderer = crownIcon.transform.GetChild(0).GetComponent<SpriteRenderer>();
            player = Player.Instance;
            crownColor = crownRenderer.material.color;
            iconPos = crownIcon.transform.position;

            iconRenderer.color = new Color(iconRenderer.color.r, iconRenderer.color.g, iconRenderer.color.b, 0f);
            targetPos = iconPos + Vector3.up * 5f;

            InitParticles();
        }

        void Update()
        {
            crownObject.transform.Rotate(Vector3.up, Time.deltaTime * 45f);
        }

        private void InitParticles()
        {
            if (crownEffect != null)
            {
                crownAura = crownEffect.GetComponent<ParticleSystem>();
                if (crownAura != null)
                {
                    var color = crownRenderer.material.color;
                    color.a = 0f;
                    var systems = crownAura.GetComponentsInChildren<ParticleSystem>();
                    foreach (var system in systems)
                    {
                        var main = system.main;
                        main.startColor = color;
                    }
                }
            }
        }

        private void RefreshParticlesColor()
        {
            if (crownAura == null) return;

            var color = crownRenderer.material.color;
            var systems = crownAura.GetComponentsInChildren<ParticleSystem>();
            foreach (var system in systems)
            {
                var main = system.main;
                main.startColor = color;
            }
        }

        private void ClearAuraTweens()
        {
            foreach (var tween in auraTweens)
                tween.Kill();
            auraTweens.Clear();
        }

        private void StopAnimations()
        {
            ClearAuraTweens();
            if (crownTween == null) return;
            crownTween.Kill();
            crownTween = null;
        }

        private void ShowSpirit()
        {
            AnimateCrown(true);
        }

        public void AnimateCrown(bool show)
        {
            crownTween = iconRenderer.DOFade(show ? 1 : 0, auraTweenDuration / 4f);
            crownTween.SetEase(Ease.OutSine);
            crownTween.OnComplete(() =>
            {
                crownTween.Kill();
                crownTween = null;
            });

            if (!show && !usedParticalDisappear && crownAura != null)
            {
                var systems = crownAura.GetComponentsInChildren<ParticleSystem>();
                crownAura.Play();

                usedParticalDisappear = true;

                crownAura.transform.DOMoveY(crownAura.transform.position.y + 10f, auraTweenDuration / 1f).SetEase(Ease.Linear);
            }
        }

        public void TriggerEnter()
        {
            if (taken) return;
            //taken = true;

            crownRenderer.enabled = false;

            if (crownEffect != null)
            {
                effectObject = Instantiate(crownEffect, crownObject.transform.position, Quaternion.Euler(Vector3.zero));

                crownAura = effectObject.GetComponent<ParticleSystem>();
                if (crownAura != null)
                {
                    RefreshParticlesColor();
                    StopAnimations();

                    crownAura.transform.position = crownObject.transform.position;
                    crownAura.Play();

                    var pos = crownIcon.transform.position;
                    auraTweens.Add(crownAura.transform.DOMoveX(pos.x, auraTweenDuration));
                    auraTweens[^1].SetEase(Ease.InOutSine);
                    auraTweens.Add(crownAura.transform.DOMoveZ(pos.z, auraTweenDuration));
                    auraTweens[^1].SetEase(Ease.InOutSine);
                    auraTweens.Add(crownAura.transform.DOMoveY(pos.y + 5f, auraTweenDuration / 2f));
                    auraTweens[^1].SetEase(Ease.InSine);

                    Tweener tween = crownAura.transform.DOMoveY(pos.y, auraTweenDuration / 2f);
                    tween.SetEase(Ease.OutSine);
                    tween.SetDelay(auraTweenDuration / 2f);
                    tween.OnStart(ShowSpirit);
                    auraTweens.Add(tween);
                    auraTweens[0].OnComplete(ClearAuraTweens);
                }
                else
                {
                    effectObject.transform.DOMove(crownIcon.transform.position, 1.5f).SetEase(Ease.OutBack).OnComplete(() =>
                    {
                        Destroy(effectObject);
                    });
                }
            }
            player.CrownCount++;
        }

        public void Revival()
        {
            if (revived)
                return;
            if (!revived)
            {
                usedParticalDisappear = false;

                effectObject = Instantiate(crownEffect, crownIcon.transform.position, Quaternion.Euler(Vector3.zero));

                crownAura = effectObject.GetComponent<ParticleSystem>();
                if (crownAura != null)
                {
                    RefreshParticlesColor();

                    crownAura.Play();

                    float disappearDuration = 1f;
                    Vector3 endPos = effectObject.transform.position + Vector3.up * 3.5f;

                    effectObject.transform.DOMove(endPos, disappearDuration).SetEase(Ease.OutSine);

                    var emission = crownAura.emission;
                    float startRate = emission.rateOverTime.constant;
                    DOTween.To(() => startRate, x =>
                    {
                        var newEmission = crownAura.emission;
                        newEmission.rateOverTime = x;
                    }, 0f, disappearDuration).SetEase(Ease.OutSine);

                    DOVirtual.DelayedCall(disappearDuration, () =>
                    {
                        if (effectObject != null)
                            Destroy(effectObject);
                    });
                }
                else
                {
                    effectObject.transform.DOMove(effectObject.transform.position + Vector3.up * 3.5f, 1f).SetEase(Ease.OutSine).OnComplete(() =>
                    {
                        Destroy(effectObject);
                    });
                }

                iconRenderer.DOFade(0f, 1f);
                revived = true;
            }
        }
    }
}