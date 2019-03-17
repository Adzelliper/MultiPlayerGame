using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System;

public class Network : MonoBehaviour {

    static SocketIOComponent socket;
    public GameObject playerPrefab;
    public SpawnerScript spawner;
    public GameObject spawnx1;
    public GameObject spawnx2;
    public GameObject spawny1;
    public GameObject spawny2;

    public int coinNum = 20;
    public GameObject coin;



    // Use this for initialization
    void Start () {
        socket = GetComponent<SocketIOComponent>();
        socket.On("open", OnConnected);
        socket.On("talkback", OnTalkBack);
        socket.On("spawn", OnSpawn);
        socket.On("move", OnMove);
        socket.On("disconnected", OnDisconnected);
        socket.On("register", OnRegister);
        socket.On("updatePosition", OnUpdatePosition);
        socket.On("requestPosition", OnRequestPosition);
        socket.On("spawnCoin", OnSpawnCoin);
    }

    private void OnSpawnCoin(SocketIOEvent obj)
    {
        for (int i = 0; i < coinNum; i++)
        {
            i++;
            Instantiate(coin,
                new Vector3(UnityEngine.Random.Range(spawnx1.transform.position.x, spawnx2.transform.position.x),
                UnityEngine.Random.Range(spawny1.transform.position.y, spawny2.transform.position.y), 0), Quaternion.identity);
        }
    }

    private void OnRequestPosition(SocketIOEvent obj)
    {
        socket.Emit("updatePosition", PosToJson(spawner.localPlayer.transform.position, spawner.localPlayer.transform.rotation.eulerAngles.z));
    }

    private void OnUpdatePosition(SocketIOEvent obj)
    {
        Debug.Log("Updating positions " + obj.data);

        //var v = float.Parse(obj.data["v"].ToString().Replace("\"", ""));
        //var h = float.Parse(obj.data["h"].ToString().Replace("\"", ""));
        var position = MakePosFromJson(obj);
        var player = spawner.findPlayer(obj.data["id"].ToString());
        var rotation = obj.data["rotz"].n;

        player.transform.position = position;
        player.transform.eulerAngles = new Vector3(0,0,rotation);
       
    }

    private void OnRegister(SocketIOEvent obj)
    {
        Debug.Log("Registered Player " + obj.data);
        spawner.AddPlayer(obj.data["id"].ToString(), spawner.localPlayer);
    }

    private void OnDisconnected(SocketIOEvent obj)
    {
        Debug.Log("Player disconnected " + obj.data);

        var id = obj.data["id"].ToString();

        spawner.RemovePlayer(id);
    }

    private void OnMove(SocketIOEvent obj)
    {
        //Debug.Log("Player Moving" + obj.data);
        var id = obj.data["id"].ToString().Replace("\"", "");
        //Debug.Log(id);

        var v = float.Parse(obj.data["v"].ToString().Replace("\"",""));
        var h = float.Parse(obj.data["h"].ToString().Replace("\"",""));

        var player = spawner.findPlayer(id);

        var playerMover = player.GetComponent<PlayerMovementNetwork>();
        playerMover.v = v;
        playerMover.h = h;

    }

    private void OnSpawn(SocketIOEvent obj)
    {
        Debug.Log("Player spawned" + obj.data);
        var player = spawner.SpawnPlayer(obj.data["id"].ToString());

        //spawn existing players at location
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
        //Debug.Log("Send Position to Server" + VectorToJson(currentPosV, currentPosH));
        socket.Emit("move", new JSONObject(VectorToJson(currentPosV, currentPosH)));
    }

    public static string VectorToJson(float dirV, float dirH)
    {

        return string.Format(@"{{""v"":""{0}"",""h"":""{1}""}}", dirV, dirH);
    }

    public static JSONObject PosToJson(Vector3 pos, float rotz)
    {
        JSONObject jpos = new JSONObject(JSONObject.Type.OBJECT);
        jpos.AddField("x", pos.x);
        jpos.AddField("y", pos.y);
        jpos.AddField("z", pos.z);
        jpos.AddField("rotz", rotz);
        return jpos;
    }

    public static Vector3 MakePosFromJson(SocketIOEvent e)
    {
        return new Vector3(e.data["x"].n, e.data["y"].n, e.data["z"].n);
    }
}



