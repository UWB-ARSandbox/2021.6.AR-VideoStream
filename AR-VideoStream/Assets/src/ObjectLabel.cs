using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class ObjectLabel : MonoBehaviour
{
    private Camera camera;
    
    Rect rect = new Rect(0, 0, 300, 100);
    
    void OnGUI()
    {
        Vector3 point = camera.WorldToScreenPoint(transform.position + (transform.up * 0.1f));
        rect.x = point.x;
        rect.y = Screen.height - point.y - rect.height; // bottom left corner set to the 3D point
        GUI.Label(rect, transform.name); // display its name, or other string
    }
    
    // Start is called before the first frame update
    void Start()
    {
        if (!Application.isMobilePlatform)
        {
            camera = GameObject.Find("DesktopCamera").GetComponent<Camera>();
        }
        else
        {
            camera = GameObject.FindObjectOfType<Camera>();
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
        
    }
}
