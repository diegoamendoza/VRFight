using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MecanicaPlanos : MonoBehaviour
{
    [SerializeField] private ARPlaneManager planeManager;
    [SerializeField] private GameObject prefab;

    private void Start()
    {
        planeManager.planesChanged += OnPlaneChanged;
    }

    private void OnDestroy()
    {
        planeManager.planesChanged -= OnPlaneChanged;
    }
    public void OnPlaneChanged(ARPlanesChangedEventArgs args)
    {
        foreach (ARPlane plane in args.added)
        {
            if (plane.alignment == PlaneAlignment.HorizontalUp)
            {
                GameObject miku = Instantiate(prefab, plane.center, Quaternion.identity);
                miku.transform.LookAt(Camera.main.transform);
            }


            
        }
    }
}
