using UnityEngine;
using System.Collections;

public class PlatformScript : MonoBehaviour {

    public BoxCollider2D boundingBox;
    

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public virtual Rect GetRect() {
        Bounds b = boundingBox.bounds;
        return new Rect(b.min.x, b.min.y, b.extents.x * 2f, b.extents.y * 2f);
    }
}
