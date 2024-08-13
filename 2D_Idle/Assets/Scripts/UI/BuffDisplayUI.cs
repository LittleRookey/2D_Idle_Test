using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;
using Litkey.Stat;

public class BuffDisplayUI : MonoBehaviour
{
    [SerializeField] private BuffDebuffSlotUI buffDebuffPrefab;
    [SerializeField] private RectTransform buffDebuffParent;
    [SerializeField] private StatContainer playerStat;
    Pool<BuffDebuffSlotUI> buffDebuffSlotPool;
    Dictionary<int, BuffDebuffSlotUI> activeSlots;

    private void Awake()
    {
        buffDebuffSlotPool = Pool.Create<BuffDebuffSlotUI>(buffDebuffPrefab);
        buffDebuffSlotPool.SetContainer(buffDebuffParent);
        activeSlots = new Dictionary<int, BuffDebuffSlotUI>();
        playerStat.OnAddBuff.AddListener(OnAddBuff);
        playerStat.OnUpdateBuff.AddListener(OnUpdateBuff);
        playerStat.OnRemoveBuff.AddListener(OnRemoveBuff);
    }

    private void OnAddBuff(BuffInfo buff)
    {
        if (!activeSlots.ContainsKey(buff.buff.BuffID))
        {
            var slot = buffDebuffSlotPool.Get();
            slot.SetSlot(buff, playerStat);
            activeSlots.Add(buff.buff.BuffID, slot);
        }
    }

    private void OnUpdateBuff(BuffInfo buff)
    {
        if (activeSlots.TryGetValue(buff.buff.BuffID, out BuffDebuffSlotUI slot))
        {
            slot.UpdateCount(buff);
        }
    }

    private void OnRemoveBuff(BuffInfo buff)
    {
        if (activeSlots.TryGetValue(buff.buff.BuffID, out BuffDebuffSlotUI slot))
        {
            buffDebuffSlotPool.Take(slot);
            activeSlots.Remove(buff.buff.BuffID);
        }
    }
    public void ClearSlots()
    {
        for (int i = 0; i < activeSlots.Count; i++)
        {
            // Clear each slot
            buffDebuffSlotPool.Take(activeSlots[i]);
        }

        activeSlots.Clear();
    }

}
