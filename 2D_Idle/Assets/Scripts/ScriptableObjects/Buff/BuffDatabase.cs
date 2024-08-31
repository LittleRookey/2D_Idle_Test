using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Litkey.Stat;
using System.Linq;

[CreateAssetMenu(menuName ="Litkey/Buff/BuffDatabase")]
public class BuffDatabase : SerializedScriptableObject
{
    public Dictionary<int, Buff> buffDatabase;

    public Buff GetBuff(int buffID)
    {
        if (!buffDatabase.ContainsKey(buffID)) return null;
        return buffDatabase[buffID];
    }

    [Button("AddBuff")]
    public void AddBuff(Buff buff)
    {
        if (buffDatabase.Values.Any(existingBuff => existingBuff.BuffName == buff.BuffName))
        {
            Debug.LogWarning($"A buff with the name '{buff.BuffName}' already exists in the database.");
            return;
        }
        if (!buffDatabase.ContainsKey(buff.BuffID))
        {
            buffDatabase.Add(buff.BuffID, buff);
        }
    }
}
