using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class Points : MonoBehaviour {
    public static SocketIOComponent socket;
    public int points;

    private void Start()
    {
        points = 0;
    }

    public void sendData()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Coin")
        {
            points++;
            GameObject.Find("Network").GetComponent<CoinSpawn>().i--;

            Destroy(collision.gameObject);
        }
    }
}
