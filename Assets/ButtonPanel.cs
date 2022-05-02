using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPanel : MonoBehaviour
{
    public GameObject buttonPrefab;
    public GameObject topLevelCanvas;

    public int across = 3;
    public int gap = 5;

    public List<string> Buttons;

    // Start is called before the first frame update
    void Start()
    {
        RectTransform panelRectTransform = gameObject.GetComponent<RectTransform>();
        RectTransform buttonRectTransform = new RectTransform();

        for (int i  = 0; i < Buttons.Count; i++)
        {
            GameObject button = Instantiate(buttonPrefab);
            buttonRectTransform = button.GetComponent<RectTransform>();

            across = (int)(panelRectTransform.rect.width / (buttonRectTransform.rect.width + gap));

            button.transform.SetParent(transform, false);
            button.transform.localPosition = new Vector3(i % across * (buttonRectTransform.rect.width + gap), panelRectTransform.rect.height - buttonRectTransform.rect.height - i / across * (buttonRectTransform.rect.height + gap), 0);
            button.GetComponentInChildren<Text>().text = Buttons[i];
            button.GetComponent<Clicked>().word = Buttons[i];
            button.name = Buttons[i];
        }
        //panelRectTransform.sizeDelta = new Vector2((buttonRectTransform.rect.width + gap) * across, panelRectTransform.rect.height);
        RectTransform topRectTransform = topLevelCanvas.GetComponent<RectTransform>();
        topRectTransform.sizeDelta = new Vector2((buttonRectTransform.rect.width + gap) * across + 25f, panelRectTransform.rect.height) ;
        //topRectTransform.ForceUpdateRectTransforms();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
