using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Module : MonoBehaviour {

    public string Tag;

    public List<Connection> GetConnections()
    {
        return new List<Connection>(GetComponentsInChildren<Connection>());
    }
    public List<Connection> GetObstacleConnections()
    {
        List<Connection> connections = new List<Connection>(GetComponentsInChildren<Connection>());
        connections.RemoveAll(c => !(c is ObstacleConnection));
        return connections;
    }

    public List<Connection> GetEntranceAndExit()
    {
        List<Connection> connections = new List<Connection>(GetComponentsInChildren<Connection>());
        connections.RemoveAll(c => (!(c is Entrance) && !(c is Exit)));
        return connections;
    }

    public void GenerateObstacles(MapGenerator mapGen)
    {
        List<Connection> obstacleConnections = GetObstacleConnections();

        foreach(Connection connection in obstacleConnections)
        {
            //Get a random tag of a object this connection can connect to
            Module newModule = mapGen.GenerateModule(connection);
            List<Connection> newModuleConnections = newModule.GetConnections();
            mapGen.MatchConnections(connection, newModuleConnections[0]);
            newModule.transform.parent = transform;
        }
    }
}
