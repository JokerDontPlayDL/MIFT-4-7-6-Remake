using System.Collections.Generic;
using UnityEngine;
using DancingLineFanmade.Level;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace DancingLineFanmade.Trigger
{
    [DisallowMultipleComponent]
    public class CrownCheckpoint : Checkpoint
    {
        [Title("Crown Settings(这三个别乱动）")]
        public GameObject crownObject;
        public GameObject crownEffect;
        public GameObject crownIcon;

        private MeshRenderer crownRenderer;
        private SpriteRenderer iconRenderer;
        private GameObject effectObject;
        private Color iconColor;
        private bool revived;

        private ParticleSystem crownAura;
        private readonly List<Tween> auraTweens = new List<Tween>();
        private Tween crownTween;
        private const float AuraTweenDuration = 1.25f;
        private bool taken;
        private bool usedParticleDisappear;
        
        new void Start()
        {
            base.Start();
            
            crownRenderer = crownObject.GetComponent<MeshRenderer>();
            iconRenderer = crownIcon.transform.GetChild(0).GetComponent<SpriteRenderer>();

            iconRenderer.color = new Color(iconRenderer.color.r, iconRenderer.color.g, iconRenderer.color.b, 0f);

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

        private void AnimateCrown(bool show)
        {
            crownTween = iconRenderer.DOFade(show ? 1 : 0, AuraTweenDuration / 4f);
            crownTween.SetEase(Ease.OutSine).SetDelay(0.2f);
            crownTween.OnComplete(() =>
            {
                crownTween.Kill();
                crownTween = null;
            });

            if (!show && !usedParticleDisappear && crownAura != null)
            {
                crownAura.GetComponentsInChildren<ParticleSystem>();
                crownAura.Play();

                usedParticleDisappear = true;

                crownAura.transform.DOMoveY(crownAura.transform.position.y + 10f, AuraTweenDuration / 1f).SetEase(Ease.Linear);
            }
        }

        public new void EnterTrigger()
        {
            base.EnterTrigger();
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
                    auraTweens.Add(crownAura.transform.DOMoveX(pos.x, AuraTweenDuration));
                    auraTweens[^1].SetEase(Ease.InOutSine);
                    auraTweens.Add(crownAura.transform.DOMoveZ(pos.z, AuraTweenDuration));
                    auraTweens[^1].SetEase(Ease.InOutSine);
                    auraTweens.Add(crownAura.transform.DOMoveY(pos.y + 5f, AuraTweenDuration / 2f));
                    auraTweens[^1].SetEase(Ease.InSine).OnComplete(ShowSpirit);

                    Tweener tween = crownAura.transform.DOMoveY(pos.y, AuraTweenDuration / 2f);
                    tween.SetEase(Ease.OutSine);
                    tween.SetDelay(AuraTweenDuration / 2f);
                    // tween.OnComplete(ShowSpirit);
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

            Player.Instance.CrownCount++;
        }

        internal override void Revival()
        {
            Debug.Log("皇冠");
            base.Revival();
            if (revived)
                return;
            if (!revived)
            {
                usedParticleDisappear = false;

                effectObject = Instantiate(crownEffect, crownIcon.transform.position, Quaternion.Euler(Vector3.zero));

                crownAura = effectObject.GetComponent<ParticleSystem>();
                if (crownAura != null)
                {
                    RefreshParticlesColor();

                    crownAura.Play();

                    float disappearDuration = 1f;
                    Vector3 endPos = effectObject.transform.position + Vector3.up * 5f;

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