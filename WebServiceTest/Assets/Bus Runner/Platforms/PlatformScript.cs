using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class PlatformScript : MonoBehaviour {

    public enum HeightLevel
    {
        Street = 0, //Cars, dustbins, pedestrians etc.
        UpperStreet = 1, //Lampposts, signs, buses, bus stops, awnings etc.
        Balcony = 2,
        Rooftop = 3,
        Sky= 4 //Birds, clouds, planes etc.
    }

    public enum District {
        VastraHamnen,
        InreHamnen,
        GamlaStaden
    }

    public bool canBeKnockedOver = false;
    public bool landlocked = false;
    public bool moving = false;
    public float bottom = 0; //ground level
    public District[] districts;
    public HeightLevel heightLevel = HeightLevel.Street;
    public BoxCollider2D boundingBox;
    public float width = 1;
    public float height = 1;
    

	// Use this for initialization
	void Start () {
        transform.localScale = new Vector3(width / 1.0f, height / 1.0f, 1.0f);
        //Debug.Log("Stuff");
        //Debug.Log(width);
	}
	
	// Update is called once per frame
	void Update () {
        transform.localScale = new Vector3(width / 1.0f, height / 1.0f, 1.0f);
        //Debug.Log("Stuff");
        //Debug.Log(GetRect().ToString());
	}

    //Only call this on an instantiated object
    public virtual Rect GetRect() {
        Bounds b = boundingBox.bounds;
        return new Rect(b.min.x, b.min.y, b.extents.x * 2f, b.extents.y * 2f);
    }
    public virtual Vector2 GetSize() {
        return boundingBox.size;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.gameObject.tag == "Player") {
            
            if (canBeKnockedOver)
            {
                GetComponent<Rigidbody2D>().isKinematic = false;
                GetComponent<Rigidbody2D>().AddForce(new Vector2(200, 600));
                GetComponent<Rigidbody2D>().AddTorque(500);
                foreach (BoxCollider2D c in GetComponents<BoxCollider2D>()){
                    c.enabled = false;
                }
                other.gameObject.GetComponent<CharacterMovement>().Brake();
                Destroy(gameObject, 0.5f);
            }
            else
            {
                other.GetComponent<CharacterMovement>().TryPlatformKill();
            }
        }
    }

    //IEnumerator Despawn(float delay) {
    //    yield return new WaitForSeconds(delay);
        
    //}

    void OnTriggerStay2D(Collider2D other)
    {

        if (other.gameObject.tag == "Player")
        {

            if (canBeKnockedOver)
            {

            }
            else
            {
                other.GetComponent<CharacterMovement>().TryPlatformKill();
            }
        }
    }

    
}
