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

        public EnemyWanderState(EnemyAI enemy, Animator animator, AIPath aiPath, float wanderRadius, float waitTime, string stateName) : base(enemy, animator, stateName)
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

            OnIdle();

            //var newWanderPos = enemy.SetWanderPoint(wanderRadius);
            //enemy.SetMovePosition(newWanderPos);
            //SetRun();
            idleTimer.Reset();
            idleTimer.Start();
        }

        public override void Update()
        {
            if (enemy.attackOnSearched)
                enemy.SearchForTarget();
            if (idleTimer.IsRunning)
                idleTimer.Tick(Time.deltaTime);

            if (!idleTimer.IsRunning)
                enemy.TurnBasedOnDirection(aiPath.desiredVelocity);

            //enemy.ChaseEnemy();

            if (HasReachedDestination() && !idleTimer.IsRunning)
            {
                idleTimer.Start();
            }
        }

        void OnIdle()
        {
            animator.CrossFade(IdleHash, crossFadeDuration);
            enemy.StopMovement();
            enemy.DisableAIPath();
        }

        void SetRun()
        {
            enemy.StartMovement();
            enemy.EnableAIPath();
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
        public override void OnExit()
        {
            idleTimer.Pause();
        }

    }
}

