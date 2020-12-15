using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class PlayerData
{
    public int Gold;
    public int Repute;
    public int ShopLv;
    public int ForgeLv;
    public int AlchmicLv;
    public int SewingLv;
}

public class ResourceManager : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKey(KeyCode.Q))
            SaveData();

        if (Input.GetKey(KeyCode.E))
            LoadData();
    }

    private void SaveData()
    {
        Player pl = GameObject.Find("Player").GetComponent<Player>();
        PlayerData plData = new PlayerData();
        plData.Gold = pl.MyGold;
        plData.Repute = pl.MyRepute;
        plData.ShopLv = Player.ShopLv;
        plData.ForgeLv = Player.ForgeLv;
        plData.AlchmicLv = Player.AlchmicLv;
        plData.SewingLv = Player.SewingLv;

        string jsonData = JsonUtility.ToJson(plData);

        string path = string.Format("Assets/Resources/SaveData/Player.json");
        FileInfo file = new FileInfo(path);
        file.Directory.Create();
        File.WriteAllText(file.FullName, jsonData);
    }

    private void LoadData()
    {
        FileInfo loadFile = new FileInfo("Assets/Resources/SaveData/Player.json");
        if(!loadFile.Exists)
        {
            Debug.LogError("파일 없음");
            return;
        }

        string sJsonData = File.ReadAllText(loadFile.FullName);

        PlayerData playerData = JsonUtility.FromJson<PlayerData>(sJsonData);
        Player pl = GameObject.Find("Player").GetComponent<Player>();
        pl.LoadData(playerData);
    }
}

/*
 * Player
 * - Gold, Repute, ShopLv, ForgeLv, AlchmicLv, SewingLv;
 * 
 * Inventory
 * - InvenSlot
 *  - Item, Count
 *      - name, Value, mIsUnLock
 * 
 * PuzzleRoad
 * - PuzzleBlock
 *  - Item, active
 *  
 * - Node
 *  - ID, IsClose
 * 
 * Npc
 *  - MoveRoad, CurRoad
 *  - IsWait, IsBuy
 *  - Transform.position
 *  
 */