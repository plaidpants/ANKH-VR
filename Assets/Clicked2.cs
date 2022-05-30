using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clicked2 : MonoBehaviour
{
    public enum MODE
        {
        BUTTON,
        WORD,
        CHARACTER
    }
    public U4_Decompiled_TITLE  engine;
    public string word = "none";
    public char character = ' ';
    public MODE mode = MODE.BUTTON;

    void Start()
    {
        Button btn = transform.GetComponent<Button>(); //Grabs the button component
        btn.onClick.AddListener(OnClicked); //Adds a listner on the button
    }

    public void OnClicked()
    {
        U4_Decompiled_AVATAR.clickedButtonName = transform.gameObject.name;
        if (engine)
        {
            if (mode == MODE.BUTTON)
            {
                //engine.CommandSayWord(transform.gameObject.name);
                engine.CommandSayCharacter(transform.gameObject.name[0]);
            }
            else if(mode == MODE.WORD)
            {
                engine.CommandSayWord(word);
            }
            else
            {
                engine.CommandSayCharacter(character);
            }
        }
    }
}
