using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ASL;
using UnityEngine;
using Random = UnityEngine.Random;

public class FrustumSpawner : MonoBehaviour
{
    private static GameObject frustumObject;
    private static ASLObject frustumASLObject;

    private Frustum frustum;

    private bool isUpdating = true;
    public readonly float UPDATES_PER_SECOND = 1.0f;

    private static ASLObject colorCubeASLObject;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(delayedInit());
    }

    private static void onFrustumCreation(GameObject frustumGameObject)
    {
        FrustumSpawner.frustumObject = frustumGameObject;
        FrustumSpawner.frustumASLObject = frustumGameObject.GetComponent<ASLObject>();
        
        ASLHelper.InstantiateASLObject(PrimitiveType.Cube, Vector3.zero,  Quaternion.identity, frustumASLObject.m_Id, String.Empty, onColorCubeCreation);
        
    }

    private static void onColorCubeCreation(GameObject colorCube)
    {
        colorCubeASLObject = colorCube.GetComponent<ASLObject>();
    }

    private IEnumerator delayedColorSet()
    {
        yield return new WaitForSeconds(0.4f);
        
        Color color = new Color(Random.value, Random.value, Random.value);
        
        Debug.Log("Setting color to: " + color);
        
        colorCubeASLObject.SendAndSetClaim(() =>
        {
            colorCubeASLObject.SendAndSetObjectColor(color, color);
            
            colorCubeASLObject.SendAndSetWorldPosition(new Vector3(10000, 10000, 10000));
        });
        
    }
    
    
    private IEnumerator delayedInit()
    {
        yield return new WaitForSeconds(0.5f);
        
        StartCoroutine(delayedColorSet());
        
        Debug.LogError("Instantiating Frustum Object");
        
        ASLHelper.InstantiateASLObject("frustum", 
            transform.position, 
            transform.localRotation, 
            "", 
            "", 
            onFrustumCreation);
        
        while (frustumObject == null)
        {
            yield return new WaitForSeconds(0.1f);
        }

        frustum = frustumObject.GetComponent<Frustum>();

        frustum.m_CylinderWidth = 0.02f;
        frustum.SetFarPlaneSize(3.0f, 2.0f);
        frustum.SetNearPlaneSize(1.0f, 0.66f);

        //frustum.SetVisible(false);
        
        StartCoroutine(timedUpdate());

        yield return new WaitForSeconds(2.0f);
        
        frustum.SetVisible(false);
    }

    // Assumes all instance variables are initialized
    private IEnumerator timedUpdate()
    {
        frustumASLObject = frustumObject.GetComponent<ASLObject>();
        
        while (isUpdating)
        {
            frustumASLObject.SendAndSetClaim(() =>
                {
                    frustumASLObject.SendAndSetWorldPosition(transform.position);
                    frustumASLObject.SendAndSetWorldRotation(transform.rotation);
                });
            
            yield return new WaitForSeconds(1 / UPDATES_PER_SECOND);
        }
        
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        isUpdating = false;
    }
}
