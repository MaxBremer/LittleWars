using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPanel : MonoBehaviour
{
    public string[] sections;
    public int curPage;

    // Start is called before the first frame update
    void Start()
    {
        curPage = 0;
        showPage();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void showPage()
    {
        gameObject.transform.GetChild(0).GetComponent<Text>().text = sections[curPage];
    }

    public void nextPage()
    {
        curPage++;
        if(curPage < sections.Length)
        {
            showPage();
        }
        else
        {
            curPage--;
        }
    }

    public void prevPage()
    {
        if(curPage > 0)
        {
            curPage--;
            showPage();
        }
    }
}
