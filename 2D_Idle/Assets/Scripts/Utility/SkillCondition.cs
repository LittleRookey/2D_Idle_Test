using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class SkillCondition
{
    public string Name;
    public int Score;

    public abstract bool EvaluateCondition(StatContainer ally);
}

public class MultipleTargetsCondition : SkillCondition
{
    public int MinTargetsInRange = 4;
    public float TargetRange = 1f;

    public override bool EvaluateCondition(StatContainer ally)
    {
        int targetsInRange = 0;
        var targets = Physics2D.CircleCastAll(ally.transform.position, TargetRange, Vector2.zero);
        foreach (var target in targets)
        {
            if (Vector3.Distance(ally.transform.position, target.transform.position) <= TargetRange)
            {
                targetsInRange++;
            }
        }
        return targetsInRange >= MinTargetsInRange;
    }
}

public class TargetExists : SkillCondition
{

    public override bool EvaluateCondition(StatContainer ally)
    {
        var targets = Physics2D.CircleCastAll(ally.transform.position, 1, Vector2.zero);
        return targets.Length > 0;
    }
}
