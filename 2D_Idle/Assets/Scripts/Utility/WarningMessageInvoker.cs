using Redcode.Pools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Litkey.Utility
{

    public class WarningMessageInvoker : MonoBehaviour
    {
        public static WarningMessageInvoker Instance;

        [SerializeField] private RectTransform msgPoolParent;
        private readonly string warningMessagePath = "Prefabs/UI/WarningMessage";

        private Pool<WarningMessage> warningMessagePool;

        private void Awake()
        {
            Instance = this;
            CheckPoolExists();
        }
        private void CheckPoolExists()
        {
            if (warningMessagePool == null)
            {
                var msgPrefab = Resources.Load<WarningMessage>(warningMessagePath);

                warningMessagePool = Pool.Create<WarningMessage>(msgPrefab).NonLazy();

                warningMessagePool.SetContainer(msgPoolParent);
            }
        }

        public void ShowMessage(string msg)
        {
            CheckPoolExists();

            var warning = warningMessagePool.Get();
            warning.SetMessage(msg, 1.0f, () => warningMessagePool.Take(warning));
        }

        public void ShowMessage(string msg, float messageRemainTime)
        {
            CheckPoolExists();

            var warning = warningMessagePool.Get();
            warning.SetMessage(msg, messageRemainTime, () => warningMessagePool.Take(warning));
        }

        public void ShowMessage(string msg, float messageRemainTime, UnityAction onCompleted)
        {
            CheckPoolExists();

            var warning = warningMessagePool.Get();
            warning.SetMessage(msg, messageRemainTime, () =>
            {
                onCompleted?.Invoke();
                warningMessagePool.Take(warning);
            });
        }
    }
}
