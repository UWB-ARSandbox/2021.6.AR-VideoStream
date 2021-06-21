using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARCameraController2 : MonoBehaviour
{
    private GameObject mobileCamera;
    
    // Start is called before the first frame update
    void Start()
    {
        //Disable this entire GameObject if on Desktop
        if (!Application.isMobilePlatform)
        {
            gameObject.SetActive(false);
            return;
        }

        StartCoroutine(delayedInit());
    }

    //Give the AR camera time to initialize
    IEnumerator delayedInit()
    {
        Debug.LogError("Starting Delayed Init");
        
        yield return new WaitForSeconds(0.2f);

        mobileCamera = getARCamera();
        Debug.LogError("Got Mobile Camera");
    }

    private GameObject getARCamera()
    {
        if (!Application.isMobilePlatform)
            return null;
        
        Transform deviceTransform = ASL.ARWorldOriginHelper.GetInstance().ARCoreDeviceTransform;

        return deviceTransform.GetChild(0).gameObject; //First child is the camera object
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!Application.isMobilePlatform)
            return;

        if (mobileCamera == null)
            return;
        
        transform.position = mobileCamera.transform.position;
        transform.rotation = mobileCamera.transform.rotation;

    }
}
