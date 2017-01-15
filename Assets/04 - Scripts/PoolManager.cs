using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoolManager : MonoBehaviour
{
    public Dictionary<int, Queue<PoolObject>> poolDictionary = new Dictionary<int, Queue<PoolObject>>();

    static PoolManager _instance;
    public static PoolManager instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<PoolManager>();
            return _instance;
        }
    }

    //Instantiate all objects from prefab and crestes poolHolder to hold them
    public void CreatePool(PoolObject prefab, int poolSize)
    {
        int poolKey = prefab.gameObject.GetInstanceID();
        GameObject poolHolder = new GameObject(prefab.name + " Pool");
        poolHolder.transform.parent = transform;
        if (!poolDictionary.ContainsKey(poolKey))
        {
            poolDictionary.Add(poolKey, new Queue<PoolObject>());
            IncrementPool(prefab, poolKey, poolHolder.transform, poolSize);
        }
    }

    void IncrementPool(PoolObject prefab, int poolKey, Transform poolHolder, int poolSize)
    {
        for (int i = 0; i < poolSize; i++)
        {
            PoolObject newObject = Instantiate(prefab);
            poolDictionary[poolKey].Enqueue(newObject);
            newObject.transform.parent = poolHolder;
            newObject.poolHolder = poolHolder;
            newObject.poolKey = poolKey;
            newObject.gameObject.SetActive(false);
        }
    }

    public GameObject ReuseObject(PoolObject prefab)
    {
        return ReuseObject(prefab, Vector3.zero, Quaternion.identity);
    }

    public GameObject ReuseObject(PoolObject prefab, Vector3 position, Quaternion rotation)
    {
        int poolKey = prefab.gameObject.GetInstanceID();

        if (poolDictionary.ContainsKey(poolKey))
        {
            if (poolDictionary[poolKey].Count == 0)//If there is no object in the pool, instantiate more
                IncrementPool(prefab, poolKey, transform.FindChild(prefab.name + " Pool"), 2);

            PoolObject objectToReuse = poolDictionary[poolKey].Dequeue();
            objectToReuse.Reuse(position, rotation);
            return objectToReuse.gameObject;
        }
        else//If the object has no pool create one and call method again
        {
            CreatePool(prefab, 5);
            return ReuseObject(prefab, position, rotation);
        }
    }
}
