using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class TargetElement
{
    public GameObject targetObject;
    public int priority;
}

public class Enemy : Charactor
{
    [SerializeField] private GameObject target;
    private NavMeshAgent navi;
    private List<GameObject> wayPoint;

    [SerializeField] private List<TargetElement> targetList;
    [SerializeField] private float viewAngle;
    [SerializeField] private float viewDistance;
    [SerializeField] private LayerMask targetMask;

    protected override void Awake()
    {
        base.Awake();
        navi = GetComponent<NavMeshAgent>();
    }

    protected override void Update()
    {
        base.Update();
        FindEnemy();
    }

    private void CheckDestination()
    {

    }

    private void MoveDestination()
    {

    }

    private void FindEnemy()
    {
        Vector3 leftBoundary = BoundaryAngle(-viewAngle * 0.5f);
        Vector3 rightBoundary = BoundaryAngle(viewAngle * 0.5f);

        Debug.DrawRay(transform.position + transform.up, leftBoundary, Color.red);
        Debug.DrawRay(transform.position + transform.up, rightBoundary, Color.red);

        Collider[] findList = Physics.OverlapSphere(transform.position, viewDistance);

        for (int i = 0; i < findList.Length; i++)
        {
            Transform targetTr = findList[i].transform;

            Vector3 dir = (targetTr.position - transform.position).normalized;
            float angle = Vector3.Angle(dir, transform.forward);

            if (angle < viewAngle * 0.5f)
            {
                RaycastHit hitInfo;

                if (Physics.Raycast(transform.position + transform.up, dir, out hitInfo, viewDistance))
                {
                    Debug.Log(hitInfo.transform.name);
                    AddEnemy(hitInfo.transform.gameObject);
                }
            }
        }
    }

    private Vector3 BoundaryAngle(float angle)
    {
        angle += transform.eulerAngles.y;

        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    private void AddEnemy(GameObject target)
    {
        if (target.CompareTag("Ground")) return;

        for (int i = 0; i < targetList.Count; i++)
        {
            if (target.Equals(targetList[i].targetObject))
                return;
        }

        TargetElement temp = new TargetElement();
        temp.targetObject = target;

        switch (target.tag)
        {
            case "Player": case "Enemy":
                temp.priority = 0;
                break;
            case "Tree": case "Structure":
                temp.priority = 3;
                break;
        }

        targetList.Add(temp);
    }
}
