using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonGame_Enemy_", menuName = "Dungeon Game/New Enemy")]
public class DungeonGame_EnemySO : ScriptableObject
{
    public string ID = string.Empty;
    public string Name = string.Empty;
    public float BaseHealth = 100f;
    public float ViewRange = 10f;
    public float AttackRange = 1f;
    public float MovementSpeed = 1f;
    public float AttackCooldown = 0f;
    public float BaseAttackDamage = 0f;
}
