using UnityEngine;
using System.Collections;

public class LevelSpawner : MonoBehaviour {

    
    public GameObject platform;
    public float spawnX = -11;

    //private Camera camera;
    private GameObject player;

    private float minGapX = 1.0f;
    private float maxGapX = 5.0f;
    private float maxGapYUp = 3.0f;
    private float maxGapYDown = 6.0f;
    

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        //camera = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {



        Debug.Log("Camera right: " + GetCameraRight());
	}

    private void SpawnSegment() { 
    
    }

    private float GetCameraRight() {
        Camera camera = Camera.main;
        float height = 2.0f * camera.orthographicSize;
        float width = height * camera.aspect;
        return camera.transform.position.x + width / 2.0f;
    }
}
