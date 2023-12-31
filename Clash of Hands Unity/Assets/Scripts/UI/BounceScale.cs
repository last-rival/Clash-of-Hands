using ClashOfHands.Systems;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace ClashOfHands.UI
{
    public class BounceScale : MonoBehaviour
    {
        [SerializeField]
        private Vector2 _scale;

        [SerializeField]
        private GameUtils.TweenTimeEase _bounceIn = new GameUtils.TweenTimeEase
        {
            Time = 0.2f,
            Ease = Ease.InExpo
        };

        [SerializeField]
        private float _idleTime = 0.1f;

        [SerializeField]
        private GameUtils.TweenTimeEase _bounceOut = new GameUtils.TweenTimeEase
        {
            Time = 0.2f,
            Ease = Ease.OutExpo
        };

        public float Duration => _bounceIn.Time + _bounceOut.Time + _idleTime;

        public void Bounce(TweenCallback onBounceComplete)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(transform.DOScale(Vector3.one * _scale.y, _bounceIn.Time).SetEase(_bounceIn.Ease));
            sequence.AppendInterval(_idleTime);
            sequence.Append(transform.DOScale(Vector3.one * _scale.x, _bounceOut.Time).SetEase(_bounceOut.Ease));
            if (onBounceComplete != null)
                sequence.onComplete = onBounceComplete;
            sequence.Play();
        }

        public float BounceIn()
        {
            transform.localScale = Vector3.one * _scale.x;
            transform.DOScale(Vector3.one * _scale.y, _bounceIn.Time).SetEase(_bounceIn.Ease);
            return _bounceIn.Time;
        }

        public float BounceOut()
        {
            transform.localScale = Vector3.one * _scale.y;
            transform.DOScale(Vector3.one * _scale.x, _bounceOut.Time).SetEase(_bounceOut.Ease);
            return _bounceOut.Time;
        }

#if UNITY_EDITOR

        [Button]
        public void DemoBounce()
        {
            Bounce(() => { print("Full Bounce Done."); });
        }

        [Button]
        public void DemoBounceIn()
        {
            BounceIn();
        }


        [Button]
        public void DemoBounceOut()
        {
            BounceOut();
        }
#endif
    }
}