using System;
using UnityEngine;
using Helpers;

[Serializable, CreateAssetMenu(fileName = "New WaveEnemy", menuName = "WaveEnemy")]
public class Enemy : ScriptableObject // TODO: export to diff script when youre not cramming to all hell
{
    public GameObject prefab;
    public Ease.EaseType easeType;
    public float speed; 
}