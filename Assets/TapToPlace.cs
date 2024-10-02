using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
public class TapToPlace : MonoBehaviour
{
    public ARRaycastManager raycastManager;
    public ARPlaneManager planeManager;
    public GameObject prefab;
    public List<ARRaycastHit> hits = new List<ARRaycastHit>();
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            PlaceObjectOnPlane(touch);
        }
    }
    private void PlaceObjectOnPlane(Touch touch)
    {
        if (raycastManager.Raycast(touch.position, hits, TrackableType.Planes))
        {
            foreach ( ARRaycastHit hit in hits)
            {
                Pose pose = hit.pose; //Contiene una posicion y una rotacion en 3D
                if (planeManager.GetPlane(hit.trackableId).alignment == PlaneAlignment.HorizontalUp)
                {
                    Instantiate(prefab, pose.position, Quaternion.identity);
                }
            }
        }
    }
}
