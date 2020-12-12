using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    #region ItemRespawn
    private Queue<Items> mSpawnItem = new Queue<Items>();
    private float mItemCurTime = 0f;
    private float mItemSpwanTime = 3f;
    private int maxItemCount = 50;
    #endregion

    #region BlockRespawn
    private List<PuzzleBlock> mSpawnBlock = new List<PuzzleBlock>();
    private float mBlockCurTime = 0f;
    [SerializeField]
    private float mBlockSpawnTime = 10f;
    #endregion

    #region NPCRespawn
    private float mNpcCurTime = 0f;
    [SerializeField]
    private float mMaxNpcSpawnTime = 0f;
    private float mNpcSpawnTime = 0f;
    #endregion

    private void Update()
    {
        ResapwnItem();
        RespawnBlock();
        RespawnNpc();
    }

    private void ResapwnItem()
    {
        mItemCurTime += Time.deltaTime;
        if (mItemCurTime < mItemSpwanTime) return;
        if (maxItemCount < mSpawnItem.Count) return;

        mSpawnItem.Enqueue(GameManager.Instance.ObjPool.GetRandomItem(Items.ITEM_TYPE.BASEMATERIAL));
        mItemCurTime = 0f;
    }

    public void RegistSpawnBlock(PuzzleBlock block)
    {
        if (mSpawnBlock.Contains(block)) return;

        mSpawnBlock.Add(block);
    }

    private void RespawnBlock()
    {
        if (mSpawnBlock.Count == 0) return;
        mBlockCurTime += Time.deltaTime;
        if (mBlockCurTime < mBlockSpawnTime) return;

        PuzzleBlock block = mSpawnBlock[Random.Range(0, mSpawnBlock.Count)];
        block.Item = mSpawnItem.Peek();
        mSpawnItem.Dequeue();
        block.itemImg.sprite = block.Item.Sprite;
        block.gameObject.SetActive(true);
        mSpawnBlock.Remove(block);

        mBlockCurTime = 0f;
    }

    private void RespawnNpc()
    {
        if (mNpcSpawnTime == 0)
            mNpcSpawnTime = Random.Range(10, mMaxNpcSpawnTime);

        mNpcCurTime += Time.deltaTime;
        if (mNpcCurTime < mNpcSpawnTime) return;

        GameObject npc = GameManager.Instance.ObjPool.GetRandomNpc();
        NpcAI ai = npc.GetComponent<NpcAI>();
        if (ai.SpawnNpc())
            npc.SetActive(true);
        
        mNpcCurTime = 0f;
        mNpcSpawnTime = 0f;
    }
}
