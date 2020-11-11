﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketController : MonoBehaviour
{
    Transform marketStartPos;
    Transform marketInvStartPos;

    GameController gc;
    public InventoryHandler ih;
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

    public gStats sg;

    // Start is called before the first frame update
    void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        sg = GameObject.FindGameObjectWithTag("stats").GetComponent<gStats>();
        unitSelectPanel = GameObject.Find("UnitSelectPanel");
        marketSlots = new List<MarketSlot>();
        storedMarketUnits = new List<bUnit>();
        invSlots = new List<MarketSlot>();

        marketStartPos = GameObject.Find("MarketStartLoc").transform;
        marketInvStartPos = GameObject.Find("MarketInvStartLoc").transform;
        
    }

    public void initMarketPhase(BaseLevel level)
    {
        if (ih == null)
        {
            ih = gc.ih;
        }

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

        for (int i = 0; i < InventoryHandler.maxInvSlots; i++)
        {
            MarketSlot invSlot = Instantiate(marketInvSlot, invPos, Quaternion.identity).GetComponent<MarketSlot>();
            invSlot.isInvSlot = true;
            if (i < fullCount)
            {
                invSlot.assignUnit(ih.unitInventory[i]);
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
            marketSlots.Add(tempSlot);

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
        storedMarketUnits.Clear();
    }

    public void loadStoredMarket()
    {
        Vector3 slotPos = marketStartPos.position;
        float origX = slotPos.x;
        for(int i = 0; i < storedMarketUnits.Count; i++)
        {
            MarketSlot tempSl = Instantiate(marketSlot, slotPos, Quaternion.identity).GetComponent<MarketSlot>();
            if (storedMarketUnits[i] != null)
            {
                tempSl.assignUnit(storedMarketUnits[i]);
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

    public void endMarketPhase()
    {
        //storeMarket();

        unitSelectPanel.SetActive(true);
        gc.curPhase = GamePhase.Prep;

        gc.refreshUnitButtons();

        //resetMarket();
        
    }

    public void resetMarket()
    {
        foreach (MarketSlot sl in marketSlots)
        {
            //emptyMarketSlot(sl);
            Destroy(sl.gameObject);
        }
        for (int i = 0; i < Mathf.Min(invSlots.Count, InventoryHandler.maxInvSlots); i++)
        {
            
            Destroy(invSlots[i].gameObject);
        }
        marketSlots.Clear();
        invSlots.Clear();
    }

    public void addToInv(bUnit unit)
    {
        invSlots[curInvCount].assignUnit(unit);
        curInvCount++;
    }

    public void emptyMarketSlot(MarketSlot sl)
    {
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
                sl.assignUnit(new bUnit(levelFrom.marketUnits[i]));
                break;
            }
        }
    }

    void chooseInfUnitFor(MarketSlot sl, GenLevel levelFrom)
    {
        int choice = Random.Range(0, chancesTotal);
        int total = 0;
        bool chosenYet = false;
        for (int i = 0; i < levelFrom.availableInMarket.Length; i++)
        {
            total += levelFrom.marketUnitChances[i];
            if (choice < total)
            {
                sl.assignUnit(new bUnit(levelFrom.availableInMarket[i]));
                chosenYet = true;
                break;
            }
        }
        if (!chosenYet)
        {
            Debug.LogError("ERROR: Did not choose unit for slot");
        }
    }

}
