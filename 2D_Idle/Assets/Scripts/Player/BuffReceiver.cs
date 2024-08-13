using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Litkey.Stat
{
    public class BuffReceiver : MonoBehaviour
    {
        StatContainer statContainer;


        private void Awake()
        {
            statContainer = GetComponent<StatContainer>();
        }

        [Button("Buff")]
        public void GiveBuff(Buff buff)
        {
            statContainer.ApplyBuff(buff);
        }
    }
}
