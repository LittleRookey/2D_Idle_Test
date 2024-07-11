using UnityEngine;


namespace Litkey.AI {
    public interface IState {
        public string StateName { get; }
        void OnEnter();
        void Update();
        void FixedUpdate();
        void OnExit();
    }
}
