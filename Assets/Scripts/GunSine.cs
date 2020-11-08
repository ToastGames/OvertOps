using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSine : MonoBehaviour
{
    // This script makes the player's weapon bob up and down/left and right to give the weapon character
    // currently all these values are one-off, and will apply to any weapon
    // but, once these values get added to the weapon def, they will be scraped off that to be used here

    private Player charMotor;   //the player controller we are skimming variables from
    public float frqBob;        //how long a "footstep" is
    public float ampBob;        //how much weapon bob there is (this gets multiplied by the character's speed in units, maybe should normalise it so we can tweak movement speeds without influencing this)
    public float ampSway;       //how much side to side sway we go with
    public float iniPos;        //the gun on the hud has an initial vertical offset from the ground plane, tweak it here
    public float curSpeed;      //storing within this script the player's speed. Maybe there's a better way, l

    //private Vector3 mLastPosition;    // not actually used

    private void Start()
    {
        charMotor = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();          // scrape the player component out of the scene
    }

    void Update()
    {
        curSpeed = charMotor.currentMovementSpeed;                                              // get the player's current movement speed from the player component
        //mLastPosition = transform.position;

        // where the magic happens, see explanation of each part (this is all one line of code)
        // transform LOCAL position of gun visual (the plane object this script is on) is modified by:
        // (it being local is why this is just setting the pos directly, rather than it being a translation)
        transform.localPosition = new Vector3((Mathf.Sin(Time.time * (frqBob*0.5f)))            // X = sin of time * half of bob frequency (really, we probably want to split X frq and Y frq out separate)
                                            * (ampSway * curSpeed),                             //     ^ that * X amplitude ("sway") * current movement speed
                                    iniPos + ((Mathf.Sin(Time.time * frqBob))                   // Y = same as above with "iniPos" as offset equal to the initial Y pos, which was non-zero (this will break if the offset is ever moved on another axis... probably want to turn this into a vector at some point)
                                            * (ampBob * curSpeed)),                             //     and with Y amplitude ("bob") instead of X ("sway")
                                               transform.localPosition.z);                      // and then it's Z is just left alone
    }
}
