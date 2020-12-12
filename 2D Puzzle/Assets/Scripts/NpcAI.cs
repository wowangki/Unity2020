using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcAI : MonoBehaviour
{
    public enum NPC_DIRECTION
    {
        IDLE,
        UMOVE,
        RMOVE,
        LMOVE,
        DMOVE
    }

    // Default Set
    public NpcInfo Info { get; set; }
    private Transform tr;
    private Vector2 dir = Vector2.zero;
    private SpriteRenderer mRenderer;
    [SerializeField]
    private float moveSpeed = 2f;

    // Frame Set
    private NPC_DIRECTION direction = NPC_DIRECTION.IDLE;
    [SerializeField]
    private float frameSpeed = 0f;
    private float curTime = 0f;
    private int spriteId = 0;

    // Move Set
    public struct MOVE_ROAD
    {
        public Node node;
        public Vector3 pos;
        public NPC_DIRECTION dir;

        public MOVE_ROAD(Node node, Vector3 pos, NPC_DIRECTION dir)
        {
            this.node = node;
            this.pos = pos;
            this.dir = dir;
        }

        public MOVE_ROAD(Node node)
        {
            this.node = node;
            pos = node.pos;

            int tDir = 5 - (int)node.Direction;
            if (tDir > 4 || tDir < 0) tDir = 0;
            dir = (NPC_DIRECTION)tDir;
        }
    }

    private Stack<MOVE_ROAD> moveRoad = new Stack<MOVE_ROAD>();
    private MOVE_ROAD curRoad;
    private bool IsWait = false;
    private bool IsBuy = false;
    [SerializeField]
    private AudioClip[] mWalkClip = null;
    [SerializeField]
    private AudioClip mAppearClip = null;
    [SerializeField]
    private AudioClip mDisAppearClip = null;
    private AudioSource mAudio;
    private int soundId = 0;

    // UI Set
    private HUD_GuestCondition mCondition = null;

    public delegate HUD_GuestCondition PopUpDel(NpcAI ai);
    public event PopUpDel PopupHandler;

    public delegate bool BuyFunc();
    public delegate bool WaitFunc();
    private BuyFunc endFunc;
    private WaitFunc waitFunc;

    public void Init(NpcInfo info)
    {
        tr = GetComponent<Transform>();
        mRenderer = GetComponent<SpriteRenderer>();
        Info = info;
        mAudio = GetComponent<AudioSource>();

        PopupHandler += HUD_Guest.GetUI;

        waitFunc = ChkBuyLocate;
    }

    private void OnEnable()
    {
        if (!mAudio) return;
        mAudio.clip = mAppearClip;
        mAudio.Play();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public bool SpawnNpc()
    {   
        Node _Start = GameManager.Instance.NpcStart;
        Node _End = null;

        List<Node> endList = GameManager.Instance.NpcEndList;
        for (int i = 0; i < endList.Count; i++)
        {
            if (!endList[i].IsClose)
            {
                _End = endList[i];
                break;
            }
        }

        if(_End == null)
        {
            List<Node> waitList = GameManager.Instance.NpcWaitList;
            for (int i = 0; i < waitList.Count; i++)
            {
                if (!waitList[i].IsClose)
                {
                    _End = waitList[i];
                    IsWait = true;
                    break;
                }
            }
        }

        if (_End == null) return false;

        FindMoveRoad(_Start, _End);
        _End.IsClose = true;

        return true;
    }

    private void Update()
    {
        MoveNpc();
    }

    private void LateUpdate()
    {
        if (direction != NPC_DIRECTION.IDLE)
            curTime += Time.deltaTime * frameSpeed;
        else
            curTime += Time.deltaTime;

        if (1.0f <= curTime)
        {
            spriteId++;
            if (spriteId >= Info.SpriteInfo[direction].Count - 1)
                spriteId = 0;

            curTime = 0;
            mRenderer.sprite = Info.SpriteInfo[direction][spriteId];
        }
    }

    private void ChangeDirection(NPC_DIRECTION _dir)
    {
        direction = _dir;
        switch (direction)
        {
            default:
                dir = Vector2.zero;
                break;
            case NPC_DIRECTION.UMOVE:
                dir = Vector2.up;
                break;
            case NPC_DIRECTION.LMOVE:
                dir = Vector2.left;
                break;
            case NPC_DIRECTION.RMOVE:
                dir = Vector2.right;
                break;
            case NPC_DIRECTION.DMOVE:
                dir = Vector2.down;
                break;
        }

        mRenderer.sprite = Info.SpriteInfo[direction][spriteId];
    }

    private void MoveNpc()
    {
        if (moveRoad.Count == 0) return;

        if ((tr.position - curRoad.pos).sqrMagnitude <= Mathf.Epsilon)
        {
            moveRoad.Pop();
            // 도착 했을 때 행동들 (대기, 구입 대기, 돌아감)
            if (moveRoad.Count == 0)
            {
                ChangeDirection(NPC_DIRECTION.IDLE);

                if (IsWait)
                    GameManager.Instance.WaitQueue.Enqueue(waitFunc);
                else 
                {
                    if (IsBuy || curRoad.node == GameManager.Instance.NpcStart)
                    {
                        StartCoroutine(DisappearCoroutine());
                    }
                    else
                    {
                        mCondition = PopupHandler(this);
                        if (mCondition)
                            mCondition.guestDoHandler += ShopOut;

                        mAudio.Stop();
                    }
                }

                return;
            }

            curRoad = moveRoad.Peek();
            ChangeDirection(curRoad.dir);
        }

        tr.position = Vector3.MoveTowards(tr.position, curRoad.pos, moveSpeed * Time.deltaTime);

        if(!mAudio.isPlaying)
        {
            mAudio.clip = mWalkClip[soundId];
            soundId++;
            if (soundId >= mWalkClip.Length)
                soundId = 0;
            mAudio.Play();
        }
    }

    public bool ChkBuyLocate()
    {
        List<Node> endList = GameManager.Instance.NpcEndList;
        Node end = null;
        for (int i = 0; i < endList.Count; i++)
        {
            if (!endList[i].IsClose)
            {
                end = endList[i];
                IsWait = false;
                break;
            }
        }

        if (end == null) return false;
        curRoad.node.ResetNode();
        curRoad.node.IsClose = false;
        FindMoveRoad(curRoad.node, end);
        end.IsClose = true;

        return true;
    }

    private void ShopOut()
    {
        if(moveRoad.Count == 0)
        {
            curRoad.node.ResetNode();
            curRoad.node.IsClose = false;
            FindMoveRoad(curRoad.node, GameManager.Instance.NpcStart);
        }
    }

    private void ResetNpc()
    {
        direction = NPC_DIRECTION.IDLE;
        moveRoad.Clear();
        IsWait = false;
        IsBuy = false;
        spriteId = 0;
        mCondition = null;
    }

    private void FindMoveRoad(Node _Start, Node _End)
    {
        List<Node> tRoad = PathFinder.PathFinding(_Start, _End);
        moveRoad.Clear();

        for (int i = 0; i < tRoad.Count; i++)
        {
            MOVE_ROAD temp = new MOVE_ROAD(tRoad[i]);
            moveRoad.Push(temp);
            tRoad[i].ResetNode();
        }

        curRoad = moveRoad.Peek();
        ChangeDirection(curRoad.dir);
        mRenderer.sprite = Info.SpriteInfo[direction][spriteId];
        tr.position = curRoad.pos;
    }

    private IEnumerator DisappearCoroutine()
    {
        WaitUntil wu = new WaitUntil(() => !mAudio.isPlaying);

        ResetNpc();
        mAudio.clip = mDisAppearClip;
        mAudio.Play();

        yield return wu;
        gameObject.SetActive(false);
    }
}
