using Redcode.Pools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Litkey.Utility
{

    public static class WarningMessageInvoker 
    {
        private static readonly string warningMessagePath = "Prefabs/UI/WarningMessage";

        private static Pool<WarningMessage> warningMessagePool;

        private static readonly float returnTime = 1.5f;

        private static void CheckPoolExists()
        {
            if (warningMessagePool == null)
            {
                var msgPrefab = Resources.Load<WarningMessage>(warningMessagePath);

                warningMessagePool = Pool.Create<WarningMessage>(msgPrefab).NonLazy();
            }
        }

    }
}
