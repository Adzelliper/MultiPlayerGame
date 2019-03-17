using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System;

public class Network : MonoBehaviour {

    static SocketIOComponent socket;
    public GameObject playerPrefab;
    public SpawnerScript spawner;
    

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
        socket.On("updateScore", OnUpdateScore);
        socket.On("requestScore", OnRequestScore);
    }

    private void OnUpdateScore(SocketIOEvent obj)
    {
       // Debug.Log("Update scores: " + obj.data);

        //var player = spawner.findPlayer(obj.data["id"].ToString());
        //var score = MakeScoreFromJson(obj);
        //Debug.Log(score);
        //player.GetComponent<Points>().points = score;


    }

    private void OnRequestScore(SocketIOEvent obj)
    {
        Debug.Log("Score Requested" + spawner.localPlayer.GetComponent<Points>().points.ToString());
        //Debug.Log("Registered Player score " + obj.data["score"]);
        //socket.Emit("updateScore", SCRtoJson(spawner.localPlayer.GetComponent<Points>().points));
    }

    public static JSONObject SCRtoJson(int score)
    {
        JSONObject spos = new JSONObject(JSONObject.Type.OBJECT);
        spos.AddField("score", score);
        return spos;
    }

    private void OnRequestPosition(SocketIOEvent obj)
    {
        socket.Emit("updatePosition", PosToJson(spawner.localPlayer.transform.position, spawner.localPlayer.transform.rotation.eulerAngles.z));
    }

    private void OnUpdatePosition(SocketIOEvent obj)
    {
       // Debug.Log("Updating positions " + obj.data);

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
       // Debug.Log("Registered Player " + obj.data);
        spawner.AddPlayer(obj.data["id"].ToString(), spawner.localPlayer);
    }

    private void OnDisconnected(SocketIOEvent obj)
    {
       // Debug.Log("Player disconnected " + obj.data);

        var id = obj.data["id"].ToString();

        string userName = spawner.findPlayer(id).GetComponent<NetworkEntity>().id;
        int points = spawner.findPlayer(id).GetComponent<Points>().points;
        JSONObject data = new JSONObject(JSONObject.Type.OBJECT);
        data.AddField("name", userName);
        data.AddField("score", points);
        //Add other fields here
        socket.Emit("updateScore", data);

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
        //Debug.Log("Player spawned" + obj.data);
        var player = spawner.SpawnPlayer(obj.data["id"].ToString());

        //spawn existing players at location
    }

    private void OnTalkBack(SocketIOEvent obj)
    {
        //Debug.Log("The Server says Hello Back");      
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
    public static int MakeScoreFromJson(SocketIOEvent e)
    {
        int x = (int)e.data["score"].n;
        return x;
    }
}



