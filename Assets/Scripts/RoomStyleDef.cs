using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomStyleDef : MonoBehaviour
{
    // this script is literally just a container for style definitions
    // each instance of this script is placed on an empty game object and saved as a prefab
    // this way, we can define as many room styles as we want by just creating prefabs with different settings on this script
    // (other scripts, namely the level generator, use this info to do stuff with it)

    public Material floorMaterial;
    public Material roofMaterial;
    public Material wallMaterial;

    public Texture DoorFloorTexture;
    public Texture DoorRoofTexture;
    public Texture DoorWallTexture;
    public Texture DoorTrimTexture;
}
