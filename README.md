# 2D Metroidvania Platformer - Project Structure

このプロジェクトは、10人規模のチーム開発を想定した2Dメトロイドヴァニア・プラットフォーマーゲームです。
コンフリクトを最小限に抑えるための構造化されたフォルダ構成を採用しています。

## プロジェクト概要

- **エンジン**: Unity 6000.3.8f1
- **ジャンル**: 2D Metroidvania Platformer
- **開発人数**: 10人規模
- **目的**: チーム開発でのマージコンフリクトの最小化

## フォルダ構造

```
Assets/
├── Scripts/              # スクリプトファイル（機能別に分類）
│   ├── Player/          # プレイヤー関連
│   ├── Enemy/           # 敵関連
│   ├── Environment/     # 環境オブジェクト
│   ├── Items/           # アイテム・収集品
│   ├── UI/              # UI関連
│   ├── Managers/        # ゲーム管理（GameManager, SaveManager等）
│   ├── Utilities/       # ユーティリティ・ヘルパー
│   ├── Combat/          # 戦闘システム
│   ├── Movement/        # 移動・物理関連
│   └── Audio/           # オーディオシステム
│
├── Scenes/              # シーンファイル（エリア別に分類）
│   ├── MainScenes/      # メインシーン（タイトル、ゲーム本編等）
│   ├── AreaScenes/      # ゲームエリア（各マップ・ステージ）
│   └── TestScenes/      # テスト・開発用シーン
│
├── Prefabs/             # プレハブ（カテゴリ別に分類）
│   ├── Player/          # プレイヤー関連
│   ├── Enemies/         # 敵キャラクター
│   ├── Environment/     # 環境オブジェクト
│   ├── Items/           # アイテム
│   ├── UI/              # UIプレハブ
│   ├── Projectiles/     # 弾・投擲物
│   └── Effects/         # エフェクト
│
├── Art/                 # アートアセット
│   ├── Sprites/         # スプライト画像
│   │   ├── Player/
│   │   ├── Enemies/
│   │   ├── Environment/
│   │   ├── Items/
│   │   ├── UI/
│   │   └── Effects/
│   ├── Animations/      # アニメーション
│   │   ├── Player/
│   │   ├── Enemies/
│   │   └── Effects/
│   ├── Tilemaps/        # タイルマップ関連
│   ├── Materials/       # マテリアル
│   └── VFX/             # ビジュアルエフェクト
│
├── Audio/               # オーディオファイル
│   ├── Music/           # BGM
│   └── SFX/             # 効果音
│       ├── Player/
│       ├── Enemy/
│       ├── Environment/
│       └── UI/
│
├── Data/                # ScriptableObject等のデータアセット
│   ├── Player/          # プレイヤーデータ
│   ├── Enemies/         # 敵データ
│   ├── Items/           # アイテムデータ
│   ├── Abilities/       # アビリティデータ
│   └── GameSettings/    # ゲーム設定
│
├── Resources/           # Resourcesフォルダ（動的ロード用）
├── Plugins/             # サードパーティプラグイン
└── Settings/            # Unity設定ファイル
```

## コンフリクト防止のためのベストプラクティス

### 1. **作業分担の原則**
- 各開発者は基本的に**1つの機能フォルダ**に集中する
- 例: プレイヤー担当は `Scripts/Player/`, `Prefabs/Player/`, `Art/Sprites/Player/` を担当
- 共通ファイル（Managers等）の編集は事前に調整する

### 2. **シーンの分割**
- メインシーンは極力編集しない
- 各エリアを個別のシーンとして作成
- Additive Scene Loadingを活用して複数シーンを組み合わせる

### 3. **Prefabの活用**
- シーン内に直接オブジェクトを配置せず、できるだけPrefabを使用
- Prefab Variantsを活用して派生を管理
- Nested Prefabsで階層的に管理

### 4. **ScriptableObjectの活用**
- 敵のステータス、アイテムデータ等はScriptableObjectで管理
- コード変更なしでデータ調整が可能

### 5. **Git設定の重要性**
- `.gitattributes` でUnityファイルのマージ戦略を設定
- 大きなバイナリファイルは Git LFS を検討

## 開発ワークフロー

詳細な開発ワークフローについては [CONTRIBUTING.md](CONTRIBUTING.md) を参照してください。

## Unity バージョン

- Unity 6000.3.8f1

## セットアップ

1. このリポジトリをクローン
2. Unity Hub で Unity 6000.3.8f1 をインストール
3. Unity Hub からプロジェクトを開く

## ライセンス

（プロジェクトのライセンスを記載）
