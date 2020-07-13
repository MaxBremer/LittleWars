using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryHandler : MonoBehaviour
{
    GameController gc;
    public List<Unit> unitInventory;
    public List<Unit> invBackup;
    public List<GameObject> buttonList;

    public GameObject basicUnit;

    public const int maxInvSlots = 8;

    void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        unitInventory = new List<Unit>();
        buttonList = new List<GameObject>();
    }

    public void addToInv(Unit unit)
    {
        //unitInventory.Add(unit);

        GameObject temp = Instantiate(basicUnit, new Vector3(100, 100, 100), Quaternion.identity);
        temp.GetComponent<Unit>().copyIn(unit);
        unitInventory.Add(temp.GetComponent<Unit>());
        //addToBackupInv(unit);
    }

    public void addToBackupInv(Unit unit)
    {
        GameObject temp = Instantiate(basicUnit, new Vector3(100, 100, 100), Quaternion.identity);
        temp.GetComponent<Unit>().copyIn(unit);
        invBackup.Add(temp.GetComponent<Unit>());
    }

    public void clearBackup()
    {
        foreach (Unit item in invBackup)
        {
            Destroy(item.gameObject);
        }
        invBackup.Clear();
    }

    public void clearInv()
    {
        foreach(Unit item in unitInventory)
        {
            Destroy(item.gameObject);
        }
        unitInventory.Clear();
    }

    public void removeFromInv(int index)
    {
        unitInventory.RemoveAt(index);
    }

    public void removeFromInv(Unit unit)
    {
        unitInventory.Remove(unit);
    }

    public void initButtons(Vector3 buttonPos, Transform parentTransform, GameObject spawnButton)
    {
        for (int i = 0; i < unitInventory.Count; i++)
        {
            GameObject button = GameObject.Instantiate(spawnButton, buttonPos, Quaternion.identity);
            
            button.transform.GetChild(0).GetComponent<Text>().text = unitInventory[i].myName;
            button.transform.SetParent(parentTransform);
            button.GetComponent<SpawnButton>().myUnit = unitInventory[i];
            buttonList.Add(button);
            buttonPos.y -= 40;
        }
    }
    public void clearButtons()
    {
        foreach (GameObject button in buttonList)
        {
            GameObject.Destroy(button);
        }
        buttonList.Clear();
    }
    void Update()
    {
    }
}
