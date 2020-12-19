using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform Target { get; set; }
    [SerializeField] private float senseitive = 0f;
    [SerializeField] private Vector3 offset = Vector3.zero;
    public Vector3 Offset { get { return offset; } }
    [SerializeField] private Vector3 originAngle = Vector3.zero;
    [SerializeField] private float clampY = 45.0f;
    private float curCamRotY = 0f;
    [HideInInspector] public Vector3 followPos;
    public Transform shootTr;

    private void OnEnable()
    {
        if (Target == null) return;

        InitializeCamera();
    }

    public void InitializeCamera()
    {
        FollowTarget();
        transform.rotation = Quaternion.Euler(originAngle);
    }

    private void LateUpdate()
    {
        if (Target == null) return;

        FollowTarget();
        RotateTarget();
    }

    private void FollowTarget()
    {
        Vector3 followOffset = (transform.right * offset.x + transform.up * offset.y + transform.forward * offset.z);
        followPos = Target.position + followOffset;
        transform.position = Vector3.Slerp(transform.position, followPos, 0.5f);
    }

    private void RotateTarget()
    {
        float x = Input.GetAxis("Mouse X") * senseitive;
        float y = Input.GetAxis("Mouse Y") * senseitive;

        // Horizontal Rot
        if (x == 0) return;
        transform.RotateAround(Target.position, Vector3.up, x);

        // Vertical Rot
        if (y == 0) return;
        curCamRotY -= y;
        curCamRotY = Mathf.Clamp(curCamRotY, -clampY, clampY);

        transform.localEulerAngles = new Vector3(curCamRotY, transform.localEulerAngles.y, 0f);
    }

    public void ShakeCam(float force)
    {
        Vector3 recoilBack = followPos + new Vector3(Random.Range(-force, force),
                                                     Random.Range(-force, force),
                                                     Random.Range(-force, force));

        transform.localPosition = Vector3.Slerp(transform.localPosition, recoilBack, 0.1f);
    }
}
