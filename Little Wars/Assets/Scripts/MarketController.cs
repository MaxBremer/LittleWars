using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketController : MonoBehaviour
{
    Transform marketStartPos;
    Transform marketInvStartPos;
    BaseLevel myLevel;
    GameController gc;
    InventoryHandler ih;
    GameObject unitSelectPanel;

    public GameObject marketSlot;
    public GameObject marketInvSlot;
    public GameObject unitObj;

    int chancesTotal;

    List<MarketSlot> marketSlots;
    public List<MarketSlot> invSlots;
    List<GameObject> storedUnitObjs;

    const float itemWidth = 0.5f;
    const float itemHeight = 0.5f;
    const int rowCount = 4;

    public int curInvCount;

    // Start is called before the first frame update
    void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        
        unitSelectPanel = GameObject.Find("UnitSelectPanel");
        marketSlots = new List<MarketSlot>();
        invSlots = new List<MarketSlot>();
        storedUnitObjs = new List<GameObject>();
        marketStartPos = GameObject.Find("MarketStartLoc").transform;
        marketInvStartPos = GameObject.Find("MarketInvStartLoc").transform;
        
    }

    public void initMarketPhase(BaseLevel level)
    {
        ih = gc.ih;
        unitSelectPanel.SetActive(false);
        gc.curPhase = GamePhase.Market;
        clearStored();

        chancesTotal = 0;
        foreach (int item in level.marketChances)
        {
            chancesTotal += item;
        }

        myLevel = level;
        Vector3 slotPos = marketStartPos.position;
        float origX = slotPos.x;
        for(int i = 0; i < level.numMarketSlots; i++)
        {
            MarketSlot tempSlot = Instantiate(marketSlot, slotPos, Quaternion.identity).GetComponent<MarketSlot>();
            tempSlot.isInvSlot = false;
            chooseUnitFor(tempSlot, level);
            marketSlots.Add(tempSlot);
            
            if(((i + 1) % rowCount) != 0)
            {
                slotPos.x -= itemWidth;
            }
            else
            {
                slotPos.x = origX;
                slotPos.y -= itemHeight;
            }
        }

        Vector3 invPos = marketInvStartPos.position;
        origX = invPos.x;
        int fullCount = ih.unitInventory.Count;
        if (invSlots.Count > 0)
        {
            for (int i = 0; i < InventoryHandler.maxInvSlots; i++)
            {
                Destroy(invSlots[i].gameObject);
            }
        }
        invSlots.Clear();
        for(int i = 0; i < InventoryHandler.maxInvSlots; i++)
        {
            MarketSlot invSlot = Instantiate(marketInvSlot, invPos, Quaternion.identity).GetComponent<MarketSlot>();
            invSlot.isInvSlot = true;
            if(i < storedUnitObjs.Count)
            {
                if (i > fullCount-1)
                {
                    Destroy(storedUnitObjs[i]);
                }
                else
                {
                    invSlot.storedUnit = storedUnitObjs[i].GetComponent<Unit>();
                }
            }else if(i < fullCount)
            {
                invSlot.storedUnit = Instantiate(unitObj, invSlot.transform.GetChild(0).transform.position, Quaternion.identity).GetComponent<Unit>();
            }
            if (i < fullCount)
            {
                invSlot.storedUnit.copyIn(ih.unitInventory[i]);
                invSlot.storedUnit.myMarketSlot = invSlot;
            }
            invSlots.Add(invSlot);

            if((i + 1) % rowCount != 0)
            {
                invPos.x -= itemWidth;
            }
            else
            {
                invPos.x = origX;
                invPos.y -= itemHeight;
            }
        }
        curInvCount = fullCount;
    }

    public int removedBeforeMe(int slotInd)
    {
        int total = 0;
        for(int i = 0; i < slotInd; i++)
        {
            if(invSlots[i].storedUnit == null)
            {
                total++;
            }
        }
        return total;
    }

    public void endMarketPhase()
    {
        unitSelectPanel.SetActive(true);
        gc.curPhase = GamePhase.Prep;

        gc.refreshUnitButtons();

        resetMarket();
        
    }

    public void clearStored()
    {
        foreach(GameObject item in storedUnitObjs)
        {
            Destroy(item);
        }
        storedUnitObjs.Clear();
    }

    public void resetMarket()
    {
        foreach (MarketSlot sl in marketSlots)
        {
            emptyMarketSlot(sl);
            Destroy(sl.gameObject);
        }
        clearStored();
        storedUnitObjs.Clear();
        //foreach (MarketSlot sl in invSlots)
        for (int i = 0; i < Mathf.Min(invSlots.Count, InventoryHandler.maxInvSlots); i++)
        {
            if (invSlots.Count > i && invSlots[i].storedUnit != null)
            {
                Destroy(invSlots[i].storedUnit.gameObject);
                //storedUnitObjs.Add(invSlots[i].storedUnit.gameObject);
                //ih.unitInventory[i] = storedUnitObjs[i].GetComponent<Unit>();
            }
            
            Destroy(invSlots[i].gameObject);
        }
        marketSlots.Clear();
        invSlots.Clear();
    }

    public void addToInv(Unit unit)
    {
        GameObject tempUnitObj = Instantiate(unitObj, invSlots[curInvCount].transform.GetChild(0).transform.position, Quaternion.identity);
        tempUnitObj.GetComponent<Unit>().copyIn(unit);
        invSlots[curInvCount].storedUnit = tempUnitObj.GetComponent<Unit>();
        invSlots[curInvCount].storedUnit.myMarketSlot = invSlots[curInvCount];
        curInvCount++;
    }

    public void emptyMarketSlot(MarketSlot sl)
    {
        if (sl.storedUnit != null && sl.storedUnit.gameObject != null)
        {
            Destroy(sl.storedUnit.gameObject);
        }
        sl.storedUnit = null;
    }

    void chooseUnitFor(MarketSlot sl, BaseLevel levelFrom)
    {
        int choice = Random.Range(0, chancesTotal);
        int total = 0;
        for(int i = 0; i < levelFrom.marketUnits.Length; i++)
        {
            total += levelFrom.marketChances[i];
            if(choice < total)
            {
                Vector3 pos = sl.transform.GetChild(0).transform.position;
                //Debug.Log("unit placement is " + sl.unitPlacement.x + "," + sl.unitPlacement.y + "," + sl.unitPlacement.z);
                sl.storedUnit = Instantiate(unitObj, pos, Quaternion.identity).GetComponent<Unit>();
                sl.storedUnit.assignType(levelFrom.marketUnits[i]);
                sl.storedUnit.myMarketSlot = sl;
                sl.gameObject.transform.GetChild(1).transform.GetChild(0).GetComponent<TextMesh>().text = "" + sl.storedUnit.curBuyCost;
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
