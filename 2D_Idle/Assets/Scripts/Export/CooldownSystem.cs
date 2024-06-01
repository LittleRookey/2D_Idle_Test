using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Litkey.Character.Cooldowns
{
    public interface IHasCooldown
    {
        string ID { get; }
        float CooldownDuration { get; }
    }

    public class CooldownSystem : MonoBehaviour
    {
        [ShowInInspector]
        public List<CooldownData> cooldowns = new List<CooldownData>();

        private void Update() => ProcessCooldowns();

        public void PutOnColdown(IHasCooldown cooldown)
        {
            cooldowns.Add(new CooldownData(cooldown));
            Debug.Log("Added to Cooldown");
        }

        // checks if the given id is in process of cooldown
        public bool IsOnCooldown(string id)
        {
            foreach (CooldownData cooldown in cooldowns)
            {
                if (cooldown.ID == id) { return true; }
            }
            return false;
        }

        public float GetRemainingDuration(string id)
        {
            foreach (CooldownData cooldown in cooldowns)
            {
                if (cooldown.ID != id) { continue; }

                return cooldown.RemainingTime;
            }
            return 0f;
        }

        private void ProcessCooldowns()
        {
            float deltaTime = Time.deltaTime; // time of last frame
            // remove cooldown have been finished. 
            // loop backwards. 
            // TODO need change with datastructure
            for (int i = cooldowns.Count - 1; i >= 0; i--)
            {
                if (cooldowns[i].DecrementCooldown(deltaTime))
                {
                    cooldowns.RemoveAt(i);
                    Debug.Log("Cooldown removed");
                }
            }

        }
    }

    [System.Serializable]
    public class CooldownData
    {
        public string ID { get; }
        public float RemainingTime { get; private set; }
        public CooldownData(IHasCooldown cooldown)
        {
            ID = cooldown.ID;
            RemainingTime = cooldown.CooldownDuration;
        }
        // get it from anywhere, but set it only in this class

        public bool DecrementCooldown(float deltaTime)
        {
            RemainingTime = Mathf.Max(RemainingTime - deltaTime, 0f);
            return RemainingTime == 0;
        }
    }
}