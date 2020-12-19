using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoHelper : MonoBehaviour
{
    [SerializeField] private Color color = new Color(1, 1, 1, 1);
    [SerializeField] private float size = 0;

    private void OnDrawGizmos()
    {
        Gizmos.color = color;

        Gizmos.DrawSphere(transform.position, size);
    }
}
