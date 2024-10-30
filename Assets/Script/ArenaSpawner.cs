using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ArenaSpawner : MonoBehaviour
{
    [SerializeField] private GameObject arenaPrefab; // Prefab de la arena
    private ARRaycastManager arRaycastManager;
    private GameObject instantiatedArena;
    private bool arenaPlaced = false;

    void Start()
    {
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
    }

    void Update()
    {
        if (arenaPlaced) return;

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Vector2 touchPosition = Input.GetTouch(0).position;
            List<ARRaycastHit> hits = new List<ARRaycastHit>();

            if (arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;

                if (instantiatedArena == null)
                {
                    instantiatedArena = Instantiate(arenaPrefab, hitPose.position, hitPose.rotation);
                    arenaPlaced = true;
                }
            }
        }
    }
}
