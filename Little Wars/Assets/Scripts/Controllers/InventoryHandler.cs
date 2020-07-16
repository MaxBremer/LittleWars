using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryHandler : MonoBehaviour
{
    GameController gc;
    public List<bUnit> unitInventory;
    public List<bUnit> invBackup;
    public List<GameObject> buttonList;

    public GameObject basicUnit;

    public const int maxInvSlots = 8;

    void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        unitInventory = new List<bUnit>();
        invBackup = new List<bUnit>();
        buttonList = new List<GameObject>();
    }

    public void addToInv(bUnit unit)
    {
        //unitInventory.Add(unit);

        /*GameObject temp = Instantiate(basicUnit, new Vector3(100, 100, 100), Quaternion.identity);
        temp.GetComponent<Unit>().copyIn(unit);
        unitInventory.Add(temp.GetComponent<Unit>());*/

        unitInventory.Add(unit);

        //addToBackupInv(unit);
    }

    /*public void addToBackupInv(Unit unit)
    {
        GameObject temp = Instantiate(basicUnit, new Vector3(100, 100, 100), Quaternion.identity);
        temp.GetComponent<Unit>().copyIn(unit);
        invBackup.Add(temp.GetComponent<Unit>());
    }*/

    public void backupInventory()
    {
        invBackup.Clear();
        invBackup.AddRange(unitInventory);
    }

    public void clearBackup()
    {
        invBackup.Clear();
    }

    public void loadBackup()
    {
        unitInventory.Clear();
        unitInventory.AddRange(invBackup);
    }

    public void clearInv()
    {
        /*foreach(Unit item in unitInventory)
        {
            Destroy(item.gameObject);
        }*/
        unitInventory.Clear();
    }

    public void removeFromInv(int index)
    {
        unitInventory.RemoveAt(index);
    }

    public void removeFromInv(bUnit unit)
    {
        unitInventory.Remove(unit);
    }

    public void initButtons(Vector3 buttonPos, Transform parentTransform, GameObject spawnButton)
    {
        for (int i = 0; i < unitInventory.Count; i++)
        {
            GameObject button = GameObject.Instantiate(spawnButton, buttonPos, Quaternion.identity);
            
            button.transform.GetChild(0).GetComponent<Text>().text = unitInventory[i].myName + "\n" + unitInventory[i].curAtk + "," + unitInventory[i].curDef + "," + unitInventory[i].curHealth;
            button.transform.SetParent(parentTransform);
            button.GetComponent<SpawnButton>().myUnit = unitInventory[i];
            buttonList.Add(button);
            buttonPos.y -= 60;
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
