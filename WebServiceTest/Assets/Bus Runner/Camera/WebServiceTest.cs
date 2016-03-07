using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.Networking;

public class WebServiceTest : MonoBehaviour {

    SpriteRenderer spriteRenderer;

	// Use this for initialization
    void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();

        StartCoroutine(GetStreetView());
        ResizeToFillScreen();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator GetStreetView() {
        UnityWebRequest www = UnityWebRequest.GetTexture("https://maps.googleapis.com/maps/api/streetview?size=640x360&location=55.608705,13.000988&heading=90&pitch=-0.76&key=AIzaSyDHygQidyMG7WN8Z1VJ_o5r1fmpfGEPePQ");
        yield return www.Send();

        if (www.isError){
            //Debug.Log(www.error);
        }  
        else
        {
            //Debug.Log("Success!");
            Texture2D myTexture = (Texture2D) DownloadHandlerTexture.GetContent(www);
            spriteRenderer.sprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), new Vector2(0.5f,0.5f));
        }
    }

    void ResizeToFillScreen()
    {

        transform.localScale = new Vector3(1, 1, 1);

        float width = spriteRenderer.sprite.bounds.size.x;
        float height = spriteRenderer.sprite.bounds.size.y;


        float worldScreenHeight = Camera.main.orthographicSize * 2f;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        Vector3 xWidth = transform.localScale;
        xWidth.x = worldScreenWidth / width;
        transform.localScale = xWidth;
        //transform.localScale.x = worldScreenWidth / width;
        Vector3 yHeight = transform.localScale;
        yHeight.y = worldScreenHeight / height;
        transform.localScale = yHeight;
        //transform.localScale.y = worldScreenHeight / height;

    }
}
