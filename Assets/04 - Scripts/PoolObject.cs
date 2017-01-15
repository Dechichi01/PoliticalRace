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

    public virtual void Destroy(float delay = 0)
    {
        if (delay != 0)
            StartCoroutine(DestroyWithDelay(delay));
        else
        {
            gameObject.SetActive(false);
            PoolManager.instance.poolDictionary[poolKey].Enqueue(this);
            transform.parent = poolHolder;
        }
    }

    IEnumerator DestroyWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy();
    }
}
