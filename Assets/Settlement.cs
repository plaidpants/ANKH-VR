using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settlement
{
    public enum NPC_MOVEMENT_MODE
    {
        FIXED = 0x00,
        WANDER = 0x01,
        FOLLOW = 0x80,
        ATTACK = 0xff
    };

    public enum NPC_STRING_INDEX
    {
        NAME = 0,
        PRONOUN = 1, //  (He, She or It)
        LOOK_DESCRIPTION = 2,
        JOB_RESPONSE = 3,
        HEALTH_RESPONSE = 4,
        KEYWORD1_RESPONSE = 5,
        KEYWORD2_RESPONSE = 6,
        QUESTION = 7,
        QUESTION_YES_RESPONSE = 8,
        QUESTION_NO_RESPONSE = 9,
        KEYWORD1 = 10,
        KEYWORD2 = 11,
        MAX = 12
    };

    // these need to line up with U4_Decompiled.LOCATIONS so we can convert from Party._loc to this enum and the LOCATIONS enum, LBC_1 is a special case
    public enum SETTLEMENT
    {
        // Castles
        LCB_1 = 0,
        LCB_2 = 1,
        LYCAEUM = 2,
        EMPATH = 3,
        SERPENT = 4,

        // Townes
        MOONGLOW = 5,
        BRITAIN = 6,
        JHELOM = 7,
        YEW = 8,
        MINOC = 9,
        TRINSIC = 10,
        SKARA = 11,
        MAGINCIA = 12,

        // Villages
        PAWS = 13,
        DEN = 14,
        VESPER = 15,
        COVE = 16,
        MAX = 17
    }

    public struct npc
    {
        public Tile.TILE tile;
        public byte pos_x;
        public byte pos_y;
        public NPC_MOVEMENT_MODE movement;
        public int conversationIndex;
        public List<string> strings;
        public int probabilityOfTurningAway;
        public bool questionAffectHumility;
        public int questionTriggerIndex;
    };

    public static npc[][] settlementNPCs = new npc[(int)SETTLEMENT.MAX][]; //32
    public static int[][] npcQuestionTriggerIndex = new int[(int)SETTLEMENT.MAX][]; //16
    public static bool[][] npcQuestionAffectHumility = new bool[(int)SETTLEMENT.MAX][]; //16
    public static int[][] npcProbabilityOfTurningAway = new int[(int)SETTLEMENT.MAX][]; //16
    public static List<string>[][] npcStrings = new List<string>[(int)SETTLEMENT.MAX][]; //16
    public static Tile.TILE[][,] settlementMap = new Tile.TILE[(int)SETTLEMENT.MAX][,]; //32,32

    public static GameObject[][,] settlementsMapGameObjects = new GameObject[(int)SETTLEMENT.MAX][,];

    public static void LoadSettlements()
    {
        /* 
           Offset 	Length (in bytes) 	Purpose
           0x0 	    1024 	32x32 town map matrix
           0x400 	32 	    Tile for NPCs 0-31
           0x420 	32 	    Start_x for NPCs 0-31
           0x440 	32 	    Start_y for NPCs 0-31
           0x460 	32 	    Repetition of 0x400-0x41F
           0x480 	32 	    Repetition of 0x420-0x43F
           0x4A0 	32 	    Repetition of 0x440-0x45F
           0x4C0 	32 	    Movement_behavior for NPCs 0-31 (0x0-fixed, 0x1-wander, 0x80-follow, 0xFF-attack)
           0x4E0 	32 	    Conversion index (tlk file) for NPCs 0-31 
        */

        for (int settlement = 0; settlement < (int)SETTLEMENT.MAX; settlement++)
        {
            settlementNPCs[settlement] = new npc[32];
            npcQuestionTriggerIndex[settlement] = new int[16];
            npcQuestionAffectHumility[settlement] = new bool[16];
            npcProbabilityOfTurningAway[settlement] = new int[16];
            npcStrings[settlement] = new List<string>[16];
            settlementMap[settlement] = new Tile.TILE[32, 32];

            if (!System.IO.File.Exists(Application.persistentDataPath + "/u4/" + ((SETTLEMENT)settlement).ToString() + ".ULT"))
            {
                Debug.Log("Could not find settlement file " + Application.persistentDataPath + "/u4/" + ((SETTLEMENT)settlement).ToString() + ".ULT");
                continue;
            }

            // read the file
            byte[] settlementFileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/u4/" + ((SETTLEMENT)settlement).ToString() + ".ULT");

            if (settlementFileData.Length != 1280)
            {
                Debug.Log("Settlement file incorrect length " + settlementFileData.Length);
                continue;
            }

            /*
                Offset 	Length (in bytes) 	Purpose
                0x0 	1 	Question Flag (3=JOB, 4=HEALTH, 5=KEYWORD1, 6=KEYWORD2)
                0x1 	1 	Does Response Affect Humility? (0=No, 1=Yes)
                0x2 	1 	Probability of Turning Away (out of 256)
                0x3 	Varies 	Name
                Varies 	Varies 	Pronoun (He, She or It)
                Varies 	Varies 	LOOK Description
                Varies 	Varies 	JOB Response
                Varies 	Varies 	HEALTH Response
                Varies 	Varies 	KEYWORD 1 Response
                Varies 	Varies 	KEYWORD 2 Response
                Varies 	Varies 	Yes/No Question
                Varies 	Varies 	YES Response
                Varies 	Varies 	NO Response
                Varies 	Varies 	KEYWORD 1
                Varies 	Varies 	KEYWORD 2
                Varies-0x119 	Varies 	00000....  
            */

            if (settlement == (int)SETTLEMENT.LCB_1 || settlement == (int)SETTLEMENT.LCB_2)
            {
                if (!System.IO.File.Exists(Application.persistentDataPath + "/u4/" + "LCB" + ".TLK"))
                {
                    Debug.Log("Could not find settlement talk file " + Application.persistentDataPath + "/u4/" + "LCB" + ".TLK");
                    continue;
                }
            }
            else
            {
                if (!System.IO.File.Exists(Application.persistentDataPath + "/u4/" + (SETTLEMENT)settlement + ".TLK"))
                {
                    Debug.Log("Could not find settlement talk file " + Application.persistentDataPath + "/u4/" + (SETTLEMENT)settlement + ".TLK");
                    continue;
                }
            }

            byte[] talkFileData;

            if (settlement == (int)SETTLEMENT.LCB_1 || settlement == (int)SETTLEMENT.LCB_2)
            {
                // read the file
                talkFileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/u4/" + "LCB" + ".TLK");
            }
            else
            {
                // read the file
                talkFileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/u4/" + (SETTLEMENT)settlement + ".TLK");
            }

            if (talkFileData.Length != 4608)
            {
                Debug.Log("Settlement talk file incorrect length " + talkFileData.Length);
                continue;
            }

            for (int talkIndex = 0; talkIndex < 16; talkIndex++)
            {
                npcStrings[settlement][talkIndex] = new List<string>();

                npcQuestionTriggerIndex[settlement][talkIndex] = talkFileData[talkIndex * 288];
                if (talkFileData[(talkIndex * 288) + 1] != 0)
                {
                    npcQuestionAffectHumility[settlement][talkIndex] = true;
                }
                else
                {
                    npcQuestionAffectHumility[settlement][talkIndex] = false;
                }
                npcProbabilityOfTurningAway[settlement][talkIndex] = talkFileData[talkIndex * 288 + 2];

                string s;
                int stringBufferIndex = 3;

                // search for strings in the .TLK file
                for (int stringIndex = 0; stringIndex < (int)NPC_STRING_INDEX.MAX; stringIndex++)
                {
                    // reset string
                    s = "";

                    // manually construct the string because C# doesn't work with null terminated C strings well
                    for (int i = 0; (i < 100) && (talkFileData[talkIndex * 288 + stringBufferIndex] != 0); i++)
                    {
                        s += (char)talkFileData[talkIndex * 288 + stringBufferIndex++];
                    }

                    // add it to the list even if it is empty
                    npcStrings[settlement][talkIndex].Add(s);

                    // skip over null terminator to go to the next string
                    stringBufferIndex++;
                }

                string wordtocheck = npcStrings[settlement][talkIndex][(int)NPC_STRING_INDEX.KEYWORD2];
                string nametocheck = npcStrings[settlement][talkIndex][(int)NPC_STRING_INDEX.NAME];
                // There is a bug in the talk files, fix it here
                if ((wordtocheck == "HEAL") && (nametocheck == "Calabrini"))
                {
                    npcStrings[settlement][talkIndex][(int)NPC_STRING_INDEX.KEYWORD2] = "INJU";
                    npcStrings[settlement][talkIndex][(int)NPC_STRING_INDEX.QUESTION] = "Dost thou seek\nan inn or art\nthou injured ?";
                }

                if ((wordtocheck == "HEAL") && (nametocheck == "Michelle"))
                {
                    npcStrings[settlement][talkIndex][(int)NPC_STRING_INDEX.KEYWORD2] = "VISI";
                }
            }

            // load settlement map data
            int bufferIndex = 0;

            for (int height = 0; height < 32; height++)
            {
                for (int width = 0; width < 32; width++)
                {
                    Tile.TILE tileIndex = (Tile.TILE)settlementFileData[bufferIndex++];
                    settlementMap[settlement][width, height] = tileIndex;
                }
            }

            // load npc data from the map data
            for (int npcIndex = 0; npcIndex < 32; npcIndex++)
            {
                Tile.TILE npcTile = (Tile.TILE)settlementFileData[0x400 + npcIndex];
                settlementNPCs[settlement][npcIndex].tile = npcTile;

                // zero indicates unused
                if (npcTile != 0)
                {
                    settlementNPCs[settlement][npcIndex].pos_x = settlementFileData[0x420 + npcIndex];
                    settlementNPCs[settlement][npcIndex].pos_y = settlementFileData[0x440 + npcIndex];
                    settlementNPCs[settlement][npcIndex].movement = (NPC_MOVEMENT_MODE)settlementFileData[0x4C0 + npcIndex];
                    int conversationIndex = settlementFileData[0x4E0 + npcIndex];
                    settlementNPCs[settlement][npcIndex].conversationIndex = conversationIndex;
                    // grab the talk data and add it to this structure
                    // zero indicates unused
                    if (conversationIndex != 0)
                    {
                        // this can be 128 for one vendor in Vincent, not sure why? TODO need to check this after I fix the npx talk loader
                        if ((conversationIndex - 1) < npcStrings.Length)
                        {
                            settlementNPCs[settlement][npcIndex].strings = npcStrings[settlement][conversationIndex - 1];
                            settlementNPCs[settlement][npcIndex].questionAffectHumility = npcQuestionAffectHumility[settlement][conversationIndex - 1];
                            settlementNPCs[settlement][npcIndex].probabilityOfTurningAway = npcProbabilityOfTurningAway[settlement][conversationIndex - 1];
                            settlementNPCs[settlement][npcIndex].questionTriggerIndex = npcQuestionTriggerIndex[settlement][conversationIndex - 1];
                        }
                    }
                }
            }
        }
    }
}
