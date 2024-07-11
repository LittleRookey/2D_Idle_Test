using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Litkey.AI
{
    [System.Serializable]
    public class StateMachine {
        
        [ShowInInspector] public StateNode current { get; private set; }
        Dictionary<Type, StateNode> nodes = new();
        HashSet<ITransition> anyTransitions = new();

        float stateEnteredTime;

        public void Update() {
            var transition = GetTransition();
            if (transition != null)
            {
                //Debug.Log($"{Time.time} - {stateEnteredTime} >= {transition.ExitTime} === {Time.time - stateEnteredTime >= transition.ExitTime}");
                if (!transition.HasExitTime || (Time.time - stateEnteredTime >= transition.ExitTime))
                {
                    //Debug.Log($"CHanged State to {transition.To.ToString()}");
                    ChangeState(transition.To);
                }

            }

            current.State?.Update();
        }
        
        public void FixedUpdate() {
            current.State?.FixedUpdate();
        }
        StateNode baseStateNode;
        public void SetState(IState state) {
            current = nodes[state.GetType()];
            baseStateNode = current;
            current.State?.OnEnter();
            stateEnteredTime = Time.time;
        }

        public void SetToBaseState()
        {
            if (baseStateNode == null) Debug.LogError("There is no base State yet");

            SetState(baseStateNode.State);
        }

        void ChangeState(IState state) {
            if (state == current.State) return;
            
            var previousState = current.State;
            var nextState = nodes[state.GetType()].State;
            //Debug.Log("Enemy entered state: " + nextState.ToString());
            previousState?.OnExit();
            nextState?.OnEnter();
            current = nodes[state.GetType()];
            stateEnteredTime = Time.time;
        }

        ITransition GetTransition() {
            foreach (var transition in anyTransitions)
                if (transition.Condition.Evaluate())
                    return transition;
            
            foreach (var transition in current.Transitions)
                if (transition.Condition.Evaluate())
                    return transition;
            
            return null;
        }

        public void AddTransition(IState from, IState to, IPredicate condition, bool hasExitTime = false, float exitTime = 0.0f)
        {
            GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition, hasExitTime, exitTime);
        }

        public void AddAnyTransition(IState to, IPredicate condition, bool hasExitTime = false, float exitTime = 0.0f)
        {
            anyTransitions.Add(new Transition(GetOrAddNode(to).State, condition, hasExitTime, exitTime));
        }

        StateNode GetOrAddNode(IState state) {
            var node = nodes.GetValueOrDefault(state.GetType());
            
            if (node == null) {
                node = new StateNode(state);
                nodes.Add(state.GetType(), node);
            }
            
            return node;
        }

        [System.Serializable]
        public class StateNode {

            [DrawWithUnity]
            public IState State { get; }
            public HashSet<ITransition> Transitions { get; }
            
            public StateNode(IState state) {
                State = state;
                Transitions = new HashSet<ITransition>();
            }
            
            public void AddTransition(IState to, IPredicate condition, bool hasExitTime = false, float exitTime = 0.0f) {
                Transitions.Add(new Transition(to, condition, hasExitTime, exitTime));
            }
        }
    }
}