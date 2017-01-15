using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]
public class Module : PoolObject {

    public string Tag;
    public BoxCollider bc;

    public List<Connection> GetConnections()
    {
        return new List<Connection>(GetComponentsInChildren<Connection>());
    }
 
}
