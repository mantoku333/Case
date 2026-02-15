# 開発ガイドライン

このドキュメントは、チーム開発でのコンフリクトを最小限に抑えるための開発ガイドラインです。

## ブランチ戦略

### メインブランチ
- `main`: 常に安定したビルド可能な状態を保つ
- `develop`: 開発中の最新コード

### フィーチャーブランチ
- 命名規則: `feature/<機能名>` (例: `feature/player-movement`, `feature/enemy-ai`)
- 1つの機能につき1つのブランチ
- 作業完了後はPull Requestを作成

### ブランチ作成例
```bash
git checkout develop
git pull origin develop
git checkout -b feature/player-dash
```

## コミットメッセージ

### 形式
```
<type>: <subject>

<body>
```

### タイプ
- `feat`: 新機能
- `fix`: バグ修正
- `docs`: ドキュメント変更
- `style`: コードスタイル修正（動作に影響なし）
- `refactor`: リファクタリング
- `test`: テスト追加・修正
- `chore`: ビルドプロセス、補助ツール変更

### 例
```
feat: プレイヤーのダッシュ機能を実装

- InputSystemを使用したダッシュ入力検知
- ダッシュアニメーション追加
- スタミナ消費システム実装
```

## フォルダ別の担当分け推奨

### 推奨される作業分担

1. **プレイヤーシステム担当** (1-2人)
   - `Scripts/Player/`
   - `Prefabs/Player/`
   - `Art/Sprites/Player/`, `Art/Animations/Player/`
   - `Data/Player/`

2. **敵AI担当** (2-3人)
   - `Scripts/Enemy/`
   - `Prefabs/Enemies/`
   - `Art/Sprites/Enemies/`, `Art/Animations/Enemies/`
   - `Data/Enemies/`
   - 各自が異なる敵タイプを担当

3. **環境・レベルデザイン担当** (2-3人)
   - `Scripts/Environment/`
   - `Prefabs/Environment/`
   - `Art/Tilemaps/`
   - `Scenes/AreaScenes/`
   - 各自が異なるエリアを担当

4. **UI担当** (1人)
   - `Scripts/UI/`
   - `Prefabs/UI/`
   - `Art/Sprites/UI/`

5. **アイテム・収集要素担当** (1人)
   - `Scripts/Items/`
   - `Prefabs/Items/`
   - `Data/Items/`, `Data/Abilities/`

6. **オーディオ担当** (1人)
   - `Scripts/Audio/`
   - `Audio/Music/`, `Audio/SFX/`

7. **システム・マネージャー担当** (1人)
   - `Scripts/Managers/`
   - `Scripts/Utilities/`
   - `Data/GameSettings/`

## コンフリクト回避のルール

### 1. シーン編集のルール

**問題**: Unityのシーンファイルは大きなYAMLファイルで、複数人が同時編集するとコンフリクトが起きやすい

**解決策**:
- **各自専用のテストシーンを作成**: `Scenes/TestScenes/PlayerTest.unity`, `Scenes/TestScenes/EnemyTest.unity` など
- **エリアごとにシーンを分割**: `Scenes/AreaScenes/Area01_Forest.unity`, `Scenes/AreaScenes/Area02_Cave.unity`
- **Additive Scene Loading を活用**: メインシーン + UI シーン + エリアシーン の組み合わせ
- **Prefab編集を優先**: シーン内の直接編集ではなく、Prefabを編集してシーンに配置

### 2. Prefab編集のルール

**推奨事項**:
- 大きなPrefabは分割する（例: プレイヤーPrefab = Body + WeaponSystem + EffectSystem）
- Prefab Variantsを活用（例: 基本敵Prefab → 色違いバリアント）
- Nested Prefabsで階層的に管理

### 3. スクリプト編集のルール

**推奨事項**:
- 1クラス1ファイルの原則
- 共通インターフェースを先に定義
- マネージャークラスは1人が責任を持つ
- Pull Request前にコードレビューを依頼

### 4. ScriptableObject の活用

**メリット**:
- データとコードを分離
- デザイナーがコード変更なしで調整可能
- マージコンフリクトが起きにくい

**使用例**:
```csharp
// Data/Enemies/Slime.asset
[CreateAssetMenu(fileName = "EnemyData", menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public int maxHealth;
    public float moveSpeed;
    public int attackDamage;
}
```

### 5. Git運用のベストプラクティス

#### 作業開始前
```bash
# 最新のdevelopを取得
git checkout develop
git pull origin develop

# 作業ブランチ作成
git checkout -b feature/your-feature
```

#### 作業中
```bash
# こまめにコミット
git add .
git commit -m "feat: 実装内容"

# 定期的にdevelopの変更を取り込む
git fetch origin
git rebase origin/develop
```

#### Pull Request前
```bash
# developの最新を取り込む
git fetch origin
git rebase origin/develop

# ビルドとテストを実行
# Unityでビルドが通ることを確認
```

## コードレビューチェックリスト

Pull Request作成時は以下を確認:

- [ ] ビルドが通る
- [ ] 新しい警告(Warning)が出ていない
- [ ] 既存機能が壊れていない
- [ ] コミットメッセージが規約に従っている
- [ ] 不要なファイルがコミットされていない（`*.meta`ファイルは含める）
- [ ] シーンファイルの変更が最小限
- [ ] コメントが適切に付いている（必要な場合）

## Unityエディタ設定

### Asset Serialization Mode
- **設定**: `Edit > Project Settings > Editor > Asset Serialization Mode`
- **推奨**: `Force Text`
- **理由**: YAMLテキスト形式でマージしやすくなる

### Version Control Mode
- **設定**: `Edit > Project Settings > Editor > Version Control Mode`
- **推奨**: `Visible Meta Files`
- **理由**: `.meta`ファイルが可視化され、Gitで管理しやすくなる

## トラブルシューティング

### マージコンフリクトが発生した場合

#### シーンファイルのコンフリクト
```bash
# 自分の変更を優先する場合
git checkout --ours path/to/scene.unity
git add path/to/scene.unity

# 相手の変更を優先する場合
git checkout --theirs path/to/scene.unity
git add path/to/scene.unity

# どちらでもない場合は、Unityエディタで再作成を検討
```

#### Prefabのコンフリクト
- Unity Editorで両方のバージョンを開いて手動マージ
- またはSmart Merge Toolを使用（Unity公式ツール）

### .metaファイルが消えた場合
```bash
# Unity Editorを閉じて、該当アセットを削除し、再インポート
```

## 参考リンク

- [Unity - Version Control](https://docs.unity3d.com/Manual/VersionControl.html)
- [Unity - Smart Merge](https://docs.unity3d.com/Manual/SmartMerge.html)
- [Git - Branching Strategy](https://git-scm.com/book/en/v2/Git-Branching-Branching-Workflows)

## 質問・相談

開発中の質問や相談は、チームのコミュニケーションツール（Discord、Slack等）で行ってください。
