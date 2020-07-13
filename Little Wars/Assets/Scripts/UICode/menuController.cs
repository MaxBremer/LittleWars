using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class menuController : MonoBehaviour
{
    public GameObject unitMenuPanel;
    public GameObject activeController;
    public Sprite expandImg;
    public Sprite collapseImg;
    bool collapsed;

    // Start is called before the first frame update
    void Start()
    {
        collapsed = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void activated()
    {
        if (unitMenuPanel == null)
        {
            unitMenuPanel = GameObject.Find("UnitSelectPanel");
        }
        if(activeController == null)
        {
            activeController = GameObject.Find("CTRLPanel");
        }

        if (collapsed)
        {
            gameObject.GetComponent<Image>().sprite = collapseImg;
            activeController.GetComponent<ActiveUIController>().moveOut();
        }
        else
        {
            gameObject.GetComponent<Image>().sprite = expandImg;
            activeController.GetComponent<ActiveUIController>().moveBack();
        }
        collapsed = !collapsed;

        /*Animator animator = unitMenuPanel.GetComponent<Animator>();
        if(animator != null)
        {
            bool open = animator.GetBool("opened");

            animator.SetBool("opened", !open);
            gameObject.GetComponent<Image>().sprite = open ? expandImg : collapseImg;
        }
        else
        {
            Debug.Log("ERROR: animator was null for menu.");
        }*/
    }
}
