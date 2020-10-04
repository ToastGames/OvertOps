using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFamesDef : MonoBehaviour
{
    // much like the room style def scripts, this script exists purely to define the animation details of a sprite
    //
    // will flesh out these comments more later, as the way this works may be changing

    public Material spriteMaterial;

    public string spriteName;

    public int idleAnimNumber;
    public int idleStartFrame;
    public int idleEndFrame;

    public int shootingAnimNumber;
    public int shootingStartFrame;
    public int shootingEndFrame;

    public int reloadingAnimNumber;
    public int reloadingStartFrame;
    public int reloadingEndFrame;

    public int walkingAnimNumber;
    public int walkingStartFrame;
    public int walkingEndFrame;

    public int hurtAnimNumber;
    public int hurtStartFrame;
    public int hurtEndFrame;

    public int dyingAnimNumber;
    public int dyingStartFrame;
    public int dyingEndFrame;

}
