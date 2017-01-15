using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Module : PoolObject {

    public string Tag;
    public BoxCollider bc;

    public List<Connection> GetConnections()
    {
        return new List<Connection>(GetComponentsInChildren<Connection>());
    }
 
}
