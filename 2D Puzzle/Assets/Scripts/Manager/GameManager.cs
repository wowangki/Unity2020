using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region SingleTonPatttern
    private static GameManager instance = null;
    public static GameManager Instance
    {
        get
        {
            if (instance == null) return null;
            return instance;
        }
    }

    #endregion

    public ObjectPool ObjPool { get; private set; }
    //public ResourceManager Resource;
    public SpawnManager Spawn { get; private set; }
    public MapManager Map { get; private set; }

    public Node NpcStart { get; set; }
    public List<Node> NpcEndList { get; private set; } = new List<Node>();
    public List<Node> NpcWaitList { get; private set; } = new List<Node>();

    public Queue<NpcAI.WaitFunc> WaitQueue { get; private set; } = new Queue<NpcAI.WaitFunc>();

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);

        ObjPool = GetComponent<ObjectPool>();
        Spawn = GetComponent<SpawnManager>();
        Map = GetComponent<MapManager>();
        //Resource = GetComponent<ResourceManager>();

        ObjPool.Init();
        Map.Init();
        StartCoroutine(NpcWaitDoCoroutine());
    }

    public void BackGroundCoroutine(IEnumerator enumerator)
    {
        StartCoroutine(enumerator);
    }


    private IEnumerator NpcWaitDoCoroutine()
    {
        WaitUntil wu = new WaitUntil(() => (WaitQueue.Count != 0));

        while (true)
        {
            yield return wu;

            NpcAI.WaitFunc wait = WaitQueue.Peek();

            if (wait.Invoke())
                WaitQueue.Dequeue();

            yield return null;
        }
    }
}
