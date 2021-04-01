using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//THIS GOES ON THE BATTLE CLASS
public class CameraControls : MonoBehaviour
{
    public Camera cam;
    private float mouseX;
    private float mouseY;
    private float startX;
    private float startY;
    private float startZ;

    // Start is called before the first frame update
    void Start()
    {
        startX = cam.transform.eulerAngles.x;
        startY = cam.transform.eulerAngles.y;
        startZ = cam.transform.eulerAngles.z;
    }

    // Update is called once per frame
    void Update()
    {
        //camera rotates VERY slightly when mouse is moved
        var view = cam.ScreenToViewportPoint(Input.mousePosition);
        var isOutside = view.x < 0 || view.x > 1 || view.y < 0 || view.y > 1;
        if(!isOutside)
        {
            mouseX = (Input.mousePosition.x - (Screen.width / 2)) * 0.01f;
            mouseY = (Input.mousePosition.y - (Screen.height / 2)) * 0.01f;
            //Debug.Log(mouseX + " " + mouseY);
            Vector3 temp = new Vector3(mouseX, mouseY, 0);

            cam.transform.rotation = Quaternion.Euler(startX - mouseY, startY + mouseX, startZ);
        }

        //cam doesn't rotate around z axis
        Vector3 eulers = cam.transform.eulerAngles;
        eulers.z = 0;
        cam.transform.eulerAngles = eulers;

    }
}
