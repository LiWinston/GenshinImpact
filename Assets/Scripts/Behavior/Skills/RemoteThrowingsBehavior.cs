using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

[System.Serializable]
public class RemoteThrowingsBehavior : MonoBehaviour, IPoolable
{
    public void SetPool(ObjectPool<GameObject> pool)
    {
        throw new NotImplementedException();
    }

    public void actionOnGet()
    {
        throw new NotImplementedException();
    }

    public void actionOnRelease()
    {
        throw new NotImplementedException();
    }
}