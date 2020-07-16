using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMesh : MonoBehaviour
{
    public MeshFilter mf;
    public MeshRenderer mr;
    //public MeshCollider mc;
    public BoxCollider mc;

    public bUnit myUnit;

    public Slot mySlot;
    public MarketSlot myMarketSlot;

    Quaternion initialTextRotation;
    Vector3 initialTextPosition;
    GameObject textObj;
    TextMesh myText;
    GameController gc;
    GameObject popMan;

    // Start is called before the first frame update
    void Start()
    {
        /*mf = GetComponent<MeshFilter>();
        mr = GetComponent<MeshRenderer>();
        mc = GetComponent<MeshCollider>();*/
        textObj = transform.GetChild(0).gameObject;
        initialTextRotation = textObj.transform.rotation;
        initialTextPosition = textObj.transform.position;

        popMan = GameObject.Find("pop");
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LateUpdate()
    {
        textObj.transform.rotation = initialTextRotation;
        textObj.transform.position = initialTextPosition;
        textObj.transform.localScale = new Vector3(1/transform.localScale.x, 1 / transform.localScale.y, 1 / transform.localScale.z);
    }

    private void OnMouseDown()
    {
        if(myMarketSlot != null)
        {
            myMarketSlot.mouseDownFunc();
        }
    }

    private void OnMouseEnter()
    {
        if(myMarketSlot != null)
        {
            myMarketSlot.mouseOverFunc();
        }
        /*if(mySlot != null)
        {
            gc.mouseOverSlotFunc(mySlot);
        }*/
        showText();
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if(myMarketSlot != null)
            {
                myMarketSlot.rightMouseDownFunc();
            }
        }
        if (Input.GetKeyDown(KeyCode.F) && myUnit.isFriendly && gc.ctrl >= 10 && myUnit.timesBuffed < 3)
        {
            gc.ctrl -= 10;
            GameObject myPopMan = Instantiate(popMan, gameObject.transform.position, Quaternion.identity);
            myUnit.curAtk++;
            myUnit.curDef++;
            myUnit.curHealth++;
            myUnit.maxHealth++;
            myUnit.timesBuffed++;
            refreshText();
            StartCoroutine(makeThePop(myPopMan));
        }
    }

    IEnumerator makeThePop(GameObject myPopMan)
    {
        myPopMan.GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(1);
        myPopMan.GetComponent<ParticleSystem>().Stop();
        Destroy(myPopMan);
    }

    private void OnMouseExit()
    {
        hideText();
        if(mySlot != null)
        {
            if(gc == null)
            {
                gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
            }
            gc.mouseExitSlotFunc(mySlot);
        }
        if(myMarketSlot != null)
        {
            myMarketSlot.mouseExitFunc();
        }
    }

    public void initUnitRenderComponents()
    {
        if (mf == null)
        {
            mf = gameObject.AddComponent<MeshFilter>();
        }
        mf.mesh = myUnit.myMesh;
        if (mr == null)
        {
            mr = gameObject.AddComponent<MeshRenderer>();
        }
        mr.sharedMaterial = myUnit.myMat;
        if (mc == null)
        {
            //mc = gameObject.AddComponent<MeshCollider>();
            mc = gameObject.AddComponent<BoxCollider>();
        }
        //mc.sharedMesh = myUnit.myMesh;

        refreshText();
        hideText();
    }

    public void initAnimation()
    {
        gameObject.GetComponent<Animator>().Play("slotBobbing");
    }

    public void enemyAnimation()
    {
        gameObject.GetComponent<Animator>().Play("slotTurning");
    }

    public void refreshText()
    {
        if (myText == null)
        {
            myText = gameObject.transform.GetChild(0).GetComponent<TextMesh>();
        }
        myText.text = myUnit.myName + "\n" + myUnit.curAtk + "," + myUnit.curDef + "," + myUnit.curHealth;
        
    }

    public void showText()
    {
        if (myText == null)
        {
            myText = gameObject.transform.GetChild(0).GetComponent<TextMesh>();
        }
        myText.color = new Color(255, 255, 255, 255);
    }

    public void hideText()
    {
        if (myText == null)
        {
            myText = gameObject.transform.GetChild(0).GetComponent<TextMesh>();
        }
        myText.color = new Color(255, 255, 255, 0);
    }
}
