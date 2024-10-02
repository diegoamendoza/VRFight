using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Subject : MonoBehaviour
{
    public event Action<Vector3> OnWhistle;
    [SerializeField] float speed = 10f;
    void Update()
    {
        float haxis = Input.GetAxisRaw("Horizontal");
        float vaxis = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(haxis, 0, vaxis).normalized;
        transform.position += direction * Time.deltaTime * speed;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnWhistle?.Invoke(transform.position);
            Debug.Log("a");
        }
    }
}
