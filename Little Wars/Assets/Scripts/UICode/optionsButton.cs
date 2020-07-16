using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class optionsButton : MonoBehaviour
{
    public GameObject optionsPanel;
    public void pressed()
    {
        optionsPanel.SetActive(true);
    }
}
