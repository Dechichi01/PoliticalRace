using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoolManager : MonoBehaviour
{

    private Dictionary<int, Queue<PoolObject>> poolDictionary = new Dictionary<int, Queue<PoolObject>>();

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
            IncrementPool(prefab, poolKey, poolHolder, poolSize);
        }
    }

    void IncrementPool(PoolObject prefab, int poolKey, GameObject poolHolder, int poolSize)
    {
        Debug.Log("Beeing called");
        for (int i = 0; i < poolSize; i++)
        {
            PoolObject newObject = Instantiate(prefab);
            poolDictionary[poolKey].Enqueue(newObject);
            newObject.transform.parent = poolHolder.transform;
            newObject.poolHolder = poolHolder.transform;
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
            PoolObject objectToReuse = poolDictionary[poolKey].Dequeue();
            poolDictionary[poolKey].Enqueue(objectToReuse);

            objectToReuse.Reuse(position, rotation);
            return objectToReuse.gameObject;
        }
        else//If the object has no pool create one and call method again
        {
            CreatePool(prefab, 30);
            return ReuseObject(prefab, position, rotation);
        }
    }
}
