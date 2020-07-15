using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketSlot : MonoBehaviour
{
    public int buyPrice;
    public bUnit storedUnit;
    public Material altMaterial;
    public Vector3 unitPlacement;
    public bool isInvSlot;
    Material defaultMaterial;

    public UnitMesh myUnitMesh;
    public GameObject myCostIcon;

    GameController gc;

    // Start is called before the first frame update
    void Start()
    {
        unitPlacement = gameObject.transform.GetChild(0).transform.position;
        defaultMaterial = gameObject.GetComponent<MeshRenderer>().material;

        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    /*void OnMouseEnter()
    {
        mouseOverFunc();
    }*/

    public void assignUnit(bUnit unit)
    {
        if (myUnitMesh == null)
        {
            myUnitMesh = transform.GetChild(0).GetComponent<UnitMesh>();
            myUnitMesh.transform.localScale = new Vector3(.5f, .5f, .5f);
        }
        myUnitMesh.gameObject.SetActive(true);
        
        
        storedUnit = unit;

        myUnitMesh.myUnit = unit;
        myUnitMesh.initUnitRenderComponents();
        myUnitMesh.myMarketSlot = this;

        if (!isInvSlot)
        {
            if (myCostIcon == null)
            {
                myCostIcon = transform.GetChild(1).gameObject;
            }
            myCostIcon.SetActive(true);
            myCostIcon.transform.GetChild(0).GetComponent<TextMesh>().text = "" + unit.curBuyCost;
        }
        
    }

    public void emptyUnit()
    {
        storedUnit = null;
        myUnitMesh.gameObject.SetActive(false);
        myCostIcon.SetActive(false);
    }

    void OnMouseOver()
    {
        mouseOverFunc();
    }

    public void mouseOverFunc()
    {
        gameObject.GetComponent<MeshRenderer>().material = altMaterial;
        if (Input.GetMouseButtonDown(1))
        {
            rightMouseDownFunc();
        }
    }

    public void rightMouseDownFunc()
    {
        if (isInvSlot)
        {
            int theIndex = gc.mk.invSlots.IndexOf(this);
            gc.ih.removeFromInv(theIndex - gc.mk.removedBeforeMe(theIndex));
            gc.mk.refreshInventory();
        }
    }

    void OnMouseExit()
    {
        mouseExitFunc();
    }

    public void mouseExitFunc()
    {
        gameObject.GetComponent<MeshRenderer>().material = defaultMaterial;
    }

    void OnMouseDown()
    {
        mouseDownFunc();
    }

    public void mouseDownFunc()
    {
        if (!isInvSlot && storedUnit != null && storedUnit.curBuyCost <= gc.ctrl && gc.ih.unitInventory.Count < InventoryHandler.maxInvSlots)
        {
            gc.ctrl -= storedUnit.curBuyCost;
            gc.ih.addToInv(storedUnit);
            gc.mk.addToInv(storedUnit);

            gc.mk.emptyMarketSlot(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
