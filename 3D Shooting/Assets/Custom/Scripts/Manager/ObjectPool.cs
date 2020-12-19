using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private static ObjectPool instance = null;
    public static ObjectPool Instance
    {
        get
        {
            if (instance == null)
                return null;

            return instance;
        }
    }

    [SerializeField] private GameObject[] bulletPrefabs = null;
    private Dictionary<GunProperty.GUN_KIND, List<GameObject>> bulletList = new Dictionary<GunProperty.GUN_KIND, List<GameObject>>();

    [SerializeField] private int[] maxCreateBulletCount = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else Destroy(this);

        RegistBullet();
    }

    private void RegistBullet()
    {
        for(int i = 0; i< bulletPrefabs.Length; i++)
        {
            List<GameObject> lTemp = new List<GameObject>();

            for (int j = 0; j < maxCreateBulletCount[i]; j++)
            {
                GameObject bullet = Instantiate(bulletPrefabs[i]);
                bullet.transform.position = Vector3.zero;
                bullet.transform.rotation = Quaternion.identity;
                bullet.SetActive(false);
                lTemp.Add(bullet);
            }

            bulletList.Add((GunProperty.GUN_KIND)i, lTemp);
        }
    }

    public GameObject AssignBullet(GunProperty.GUN_KIND kind)
    {
        for (int i = 0; i < bulletList[kind].Count; i++)
        {
            if (bulletList[kind][i].activeSelf) continue;

            bulletList[kind][i].SetActive(true);
            return bulletList[kind][i];
        }

        return null;
    }

    private void OnDestroy()
    {
        bulletList.Clear();
    }
}
