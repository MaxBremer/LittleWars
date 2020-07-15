using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    Material sub1orig;
    Material sub2orig;


    public Material baseSlotMaterial;
    public Material selectMaterial;
    public Material fightingMaterial;

    GameObject selected;

    public Slot[] friendlyBoard;
    public Slot[] enemyBoard;
    List<UnitMesh> availableFriends;
    List<UnitMesh> availableEnemies;

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

    public bUnit typeToAssign;

    public GameObject friendSlot;
    public GameObject enemySlot;
    public GameObject spawnButton;

    public GameObject loseScreen;

    GameObject unitSelectPanel;
    public MarketController mk;
    public LevelGenerator lg;

    public int backupCtrlCount;

    public BaseLevel testLevel;
    public BaseLevel[] levels;
    public int levelCount;

    public GenLevel curLevel;

    float startingDifficulty;
    float curDifficulty;
    const float diffIncr = 5f;

    bool firstTimeSetup = true;
    bool firstMarket = true;

    public gStats st;

    const int startingCtrlBoost = 10;

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

        //material control for combatants
        sub1orig = null;
        sub2orig = null;

        ih = GameObject.Find("InventoryController").GetComponent<InventoryHandler>();
        
        mk = GameObject.Find("MarketController").GetComponent<MarketController>();

        lg = gameObject.GetComponent<LevelGenerator>();

        levelCount = 0;
        //uic = GameObject.Find("CTRLPanel").GetComponent<ActiveUIController>();
        //mk.initMarketPhase(testLevel);

        //DontDestroyOnLoad(gameObject);

        startingDifficulty = 5f;
        curDifficulty = startingDifficulty;
        if (!st.isInfinite)
        {
            int temp = 0;
        }
        else
        {
            ctrl += startingCtrlBoost;
            //GameObject.Find("TutorialPanel").SetActive(false);
            //FOR SOME REASON, IF I JUST DISABLE THE TUTORIAL PANEL THEN UNITSELECTPANEL MOVES TWICE AS MUCH
            //WHEN EXPANDING.
            //TODO: FIGURE OUT WHY THE FUCK AND REMOVE THIS SHITTY ASS SOLUTION BELOW.
            Color setTo = new Color(0, 0, 0, 0);
            GameObject target = GameObject.Find("TutorialPanel");
            target.GetComponent<Image>().color = setTo;
            target.transform.GetChild(0).GetComponent<Text>().color = setTo;
            target.transform.GetChild(1).GetComponent<Image>().color = setTo;
            target.transform.GetChild(1).GetChild(0).GetComponent<Text>().color = setTo;
            target.transform.GetChild(2).GetComponent<Image>().color = setTo;
            target.transform.GetChild(2).GetChild(0).GetComponent<Text>().color = setTo;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (firstTimeSetup)
        {
            if (st.isInfinite)
            {
                curLevel = lg.generateLevel(curDifficulty);
                loadGenLevel(curLevel, false);
            }
            else
            {
                loadLevel(levels[levelCount], false);
            }
            firstTimeSetup = false;
        }
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(st.gameObject);
            SceneManager.LoadScene(0);
        }
        switch (curPhase)
        {
            case GamePhase.Prep:
                if (Input.GetKeyDown(marketKey))
                {
                    /*if (firstMarket)
                    {
                        unitSelectPanel.SetActive(false);
                        curPhase = GamePhase.Market;
                        *//*if (!st.isInfinite)
                        {
                            mk.initMarketPhase(levels[levelCount]);
                        }
                        else
                        {
                            mk.initMarketPhase(null);
                        }
                        firstMarket = false;*//*
                    }
                    else
                    {*/
                        unitSelectPanel.SetActive(false);
                        curPhase = GamePhase.Market;
                        mk.refreshInventory();
                    //}
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
                                //fight isn't over, initiate a new attack.
                                chooseAttackers();
                            }
                            else
                            {
                                //fight is over, determine who won/tie.
                                if (friendlyUnitCount > 0)
                                {
                                    //win condition.
                                    //basic next-level stuff
                                    levelCount++;
                                    //mk.emptyStoredMarket();
                                    if (!st.isInfinite)
                                    {
                                        //pre-set levels condition.
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
                                    else
                                    {
                                        //infinite level generation condition.
                                        curDifficulty += diffIncr;
                                        curPhase = GamePhase.Transition;
                                        clearLevel();
                                        curLevel = lg.generateLevel(curDifficulty);
                                        loadGenLevel(curLevel, false);
                                    }
                                }
                                else if (enemyUnitCount > 0)
                                {
                                    //loseCondition
                                    string printer = "Unfortunately, you died.";
                                    if (st.isInfinite)
                                    {
                                        printer += "\n You beat: " + levelCount + " levels.";
                                    }
                                    endScreen(printer);
                                }
                                else
                                {
                                    //tie condition
                                    curPhase = GamePhase.Transition;
                                    clearLevel();
                                    ih.clearInv();
                                    ih.unitInventory.AddRange(ih.invBackup);
                                    if (!st.isInfinite)
                                    {
                                        loadLevel(levels[levelCount], true);
                                    }
                                    else
                                    {
                                        loadGenLevel(curLevel, true);
                                    }
                                }
                                curPhase = GamePhase.Prep;
                                timer = 0;
                                GameObject.Find("TurnPhaseText").GetComponent<Text>().text = "Prep Phase";
                            }
                        }
                        else
                        {
                            //Totally unimplemented fights that consist of a single pass through.
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
        /*if(curPhase == GamePhase.Prep)
        {
            sl.transform.GetChild(0).GetComponent<UnitMesh>().initAnimation();
        }*/
    }

    public void mouseExitSlotFunc(Slot sl)
    {
        if (curPhase == GamePhase.Prep && sl.gameObject.tag=="FriendSlot")
        {
            sl.gameObject.GetComponent<MeshRenderer>().material = baseSlotMaterial;
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
        if (sl.gameObject.tag == "FriendSlot" && curPhase == GamePhase.Prep && sl.myUnit == null && typeToAssign != null)
        {
            spawnAtSlot(typeToAssign, sl);
            sl.myUnit.isFriendly = true;
            ih.removeFromInv(typeToAssign);
            refreshUnitButtons();
            typeToAssign = null;
            sl.myUnitMesh.initAnimation();
            friendlyUnitCount++;
        }
    }

    public void rightMouseSlotFunc(Slot sl)
    {
        if(sl.gameObject.tag == "FriendSlot" && curPhase == GamePhase.Prep && sl.myUnit != null)
        {
            /*Unit temp = Instantiate(basicUnit, new Vector3(100,100,100), Quaternion.identity).GetComponent<Unit>();
            temp.copyIn(sl.containing.GetComponent<Unit>());*/
            ih.addToInv(sl.myUnit);
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
            if (sl.myUnit != null)
            {
                //Unit temp = Instantiate(basicUnit, new Vector3(100, 100, 100), Quaternion.identity).GetComponent<Unit>();
                //temp.copyIn(sl.containing.GetComponent<Unit>());
                ih.addToInv(sl.myUnit);
                //Destroy(sl.containing.gameObject);
                friendlyUnitCount--;
            }
            //refreshUnitButtons();
            //emptySlot(sl);
            Destroy(sl.gameObject);
        }
        foreach (Slot s in enemyBoard)
        {
            emptySlot(s);
            Destroy(s.gameObject);
        }
        mk.resetMarket();
        //mk.clearStored();
        ih.clearButtons();
    }

    public void loadLevel(BaseLevel level, bool refresh)
    {
        friendlyBoard = new Slot[level.friendlySlotNum];
        enemyBoard = new Slot[level.enemySlotNum];

        unitSelectPanel = GameObject.Find("UnitSelectPanel");

        //Slot instantiation
        Vector3 curSlotPos = countToPos[level.friendlySlotNum];// - new Vector3((countToOffset[level.friendlySlotNum]/2),0,0);
        float slotOffset = countToOffset[level.friendlySlotNum];
        for (int i = 0; i < level.friendlySlotNum ; i++)
        {
            GameObject tempSlotObj = Instantiate(friendSlot, curSlotPos, Quaternion.identity);
            tempSlotObj.transform.Rotate(new Vector3(270, 0, 0));
            friendlyBoard[i] = tempSlotObj.GetComponent<Slot>();
            tempSlotObj.GetComponent<Slot>().slotIndex = i;
            //tempSlotObj.transform.GetChild(0).GetComponent<UnitMesh>().initAnimation();
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
            //tempSlotObj.transform.GetChild(0).GetComponent<UnitMesh>().initAnimation();
            curSlotPos.x += slotOffset;
        }

        //spawn enemy units based on level
        for(int i = 0; i < Mathf.Min(level.enemyUnits.Length, level.enemySlotNum); i++)
        {
            spawnAtSlot(new bUnit(level.enemyUnits[i]), enemyBoard[i]);
            enemyBoard[i].myUnitMesh.enemyAnimation();
            enemyUnitCount++;
            enemyBoard[i].myUnit.isFriendly = false;
        }
        //initialize spawn buttons for player
        Vector3 buttonTransform = GameObject.Find("ButtonPlacement").transform.position;
        ih.initButtons(buttonTransform, unitSelectPanel.transform, spawnButton);

        handleBackup(refresh, level.startingCtrl);

        curPhase = GamePhase.Prep;
        mk.initMarketPhase(level);
    }

    public void loadGenLevel(GenLevel level, bool refresh)
    {
        friendlyBoard = new Slot[level.friendlySlotNum];
        enemyBoard = new Slot[level.enemySlotNum];

        unitSelectPanel = GameObject.Find("UnitSelectPanel");

        Vector3 curSlotPos = countToPos[level.friendlySlotNum];// - new Vector3((countToOffset[level.friendlySlotNum]/2),0,0);
        float slotOffset = countToOffset[level.friendlySlotNum];
        for (int i = 0; i < level.friendlySlotNum; i++)
        {
            GameObject tempSlotObj = Instantiate(friendSlot, curSlotPos, Quaternion.identity);
            tempSlotObj.transform.Rotate(new Vector3(270, 0, 0));
            friendlyBoard[i] = tempSlotObj.GetComponent<Slot>();
            tempSlotObj.GetComponent<Slot>().slotIndex = i;
            curSlotPos.x += slotOffset;
        }
        curSlotPos = countToPos[level.enemySlotNum] + new Vector3(0, .5f, 5);
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
        for (int i = 0; i < Mathf.Min(level.enemyUnits.Length, level.enemySlotNum); i++)
        {
            spawnAtSlot(level.enemyUnits[i], enemyBoard[i]);
            enemyBoard[i].myUnitMesh.enemyAnimation();
            enemyUnitCount++;
            enemyBoard[i].myUnit.isFriendly = false;
        }
        //initialize spawn buttons for player
        Vector3 buttonTransform = GameObject.Find("ButtonPlacement").transform.position;
        ih.initButtons(buttonTransform, unitSelectPanel.transform, spawnButton);

        handleBackup(refresh, level.startingCtrl);

        curPhase = GamePhase.Prep;

        mk.initMarketPhase(null);
    }

    void handleBackup(bool refresh, int startCtrl)
    {
        if (!refresh)
        {
            ctrl += startCtrl;
            backupCtrlCount = ctrl;
        }
        else
        {
            ctrl = backupCtrlCount;
        }

        curPhase = GamePhase.Prep;

        if (refresh)
        {
            ih.loadBackup();
        }
        else
        {
            ih.clearBackup();
            if (ih.unitInventory.Count > 0)
            {
                ih.backupInventory();
            }
        }
    }

    public void spawnAtSlot(bUnit unit, Slot sl)
    {
        if (unit != null)
        {
            sl.assignUnit(unit);
        }
    }

    public void emptySlot(Slot sl)
    {
        if (sl.myUnit != null && sl.myUnit.isFriendly)
        {
            friendlyUnitCount--;
        }
        else if(sl.myUnit != null)
        {
            enemyUnitCount--;
        }
        //Destroy(sl.containing);
        sl.emptyUnit();
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
            if(item.myUnit != null)
            {
                item.myUnit.myData.passInData(friendlyBoard, enemyBoard, item.myUnit, item.slotIndex);
            }
        }

        foreach(Slot item in enemyBoard)
        {
            if (item.myUnit != null)
            {
                item.myUnit.myData.passInData(enemyBoard, friendlyBoard, item.myUnit, item.slotIndex);
            }
        }
    }

    void chooseAttackers()
    {
        setupAvailables();
        
        if (playerTurn)
        {
            UnitMesh contender = availableFriends[0];//.gameObject;// friendlyBoard[playerAtkInd].containing;
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
            int choiceInd = availableFriends[0].myUnit.chooseTarget(unitMeshListToUnitList(availableEnemies));
            UnitMesh opponent = availableEnemies[choiceInd];//.gameObject;
            //TODO: UnitMesh gameobjects
            drawAttackBetween(contender.gameObject, opponent.gameObject);
            //TODO: actual index iteration maybe?
            //playerAtkInd = (playerAtkInd + 1) % friendlyUnitCount;

        }
        else
        {
            UnitMesh contender = availableEnemies[0];//.gameObject;// enemyBoard[enemyAtkInd].containing;
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
            int choiceInd = availableEnemies[0].myUnit.chooseTarget(unitMeshListToUnitList(availableFriends));
            UnitMesh opponent = availableFriends[choiceInd];//.gameObject;
            //TODO: SAME AS ABOVE
            drawAttackBetween(contender.gameObject, opponent.gameObject);
            //enemyAtkInd = (enemyAtkInd + 1) % enemyUnitCount;
        }
        curAttackersHighlighted = true;
    }

    bUnit[] unitMeshListToUnitList(List<UnitMesh> ls)
    {
        bUnit[] ret = new bUnit[ls.Count];
        for(int i = 0; i < ls.Count; i++)
        {
            ret[i] = ls[i].myUnit;
        }
        return ret;
    }

    void executeAttack()
    {
        bUnit sub1unit = trueSub1.GetComponent<UnitMesh>().myUnit;
        bUnit sub2unit = trueSub2.GetComponent<UnitMesh>().myUnit;

        

        sub1unit.atkDamage(sub2unit.curAtk);
        //STORE INDEX IN METADATA
        sub1unit.myData.onAtk(sub2unit.myData.myIndex);
        sub2unit.atkDamage(sub1unit.curAtk);
        sub2unit.myData.onHitBy(sub1unit.myData.myIndex);

        if (sub1unit.isDead())
        {
            emptySlot(trueSub1.GetComponent<UnitMesh>().mySlot);
        }
        else
        {
            trueSub1.GetComponent<UnitMesh>().refreshText();
        }


        if (sub2unit.isDead())
        {
            emptySlot(trueSub2.GetComponent<UnitMesh>().mySlot);
        }
        else
        {
            trueSub2.GetComponent<UnitMesh>().refreshText();
        }
        
        restoreMats(trueSub1, trueSub2);

        playerTurn = !playerTurn;
        curAttackersHighlighted = false;
        refreshMetaData();
    }

    void setupAvailables()
    {
        availableEnemies = new List<UnitMesh>();
        for (int i = 0; i < enemyBoard.Length; i++)
        {
            if(enemyBoard[i].myUnit != null)
            {
                availableEnemies.Add(enemyBoard[i].myUnitMesh);
            }
        }

        availableFriends = new List<UnitMesh>();
        for(int i = 0; i < friendlyBoard.Length; i++)
        {
            if(friendlyBoard[i].myUnit != null)
            {
                availableFriends.Add(friendlyBoard[i].myUnitMesh);
            }
        }
    }
}

public enum GamePhase { Prep, Battle, Market, Transition}
