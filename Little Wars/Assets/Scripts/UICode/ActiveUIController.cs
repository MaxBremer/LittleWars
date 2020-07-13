using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveUIController : MonoBehaviour
{
    GameController gc;
    Text ctrlText;

    // Start is called before the first frame update
    void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        ctrlText = gameObject.transform.GetChild(1).GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        ctrlText.text = "" + gc.ctrl;
    }
}
