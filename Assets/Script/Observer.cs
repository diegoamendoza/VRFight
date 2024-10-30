using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : MonoBehaviour
{
    public Subject subject;
    private void Start()
    {
        subject.OnWhistle += GoToSubject;
    }
    private void OnEnable()
    {
        
    }

    private void OnDestroy()
    {
        subject.OnWhistle -= GoToSubject;
        
    }

    void GoToSubject(Vector3 pos)
    {
        transform.position = pos + Vector3.right;
    }
}
