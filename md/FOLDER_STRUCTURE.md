# プロジェクト構造ダイアグラム

## 完全なフォルダツリー

```
Case/
├── .git/
├── .gitignore                      # Unity用.gitignore
├── .gitattributes                  # Unity用マージ設定
├── .vsconfig                       # Visual Studio設定
├── README.md                       # プロジェクト説明
├── CONTRIBUTING.md                 # 開発ガイドライン
│
├── Assets/                         # Unityアセットフォルダ
│   │
│   ├── Scripts/                    # C#スクリプト
│   │   ├── README.md
│   │   ├── Player/                 # プレイヤー関連
│   │   │   └── .gitkeep
│   │   ├── Enemy/                  # 敵関連
│   │   │   └── .gitkeep
│   │   ├── Environment/            # 環境オブジェクト
│   │   │   └── .gitkeep
│   │   ├── Items/                  # アイテム
│   │   │   └── .gitkeep
│   │   ├── UI/                     # UI
│   │   │   └── .gitkeep
│   │   ├── Managers/               # マネージャー
│   │   │   └── .gitkeep
│   │   ├── Utilities/              # ユーティリティ
│   │   │   └── .gitkeep
│   │   ├── Combat/                 # 戦闘システム
│   │   │   └── .gitkeep
│   │   ├── Movement/               # 移動システム
│   │   │   └── .gitkeep
│   │   ├── Audio/                  # オーディオシステム
│   │   │   └── .gitkeep
│   │   └── Data/                   # ScriptableObjectクラス定義
│   │       ├── EnemyData.cs
│   │       ├── ItemData.cs
│   │       └── AbilityData.cs
│   │
│   ├── Scenes/                     # シーンファイル
│   │   ├── README.md
│   │   ├── SampleScene.unity       # デフォルトシーン
│   │   ├── MainScenes/             # メインシーン
│   │   │   └── .gitkeep
│   │   ├── AreaScenes/             # エリアシーン
│   │   │   └── .gitkeep
│   │   └── TestScenes/             # テストシーン
│   │       └── .gitkeep
│   │
│   ├── Prefabs/                    # プレハブ
│   │   ├── README.md
│   │   ├── Player/
│   │   │   └── .gitkeep
│   │   ├── Enemies/
│   │   │   └── .gitkeep
│   │   ├── Environment/
│   │   │   └── .gitkeep
│   │   ├── Items/
│   │   │   └── .gitkeep
│   │   ├── UI/
│   │   │   └── .gitkeep
│   │   ├── Projectiles/
│   │   │   └── .gitkeep
│   │   └── Effects/
│   │       └── .gitkeep
│   │
│   ├── Art/                        # アートアセット
│   │   ├── Sprites/                # スプライト
│   │   │   ├── Player/
│   │   │   │   └── .gitkeep
│   │   │   ├── Enemies/
│   │   │   │   └── .gitkeep
│   │   │   ├── Environment/
│   │   │   │   └── .gitkeep
│   │   │   ├── Items/
│   │   │   │   └── .gitkeep
│   │   │   ├── UI/
│   │   │   │   └── .gitkeep
│   │   │   └── Effects/
│   │   │       └── .gitkeep
│   │   ├── Animations/             # アニメーション
│   │   │   ├── Player/
│   │   │   │   └── .gitkeep
│   │   │   ├── Enemies/
│   │   │   │   └── .gitkeep
│   │   │   └── Effects/
│   │   │       └── .gitkeep
│   │   ├── Tilemaps/               # タイルマップ
│   │   │   └── .gitkeep
│   │   ├── Materials/              # マテリアル
│   │   │   └── .gitkeep
│   │   └── VFX/                    # ビジュアルエフェクト
│   │       └── .gitkeep
│   │
│   ├── Audio/                      # オーディオ
│   │   ├── Music/                  # BGM
│   │   │   └── .gitkeep
│   │   └── SFX/                    # 効果音
│   │       ├── Player/
│   │       │   └── .gitkeep
│   │       ├── Enemy/
│   │       │   └── .gitkeep
│   │       ├── Environment/
│   │       │   └── .gitkeep
│   │       └── UI/
│   │           └── .gitkeep
│   │
│   ├── Data/                       # ScriptableObjectアセット
│   │   ├── README.md
│   │   ├── Player/
│   │   │   └── .gitkeep
│   │   ├── Enemies/
│   │   │   └── .gitkeep
│   │   ├── Items/
│   │   │   └── .gitkeep
│   │   ├── Abilities/
│   │   │   └── .gitkeep
│   │   └── GameSettings/
│   │       └── .gitkeep
│   │
│   ├── Resources/                  # Resourcesフォルダ
│   │   └── .gitkeep
│   │
│   ├── Plugins/                    # サードパーティプラグイン
│   │   └── .gitkeep
│   │
│   └── Settings/                   # Unity設定
│       ├── Renderer2D.asset
│       ├── UniversalRP.asset
│       └── ...
│
├── Packages/                       # Unityパッケージ管理
│   ├── manifest.json
│   └── packages-lock.json
│
└── ProjectSettings/                # プロジェクト設定
    └── ProjectVersion.txt
```

## 担当分担例（10人チーム）

### 開発者A: プレイヤーシステム
- Scripts/Player/
- Prefabs/Player/
- Art/Sprites/Player/
- Art/Animations/Player/
- Data/Player/

### 開発者B, C, D: 敵AI（3人）
各自が異なる敵タイプを担当
- Scripts/Enemy/
- Prefabs/Enemies/
- Art/Sprites/Enemies/
- Art/Animations/Enemies/
- Data/Enemies/

### 開発者E, F: レベルデザイン（2人）
エリアを分担
- Scripts/Environment/
- Scenes/AreaScenes/
- Art/Tilemaps/
- Prefabs/Environment/

### 開発者G: UI
- Scripts/UI/
- Prefabs/UI/
- Art/Sprites/UI/

### 開発者H: アイテム・収集要素
- Scripts/Items/
- Prefabs/Items/
- Data/Items/
- Data/Abilities/

### 開発者I: オーディオ
- Scripts/Audio/
- Audio/Music/
- Audio/SFX/

### 開発者J: システム・マネージャー
- Scripts/Managers/
- Scripts/Utilities/
- Data/GameSettings/

## コンフリクト発生確率

### 低リスク（緑）
- 個別のプレハブファイル
- ScriptableObjectアセット
- 個別のスクリプトファイル
- テストシーン

### 中リスク（黄）
- エリアシーン（複数人で編集する場合）
- 共通スクリプト（Managers等）

### 高リスク（赤）
- メインシーン
- 共通設定ファイル

⚠️ **高リスク項目は、編集前にチームで調整すること**
