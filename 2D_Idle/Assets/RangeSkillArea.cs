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
        [SerializeField] private SpriteRenderer outerRange;
        [SerializeField] private SpriteRenderer innerRange;

        public eSkillRangeType SkillRangeType => skillRangeType;

        private float sizeMultiplier = 1f;

        private readonly string dotAnimParam = "Start";

        public bool destroyOnEnd = false;

        private Tween CurerntTween;

        public Collider2D Collider { get; private set; }

        [Header("Damage Logics")]
        [SerializeField] private int attackNumber;
        [SerializeField] private int targetNumber;
        [SerializeField] private float damagePercent;
        [SerializeField] private float skillRadius;
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private StatContainer ally;

        private void Awake()
        {
            Collider = GetComponent<Collider2D>();
        }

        #region Setters DamageLogic
        public RangeSkillArea SetAttackNumber(int value)
        {
            attackNumber = value;
            return this;
        }

        public RangeSkillArea SetTargetNumber(int value)
        {
            targetNumber = value;
            return this;
        }

        public RangeSkillArea SetDamagePercent(float value)
        {
            damagePercent = value;
            return this;
        }

        public RangeSkillArea SetSkillRadius(float value)
        {
            skillRadius = value;
            return this;
        }

        public RangeSkillArea SetEnemyLayer(LayerMask value)
        {
            enemyLayer = value;
            return this;
        }

        public RangeSkillArea SetAlly(StatContainer value)
        {
            ally = value;
            return this;
        }

        public void ExecuteAttack(bool showDmgText=false)
        {
            if (ally == null)
            {
                Debug.LogError("Ally not set for RangeSkillArea");
                return;
            }

            List<Health> targetEnemies = new List<Health>();
            Collider2D[] overlappedColliders = Physics2D.OverlapCircleAll(transform.position, skillRadius, enemyLayer);
            
            Debug.Log($"Overlapped Colliders with radius {skillRadius} and layer {enemyLayer} colliders count: {overlappedColliders.Length}");

            foreach (Collider2D enemyCollider in overlappedColliders)
            {
                if (Collider.OverlapPoint(enemyCollider.transform.position))
                {
                    Health enemyHealth = enemyCollider.GetComponent<Health>();
                    if (enemyHealth != null && !enemyHealth.IsDead)
                    {
                        targetEnemies.Add(enemyHealth);
                    }
                }
            }

            targetEnemies.Sort((a, b) => Vector2.Distance(a.transform.position, transform.position)
                .CompareTo(Vector2.Distance(b.transform.position, transform.position)));

            if (targetEnemies.Count > 0)
            {
                ApplyDamageToTargets(targetEnemies, showDmgText);
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, skillRadius);
        }

        private void ApplyDamageToTargets(List<Health> targetEnemies, bool showDmgText=false)
        {
            int targetsHit = Mathf.Min(targetNumber, targetEnemies.Count);
            for (int i = 0; i < targetsHit; i++)
            {
                Debug.Log("Applied damage to enemy");
                var dmgs = ally.GetDamagesAgainst(targetEnemies[i].GetComponent<StatContainer>(), attackNumber, damagePercent);
                targetEnemies[i].TakeDamage(ally, dmgs, showDmgText, true);
            }
        }

        #endregion

        public void SetRange(float duration, float sizeMultiplier, UnityAction OnEnd=null)
        {
            dotweenAnim.onComplete.RemoveAllListeners();
            this.sizeMultiplier = sizeMultiplier;
            gameObject.transform.localScale = Vector3.one * sizeMultiplier;

            dotweenAnim.duration = duration;

            if (OnEnd != null)
                dotweenAnim.onComplete.AddListener(() =>
                {
                    OnEnd?.Invoke();
                    SkillRangeCreator.ReturnSkillRange(this);
                });
            if (destroyOnEnd)
                dotweenAnim.onComplete.AddListener(DestroyIt);
            
        }

        public void SetRange(float duration, float width, float height, UnityAction OnEnd=null)
        {
            dotweenAnim.onComplete.RemoveAllListeners();
            gameObject.transform.localScale = new Vector3(width, height, 1f);
            var baseDuration = dotweenAnim.tween.Duration();
            StartCoroutine(SkillRangeCreator.ReturnSkillRange(this, duration));
            // timescale = 1 * 0.25;
            dotweenAnim.tween.timeScale = baseDuration / duration;

            if (OnEnd != null)
                dotweenAnim.onComplete.AddListener(() =>
                {
                    OnEnd?.Invoke();
                    SkillRangeCreator.ReturnSkillRange(this);
                });
            if (destroyOnEnd)
                dotweenAnim.onComplete.AddListener(DestroyIt);
        }



        public RangeSkillArea AddListenerOnEnd(UnityAction OnEnd)
        {
            dotweenAnim.onComplete.RemoveAllListeners();
            
            
            dotweenAnim.onComplete.AddListener(() =>
            {
                OnEnd?.Invoke();
                SkillRangeCreator.ReturnSkillRange(this);
            });
            if (destroyOnEnd)
                dotweenAnim.onComplete.AddListener(DestroyIt);

            return this;
        }
        public RangeSkillArea FaceDirection(Vector2 direction)
        {
            // Calculate the angle in degrees from the direction vector
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Create a quaternion representing the z-axis rotation
            transform.rotation = Quaternion.Euler(0, 0, angle);
            return this;
        }
        public RangeSkillArea FaceTarget(Transform self, Transform Target)
        {

            Vector3 direction = Target.position - self.position;

            // Calculate the angle in degrees from the direction vector
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Create a quaternion representing the z-axis rotation
            transform.rotation = Quaternion.Euler(0, 0, angle);
            return this;
        }

        public RangeSkillArea SetOuterColor(Color color)
        {
            outerRange.color = color;
            return this;
        }

        public RangeSkillArea SetInnerColor(Color color)
        {
            innerRange.color = color;
            return this;
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
