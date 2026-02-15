using UnityEngine;

namespace Metroidvania.Data
{
    /// <summary>
    /// 敵キャラクターのデータを定義するScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Metroidvania/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [Header("基本情報")]
        [Tooltip("敵の名前")]
        public string enemyName;
        
        [Tooltip("敵のアイコン")]
        public Sprite icon;
        
        [Header("ステータス")]
        [Tooltip("最大体力")]
        public int maxHealth = 100;
        
        [Tooltip("攻撃力")]
        public int attackDamage = 10;
        
        [Tooltip("移動速度")]
        public float moveSpeed = 3f;
        
        [Tooltip("ジャンプ力（必要な場合）")]
        public float jumpForce = 5f;
        
        [Header("AI設定")]
        [Tooltip("視界範囲")]
        public float detectionRange = 10f;
        
        [Tooltip("攻撃範囲")]
        public float attackRange = 2f;
        
        [Tooltip("巡回速度")]
        public float patrolSpeed = 1.5f;
        
        [Header("報酬")]
        [Tooltip("倒したときの経験値")]
        public int experiencePoints = 50;
        
        [Tooltip("倒したときのコイン")]
        public int coinDrop = 10;
        
        [Header("オーディオ")]
        [Tooltip("攻撃時の効果音")]
        public AudioClip attackSound;
        
        [Tooltip("ダメージを受けたときの効果音")]
        public AudioClip hitSound;
        
        [Tooltip("死亡時の効果音")]
        public AudioClip deathSound;
    }
}
