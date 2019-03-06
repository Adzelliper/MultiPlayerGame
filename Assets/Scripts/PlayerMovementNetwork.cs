using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementNetwork : MonoBehaviour {
    Rigidbody2D rb2d;
    public float v, h;
	// Use this for initialization
	void Start () {
        rb2d = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	public void FixedUpdate () {
        transform.Rotate(0,0, -h * 100f * Time.deltaTime);

        if (v > 0)
        {
            rb2d.AddForce(transform.up * 5f * v);
            
        }
        else
        {
            rb2d.velocity = Vector2.zero;
        }
        //Network.Move(v, h);
    }
}
