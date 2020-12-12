using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftMenu : MenuController, IPointerDownHandler
{
    public enum CRAFT_TYPE
    {
        FORGE,
        ALCHMIC,
        SEWING,
        NONE
    }

    [SerializeField]
    private CRAFT_TYPE mType = CRAFT_TYPE.NONE;
    private List<CraftLine> mCraftLine = new List<CraftLine>();
    private SelectItem mSelect;
    public Dictionary<int, List<Items>> CraftingList { get; private set; } = new Dictionary<int, List<Items>>();
    public CRAFT_TYPE Type { get { return mType; } }

    public override void Init()
    {
        base.Init();

        mCraftLine = GetComponentsInChildren<CraftLine>().ToList();

        for (int i = 0; i < mCraftLine.Count; i++)
        {
            mCraftLine[i].Init();
        }

        mSelect = transform.Find("SelectWindow").GetComponent<SelectItem>();
        mSelect.Init();
        MakeCraftingList();
    }

    private void MakeCraftingList()
    {
        CraftingList = GameManager.Instance.ObjPool.GetItems(mType);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!mSelect.gameObject.activeSelf) return;

        Vector2 mPos = eventData.position;
        RectTransform selectTr = mSelect.GetComponent<RectTransform>();
        Vector2 min = new Vector2(selectTr.position.x + selectTr.rect.xMin, selectTr.position.y + selectTr.rect.yMin);
        Vector2 max = new Vector2(selectTr.position.x + selectTr.rect.xMax, selectTr.position.y + selectTr.rect.yMax);

        if (mPos.x >= min.x && mPos.x <= max.x &&
            mPos.y >= min.y && mPos.y <= max.y)
            return;

        mSelect.HideWindow();
    }

    public override void CloseMenu()
    {
        base.CloseMenu();
        mSelect.HideWindow();
    }
}
