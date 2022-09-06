using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{

    public float sensitivityX;
    public float sensitivityY;

    public Transform orientation;

    private float RotationX;
    private float rotationY;
    
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensitivityX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensitivityY;

        rotationY += mouseX;
        RotationX -= mouseY;
        RotationX = Mathf.Clamp(RotationX, -90f, 90f);
        
        // Rotate camera and orientation
        transform.rotation = Quaternion.Euler(RotationX, rotationY, 0);
        orientation.rotation = Quaternion.Euler(0, rotationY, 0);
        
        
    }
}
