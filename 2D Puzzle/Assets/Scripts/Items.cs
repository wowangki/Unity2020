using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Object/Item", order = int.MaxValue)]
public class Items : ScriptableObject
{
    public enum ITEM_KIND
    {
        RESOURCE,
        CRAFTING
    }

    public enum ITEM_TYPE
    {
        BASEMATERIAL,
        MATERIAL,
        WEAPON,
        ARMOR,
        TOOL
    }
    
    public enum ITEM_DETAILTYPE
    {
        ORE,
        GEMSTONE,
        WOOD,
        TOOL,
        BONE,
        MEDICALHURB,
        CLOTH,
        LEATHER,
        SWORD,
        SPEAR,
        BOW,
        AXE,
        SHIELD,
        MEDICINE,
        GEM,
        HELM,
        ARMOR,
        GLOVE,
        SHOES
    }

    [System.Serializable]
    public class ItemRecipe
    {
        public ITEM_DETAILTYPE[] Recipe = null;
        public Items[] SpecialRecipe = null;
    }

    [SerializeField] private ITEM_KIND mKind = ITEM_KIND.RESOURCE;
    [SerializeField] private ITEM_TYPE mType = ITEM_TYPE.BASEMATERIAL;
    [SerializeField] private ITEM_DETAILTYPE mDetail = ITEM_DETAILTYPE.ORE;
    [SerializeField] private Sprite mSprite = null;
    [SerializeField] private string mItemName = null;
    [SerializeField] private int mTier = 0;
    [SerializeField] private int mOriginValue = 0;
    [SerializeField] private int mValue = 0;
    [SerializeField] private CraftMenu.CRAFT_TYPE mCraftPlace = CraftMenu.CRAFT_TYPE.NONE;
    [SerializeField] private bool mIsUnlock = true;
    [SerializeField] private ItemRecipe[] mRecipe = null;

    public ITEM_KIND Kind { get { return mKind; } }
    public ITEM_TYPE Type { get { return mType; } }
    public ITEM_DETAILTYPE DetailType { get { return mDetail; } }
    public Sprite Sprite { get { return mSprite; } }
    public string ItemName { get { return mItemName; } }
    public int Tier { get { return mTier; } }
    public int Value { get { return mValue; } set { mValue = value; } }
    public CraftMenu.CRAFT_TYPE CraftPlace { get { return mCraftPlace; } }
    public bool UnLock { get { return mIsUnlock; } set { mIsUnlock = value; } }
    public bool IsUse { get; set; } = false;
    public ItemRecipe[] Recipe { get { return mRecipe; } }
    
    public void Init()
    {
        mValue = mOriginValue;
    }

    public void ResetItem()
    {
        IsUse = false;
        if (mOriginValue != mValue)
            mValue = mOriginValue;
    }
}