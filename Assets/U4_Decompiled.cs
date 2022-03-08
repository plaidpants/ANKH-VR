using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;



public class U4_Decompiled : MonoBehaviour
{
    enum MODE
    {
        MODE_VISION = 0,
        MODE_OUTDOORS = 1,
        MODE_BUILDING = 2,
        MODE_DUNGEON = 3,
        MODE_COMBAT = 4,
        MODE_COM_CAMP = 5,
        MODE_COM_ROOM = 6,
        MODE_SHRINE = 7
    };

    enum LOCATIONS
    {
        LCB_1 = 0
        /*
	//Castles
	"LCB_1.ULT",
	"LYCAEUM.ULT",
	"EMPATH.ULT",
	"SERPENT.ULT",
	//Townes
	"MOONGLOW.ULT",
	"BRITAIN.ULT",
	"JHELOM.ULT",
	"YEW.ULT",
	"MINOC.ULT",
	"TRINSIC.ULT",
	"SKARA.ULT",
	"MAGINCIA.ULT",
	//Villages
	"PAWS.ULT",
	"DEN.ULT",
	"VESPER.ULT",
	"COVE.ULT"
        */
    };


    [DllImport("UN_U4.dll")]
    public static extern void main_start();
    [DllImport("UN_U4.dll")]
    public static extern void main_loop();
    [DllImport("UN_U4.dll")]
    public static extern int main_CurMode();
    [DllImport("UN_U4.dll")]
    public static extern void main_keyboardHit(char wParam);
    [DllImport("UN_U4.dll")]
    public static extern void main_CurMap(byte[] buffer, int length);
    [DllImport("UN_U4.dll")]
    public static extern void main_Party(byte[] buffer, int length);

    float timer = 0.0f;
    float timerExpired = 0.0f;
    public float timerPeriod = 0.2f;
    byte[] buffer = new byte[2000];
    byte[] buffer2 = new byte[2000];

    // Start is called before the first frame update
    void Start()
    {
        main_start();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer > timerExpired)
        {
            main_loop();

            timer = timer - timerExpired;
            timerExpired = timerPeriod;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) )
        {
            main_keyboardHit((char)0x28);
        }

        main_CurMap(buffer, buffer.Length);
        main_Party(buffer2, buffer2.Length);
    }
}
