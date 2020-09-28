using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomStyleTypes : MonoBehaviour
{
    // this script is so criminally short it almost seems like it doesn't need to exist at all
    // and maybe it doesn't, maybe there's another way to do this, who knows? who cares, I'm doing it this way
    // Just like the "RoomStyleRef" script, this script exists to be placed on an empty game object to store info
    //
    // this script is literally just a list of references to "RoomStyleDef" prefabs
    //
    // this script will be placed on the "RoomStyle" child object of a room template prefab, to definte WHICH possible styles are available to randomly choose between for a given room
    // you can force a room to be a certain style by making this list one element long
    // you can weight the chances of one style or another by having multiples of the same style in the list

    public List<GameObject> roomStyleDefs;
}
