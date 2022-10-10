using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelSettings : MonoBehaviour
{
    public static LevelSettings level;
    public List<Castle> castleList;
    public List<Tower> towerList;
    public List<Unit> unitList;

    public void Awake() {
        level = this;
    }

}
