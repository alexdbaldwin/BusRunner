using UnityEngine;
using System.Collections;

public class CameraTrackingScript : MonoBehaviour {

    public GameObject target;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 pos = gameObject.transform.position;
        pos.x = target.transform.position.x + 1;
        pos.y = target.transform.position.y;
        gameObject.transform.position = pos;
	}
}
