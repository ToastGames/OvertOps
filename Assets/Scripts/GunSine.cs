using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSine : MonoBehaviour
{
    private Player charMotor;    //the player controller we are skimming variables from
    public float frqBob;        //how long a "footstep" is
    public float ampBob;        //how much weapon bob there is (this gets multiplied by the character's speed in units, maybe should normalise it so we can tweak movement speeds without influencing this)
    public float ampSway;       //how much side to side sway we go with
    public float iniPos;        //the gun on the hud has an initial vertical offset from the ground plane, tweak it here
    public float curSpeed;      //storing within this script the player's speed. Maybe there's a better way, l

    private Vector3 mLastPosition;

    private void Start()
    {
        charMotor = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    void Update()
    {
        curSpeed = charMotor.currentMovementSpeed;
        mLastPosition = transform.position;
        transform.localPosition = new Vector3((Mathf.Sin(Time.time * (frqBob*0.5f))) * (ampSway * curSpeed), iniPos + ((Mathf.Sin(Time.time * frqBob)) * (ampBob*curSpeed)), transform.localPosition.z);
    }
}
