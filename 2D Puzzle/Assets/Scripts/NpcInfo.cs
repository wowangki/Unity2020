using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPC", menuName = "Scriptable Object/NPC", order = int.MaxValue)]
public class NpcInfo : ScriptableObject
{
    [SerializeField] private GameObject mPrefab = null;
    [SerializeField] private string mNpcName = null;
    [SerializeField] private List<Sprite> mUSprite = new List<Sprite>();
    [SerializeField] private List<Sprite> mDSprite = new List<Sprite>();
    [SerializeField] private List<Sprite> mLSprite = new List<Sprite>();
    [SerializeField] private List<Sprite> mRSprite = new List<Sprite>();
    [SerializeField] private float mAngryValue = 0f;

    public GameObject Prefab { get { return mPrefab; } }
    public string NpcName { get { return mNpcName; } }
    public Dictionary<NpcAI.NPC_DIRECTION, List<Sprite>> SpriteInfo { get; set; } = new Dictionary<NpcAI.NPC_DIRECTION, List<Sprite>>();
    public float AngryValue { get { return mAngryValue; } }
    //public bool IsAngry { get; set; }

    public void Init()
    {
        for (NpcAI.NPC_DIRECTION i = 0; i <= NpcAI.NPC_DIRECTION.DMOVE; i++)
        {
            switch (i)
            {
                default:
                case NpcAI.NPC_DIRECTION.IDLE:
                case NpcAI.NPC_DIRECTION.DMOVE:
                    SpriteInfo.Add(i, mDSprite);
                    break;
                case NpcAI.NPC_DIRECTION.UMOVE:
                    SpriteInfo.Add(i, mUSprite);
                    break;
                case NpcAI.NPC_DIRECTION.RMOVE:
                    SpriteInfo.Add(i, mRSprite);
                    break;
                case NpcAI.NPC_DIRECTION.LMOVE:
                    SpriteInfo.Add(i, mLSprite);
                    break;
            }
        }
    }
}
