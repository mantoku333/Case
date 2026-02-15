# ワークフロー例

このドキュメントでは、実際の開発シナリオでのワークフロー例を示します。

## シナリオ1: 新しい敵キャラクターを追加

### タスク
スライム敵キャラクターを実装する

### 手順

#### 1. ブランチ作成
```bash
git checkout develop
git pull origin develop
git checkout -b feature/enemy-slime
```

#### 2. データアセット作成
1. `Assets/Data/Enemies/` を右クリック
2. `Create > Metroidvania > Enemy Data`
3. ファイル名: `Slime_Normal`
4. Inspectorでパラメータを設定:
   - Enemy Name: "Slime"
   - Max Health: 50
   - Attack Damage: 5
   - Move Speed: 2.0
   - など

#### 3. スクリプト作成
`Assets/Scripts/Enemy/SlimeAI.cs` を作成:

```csharp
using UnityEngine;
using Metroidvania.Data;

namespace Metroidvania.Enemy
{
    public class SlimeAI : MonoBehaviour
    {
        [SerializeField] private EnemyData enemyData;
        private int currentHealth;
        
        void Start()
        {
            currentHealth = enemyData.maxHealth;
        }
        
        // AI実装...
    }
}
```

#### 4. Prefab作成
1. Hierarchy で GameObject を作成 > "Slime"
2. `SlimeAI.cs` をアタッチ
3. スプライトを設定
4. Prefabに変換: `Assets/Prefabs/Enemies/Slime.prefab`
5. Hierarchyから削除

#### 5. テスト
1. `Assets/Scenes/TestScenes/` に自分専用のテストシーン作成
2. Slime Prefab を配置してテスト

#### 6. コミット & プッシュ
```bash
git add .
git commit -m "feat: スライム敵の基本実装

- SlimeAI.cs 追加
- Slime_Normal データアセット作成
- Slime Prefab 作成"
git push origin feature/enemy-slime
```

#### 7. Pull Request作成
GitHubでPull Requestを作成し、レビューを依頼

---

## シナリオ2: プレイヤーのダッシュ機能を実装

### タスク
プレイヤーがダッシュできるようにする

### 手順

#### 1. ブランチ作成
```bash
git checkout develop
git pull origin develop
git checkout -b feature/player-dash
```

#### 2. アビリティデータ作成
1. `Assets/Data/Abilities/` を右クリック
2. `Create > Metroidvania > Ability Data`
3. ファイル名: `Dash`
4. パラメータ設定:
   - Ability Name: "Dash"
   - Ability Type: Dash
   - Stamina Cost: 20
   - Dash Speed Multiplier: 2.0
   - Dash Duration: 0.3
   - など

#### 3. スクリプト実装
`Assets/Scripts/Player/PlayerDash.cs` を作成:

```csharp
using UnityEngine;
using Metroidvania.Data;

namespace Metroidvania.Player
{
    public class PlayerDash : MonoBehaviour
    {
        [SerializeField] private AbilityData dashData;
        [SerializeField] private Rigidbody2D rb;
        
        private bool isDashing = false;
        
        public void PerformDash(Vector2 direction)
        {
            if (!isDashing)
            {
                StartCoroutine(DashCoroutine(direction));
            }
        }
        
        private IEnumerator DashCoroutine(Vector2 direction)
        {
            isDashing = true;
            float originalSpeed = rb.velocity.magnitude;
            rb.velocity = direction * dashData.dashSpeedMultiplier * originalSpeed;
            
            yield return new WaitForSeconds(dashData.dashDuration);
            
            isDashing = false;
        }
    }
}
```

#### 4. Player Prefab に追加
1. `Assets/Prefabs/Player/Player.prefab` を開く
2. `PlayerDash.cs` をアタッチ
3. パラメータを設定

#### 5. 入力システム設定
`Assets/InputSystem_Actions.inputactions` にダッシュ入力を追加
- Action: "Dash"
- Binding: Shift キー

#### 6. テスト
自分専用のテストシーンでダッシュ機能をテスト

#### 7. コミット & プッシュ
```bash
git add .
git commit -m "feat: プレイヤーダッシュ機能実装

- PlayerDash.cs 追加
- Dash AbilityData 作成
- InputSystemにダッシュアクション追加"
git push origin feature/player-dash
```

---

## シナリオ3: 新しいゲームエリアを作成

### タスク
森のエリア（Area01_Forest）を作成

### 手順

#### 1. ブランチ作成
```bash
git checkout develop
git pull origin develop
git checkout -b feature/area-forest
```

#### 2. シーン作成
1. `Assets/Scenes/AreaScenes/` を右クリック
2. `Create > Scene`
3. シーン名: `Area01_Forest.unity`

#### 3. タイルマップ作成
1. Hierarchy で右クリック > `2D Object > Tilemap > Rectangular`
2. タイルパレットを作成: `Assets/Art/Tilemaps/ForestTilePalette`
3. タイルセットをインポート: `Assets/Art/Tilemaps/ForestTileset.png`
4. タイルパレットで配置

#### 4. 環境オブジェクト配置
1. `Assets/Prefabs/Environment/` から必要なPrefabを配置
2. 敵配置: `Assets/Prefabs/Enemies/` からPrefabを配置
3. アイテム配置: `Assets/Prefabs/Items/` からPrefabを配置

#### 5. ライティング設定
1. Global Light 2D を設定
2. 雰囲気に合わせた色調整

#### 6. BGM設定
1. Audio Manager を配置（Prefab）
2. 森エリア用BGMを設定

#### 7. テスト
シーンを再生して動作確認

#### 8. コミット & プッシュ
```bash
git add .
git commit -m "feat: 森エリア（Area01_Forest）作成

- Area01_Forest.unity シーン作成
- タイルマップ配置
- 敵・アイテム配置
- ライティング設定"
git push origin feature/area-forest
```

---

## シナリオ4: UI - 体力バーの実装

### タスク
画面上部に体力バーを表示

### 手順

#### 1. ブランチ作成
```bash
git checkout develop
git pull origin develop
git checkout -b feature/ui-health-bar
```

#### 2. UIスクリプト作成
`Assets/Scripts/UI/HealthBar.cs`:

```csharp
using UnityEngine;
using UnityEngine.UI;

namespace Metroidvania.UI
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Image fillImage;
        [SerializeField] private Gradient healthGradient;
        
        public void SetMaxHealth(int maxHealth)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
            fillImage.color = healthGradient.Evaluate(1f);
        }
        
        public void SetHealth(int health)
        {
            healthSlider.value = health;
            fillImage.color = healthGradient.Evaluate(
                healthSlider.normalizedValue
            );
        }
    }
}
```

#### 3. UI Prefab 作成
1. Hierarchy で UI > Canvas 作成
2. Canvas 下に UI > Slider 作成
3. 見た目を調整（位置、サイズ、色等）
4. `HealthBar.cs` をアタッチ
5. Prefab化: `Assets/Prefabs/UI/HealthBar.prefab`

#### 4. Player との連携
`Assets/Scripts/Player/PlayerHealth.cs`:

```csharp
using UnityEngine;
using Metroidvania.UI;

namespace Metroidvania.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private HealthBar healthBar;
        
        private int currentHealth;
        
        void Start()
        {
            currentHealth = maxHealth;
            healthBar.SetMaxHealth(maxHealth);
        }
        
        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            healthBar.SetHealth(currentHealth);
        }
    }
}
```

#### 5. テスト
テストシーンで体力バーの動作確認

#### 6. コミット & プッシュ
```bash
git add .
git commit -m "feat: 体力バーUI実装

- HealthBar.cs 追加
- HealthBar Prefab 作成
- PlayerHealth.cs で体力バー連携"
git push origin feature/ui-health-bar
```

---

## シナリオ5: 複数人での並行作業

### 状況
- 開発者A: プレイヤー実装
- 開発者B: 敵AI実装
- 開発者C: レベルデザイン

### ワークフロー

#### 開発者A（プレイヤー担当）
```bash
git checkout -b feature/player-jump
# Assets/Scripts/Player/ で作業
# Assets/Prefabs/Player/ で作業
git commit -m "feat: ジャンプ機能実装"
git push origin feature/player-jump
```

#### 開発者B（敵AI担当）
```bash
git checkout -b feature/enemy-bat
# Assets/Scripts/Enemy/ で作業
# Assets/Prefabs/Enemies/ で作業
git commit -m "feat: コウモリ敵実装"
git push origin feature/enemy-bat
```

#### 開発者C（レベルデザイン担当）
```bash
git checkout -b feature/area-cave
# Assets/Scenes/AreaScenes/ で作業
# 既存のPrefabを使用
git commit -m "feat: 洞窟エリア作成"
git push origin feature/area-cave
```

### ポイント
- 各開発者は異なるフォルダで作業しているため、コンフリクトが発生しにくい
- 共通Prefabを使用する場合は、直接編集せずVariantを作成
- 定期的に `git fetch origin` & `git rebase origin/develop` で最新を取り込む

---

## まとめ

- **機能ごとにブランチを作成**
- **自分の担当フォルダで作業**
- **こまめにコミット**
- **テストシーンで動作確認**
- **Pull Requestでレビュー**

これらのワークフローに従うことで、チーム開発でのコンフリクトを最小限に抑えられます。
