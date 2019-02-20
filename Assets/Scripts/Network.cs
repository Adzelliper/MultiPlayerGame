using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System;

public class Network : MonoBehaviour {

    SocketIOComponent socket;
    public GameObject playerPrefab;

	// Use this for initialization
	void Start () {
        socket = GetComponent<SocketIOComponent>();
        socket.On("open", OnConnected);
        socket.On("talkback", OnTalkBack);
        socket.On("spawn", OnSpawn);
    }

    private void OnSpawn(SocketIOEvent obj)
    {
        Instantiate(playerPrefab);
        Debug.Log("Player spawned");
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
}
