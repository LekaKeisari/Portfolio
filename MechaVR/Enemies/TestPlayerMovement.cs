using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerMovement : MonoBehaviour
{
    public Rigidbody rb;

    private float horizontalMove = 100f;
    private Vector3 velocity = Vector3.zero;
    private float movementSmoothing = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.D))
        {
            SideMove(horizontalMove * Time.fixedDeltaTime);

        }
        if (Input.GetKey(KeyCode.A))
        {
            SideMove(-horizontalMove * Time.fixedDeltaTime);

        }
        if (Input.GetKey(KeyCode.S))
        {
            FrontBackMove(-horizontalMove * Time.fixedDeltaTime);

        }
        if (Input.GetKey(KeyCode.W))
        {
            FrontBackMove(horizontalMove * Time.fixedDeltaTime);

        }
    }

    public void SideMove(float move)                                                //Move player with smoothing
    {
        
            Vector3 targetVelocity = new Vector3(move * 10f, rb.velocity.y, rb.velocity.z);                      // Move the character by finding the target velocity            
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);  // And then smoothing it out and applying it to the character
        
    }
    public void FrontBackMove(float move)                                                //Move player with smoothing
    {
        
            Vector3 targetVelocity = new Vector3(rb.velocity.x, rb.velocity.y, move * 10f);                      // Move the character by finding the target velocity            
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);  // And then smoothing it out and applying it to the character
        
    }
}
