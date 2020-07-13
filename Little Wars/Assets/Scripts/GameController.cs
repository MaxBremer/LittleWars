using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    const KeyCode marketKey = KeyCode.Tab;

    public GamePhase curPhase;

    //Battle Data, move to another class?
    int friendlyUnitCount;
    int enemyUnitCount;
    int counter;
    bool fightToDeath;
    bool playerTurn;
    bool curAttackersHighlighted;
    float timer;
    int playerAtkInd;
    int enemyAtkInd;

    GameObject trueSub1;
    GameObject trueSub2;

    Ray ray;
    RaycastHit hit;

    public Material baseSlotMaterial;
    public Material selectMaterial;
    public Material fightingMaterial;

    Material sub1orig;
    Material sub2orig;

    GameObject selected;

    public Slot[] friendlyBoard;
    public Slot[] enemyBoard;
    List<Unit> availableFriends;
    List<Unit> availableEnemies;

    public BaseUnit firstUnit;
    public InventoryHandler ih;

    public int ctrl;

    public float attackDelay;

    public float cameraRotSpeed;

    //DIFFERENT LAYOUTS FOR DIFFERENT NUMBERS OF SLOTS
    

    static Dictionary<int, float> countToOffset = new Dictionary<int, float>()
    {
        {4, 2.5f },
        {5, 2f},
        {3, 3.333f },
        {6, 1.6f },
        {2, 5f }
    };
    static Dictionary<int, Vector3> countToPos = new Dictionary<int, Vector3>()
    {
        {4, new Vector3(-3.75f,0,-8) },
        {5,  new Vector3(-4f, 0, -8)},
        {3, new Vector3(-5f + (countToOffset[3]/2), 0, -8) },
        {6, new Vector3(-5f + (countToOffset[6]/2), 0, -8) },
        {2, new Vector3(-5f + (countToOffset[2]/2), 0, -8) }
    };


    public GameObject basicUnit;
    public Unit typeToAssign;

    public GameObject friendSlot;
    public GameObject enemySlot;

    public GameObject loseScreen;

    public GameObject spawnButton;
    GameObject unitSelectPanel;
    public MarketController mk;
    //public ActiveUIController uic;

    public int backupCtrlCount;

    public BaseLevel testLevel;
    public BaseLevel[] levels;
    public int levelCount;

    public GenLevel[] infLevels;
    

    gStats st;

    // Start is called before the first frame update
    void Start()
    {
        st = GameObject.FindGameObjectWithTag("stats").GetComponent<gStats>();

        ctrl = 0;
        friendlyUnitCount = 0;
        enemyUnitCount = 0;
        timer = 0;
        curAttackersHighlighted = false;
        playerTurn = true;
        curPhase = GamePhase.Prep;
        selected = null;
        typeToAssign = null;
        sub1orig = null;
        sub2orig = null;

        ih = GameObject.Find("InventoryController").GetComponent<InventoryHandler>();
        
        mk = GameObject.Find("MarketController").GetComponent<MarketController>();
        levelCount = 0;
        //uic = GameObject.Find("CTRLPanel").GetComponent<ActiveUIController>();
        //mk.initMarketPhase(testLevel);

        //DontDestroyOnLoad(gameObject);
        loadLevel(levels[levelCount], false);
    }


    // Update is called once per frame
    void Update()
    {
        
        switch (curPhase)
        {
            case GamePhase.Prep:
                if (Input.GetKeyDown(marketKey))
                {
                    mk.initMarketPhase(levels[levelCount]);
                }
                if (Mathf.CeilToInt(Camera.main.transform.eulerAngles.y) != 0)
                {
                    Camera.main.transform.eulerAngles += new Vector3(0, cameraRotSpeed, 0);
                }
                break;
            case GamePhase.Battle:
                if(selected != null)
                {
                    selected.GetComponent<MeshRenderer>().material = baseSlotMaterial;
                    selected = null;
                }
                timer += Time.deltaTime;
                if (timer > attackDelay)
                {
                    //Debug.Log("battle tick");
                    timer -= attackDelay;
                    if (curAttackersHighlighted)
                    {
                        executeAttack();
                    }
                    else
                    {
                        if (fightToDeath)
                        {
                            if (friendlyUnitCount > 0 && enemyUnitCount > 0)
                            {
                                chooseAttackers();
                            }
                            else
                            {
                                if (friendlyUnitCount > 0)
                                {
                                    
                                    levelCount++;
                                    if (levelCount == levels.Length)
                                    {
                                        endScreen("You Win!");
                                    }
                                    else
                                    {
                                        curPhase = GamePhase.Transition;
                                        clearLevel();
                                        loadLevel(levels[levelCount], false);
                                    }
                                }
                                else if (enemyUnitCount > 0)
                                {
                                    endScreen("You Lose!");
                                }
                                else
                                {
                                    curPhase = GamePhase.Transition;
                                    clearLevel();
                                    ih.clearInv();
                                    ih.unitInventory.AddRange(ih.invBackup);
                                    loadLevel(levels[levelCount], true);
                                }
                                curPhase = GamePhase.Prep;
                                timer = 0;
                                GameObject.Find("TurnPhaseText").GetComponent<Text>().text = "Prep Phase";
                            }
                        }
                        else
                        {
                            Debug.Log("One iteration attack");
                        }
                    }
                }
                break;

            case GamePhase.Market:
                if(Mathf.CeilToInt(Camera.main.transform.eulerAngles.y) != 180)
                {
                    Camera.main.transform.eulerAngles += new Vector3(0,cameraRotSpeed,0);
                }
                if (Input.GetKeyDown(marketKey))
                {
                    mk.endMarketPhase();
                }
                break;
            default:
                break;
        }
    }

    void endScreen(string toShow)
    {
        loseScreen.SetActive(true);
        loseScreen.transform.GetChild(0).GetComponent<Text>().text = toShow;
    }
    
    public void mouseOverSlotFunc(Slot sl)
    {
        if(curPhase == GamePhase.Prep && (selected == null || selected==sl.gameObject) && sl.gameObject.tag == "FriendSlot")
        {
            sl.gameObject.GetComponent<MeshRenderer>().material = selectMaterial;
            selected = sl.gameObject;
        }
    }

    public void mouseExitSlotFunc(Slot sl)
    {
        if (curPhase == GamePhase.Prep && sl.gameObject.tag=="FriendSlot")
        {
            selected.GetComponent<MeshRenderer>().material = baseSlotMaterial;
            selected = null;
        }
    }

    public void leftMouseSlotFunc(Slot sl)
    {
        /*if(sl.containing == null)
        {
            Debug.Log("slot available");
        }
        if(typeToAssign != null)
        {
            Debug.Log("have a boy to place");
        }*/
        if (sl.gameObject.tag == "FriendSlot" && curPhase == GamePhase.Prep && sl.containing == null && typeToAssign != null)
        {
            spawnUnitAtSlot(typeToAssign, sl);
            sl.containing.GetComponent<Unit>().isFriendly = true;
            ih.removeFromInv(typeToAssign);
            refreshUnitButtons();
            typeToAssign = null;
            friendlyUnitCount++;
        }
    }

    public void rightMouseSlotFunc(Slot sl)
    {
        if(sl.gameObject.tag == "FriendSlot" && curPhase == GamePhase.Prep && sl.containing != null)
        {
            Unit temp = Instantiate(basicUnit, new Vector3(100,100,100), Quaternion.identity).GetComponent<Unit>();
            temp.copyIn(sl.containing.GetComponent<Unit>());
            ih.addToInv(temp);
            refreshUnitButtons();
            emptySlot(sl);
        }
    }

    public void refreshUnitButtons()
    {
        ih.clearButtons();
        ih.initButtons(GameObject.Find("ButtonPlacement").transform.position, unitSelectPanel.transform, spawnButton);
    }

    public void clearLevel()
    {
        foreach(Slot sl in friendlyBoard)
        {
            if (sl.containing != null)
            {
                //Unit temp = Instantiate(basicUnit, new Vector3(100, 100, 100), Quaternion.identity).GetComponent<Unit>();
                //temp.copyIn(sl.containing.GetComponent<Unit>());
                ih.addToInv(sl.containing.GetComponent<Unit>());
                Destroy(sl.containing.gameObject);
                friendlyUnitCount--;
            }
            //refreshUnitButtons();
            //emptySlot(sl);
            //emptySlot(s);
            Destroy(sl.gameObject);
        }
        foreach (Slot s in enemyBoard)
        {
            emptySlot(s);
            Destroy(s.gameObject);
        }
        mk.resetMarket();
        mk.clearStored();
        ih.clearButtons();
    }

    public void loadLevel(BaseLevel level, bool refresh)
    {
        friendlyBoard = new Slot[level.friendlySlotNum];
        enemyBoard = new Slot[level.enemySlotNum];

        unitSelectPanel = GameObject.Find("UnitSelectPanel");

        Vector3 curSlotPos = countToPos[level.friendlySlotNum];// - new Vector3((countToOffset[level.friendlySlotNum]/2),0,0);
        float slotOffset = countToOffset[level.friendlySlotNum];
        for (int i = 0; i < level.friendlySlotNum ; i++)
        {
            GameObject tempSlotObj = Instantiate(friendSlot, curSlotPos, Quaternion.identity);
            tempSlotObj.transform.Rotate(new Vector3(270, 0, 0));
            friendlyBoard[i] = tempSlotObj.GetComponent<Slot>();
            tempSlotObj.GetComponent<Slot>().slotIndex = i;
            curSlotPos.x += slotOffset;
        }
        curSlotPos = countToPos[level.enemySlotNum] + new Vector3(0,.5f,5);
        slotOffset = countToOffset[level.enemySlotNum];
        for (int i = 0; i < level.enemySlotNum; i++)
        {
            GameObject tempSlotObj = Instantiate(enemySlot, curSlotPos, Quaternion.identity);
            tempSlotObj.transform.Rotate(new Vector3(270, 0, 0));
            enemyBoard[i] = tempSlotObj.GetComponent<Slot>();
            tempSlotObj.GetComponent<Slot>().slotIndex = i;
            curSlotPos.x += slotOffset;
        }

        //spawn enemy units based on level
        for(int i = 0; i < Mathf.Min(level.enemyUnits.Length, level.enemySlotNum); i++)
        {
            spawnAtSlot(level.enemyUnits[i], enemyBoard[i]);
            enemyUnitCount++;
            enemyBoard[i].containing.GetComponent<Unit>().isFriendly = false;
        }
        //initialize spawn buttons for player
        Vector3 buttonTransform = GameObject.Find("ButtonPlacement").transform.position;
        ih.initButtons(buttonTransform, unitSelectPanel.transform, spawnButton);
        

        if (!refresh)
        {
            ctrl += level.startingCtrl;
            backupCtrlCount = ctrl;
        }
        else
        {
            ctrl = backupCtrlCount;
        }

        curPhase = GamePhase.Prep;

        if(ih.unitInventory.Count > 0)
        {
            ih.clearBackup();
            ih.invBackup.AddRange(ih.unitInventory);
        }
    }

    public void spawnAtSlot(BaseUnit unit, Slot sl)
    {
        if (unit != null)
        {
            GameObject instUnit = Instantiate(basicUnit, sl.pos.position, Quaternion.identity);
            instUnit.GetComponent<Unit>().assignType(unit);
            sl.containing = instUnit;
            instUnit.GetComponent<Unit>().mySlot = sl;
        }
    }

    public void spawnUnitAtSlot(Unit unit, Slot sl)
    {
        if (unit != null)
        {
            GameObject instUnit = Instantiate(basicUnit, sl.pos.position, Quaternion.identity);
            instUnit.GetComponent<Unit>().copyIn(unit);
            sl.containing = instUnit;
            instUnit.GetComponent<Unit>().mySlot = sl;
        }
    }

    public void emptySlot(Slot sl)
    {
        if (sl.containing != null && sl.containing.GetComponent<Unit>().isFriendly)
        {
            friendlyUnitCount--;
        }
        else if(sl.containing != null)
        {
            enemyUnitCount--;
        }
        Destroy(sl.containing);
        sl.containing = null;
    }

    public void drawAttackBetween(GameObject sub1, GameObject sub2)
    {
        sub1orig = sub1.GetComponent<MeshRenderer>().material;
        trueSub1 = sub1;
        sub1.GetComponent<MeshRenderer>().material = fightingMaterial;
        sub2orig = sub2.GetComponent<MeshRenderer>().material;
        trueSub2 = sub2;
        sub2.GetComponent<MeshRenderer>().material = fightingMaterial;
    }

    //I'd get rid of dumb redundancy but I'm scared.
    public void restoreMats(GameObject sub1, GameObject sub2)
    {
        sub1.GetComponent<MeshRenderer>().material = sub1orig;
        sub2.GetComponent<MeshRenderer>().material = sub2orig;
        trueSub1 = null;
        trueSub2 = null;
    }

    public void initFight(bool toDeath)
    {
        fightToDeath = toDeath;
        playerAtkInd = 0;
        enemyAtkInd = 0;
        counter = friendlyUnitCount + enemyUnitCount;
        refreshMetaData();
        curPhase = GamePhase.Battle;
    }

    void refreshMetaData()
    {
        foreach (Slot item in friendlyBoard)
        {
            if(item.containing != null)
            {
                item.containing.GetComponent<Unit>().myData.passInData(friendlyBoard, enemyBoard, item.containing.GetComponent<Unit>(), item.slotIndex);
            }
        }

        foreach(Slot item in enemyBoard)
        {
            if (item.containing != null)
            {
                item.containing.GetComponent<Unit>().myData.passInData(enemyBoard, friendlyBoard, item.containing.GetComponent<Unit>(), item.slotIndex);
            }
        }
    }

    void chooseAttackers()
    {
        setupAvailables();
        
        if (playerTurn)
        {
            GameObject contender = availableFriends[0].gameObject;// friendlyBoard[playerAtkInd].containing;
            /*Unit[] options = new Unit[enemyUnitCount];
            int optCount = 0;
            for (int i = 0; i < enemyBoard.Length; i++)
            {
                if(enemyBoard[i].containing != null)
                {
                    options[optCount] = enemyBoard[i].containing.GetComponent<Unit>();
                    optCount++;
                }
            }*/
            int choiceInd = availableFriends[0].chooseTarget(availableEnemies);
            GameObject opponent = availableEnemies[choiceInd].gameObject;
            drawAttackBetween(contender, opponent);
            playerAtkInd = (playerAtkInd + 1) % friendlyUnitCount;

        }
        else
        {
            GameObject contender = availableEnemies[0].gameObject;// enemyBoard[enemyAtkInd].containing;
            /*Unit[] options = new Unit[friendlyUnitCount];
            int optCount = 0;
            for (int i = 0; i < friendlyBoard.Length; i++)
            {
                if (friendlyBoard[i].containing != null)
                {
                    options[optCount] = friendlyBoard[i].containing.GetComponent<Unit>();
                    optCount++;
                }
            }*/
            int choiceInd = availableEnemies[0].chooseTarget(availableFriends);
            GameObject opponent = availableFriends[choiceInd].gameObject;
            drawAttackBetween(contender, opponent);
            enemyAtkInd = (enemyAtkInd + 1) % enemyUnitCount;
        }
        curAttackersHighlighted = true;
    }
    void executeAttack()
    {
        Unit sub1unit = trueSub1.GetComponent<Unit>();
        Unit sub2unit = trueSub2.GetComponent<Unit>();

        restoreMats(trueSub1, trueSub2);

        sub1unit.atkDamage(sub2unit.curAtk);
        sub1unit.myData.onAtk(sub2unit.mySlot.slotIndex);
        sub2unit.atkDamage(sub1unit.curAtk);
        sub2unit.myData.onHitBy(sub1unit.mySlot.slotIndex);

        
        /*if (sub1unit.curHealth <= 0)
        {
            emptySlot(sub1unit.mySlot);
        }
        if (sub2unit.curHealth <= 0)
        {
            emptySlot(sub2unit.mySlot);
        }*/
        playerTurn = !playerTurn;
        curAttackersHighlighted = false;
        refreshMetaData();
    }

    void setupAvailables()
    {
        availableEnemies = new List<Unit>();
        for (int i = 0; i < enemyBoard.Length; i++)
        {
            if(enemyBoard[i].containing != null)
            {
                availableEnemies.Add(enemyBoard[i].containing.GetComponent<Unit>());
            }
        }

        availableFriends = new List<Unit>();
        for(int i = 0; i < friendlyBoard.Length; i++)
        {
            if(friendlyBoard[i].containing != null)
            {
                availableFriends.Add(friendlyBoard[i].containing.GetComponent<Unit>());
            }
        }
    }
}

public enum GamePhase { Prep, Battle, Market, Transition}
