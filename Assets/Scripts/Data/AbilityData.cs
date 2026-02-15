using UnityEngine;

namespace Metroidvania.Data
{
    /// <summary>
    /// プレイヤーのアビリティ（能力）データを定義するScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "AbilityData", menuName = "Metroidvania/Ability Data")]
    public class AbilityData : ScriptableObject
    {
        [Header("基本情報")]
        [Tooltip("アビリティ名")]
        public string abilityName;
        
        [Tooltip("アビリティの説明")]
        [TextArea(3, 5)]
        public string description;
        
        [Tooltip("アイコン")]
        public Sprite icon;
        
        [Header("設定")]
        [Tooltip("スタミナ消費量")]
        public int staminaCost = 10;
        
        [Tooltip("クールダウン時間（秒）")]
        public float cooldownTime = 1f;
        
        [Header("能力の種類")]
        public AbilityType abilityType;
        
        [Header("移動系アビリティの設定")]
        [Tooltip("ダッシュ速度倍率")]
        public float dashSpeedMultiplier = 2f;
        
        [Tooltip("ダッシュ持続時間")]
        public float dashDuration = 0.3f;
        
        [Tooltip("ジャンプ力倍率")]
        public float jumpForceMultiplier = 1f;
        
        [Header("オーディオ")]
        [Tooltip("アビリティ使用時の効果音")]
        public AudioClip useSound;
        
        [Header("ビジュアルエフェクト")]
        [Tooltip("アビリティ使用時のエフェクト")]
        public GameObject effectPrefab;
    }
    
    public enum AbilityType
    {
        Dash,          // ダッシュ
        DoubleJump,    // 二段ジャンプ
        WallJump,      // 壁ジャンプ
        Glide,         // 滑空
        Attack,        // 攻撃
        Special        // 特殊能力
    }
}
