using UnityEngine;

namespace Litkey.AI
{
    [System.Serializable]
    public class ChaseStrategy : BehaviorStrategy
    {
        public override void Execute(EnemyAI enemy)
        {

        }
    }

    [System.Serializable]
    public class SimpleChaseStrategy : ChaseStrategy
    {
        public override void Execute(EnemyAI enemy)
        {
            enemy.ChaseEnemy();
        }
    }
    [System.Serializable]
    public class WanderAroundTargetStrategy : ChaseStrategy
    {
        private float minDistance = 1f;
        private float maxDistance = 2f;

        public WanderAroundTargetStrategy() { }

        public WanderAroundTargetStrategy(float minDist, float maxDist)
        {
            this.minDistance = minDist;
            this.maxDistance = maxDist;
        }
        public override void Execute(EnemyAI enemy)
        {
            Vector3 targetPosition = enemy.GetTarget().transform.position;
            Vector3 enemyPosition = enemy.transform.position;
            float distanceToTarget = Vector3.Distance(enemyPosition, targetPosition);

            if (distanceToTarget < minDistance)
            {
                // Move away from target
                Vector3 directionAway = (enemyPosition - targetPosition).normalized;
                enemy.SetMovePosition(enemyPosition + directionAway * (minDistance - distanceToTarget));
            }
            else if (distanceToTarget > maxDistance)
            {
                // Move towards target
                enemy.ChaseEnemy();
            }
            else
            {
                // Wander around
                Vector3 randomOffset = Random.insideUnitSphere * 2f;
                randomOffset.z = 0;
                enemy.SetMovePosition(enemyPosition + randomOffset);
            }
        }
    }
}

