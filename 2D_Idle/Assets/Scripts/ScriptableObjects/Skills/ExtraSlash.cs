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
            1.5f,
            1f,
            TimeTilActive
        );

        skillRange
            .SetAttackNumber(attackNumber)
            .SetTargetNumber(targetNumber)
            .SetDamagePercent(finalDamage / 100f)
            .SetSkillRadius(skillRadius)
            .SetEnemyLayer(enemyLayer)
            .SetAlly(ally)
            .FaceTarget(ally.transform, target.transform)
            .AddListenerOnEnd(() => skillRange.ExecuteAttack(true))
            .StartRange();
    }

    private void SpawnSkillEffects(Vector2 position, float angle)
    {
        for (int i = 0; i < 2; i++)
        {
            var skillFX = Instantiate(skillEffect);
            skillFX.transform.position = position;
            skillFX.transform.rotation = Quaternion.AngleAxis(angle + (i == 0 ? -10f : 10f), Vector3.forward);
        }
    }
}
