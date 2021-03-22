using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//THIS GOES ON THE BATTLE CLASS
public class CameraControls : MonoBehaviour
{
    public Camera cam;
    private float mouseX;
    private float mouseY;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //camera rotates VERY slightly when mouse is moved
        var view = cam.ScreenToViewportPoint(Input.mousePosition);
        var isOutside = view.x < 0 || view.x > 1 || view.y < 0 || view.y > 1;
        if(!isOutside)
        {
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");
            cam.transform.Rotate(-mouseY * 0.5f, mouseX * 0.5f, 0);
        }

        //cam doesn't rotate around z axis
        Vector3 eulers = cam.transform.eulerAngles;
        eulers.z = 0;
        cam.transform.eulerAngles = eulers;

    }
}
