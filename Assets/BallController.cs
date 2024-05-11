using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        this.rb = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnCollisionEnter(Collision collision)
    {
        // check if the collision involves an object with the "wall" tag.
        if (collision.gameObject.CompareTag("paddle"))
        {
            Debug.Log("Collision");
            // // reflect the velocity off the wall.
            // Vector3 normal = collision.contacts[0].normal;
            // rb.velocity = Vector3.Reflect(rb.velocity * 3, normal);
            //
            
            // Calculate Angle Between the collision point and the player
            Vector3 dir = collision.contacts[0].point - transform.position;
            // We then get the opposite (-Vector3) and normalize it
            dir = -dir.normalized;
            // And finally we add force in the direction of dir and multiply it by force. 
            // This will push back the player
            GetComponent<Rigidbody>().AddForce(dir*40f);
        }
        else
        {
            //probably hit something else, like an entity. do damage.
        }
    }
}
