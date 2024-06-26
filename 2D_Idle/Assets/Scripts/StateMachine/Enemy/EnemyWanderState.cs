using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Litkey.AI
{
    public class EnemyWanderState : EnemyBaseState
    {
        AIPath aiPath;
        readonly float wanderRadius;
        readonly float waitTime;
        CountdownTimer idleTimer;

        public EnemyWanderState(EnemyAI enemy, Animator animator, AIPath aiPath, float wanderRadius, float waitTime) : base(enemy, animator)
        {
            this.aiPath = aiPath;
            this.wanderRadius = wanderRadius;
            this.waitTime = waitTime;
            idleTimer = new CountdownTimer(this.waitTime);
            idleTimer.OnTimerStop += MoveToNewWanderPoint;
            idleTimer.OnTimerStart += OnIdle;
        }

        public override void OnEnter()
        {
            Debug.Log("Entered WanderState");
            SetRun();

            var newWanderPos = enemy.SetWanderPoint(wanderRadius);
            enemy.SetMovePosition(newWanderPos);
            idleTimer.Reset();
        }

        public override void Update()
        {
            enemy.SearchForTarget();
            if (idleTimer.IsRunning)
                idleTimer.Tick(Time.deltaTime);

            if (!idleTimer.IsRunning)
                enemy.TurnBasedOnDirection(aiPath.desiredVelocity);

            if (HasReachedDestination() && !idleTimer.IsRunning)
            {
                idleTimer.Start();
            }
        }

        void OnIdle()
        {
            animator.CrossFade(IdleHash, crossFadeDuration);

        }

        void SetRun()
        {
            animator.CrossFade(RunHash, crossFadeDuration);
        }

        void MoveToNewWanderPoint()
        {
            SetRun();
            var newWanderPos = enemy.SetWanderPoint(wanderRadius);
            enemy.SetMovePosition(newWanderPos);
        }

        bool HasReachedDestination()
        {
            return aiPath.reachedDestination;
        }

    }
}

