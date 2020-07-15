using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketController : MonoBehaviour
{
    Transform marketStartPos;
    Transform marketInvStartPos;

    GameController gc;
    InventoryHandler ih;
    GameObject unitSelectPanel;

    public GameObject marketSlot;
    public GameObject marketInvSlot;
    public GameObject unitObj;

    int chancesTotal;

    List<MarketSlot> marketSlots;
    List<bUnit> storedMarketUnits;
    public List<MarketSlot> invSlots;

    const float itemWidth = 0.5f;
    const float itemHeight = 0.5f;
    const int rowCount = 4;

    public int curInvCount;

    gStats sg;

    // Start is called before the first frame update
    void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        sg = GameObject.FindGameObjectWithTag("stats").GetComponent<gStats>();
        unitSelectPanel = GameObject.Find("UnitSelectPanel");
        marketSlots = new List<MarketSlot>();
        storedMarketUnits = new List<bUnit>();
        invSlots = new List<MarketSlot>();
        //storedUnitObjs = new List<GameObject>();
        marketStartPos = GameObject.Find("MarketStartLoc").transform;
        marketInvStartPos = GameObject.Find("MarketInvStartLoc").transform;
        
    }

    public void initMarketPhase(BaseLevel level)
    {
        if (ih == null)
        {
            ih = gc.ih;
        }
        //unitSelectPanel.SetActive(false);
        //gc.curPhase = GamePhase.Market;
        //clearStored();

        chancesTotal = 0;
        if (sg.isInfinite)
        {
            foreach (int item in gc.curLevel.marketUnitChances)
            {
                chancesTotal += item;
            }
        }
        else
        {
            foreach (int item in level.marketChances)
            {
                chancesTotal += item;
            }
        }

        int slotCount;
        if (sg.isInfinite)
        {
            slotCount = gc.curLevel.numShopSlots;
        }
        else
        {
            slotCount = level.numMarketSlots;
        }
        if (storedMarketUnits.Count <= 0)
        {
            chooseAvailable(slotCount, level);
        }
        else
        {
            loadStoredMarket();
        }

        refreshInventory();
    }

    public void refreshInventory()
    {
        //inventory slots in market.
        Vector3 invPos = marketInvStartPos.position;
        float origX = invPos.x;
        int fullCount = ih.unitInventory.Count;
        if (invSlots.Count > 0)
        {
            for (int i = 0; i < InventoryHandler.maxInvSlots; i++)
            {
                Destroy(invSlots[i].gameObject);
            }
        }
        invSlots.Clear();

        //TODO: Clean this shit up. It's gross af.
        for (int i = 0; i < InventoryHandler.maxInvSlots; i++)
        {
            MarketSlot invSlot = Instantiate(marketInvSlot, invPos, Quaternion.identity).GetComponent<MarketSlot>();
            invSlot.isInvSlot = true;
            /*if(i < storedUnitObjs.Count)
            {
                if (i > fullCount-1)
                {
                    Destroy(storedUnitObjs[i]);
                }
                else
                {
                    invSlot.storedUnit = storedUnitObjs[i].GetComponent<Unit>();
                }
            }else */
            if (i < fullCount)
            {
                invSlot.assignUnit(ih.unitInventory[i]);/*Instantiate(unitObj, invSlot.transform.GetChild(0).transform.position, Quaternion.identity).GetComponent<Unit>();
                invSlot.storedUnit.copyIn(ih.unitInventory[i]);
                invSlot.storedUnit.myMarketSlot = invSlot;*/
            }
            invSlots.Add(invSlot);

            if ((i + 1) % rowCount != 0)
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

    void chooseAvailable(int slotCount, BaseLevel level)
    {
        Vector3 slotPos = marketStartPos.position;
        float origX = slotPos.x;
        for (int i = 0; i < slotCount; i++)
        {
            MarketSlot tempSlot = Instantiate(marketSlot, slotPos, Quaternion.identity).GetComponent<MarketSlot>();
            tempSlot.isInvSlot = false;
            if (!sg.isInfinite)
            {
                chooseUnitFor(tempSlot, level);
            }
            else
            {
                chooseInfUnitFor(tempSlot, gc.curLevel);
            }
            Debug.Log("adding slot to slots list");
            marketSlots.Add(tempSlot);

            //MOVING STORAGE TO OCCUR ON END MARKET PHASE
            /*MarketSlot forStorage = Instantiate(tempSlot.gameObject, new Vector3(100, 100, 100), Quaternion.identity).GetComponent<MarketSlot>();
            forStorage.storedUnit = Instantiate(tempSlot.storedUnit.gameObject, new Vector3(100, 100, 100), Quaternion.identity).GetComponent<Unit>();
            forStorage.storedUnit.myMarketSlot = forStorage;
            storedMarketSlots.Add(forStorage);*/

            if (((i + 1) % rowCount) != 0)
            {
                slotPos.x -= itemWidth;
            }
            else
            {
                slotPos.x = origX;
                slotPos.y -= itemHeight;
            }
        }
    }

    public void emptyStoredMarket()
    {
        /*foreach (MarketSlot mk in storedMarketSlots)
        {
            *//*if(mk.storedUnit != null)
            {
                Destroy(mk.storedUnit.gameObject);
            }*//*
            Destroy(mk.gameObject);
        }*/
        storedMarketUnits.Clear();
    }

    public void loadStoredMarket()
    {
        Vector3 slotPos = marketStartPos.position;
        float origX = slotPos.x;
        //foreach (MarketSlot mk in storedMarketSlots)
        for(int i = 0; i < storedMarketUnits.Count; i++)
        {
            MarketSlot tempSl = Instantiate(marketSlot, slotPos, Quaternion.identity).GetComponent<MarketSlot>();
            if (storedMarketUnits[i] != null)
            {
                tempSl.assignUnit(storedMarketUnits[i]);/*Instantiate(storedMarketSlots[i].storedUnit.gameObject, tempSl.transform.GetChild(0).transform.position, Quaternion.identity).GetComponent<Unit>();
                tempSl.storedUnit.myMarketSlot = tempSl;*/
            }
            else
            {
                tempSl.emptyUnit();
            }
            tempSl.isInvSlot = false;

            marketSlots.Add(tempSl);

            if (((i + 1) % rowCount) != 0)
            {
                slotPos.x -= itemWidth;
            }
            else
            {
                slotPos.x = origX;
                slotPos.y -= itemHeight;
            }
        }
        emptyStoredMarket();
    }

    void storeMarket()
    {
        //MIGHT NOT NEED THIS
        storedMarketUnits.Clear();
        for(int i = 0; i < marketSlots.Count; i++)
        {
            //MarketSlot tempSlot = marketSlots[i];
            //MarketSlot forStorage = Instantiate(tempSlot.gameObject, new Vector3(100, 100, 100), Quaternion.identity).GetComponent<MarketSlot>();
            /*if (tempSlot.storedUnit != null)
            {
                forStorage.storedUnit = Instantiate(tempSlot.storedUnit.gameObject, new Vector3(100, 100, 100), Quaternion.identity).GetComponent<Unit>();
                forStorage.storedUnit.myMarketSlot = forStorage;
            }*/
            storedMarketUnits.Add(marketSlots[i].storedUnit);
            if (marketSlots[i].storedUnit == null)
            {
                Debug.Log("storing null");
            }
        }
    }

    public void endMarketPhase()
    {
        //storeMarket();

        unitSelectPanel.SetActive(true);
        gc.curPhase = GamePhase.Prep;

        gc.refreshUnitButtons();

        //resetMarket();
        
    }

    /*public void clearStored()
    {
        foreach(GameObject item in storedUnitObjs)
        {
            Destroy(item);
        }
        storedUnitObjs.Clear();
    }*/

    public void resetMarket()
    {
        foreach (MarketSlot sl in marketSlots)
        {
            //emptyMarketSlot(sl);
            Destroy(sl.gameObject);
        }
        /*clearStored();
        storedUnitObjs.Clear();*/
        //foreach (MarketSlot sl in invSlots)
        for (int i = 0; i < Mathf.Min(invSlots.Count, InventoryHandler.maxInvSlots); i++)
        {
            /*if (invSlots.Count > i && invSlots[i].storedUnit != null)
            {
                Destroy(invSlots[i].storedUnit.gameObject);
                //storedUnitObjs.Add(invSlots[i].storedUnit.gameObject);
                //ih.unitInventory[i] = storedUnitObjs[i].GetComponent<Unit>();
            }*/
            
            Destroy(invSlots[i].gameObject);
        }
        marketSlots.Clear();
        invSlots.Clear();
    }

    public void addToInv(bUnit unit)
    {
        /*GameObject tempUnitObj = Instantiate(unitObj, invSlots[curInvCount].transform.GetChild(0).transform.position, Quaternion.identity);
        tempUnitObj.GetComponent<Unit>().copyIn(unit);*/
        invSlots[curInvCount].assignUnit(unit);
        curInvCount++;
    }

    public void emptyMarketSlot(MarketSlot sl)
    {
        /*if (sl.storedUnit != null && sl.storedUnit.gameObject != null)
        {
            Destroy(sl.storedUnit.gameObject);
        }*/
        sl.emptyUnit();
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
                /*Vector3 pos = sl.transform.GetChild(0).transform.position;
                //Debug.Log("unit placement is " + sl.unitPlacement.x + "," + sl.unitPlacement.y + "," + sl.unitPlacement.z);
                sl.storedUnit = Instantiate(unitObj, pos, Quaternion.identity).GetComponent<Unit>();
                sl.storedUnit.assignType(levelFrom.marketUnits[i]);
                sl.storedUnit.myMarketSlot = sl;
                sl.gameObject.transform.GetChild(1).transform.GetChild(0).GetComponent<TextMesh>().text = "" + sl.storedUnit.curBuyCost;*/
                sl.assignUnit(new bUnit(levelFrom.marketUnits[i]));
                break;
            }
        }
    }

    void chooseInfUnitFor(MarketSlot sl, GenLevel levelFrom)
    {
        int choice = Random.Range(0, chancesTotal);
        int total = 0;
        for (int i = 0; i < levelFrom.numShopSlots; i++)
        {
            total += levelFrom.marketUnitChances[i];
            if (choice < total)
            {
                /*Vector3 pos = sl.transform.GetChild(0).transform.position;
                //Debug.Log("unit placement is " + sl.unitPlacement.x + "," + sl.unitPlacement.y + "," + sl.unitPlacement.z);
                sl.storedUnit = Instantiate(unitObj, pos, Quaternion.identity).GetComponent<Unit>();
                sl.storedUnit.assignType(levelFrom.availableInMarket[i]);
                sl.storedUnit.myMarketSlot = sl;
                sl.gameObject.transform.GetChild(1).transform.GetChild(0).GetComponent<TextMesh>().text = "" + sl.storedUnit.curBuyCost;*/
                sl.assignUnit(new bUnit(levelFrom.availableInMarket[i]));
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
