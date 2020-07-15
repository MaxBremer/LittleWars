using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveUIController : MonoBehaviour
{
    GameController gc;
    GameObject invPanel;
    Vector3 homePos;
    public Vector3 whereItShouldBe;
    Text ctrlText;

    // Start is called before the first frame update
    void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        invPanel = GameObject.Find("UnitSelectPanel");
        homePos = invPanel.transform.position;
        whereItShouldBe = homePos;
    }

    // Update is called once per frame
    void Update()
    {
        if(ctrlText == null)
        {
            ctrlText = GameObject.Find("ctrlValueText").GetComponent<Text>();
        }
        ctrlText.text = "" + gc.ctrl;
        if(Mathf.Ceil(invPanel.transform.position.x) != Mathf.Ceil(whereItShouldBe.x))
        {
            invPanel.transform.position += (whereItShouldBe - invPanel.transform.position) / 20;
        }
    }

    public void moveOut()
    {
        whereItShouldBe += new Vector3(230, 0, 0);
    }
    public void moveBack()
    {
        whereItShouldBe -= new Vector3(230, 0, 0);
    }
}
