using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawn : MonoBehaviour {

    public GameObject spawnx1;
    public GameObject spawnx2;
    public GameObject spawny1;
    public GameObject spawny2;

    public int coinNum = 20;
    public GameObject coin;
    public int i = 0;



    // Update is called once per frame
    void Update () {
        if ( i < coinNum)
        {
            i++;
            Instantiate(coin,
                new Vector3(Random.Range(spawnx1.transform.position.x, spawnx2.transform.position.x), 
                Random.Range(spawny1.transform.position.y, spawny2.transform.position.y), 0) , Quaternion.identity);
        }

	}
}
