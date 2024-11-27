using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PruebaPosition : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Local Position: " + transform.localPosition);
            Debug.Log("Position: " + transform.position);
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            transform.localPosition = new Vector3(14,0,21);
        }
    }
}
