using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAmmo : MonoBehaviour
{
    public enum AmmoType { Bullets, Shells, Cells, Rockets }
    public AmmoType ammoType;

    public int ammoAmount;
}
