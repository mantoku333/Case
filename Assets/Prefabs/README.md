# Prefabs Folder

プレハブを管理するフォルダです。シーン内に直接配置せず、プレハブ化することでコンフリクトを減らします。

## フォルダ構成

### Player/
プレイヤー関連のプレハブ
- `Player.prefab` - プレイヤーキャラクター本体
- `PlayerWeapon.prefab` - プレイヤーの武器
- `PlayerUI.prefab` - プレイヤー専用UI

### Enemies/
敵キャラクターのプレハブ
- `Enemy_Slime.prefab`
- `Enemy_Bat.prefab`
- `Enemy_Knight.prefab`
- など、各敵タイプごとに作成

### Environment/
環境オブジェクト
- `Door.prefab` - ドア
- `Switch.prefab` - スイッチ
- `MovingPlatform.prefab` - 動くプラットフォーム
- `Checkpoint.prefab` - チェックポイント

### Items/
アイテム・収集品
- `HealthPotion.prefab`
- `Collectible_Coin.prefab`
- `PowerUp_DoubleJump.prefab`

### UI/
UIプレハブ
- `MainMenu.prefab`
- `PauseMenu.prefab`
- `HUD.prefab`
- `DialogBox.prefab`

### Projectiles/
弾・投擲物
- `PlayerBullet.prefab`
- `EnemyProjectile.prefab`

### Effects/
エフェクト
- `HitEffect.prefab`
- `ExplosionEffect.prefab`
- `DustParticle.prefab`

## Prefab活用のベストプラクティス

### 1. Prefab Variants の活用
基本プレハブから派生を作成

```
Enemy_Base.prefab (基本)
  ├─ Enemy_Slime.prefab (バリアント: 緑色)
  ├─ Enemy_Slime_Red.prefab (バリアント: 赤色、強化版)
  └─ Enemy_Slime_Boss.prefab (バリアント: ボス版)
```

### 2. Nested Prefabs
複雑なオブジェクトは分割して管理

```
Player.prefab
  ├─ PlayerBody (ビジュアル)
  ├─ WeaponSystem.prefab (Nested)
  ├─ EffectSystem.prefab (Nested)
  └─ AudioSystem.prefab (Nested)
```

### 3. Prefab Override の最小化
- シーン内でのOverrideは最小限に
- 必要な場合は Prefab Variants を作成

### 4. 命名規則
- わかりやすい名前を付ける
- プレフィックスを活用: `UI_`, `FX_`, `Enemy_` 等

## コンフリクト回避

- 1人1プレハブの原則
- 共通プレハブの編集は事前相談
- テスト用のプレハブコピーを作成して実験
