using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New List", menuName = "SO/LevelList")]
public class LevelListSO : ScriptableObject
{
    List<GameObject> levels = new List<GameObject>();
}
