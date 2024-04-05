using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_Script : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    float xRotation = 0f;
    float yRotation = 0f;

    public float topClamp = -90f;
    public float bottomClamp = 90f;
    
    void Start()
    {
       //Cursor lock
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //mouse inputlarý
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime ;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        //Rotation through x axis
        xRotation -= mouseY;

        //Rotation kitleme
        xRotation = Mathf.Clamp(xRotation, topClamp, bottomClamp);
        
        //Rotation through y axis
        yRotation += mouseX;

        //Apply Rotations to transform
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);


    }
}
