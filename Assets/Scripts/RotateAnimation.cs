using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAnimation : MonoBehaviour
{
    [SerializeField]
    public float X = 0f;
    public float Y = 0f;
    public float Z = 0f;
    public float speed = 1f;




    private void FixedUpdate()
    {
        transform.Rotate(new Vector3(X,Y,Z) * speed);
    }
}
