using UnityEngine;

namespace Litkey.AI {
    public interface IState {
        void OnEnter();
        void Update();
        void FixedUpdate();
        void OnExit();
    }
}
