using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMainCameraPosition : MonoBehaviour
{
    public Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 temp = new Vector3(cam.gameObject.transform.position.x, cam.gameObject.transform.position.y, cam.gameObject.transform.position.z - 3f);
        this.gameObject.transform.position = temp;   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
