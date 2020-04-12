﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generic;

public class WeaponHolder : MonoBehaviour
{
    //ミサイルの管理も無くしたらこの関数は不要？後で検討
    [SerializeField]
    GameObject Missile;
    [SerializeField]
    GameObject Stone;
    [SerializeField]
    GameObject ColaCan;
    [SerializeField]
    GameObject Shoes;
    [SerializeField]
    GameObject Hammer;

    //ミサイルの管理も無くしたらこの関数は不要？後で検討
    public GameObject GetWeaponObjectByKey(Weapons key)
    {
        GameObject weapon = new GameObject();
        switch(key)
        {
            case Weapons.None:
                weapon = null;
                break;
            case Weapons.Missile:
                weapon = Missile;
                break;
            case Weapons.Stone:
                weapon = Stone;
                break;
            case Weapons.ColaCan:
                weapon = ColaCan;
                break;
            case Weapons.Shoes:
                weapon = Shoes;
                break;
            case Weapons.Hammer:
                weapon = Hammer;
                break;
            default:
                weapon = Stone;
                break;
        }
        return weapon;
    }
}
