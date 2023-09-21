using System;
using UnityEngine;

namespace AttributeRelatedScript
{
    public interface IItemEffectStrategy
    {
        void ApplyEffect(GameObject player);
    }
}