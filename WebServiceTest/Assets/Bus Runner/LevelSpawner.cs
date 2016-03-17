using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class LevelSpawner : MonoBehaviour {


    public GameObject[] platformTypes;

    private PlatformScript.HeightLevel heightLevel = PlatformScript.HeightLevel.Street;
    GameObject player;

    private float minGapX = 1.5f;
    private float maxGapX = 2.5f;
    private float maxGapY = 4f;
    private float minGapY = -4.0f;

    private GameObject lastSpawn = null; //null means we are on ground level
    private float lastSpawnX = 0;
    

	// Use this for initialization
	void Start () {
        SpawnStart();
        player = GameObject.FindGameObjectWithTag("Player");
        //camera = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(lastSpawn.GetComponent<PlatformScript>().GetRect().xMax);
        if (lastSpawn.GetComponent<PlatformScript>().GetRect().xMax - GetCameraRight() < 10.0f) {
            SpawnSegment(5.0f);
        }

        //Debug.Log("Camera right: " + GetCameraRight());
	}

    private void SpawnStart() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 position = new Vector3(player.transform.position.x + platformTypes[1].GetComponent<PlatformScript>().width/2.0f, player.transform.position.y - player.GetComponent<BoxCollider2D>().bounds.extents.y - platformTypes[1].GetComponent<PlatformScript>().GetSize().y / 2.0f, 0);
        lastSpawn = (GameObject)Instantiate(platformTypes[1], position, Quaternion.identity);
        //Debug.Log(platformTypes[0].GetComponent<PlatformScript>().GetRect().height);

    }

    private GameObject GetNextPlatformType() {

        //Only take objects from adjacent height levels and objects which can be reached by jumping
        GameObject[] reachable = System.Array.FindAll(platformTypes, (GameObject o) => {
            if ((lastSpawn == null && o.GetComponent<PlatformScript>().heightLevel == PlatformScript.HeightLevel.Street)
                || Mathf.Abs((int)o.GetComponent<PlatformScript>().heightLevel - (int)lastSpawn.GetComponent<PlatformScript>().heightLevel) < 2) {
                return IsReachable(o);
            }
            return false;
        });

        //Debug.Log(reachable.Length);
        return reachable[Random.Range(0, reachable.Length)];

    }

    bool IsReachable(GameObject obj) {
        float lastTop = (lastSpawn == null) ? 0 : lastSpawn.GetComponent<PlatformScript>().height;
        float nextTop = obj.GetComponent<PlatformScript>().bottom + obj.GetComponent<PlatformScript>().height;
        if (nextTop > lastTop)
            return nextTop - lastTop < maxGapY;
        else
            return nextTop - lastTop > minGapY;
    }

    private void SpawnSegment(float width = 10.0f) {
        while (width > 0) {
            width -= SpawnNextPlatform();
        }
    }

    private float SpawnNextPlatform() {
        GameObject platformType = GetNextPlatformType();

        float lastTop = (lastSpawn == null) ? 0 : lastSpawn.GetComponent<PlatformScript>().height;
        float nextTop = platformType.GetComponent<PlatformScript>().bottom + platformType.GetComponent<PlatformScript>().height;

        float realMaxX = maxGapX;
        if (nextTop - lastTop > 0) {
            realMaxX = (maxGapX / 2) * (2 - (nextTop - lastTop) / maxGapY);
        }

        float xGap = Random.Range(minGapX * (player.GetComponent<CharacterMovement>().maxSpeed / player.GetComponent<CharacterMovement>().startingSpeed), realMaxX);

        

        float lastRight = lastSpawn.GetComponent<PlatformScript>().GetRect().xMax;

        Vector3 position = new Vector3(lastRight + xGap + platformType.GetComponent<PlatformScript>().width / 2.0f, platformType.GetComponent<PlatformScript>().bottom + platformType.GetComponent<PlatformScript>().height / 2.0f, 0);
        lastSpawn = (GameObject)Instantiate(platformType, position, Quaternion.identity);

        return xGap + platformType.GetComponent<PlatformScript>().width;
    }

    private float GetCameraRight() {
        Camera camera = Camera.main;
        float height = 2.0f * camera.orthographicSize;
        float width = height * camera.aspect;
        return camera.transform.position.x + width / 2.0f;
    }
}
