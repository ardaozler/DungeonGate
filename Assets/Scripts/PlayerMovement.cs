using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    
    public float speed = 12f;
    public float gravity = -9.81f * 2;
    public float jumpHeight = 3f;


    public Transform groundcheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;



    Vector3 velocity;

    bool isGrounded;
    bool isMoving;

    private Vector3 lastPosition = new Vector3(0f, 0f, 0f);
    
    
    
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Ground check

        isGrounded = Physics.CheckSphere(groundcheck.position, groundDistance, groundMask);
        //Reseting Velocity
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;

        }
        //Getting inputs
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");


        //Creating moving vector
        Vector3 move = transform.right * x + transform.forward * z;

        //Character movement
        controller.Move(move * speed * Time.deltaTime);

        //No infinite jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        }
        //Falling Down
        velocity.y += gravity * Time.deltaTime;

        // Jumping
        controller.Move(velocity * Time.deltaTime);
        if(lastPosition!=gameObject.transform.position&&isGrounded == true)
        {
            isMoving = true;

        }
        else
        {
            isMoving = false;

        }
        lastPosition=gameObject.transform.position; 
    }
}
