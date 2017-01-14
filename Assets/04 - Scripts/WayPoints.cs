using UnityEngine;
using System.Collections;

public class WayPoints : MonoBehaviour {

    public Transform[] wayPoints;

    protected void OnDrawGizmos()
    {
        foreach (Transform item in wayPoints)
        {
            var scale = 5.0f;

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(item.position, item.position + item.forward * scale);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(item.position, item.position - item.right * scale);
            Gizmos.DrawLine(item.position, item.position + item.right * scale);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(item.position, item.position + Vector3.up * scale);

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(item.position, 0.125f);
        }
    }
}
