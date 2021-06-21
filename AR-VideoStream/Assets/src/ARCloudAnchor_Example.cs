using System.Linq;
using ASL;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace SimpleDemos
{
    /// <summary> A simple demo showcasing how Cloud Anchors can be spawned</summary>
    public class ARCloudAnchor_Example : MonoBehaviour
    {
        public Toggle togglePlanes;
        
        
        /// <summary>
        /// Object that will be parented to the cloud anchor
        /// </summary>
        public GameObject m_ObjectToPairWithCloudAnchor;
        
        /// <summary>
        /// Update function - checks for user input and then creates a cloud anchor wherever the user tapped
        /// </summary>
        void Update()
        {
            // Show/hide AR Planes - planes generate non-stop, so this must be in Update
            ARPlaneMeshVisualizer[] planes = (ARPlaneMeshVisualizer[])GameObject.FindObjectsOfType(typeof(ARPlaneMeshVisualizer));
            foreach (ARPlaneMeshVisualizer plane in planes)
            {
                if (togglePlanes.isOn)
                {
                    plane.enabled = true;
                    plane.gameObject.layer = 0;
                }
                else
                {
                    plane.enabled = false;
                    plane.gameObject.layer = 2;
                }
            }
            
            
            Touch touch;
            // If the player has not touched the screen then the update is complete.
            if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
            {
                return;
            }

            // Ignore the touch if it's pointing on UI objects.
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                return;
            }
            
            Pose? m_LastHitPose = ASL.ARWorldOriginHelper.GetInstance().Raycast(Input.GetTouch(0).position);

            // If there was a successful hit
            //If we haven't set a cloud anchor yet && we are the Host -> then we can set a cloud anchor
            //Note: ASL does not prevent users from create unlimited cloud anchors, this means if two users create a cloud anchor at the same time
            //With the same ASL object, there is the chance things will become out of sync. This if statement is one way to avoid that synchronization problem
            if (m_LastHitPose != null && ASL.ASLHelper.m_CloudAnchors.Count <= 0 && ASL.GameLiftManager.GetInstance().AmLowestPeer())
            {
                m_ObjectToPairWithCloudAnchor.GetComponent<ASL.ASLObject>().SendAndSetClaim(() =>
                {
                    //Hit result, ASLObject to follow anchor (by becoming a child at (0,0,0), function to call after creation, sync start or not, set world origin or not 
                    ASL.ASLHelper.CreateARCoreCloudAnchor(m_LastHitPose, m_ObjectToPairWithCloudAnchor.GetComponent<ASL.ASLObject>(), null, false, true);
                });
            }
            
            
            //Raycast on touch position
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            RaycastHit hitObject;
            float distance;
                
            
            //Check for raycast hit
            if (Physics.Raycast(ray, out hitObject))
            {
                if (hitObject.collider != null)
                {
                    if(hitObject.collider.gameObject.name.Contains("ARPlane")) // Original Code for setting world anchor
                    {
                        ARPlane plane = (ARPlane) hitObject.transform.gameObject.GetComponent<ARPlane>();

                        ASL.ASLHelper.InstantiateASLObject("smallSphere",
                            plane.center,
                            hitObject.collider.transform.rotation, 
                            string.Empty, 
                            string.Empty, 
                            OnPlaneCreation);
                        
                        
                        /*
                        //Create new platform object, set position and rotation
                        ASL.ASLHelper.InstantiateASLObject(PrimitiveType.Plane, 
                            hitObject.collider.transform.position, 
                            hitObject.collider.transform.rotation, 
                            string.Empty, 
                            string.Empty, 
                            OnPlaneCreation);
                            */
                    }
                    if (hitObject.collider.gameObject.name.Contains("Cube")) // Assign a random color when a cube is clicked
                    {
                        ASLObject aslObject = hitObject.collider.gameObject.GetComponent<ASLObject>();

                        if (aslObject != null)
                        {
                            Color randomColor = new Color(Random.value, Random.value, Random.value);
                        
                            aslObject.SendAndSetClaim(() =>
                            {
                                aslObject.SendAndSetObjectColor(randomColor, randomColor);
                            });
                            
                        }
                    }
                }
            }
            
            // Separate Check for hit on an existing plane that's been created
            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.transform.name.Contains("Plane") && !hit.transform.name.Contains("ARPlane"))
                {
                    ASL.ASLHelper.InstantiateASLObject("SmallSphere",
                        new Vector3(hit.point.x, hit.transform.position.y + 0.1f, hit.point.z),
                        hit.transform.rotation,
                        string.Empty, 
                        string.Empty, 
                        OnPlaneCreation);
                }
            }
            
            
        }
        
        private static void OnPlaneCreation(GameObject gObject)
        {
            ASLObject aslObject = gObject.GetComponent<ASLObject>();
            
            aslObject.SendAndSetClaim(() =>
            {
                aslObject.SendAndSetLocalScale(new Vector3(0.1f, 0.1f, 0.1f));
            
                Color myColor = new Color(Random.value, Random.value, Random.value);
            
                aslObject.SendAndSetObjectColor(myColor, myColor);
            });
        }
        
    }
}