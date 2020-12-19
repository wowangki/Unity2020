using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraSet
{
    public enum SET_KIND
    {
        TITLE,
        PLAY
    }

    public SET_KIND kind;
    public List<Camera> camList = new List<Camera>();
}

public class CameraManager : MonoBehaviour
{
    private static CameraManager instance = null;

    public static CameraManager Instance
    {
        get {
            if (instance == null)
                return null;
            
            return instance;
        } 
    }

    [SerializeField] private List<CameraSet> camSetList = null;
    public Camera curCam;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else Destroy(this);

        RegistCam();
    }

    private void RegistCam()
    {
        for (int i = 0; i< transform.childCount; i++)
        {
            // Camera 모음
            Transform child = transform.GetChild(i);
            CameraSet camSet = new CameraSet();
            camSet.kind = (CameraSet.SET_KIND)i;

            for (int j = 0; j < child.childCount; j++)
            {
                Camera tCam = child.GetChild(j).GetComponent<Camera>();
                if (!tCam) continue;

                camSet.camList.Add(tCam);
            }

            camSetList.Add(camSet);
        }

        //curCam = camSetList[(int)CameraSet.SET_KIND.PLAY].camList[0];

        for (int i = 0; i < camSetList[(int)CameraSet.SET_KIND.PLAY].camList.Count; i++)
        {
            if (camSetList[(int)CameraSet.SET_KIND.PLAY].camList[i].gameObject.activeSelf)
            {
                curCam = camSetList[(int)CameraSet.SET_KIND.PLAY].camList[i];
                return;
            }
        }
    }

    public void ChangeCamera(int index, CameraSet.SET_KIND kind = CameraSet.SET_KIND.PLAY)
    {
        List<Camera> camSet = camSetList[(int)kind].camList;

        for (int i = 0; i < camSet.Count; i++)
        {
            if (i != index)
                camSet[i].gameObject.SetActive(false);
            else
            {
                curCam = camSet[i];
                curCam.gameObject.SetActive(true);
            }
        }
    }

    public void SetTarget(Transform target)
    {
        CameraController camController;

        if(camController = curCam.GetComponent<CameraController>())
        {
            camController.Target = target;
            camController.InitializeCamera();
        }
    }
}
