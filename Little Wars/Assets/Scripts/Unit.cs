using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string nickName;
    public string myName;
    public Slot mySlot;
    public MarketSlot myMarketSlot;

    GameController gc;

    public AttackRule myRule;
    public Metadata myData;

    public BaseUnit unitType;
    public bool isFriendly;

    MeshFilter mf;
    MeshRenderer mr;
    MeshCollider mc;
    public Mesh myMesh;
    public Material myMat;

    public int curHealth;
    public int curAtk;
    public int curDef;

    public int curBuyCost;
    public int curDeployCost;



    void Start()
    {
        myRule = AttackRule.random;
        hideText();
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        initComponents();
    }

    public void assignType(BaseUnit type)
    {
        unitType = type;

        initComponents();

        myName = unitType.unitName;

        setHealth(type.health);
        curAtk = unitType.attack;
        curDef = unitType.defense;
        curBuyCost = unitType.buyCost;
        curDeployCost = unitType.deployCost;

        myMesh = unitType.mesh;
        myMat = unitType.unitMat;
        mf.mesh = myMesh;
        mc.sharedMesh = myMesh;
        mr.material = myMat;
        //mf.mesh.RecalculateNormals();


        //IF NEW METADATA, ADD TO THIS SWITCH STATEMENT
        switch (unitType.myDataType)
        {
            case dataType.basic:
                myData = new Metadata();
                break;
            case dataType.swipe:
                myData = new bobData();
                break;
            case dataType.poison:
                myData = new poisonData();
                break;
            default:
                Debug.Log("ERROR: metadata type not assigned or unrecognized");
                break;
        }

        refreshText();
    }


    public int chooseTarget(List<Unit> options)
    {
        switch (myRule)
        {
            case AttackRule.random:
                return Random.Range(0, options.Count);
            case AttackRule.leftToRight:
                Debug.Log("ERROR: leftToRight attackRule still undefined.");
                return -1;
            case AttackRule.leftMost:
                return 0;
            case AttackRule.rightMost:
                return options.Count - 1;
            default:
                Debug.Log("ERROR: unrecognized attack rule.");
                return -1;
                
        }
    }

    void initComponents()
    {
        if (gameObject.GetComponent<MeshFilter>() == null)
        {
            mf = gameObject.AddComponent<MeshFilter>();
        }
        if (gameObject.GetComponent<MeshRenderer>() == null)
        {
            mr = gameObject.AddComponent<MeshRenderer>();
            mr.stitchLightmapSeams = true;
        }
        if (gameObject.GetComponent<MeshCollider>() == null)
        {
            mc = gameObject.AddComponent<MeshCollider>();
        }
        
    }

    public void setHealth(int val)
    {
        curHealth = val;
        refreshText();
        checkDeath();
    }

    public void atkDamage(int atk)
    {
        curHealth = curHealth - Mathf.Max(atk - curDef, 0);
        refreshText();
        checkDeath();
    }

    public void checkDeath()
    {
        if(curHealth <= 0 && mySlot.containing == this.gameObject)
        {
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().emptySlot(mySlot);
        }
    }

    public void refreshText()
    {
        if (gameObject.transform.GetChild(0) != null && gameObject.transform.GetChild(0).GetComponent<TextMesh>() != null)
        {
            gameObject.transform.GetChild(0).GetComponent<TextMesh>().text = curAtk + "," + curDef + "," + curHealth;
        }
    }

    public void showText()
    {
        gameObject.transform.GetChild(0).GetComponent<TextMesh>().color = new Color(255, 255, 255, 255);
    }

    public void hideText()
    {
        gameObject.transform.GetChild(0).GetComponent<TextMesh>().color = new Color(255, 255, 255, 0);
    }

    public void copyIn(Unit other)
    {
        mySlot = other.mySlot;
        myRule = other.myRule;
        myData = other.myData;

        myName = other.myName;

        unitType = other.unitType;
        isFriendly = other.isFriendly;

        myMesh = other.myMesh;
        myMat = other.myMat;

        curHealth = other.curHealth;
        curAtk = other.curAtk;
        curDef = other.curDef;

        curBuyCost = other.curBuyCost;
        curDeployCost = other.curDeployCost;

        myData = other.myData;

        initComponents();

        mf.mesh = myMesh;
        mc.sharedMesh = myMesh;
        mr.material = myMat;
        refreshText();
    }

    void Update()
    {
        
    }

    void OnMouseOver()
    {
        showText();
        if(myMarketSlot != null)
        {
            myMarketSlot.mouseOverFunc();
            if (Input.GetMouseButtonDown(1))
            {
                myMarketSlot.rightMouseDownFunc();
            }
        }
        if(mySlot != null)
        {
            gc.mouseOverSlotFunc(mySlot);
            if (Input.GetMouseButtonDown(1))
            {
                
                gc.rightMouseSlotFunc(mySlot);
            }
        }
    }

    void OnMouseDown()
    {
        if(myMarketSlot != null)
        {
            myMarketSlot.mouseDownFunc();
        }
        if (mySlot != null)
        {
            gc.leftMouseSlotFunc(mySlot);
        }
    }
    void OnMouseExit()
    {
        hideText();
        if (mySlot != null)
        {
            gc.mouseExitSlotFunc(mySlot);
        }
        if(myMarketSlot != null)
        {
            myMarketSlot.mouseExitFunc();
        }
    }
}
public enum AttackRule { random, leftToRight, leftMost, rightMost}
