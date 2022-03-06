using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 4 types of maps
// city
// dungeon
// world
// combat

// city maps have an associate dialog file

enum MOVEMENT_STYLE
{
    Fixed = 0x00,
    Wander = 0x01,
    Follow = 0x80,
    Attack = 0xFF
}

public class MapReader : MonoBehaviour
{
    //0x0 	1024 	32x32 Town Map Matrix 
    //0x400 	32 	Tile index for NPCs 0-31 
    //0x420 	32 	Start_x for NPCs 0-31
    //0x440 	32 	Start_y for NPCs 0-31 

    //0x460 	32 	Unused? - Same structure as monster.sav so previous tile?
    //0x480 	32 	Unused? - Same structure as monster.sav so previous x?
    //0x4A0 	32 	Unused? - Same structure as monster.sav so previous y? 
    //0x4C0 	32 	Movement Behavior for NPCs 0-31 
    //0x4E0 	32 	1-based conversation index (in the corresponding tlk file) for NPCs 0-31. 0 indicates no tlk record.
}
