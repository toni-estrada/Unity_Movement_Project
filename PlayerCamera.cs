using UnityEngine;

public class PlayerCamera : MonoBehaviour
{

    public float sensitivityX;
    public float sensitivityY;
    private float m_RotationX;
    private float m_RotationY;

    public Transform orientation;
    
    // Start is called before the first frame update
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    private void Update()
    {
        Rotate();
    }

    private void Rotate()
    {
        // GETTING MOUSE INPUTS
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.fixedDeltaTime * sensitivityX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.fixedDeltaTime * sensitivityY;

        m_RotationY += mouseX;
        m_RotationX -= mouseY;
        m_RotationX = Mathf.Clamp(m_RotationX, -90f, 90f);
        
        // ROTATE CAMERA AND ORIENTATION
        transform.rotation = Quaternion.Euler(m_RotationX, m_RotationY, 0);
        orientation.rotation = Quaternion.Euler(0, m_RotationY, 0);
    }
}
