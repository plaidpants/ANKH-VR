using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPopulateButtons : MonoBehaviour
{
    public U4_Decompiled_AVATAR u4;

    public GameObject[] CharacterButtons = new GameObject[8];
    public Text[] CharacterButtonsText = new Text[8];

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnEnable()
    {
        for (int i = 0; i < u4.Party.f_1d8; i++)
        {
            CharacterButtons[i].SetActive(true);
            CharacterButtonsText[i].text = u4.Party.chara[i].name;
        }

        for (int i = u4.Party.f_1d8; i < 8; i++)
        {
            CharacterButtons[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
