
using Litkey.Projectile;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Litkey.Skill
{
    [CreateAssetMenu(menuName ="Litkey/Skill/Kenki")]
    public class KenKi : ActiveSkill
    {
        [SerializeField] private ProjectileBehavior projectilePrefab;

        public override void ApplyEffect(StatContainer allyStat, StatContainer target)
        {
            throw new System.NotImplementedException();
        }

        public override void SetInitialState()
        {
            throw new System.NotImplementedException();
        }

        public override void Use(StatContainer ally, Health target)
        {
            if (!string.IsNullOrEmpty(animationName))
            {
                ally.transform.GetChild(0).GetComponent<Animator>().Play(animationName);
            }

            Vector2 userPosition = ally.transform.position;
            Vector2 targetPosition = target.transform.position;
            float angle = Mathf.Atan2(targetPosition.y - userPosition.y, targetPosition.x - userPosition.x) * Mathf.Rad2Deg;

            // Create skillRange and pass ExecuteAttack as a lambda that calls our local function
            RangeSkillArea skillRange = SkillRangeCreator.CreateSquareSkillRange(
                ally.transform.position,
                10f,
                1f,
                TimeTilActive
            );
           
            skillRange
                //.SetAttackNumber(attackNumber)
                //.SetTargetNumber(targetNumber)
                //.SetDamagePercent(finalDamage / 100f)
                //.SetSkillRadius(skillRadius)
                .SetEnemyLayer(enemyLayer)
                .SetAlly(ally)
                .FaceDirection((target.transform.position - ally.transform.position).normalized)
                .AddListenerOnEnd(() => {
                    var projectile = ProjectileCreator.CreateProjectile("°Ë±â", ally.transform.position, ally, enemyLayer);

                    projectile.SetDamagePercent(finalDamage / 100f)
                        .SetDirection((target.transform.position - ally.transform.position).normalized)
                        .SetTargetCount(10)
                        .SetSpeed(5)
                        .SetStrategy(EnemyAI.eProjectileStrategy.Straight, 5, target.transform.position, null)
                        .AddDecorator(new DamageDecorator()
                            .ShowDamagePopup(false)
                            .SetDamageCount(1))
                        .SetDisappearTimer(5)
                        .SetEndOfLifeStrategy(new DefaultEndOfLifeStrategy());
                })
                .StartRange();
        }
    }
}