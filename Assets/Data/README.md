# Data Folder

ScriptableObject等のゲームデータアセットを管理するフォルダです。

## なぜ ScriptableObject を使うのか？

1. **データとコードの分離**: プログラマーとデザイナーが並行作業可能
2. **コンフリクトの減少**: データファイルは個別に管理できる
3. **調整が容易**: コード変更なしでバランス調整可能
4. **再利用性**: 同じデータを複数のオブジェクトで共有

## フォルダ構成

### Player/
プレイヤー関連のデータ
- `PlayerStats.asset` - プレイヤーの基本ステータス
- `PlayerAbilities/` - 各アビリティのデータ

### Enemies/
敵のデータ
- `Slime_Normal.asset`
- `Slime_Elite.asset`
- `Bat_Normal.asset`
- など、各敵タイプのステータス

### Items/
アイテムデータ
- `HealthPotion.asset`
- `Coin.asset`
- `Key_Red.asset`

### Abilities/
プレイヤーのアビリティ・スキルデータ
- `DoubleJump.asset`
- `Dash.asset`
- `WallJump.asset`

### GameSettings/
ゲーム全体の設定
- `AudioSettings.asset` - 音量設定等
- `DifficultySettings.asset` - 難易度設定
- `InputSettings.asset` - 入力設定

## 実装例

### 敵データの定義

```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("基本情報")]
    public string enemyName;
    public Sprite icon;
    
    [Header("ステータス")]
    public int maxHealth = 100;
    public int attackDamage = 10;
    public float moveSpeed = 3f;
    
    [Header("報酬")]
    public int experiencePoints = 50;
    public int coinDrop = 10;
}
```

### 敵スクリプトでの使用

```csharp
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;
    
    private int currentHealth;
    
    void Start()
    {
        currentHealth = enemyData.maxHealth;
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }
}
```

## アセット作成方法

1. Projectウィンドウで `Data/Enemies/` を右クリック
2. `Create > Game > Enemy Data` を選択
3. ファイル名を設定（例: `Slime_Normal`）
4. Inspectorでパラメータを設定

## 命名規則

- わかりやすい名前を付ける
- 種類がわかるプレフィックス/サフィックス
  - `Slime_Normal`, `Slime_Elite`
  - `Item_HealthPotion`, `Item_Key`

## コンフリクト回避

- 各自が担当する敵/アイテムのデータを作成
- 共通データの変更は事前相談
- データの複製を作って実験可能
