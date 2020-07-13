using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class menuController : MonoBehaviour
{
    public GameObject unitMenuPanel;
    public Sprite expandImg;
    public Sprite collapseImg;

    // Start is called before the first frame update
    void Start()
    {
        
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

        Animator animator = unitMenuPanel.GetComponent<Animator>();
        if(animator != null)
        {
            bool open = animator.GetBool("opened");

            animator.SetBool("opened", !open);
            gameObject.GetComponent<Image>().sprite = open ? expandImg : collapseImg;
        }
        else
        {
            Debug.Log("ERROR: animator was null for menu.");
        }
    }
}
