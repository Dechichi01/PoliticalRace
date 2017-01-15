using UnityEngine;
using System.Collections;

public class SideEnviroment : Module {

    [Range(0,1)]
    public float minDistFactor = 0.2f, maxDistFactor = 0.35f;

    Connection exit;

    public void Initialize()
    {
        if (bc == null) bc = GetComponent<BoxCollider>();
        if (exit == null)
        {
            exit = new GameObject("Exit").AddComponent<Connection>();
            exit.isDefaultConnection = true;
            exit.transform.position = new Vector3(transform.position.x, bc.bounds.min.y, transform.position.z);
            exit.transform.forward = -transform.forward;
            exit.transform.parent = transform;
        }
    }



}
