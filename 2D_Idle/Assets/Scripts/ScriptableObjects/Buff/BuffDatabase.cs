using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Litkey.Stat;

[CreateAssetMenu(menuName ="Litkey/Buff/BuffDatabase")]
public class BuffDatabase : SerializedScriptableObject
{
    public Dictionary<int, Buff> buffDatabase;

    public Buff GetBuff(int buffID)
    {
        if (!buffDatabase.ContainsKey(buffID)) return null;
        return buffDatabase[buffID];
    }
}
