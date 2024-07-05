using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Litkey.Interface
{
    public interface IParryable
    {

        public void OnParried();
    }

    public interface IRewardable<T>
    {
        public T GetReward();
    }

    public interface ILoadable
    {
        public void Load();
    }


    public interface ISavable
    {
        public void Save();
    }

    public interface IInteractactable
    {
        public void Interact(int interactTime, PlayerController player, UnityAction OnEnd=null);
    }

    // �������� ������ �ٰ������� ������ ����
    public interface ISelectable
    {
        public void Select();
    }

    public interface IDeselectable
    {
        public void Deselect();
    }
}
