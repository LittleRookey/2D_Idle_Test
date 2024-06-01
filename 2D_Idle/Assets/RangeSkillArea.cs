using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using Redcode.Pools;

namespace Litkey.Skill
{
    public enum eSkillRangeType
    {
        circle,
        arc,
        square
    }

    public class RangeSkillArea : MonoBehaviour, IPoolObject
    {
        [SerializeField] private DOTweenAnimation dotweenAnim;
        [SerializeField] private eSkillRangeType skillRangeType;
        private float sizeMultiplier = 1f;

        private readonly string dotAnimParam = "Start";

        public bool destroyOnEnd = false;

        private Tween CurerntTween;

        public void SetRange(float duration, float sizeMultiplier, UnityAction OnEnd)
        {
            this.sizeMultiplier = sizeMultiplier;
            gameObject.transform.localScale = Vector3.one * sizeMultiplier;

            dotweenAnim.duration = duration;
            if (dotweenAnim != null && dotweenAnim.onComplete != null)
            {
                Debug.Log("Events successfully added?");
                if (OnEnd != null)
                    dotweenAnim.onComplete.AddListener(() => OnEnd());
                if (destroyOnEnd)
                    dotweenAnim.onComplete.AddListener(DestroyIt);
            }
        }

        public void SetRange(float duration, float width, float height, UnityAction OnEnd)
        {
            gameObject.transform.localScale = new Vector3(width, height, 1f);
            var baseDuration = dotweenAnim.tween.Duration();

            // timescale = 1 * 0.25;
            dotweenAnim.tween.timeScale = baseDuration / duration;
            //dotweenAnim.duration
            if (dotweenAnim != null && dotweenAnim.onComplete != null)
            {
                Debug.Log("Events successfully added?");
                if (OnEnd != null)
                    dotweenAnim.onComplete.AddListener(() => OnEnd());
                if (destroyOnEnd)
                    dotweenAnim.onComplete.AddListener(DestroyIt);
            }
        }



        [Button("StartRange")]
        public void StartRange()
        {
            
            dotweenAnim.DORestartById(dotAnimParam);
        }

        public void OnCreatedInPool()
        {
            
        }

        public void OnGettingFromPool()
        {
            if (dotweenAnim != null && dotweenAnim.onComplete != null)
                dotweenAnim.onComplete.RemoveAllListeners();
        }

        public void DestroyIt()
        {
            Destroy(gameObject);
        }
    }
}
