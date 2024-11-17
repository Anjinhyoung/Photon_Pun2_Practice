using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    None = -1,
    PistolType = 0,
    RifleType = 1

}
[System.Serializable]
public class WeaponInfo
{
    public int ammo;
    public float attackPower;
    public float range;
    public WeaponType weaponType;
}