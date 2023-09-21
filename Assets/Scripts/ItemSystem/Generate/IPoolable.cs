using UnityEngine;
using UnityEngine.Pool;

public interface IPoolable
{
    void SetPool(ObjectPool<GameObject> pool);
    void actionOnGet();
    void actionOnRelease();
}