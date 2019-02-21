using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System;

public class Network : MonoBehaviour {

    static SocketIOComponent socket;
    public GameObject playerPrefab;


    Dictionary<string, GameObject> players;
	// Use this for initialization
	void Start () {
        socket = GetComponent<SocketIOComponent>();
        socket.On("open", OnConnected);
        socket.On("talkback", OnTalkBack);
        socket.On("spawn", OnSpawn);
        socket.On("move", OnMove);

        players = new Dictionary<string, GameObject>();
    }

    private void OnMove(SocketIOEvent obj)
    {
        //Debug.Log("Player Moving" + obj.data);
        var id = obj.data["id"].ToString();
        //Debug.Log(id);

        var v = float.Parse(obj.data["v"].ToString().Replace("\"",""));
        var h = float.Parse(obj.data["h"].ToString().Replace("\"",""));

        players[id].GetComponent<PlayerMovementNetwork>().v = v;
        players[id].GetComponent<PlayerMovementNetwork>().h = h;
    }

    private void OnSpawn(SocketIOEvent obj)
    {
        Debug.Log("Player spawned" + obj.data);
        var player = Instantiate(playerPrefab);
        players.Add(obj.data["id"].ToString(), player);
        Debug.Log(players.Count);
    }

    private void OnTalkBack(SocketIOEvent obj)
    {
        Debug.Log("The Server says Hello Back");      
    }

    private void OnConnected(SocketIOEvent obj)
    {
        Debug.Log("Connected to Server");
        socket.Emit("sayhello");
    }

    public static void Move(float currentPosV, float currentPosH)
    {
        Debug.Log("Send Position to Server" + VectorToJson(currentPosV, currentPosH));
        socket.Emit("move", new JSONObject(VectorToJson(currentPosV, currentPosH)));
    }

    public static string VectorToJson(float dirV, float dirH)
    {

        return string.Format(@"{{""v"":""{0}"",""h"":""{1}""}}", dirV, dirH);
    }
}



