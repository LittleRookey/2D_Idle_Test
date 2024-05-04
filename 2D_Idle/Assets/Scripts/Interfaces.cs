using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

}
