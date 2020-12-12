using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Testing : MonoBehaviour
{
    public RectTransform cloud;
    public float rotSpeed;


    private void Update()
    {
        cloud.Rotate(Vector3.forward * rotSpeed * rotSpeed);
        cloud.GetChild(0).Rotate(Vector3.back * rotSpeed * rotSpeed);
    }

}
