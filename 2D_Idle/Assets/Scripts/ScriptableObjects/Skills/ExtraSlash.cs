using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;
using Litkey.Stat;
using Litkey.Skill;

[CreateAssetMenu(menuName ="Litkey/Skills/ExtraSlash")]
public class ExtraSlash : ActiveSkill
{
    public float skillRadius;
    
    public override void ApplyEffect(StatContainer allyStat, StatContainer target)
    {
        
    }

    public override void SetInitialState()
    {
        throw new System.NotImplementedException();
    }

    public override void Use(StatContainer ally, Health target)
    {
        if (!animationName.Equals(string.Empty))
        {
            ally.transform.GetChild(0).GetComponent<Animator>().Play(animationName);
        }
        Debug.Log("Used Skill Extraslash");
        // Get the position of the skill user
        Vector2 userPosition = ally.transform.position;

        // Create a list to store the enemies we'll damage
        List<Health> targetEnemies = new List<Health>();
        if (skillRangePool == null)
        {
            skillRangePool = Pool.Create<RangeSkillArea>(skillRange).NonLazy();
        }

        var range = skillRangePool.Get();
;
        range.transform.position = ally.transform.position;
        float angle = Mathf.Atan2(target.transform.position.y - userPosition.y, target.transform.position.x - userPosition.x) * Mathf.Rad2Deg;

        // Rotate the skillRange collider to face the enemy
        range.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        Collider2D skillCollider = range.transform.GetComponent<Collider2D>();

        if (skillCollider is BoxCollider2D b2D)
        {
            range.SetRange(TimeTilActive, 1.5f, 1f, SearchAttack);
        } else if (skillCollider is CircleCollider2D c2D)
        {
            range.SetRange(TimeTilActive, 1f, SearchAttack);
        } else if (skillCollider is PolygonCollider2D p2D)
        {
            range.SetRange(TimeTilActive, 1f, SearchAttack);
        }

        range.gameObject.SetActive(true);
        range.StartRange();

        void SearchAttack()
        {
            // Get all colliders within a circle area around the skill user
            Collider2D[] overlappedColliders = Physics2D.OverlapCircleAll(userPosition, skillRadius, enemyLayer);

            foreach (Collider2D enemyCollider in overlappedColliders)
            {
                // Check if the enemy collider overlaps with the skillRange collider
                if (skillCollider.OverlapPoint(enemyCollider.transform.position))
                {
                    Health enemyHealth = enemyCollider.GetComponent<Health>();
                    if (enemyHealth != null)
                    {
                        targetEnemies.Add(enemyHealth);
                    }
                }
            }

            // Sort the list of enemies based on their distance from the skill user
            targetEnemies.Sort((a, b) => Vector2.Distance(a.transform.position, ally.transform.position)
                .CompareTo(Vector2.Distance(b.transform.position, ally.transform.position)));


            // Get the position of the enemy
            if (targetEnemies.Count > 0)
            {
                Vector2 enemyPosition = targetEnemies[0].transform.position;

                for (int i = 0; i < 2; i++)
                {
                    var skillFX = Instantiate(skillEffect);
                    skillFX.transform.position = ally.transform.position;
                    if (i == 0)
                        skillFX.transform.rotation = Quaternion.AngleAxis(angle-10f, Vector3.forward);
                    if (i == 1)
                        skillFX.transform.rotation = Quaternion.AngleAxis(angle + 10f, Vector3.forward);
                }
            }
            // Apply damage to the first skillTargetNumber of enemies in the sorted list
            //Debug.Log("ExtraSLash: " + Mathf.Min(targetNumber, targetEnemies.Count));
            
            for (int i = 0; i < Mathf.Min(targetNumber, targetEnemies.Count); i++)
            {
                //ApplyEffect(ally, targetEnemies[i]);

                var dmgs = ally.GetDamagesAgainst(targetEnemies[i].GetComponent<StatContainer>(), attackNumber, finalDamage / 100f);

                if (targetEnemies[i].TakeDamage(ally, dmgs, true, true))
                {

                }
            }

            OnSkillUse?.Invoke();

            skillRangePool.Take(range);
        }
        
    }




}
