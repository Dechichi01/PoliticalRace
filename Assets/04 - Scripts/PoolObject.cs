using UnityEngine;
using System.Collections;

public class PoolObject : MonoBehaviour
{
    [System.NonSerialized]
    public Transform poolHolder;
    [System.NonSerialized]
    public int poolKey;

    public GameObject Instantiate(Vector3 position, Quaternion rotation)
    {
        return PoolManager.instance.ReuseObject(this, position, rotation);
    }
    public GameObject Instantiate()
    {
        return PoolManager.instance.ReuseObject(this);
    }

    public virtual void Reuse(Vector3 position, Quaternion rotation)
    {
        transform.parent = null;
        transform.position = position;
        transform.rotation = rotation;
        gameObject.SetActive(true);
    }

    public virtual void Destroy()
    {
        gameObject.SetActive(false);
        PoolManager.instance.poolDictionary[poolKey].Enqueue(this);
        transform.parent = poolHolder;
    }
}
