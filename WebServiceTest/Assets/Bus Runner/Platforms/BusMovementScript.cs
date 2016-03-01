using UnityEngine;
using System.Collections;

public class BusMovementScript : MonoBehaviour {

    public float speed = 5.0f;

	// Use this for initialization
	void Start () {
        //gameObject.GetComponent<Rigidbody2D>().velocity = new Vector3(-speed, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
        
        gameObject.transform.position = gameObject.transform.position - new Vector3(Time.deltaTime * speed, 0, 0);
	}
}
