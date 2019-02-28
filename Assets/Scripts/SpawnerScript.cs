using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class SpawnerScript : MonoBehaviour {
    public GameObject localPlayer;
    public GameObject playerPrefab;
    public SocketIOComponent socket;

    Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();


    public GameObject SpawnPlayer(string id)
    {
        var player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity) as GameObject;

        player.GetComponent<NetworkEntity>().id = id.Replace("\"", "");
        AddPlayer(id, player);
        return player;
    }

    public void AddPlayer(string id, GameObject player)
    {
        players.Add(id, player);
    }

    public GameObject findPlayer(string id)
    {

        return players[id];
    }

    public void RemovePlayer(string id)
    {
        var player = players[id];
        Destroy(player);
        players.Remove(id);

    }

}
