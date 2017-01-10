using UnityEngine;
using System.Collections;

public class PoolObject : MonoBehaviour
{
    [System.NonSerialized]
    public Transform poolHolder;

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
        Invoke("Inactivate", delay);
    }

    private void Inactivate()
    {
        transform.parent = poolHolder;
        gameObject.SetActive(false);
    }


}
