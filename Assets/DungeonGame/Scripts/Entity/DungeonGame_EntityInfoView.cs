using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DungeonGame_EntityInfoView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _level;

    public void UpdateInfoView(string name, int level)
    {
        _name.text = name;
        _name.gameObject.SetActive(true);
        _level.text = "Level " + level.ToString();
        _level.gameObject.SetActive(true);
    }
}
