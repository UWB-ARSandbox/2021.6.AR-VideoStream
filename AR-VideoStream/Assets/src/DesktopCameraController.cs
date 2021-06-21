using System;
using System.Collections;
using System.Collections.Generic;
using ASL;
using UnityEngine;

public class DesktopCameraController : MonoBehaviour
{
    public static readonly float MOVEMENT_SPEED = 1.0f;
    public static readonly float SPRINT_MULTIPLIER = 2.0f;

    public static readonly float GRID_SPACING = 0.1f;
    public static readonly float OBJECT_PLACEMENT_STANDOFF = 1.0f;

    private bool gridMode = false;
    
    // Start is called before the first frame update
    void Start()
    {
        if(Application.isMobilePlatform)
            gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currentPosition = transform.position;
        float currentSpeed = MOVEMENT_SPEED;

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentSpeed *= SPRINT_MULTIPLIER;
        }

        if (!gridMode)
        {
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                currentPosition += transform.forward * (Time.deltaTime * currentSpeed);
            }

            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                currentPosition += -transform.forward * (Time.deltaTime * currentSpeed);
            }

            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.A))
            {
                currentPosition += -transform.right * (Time.deltaTime * currentSpeed);
            }

            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.D))
            {
                currentPosition += transform.right * (Time.deltaTime * currentSpeed);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                currentPosition += transform.forward * GRID_SPACING;
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                currentPosition += -transform.forward * GRID_SPACING;
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                currentPosition += -transform.right * GRID_SPACING;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                currentPosition += transform.right * GRID_SPACING;
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentPosition += transform.up * GRID_SPACING;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentPosition += -transform.up * GRID_SPACING;
            }
        }

        transform.position = currentPosition;

        if (Input.GetKeyDown(KeyCode.G))
        {
            gridMode = !gridMode;

            if (gridMode)
            {
                GetComponent<SmoothMouseLook>().enabled = false;
                transform.forward = Vector3.forward;
                transform.position = new Vector3(
                    (int) transform.position.x,
                    (int) transform.position.y,
                    (int) transform.position.z);
            }
            else
            {
                GetComponent<SmoothMouseLook>().enabled = true;
            }
            
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (gridMode)
            {
                ASLHelper.InstantiateASLObject("SmallCube", 
                    transform.position + transform.forward * OBJECT_PLACEMENT_STANDOFF, 
                    transform.rotation,
                    String.Empty,
                    String.Empty,
                    OnCubeCreation);
            }
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private static void OnCubeCreation(GameObject cube)
    {
        ASLObject aslObject = cube.GetComponent<ASLObject>();
        
        aslObject.SendAndSetClaim(() =>
        {
            aslObject.SendAndSetLocalScale(new Vector3(GRID_SPACING, GRID_SPACING, GRID_SPACING));
        });
    }
    
    
    
}
