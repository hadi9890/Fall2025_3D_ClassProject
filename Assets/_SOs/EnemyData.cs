using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace _SOs
{
    [CreateAssetMenu(fileName = "NewEnemyData", menuName = "Enemy/EnemyData")]
    public class EnemyData : ScriptableObject
    {
        [Header("General")]
        public EnemyType enemyType;
        public float health;
        public float walkSpeed;
        public float chaseSpeed;
        
        [Space(5), Header("Detection")]
        public float sightRange;
        [Range(0f, 180f)] public float sightAngle;
        
        [Space(5), Header("Attack State")]
        public float attackRange;
        public float attackCooldown;
        [CanBeNull] public GameObject projectilePrefab;
        [CanBeNull] public AudioClip[] gunshotSfx;
        public float projectileSpeed;
        public float damageValue;
    }

    public enum EnemyType
    {
        MeleeCombatant,
        Shooter
    }
}