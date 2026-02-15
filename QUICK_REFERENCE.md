# クイックリファレンス

開発中によく使うコマンドやパスをまとめた簡易リファレンスです。

## 📁 重要なフォルダパス

### スクリプト
```
Assets/Scripts/Player/      - プレイヤー関連
Assets/Scripts/Enemy/       - 敵関連
Assets/Scripts/Environment/ - 環境オブジェクト
Assets/Scripts/UI/          - UI関連
Assets/Scripts/Managers/    - マネージャー
```

### データ
```
Assets/Data/Enemies/        - 敵データ（ScriptableObject）
Assets/Data/Items/          - アイテムデータ
Assets/Data/Abilities/      - アビリティデータ
Assets/Data/Player/         - プレイヤーデータ
```

### シーン
```
Assets/Scenes/MainScenes/   - メインシーン
Assets/Scenes/AreaScenes/   - エリアシーン
Assets/Scenes/TestScenes/   - テストシーン（各自作成）
```

### アセット
```
Assets/Prefabs/             - プレハブ
Assets/Art/Sprites/         - スプライト画像
Assets/Art/Animations/      - アニメーション
Assets/Audio/Music/         - BGM
Assets/Audio/SFX/           - 効果音
```

## 🔧 よく使うGitコマンド

### ブランチ作成・切り替え
```bash
# developブランチの最新を取得
git checkout develop
git pull origin develop

# 新しいブランチ作成
git checkout -b feature/機能名

# ブランチ一覧
git branch -a
```

### コミット・プッシュ
```bash
# 変更を確認
git status

# ファイル追加
git add .

# コミット
git commit -m "種類: 説明"

# プッシュ
git push origin feature/機能名
```

### コミットメッセージの種類
```
feat:      新機能
fix:       バグ修正
docs:      ドキュメント
refactor:  リファクタリング
test:      テスト追加
```

### 最新の変更を取り込む
```bash
git fetch origin
git rebase origin/develop
```

## 🎮 Unity よく使う操作

### ScriptableObject 作成
```
1. Assets/Data/該当フォルダ を右クリック
2. Create > Metroidvania > データタイプ
3. ファイル名を設定
4. Inspectorでパラメータ設定
```

### Prefab 作成
```
1. Hierarchy でオブジェクト作成
2. 必要なコンポーネント追加
3. Assets/Prefabs/該当フォルダ にドラッグ
4. Hierarchyから削除（必要に応じて）
```

### Scene 作成
```
1. Assets/Scenes/TestScenes/ を右クリック
2. Create > Scene
3. シーン名を設定（自分の名前_機能名）
```

## 📝 命名規則

### スクリプト
```csharp
// クラス名: PascalCase
public class PlayerMovement { }

// メソッド名: PascalCase
public void MovePlayer() { }

// 変数名: camelCase
private float moveSpeed;

// SerializeField: camelCase
[SerializeField] private int maxHealth;
```

### ファイル・フォルダ
```
PascalCase          - スクリプト、Prefab
snake_case          - アセット（画像、音声等）
数字プレフィックス  - 順序が重要な場合（例: 01_Forest, 02_Cave）
```

### ScriptableObject
```
EnemyType_Variant   - 例: Slime_Normal, Slime_Elite
Item_Name           - 例: Item_HealthPotion
```

## 🚫 やってはいけないこと

```bash
# ❌ mainブランチに直接コミット
git checkout main
git commit -m "変更"

# ⭕ featureブランチを作成
git checkout -b feature/my-feature
```

```bash
# ❌ force push
git push -f origin branch-name

# ⭕ 通常のpush
git push origin branch-name
```

```
❌ 他人のテストシーンを編集
❌ MainSceneを直接大幅変更（事前相談必須）
❌ Managersフォルダを無断で変更
❌ .metaファイルを削除
```

## ✅ 推奨事項

```
⭕ 自分専用のテストシーンで開発
⭕ ScriptableObjectでデータ管理
⭕ Prefabを活用
⭕ こまめにコミット
⭕ Pull Request前にビルド確認
⭕ コードレビューを依頼
```

## 🔗 ドキュメントへのリンク

| ドキュメント | 内容 |
|------------|------|
| [README.md](README.md) | プロジェクト概要 |
| [SETUP.md](SETUP.md) | セットアップ手順 |
| [CONTRIBUTING.md](CONTRIBUTING.md) | 開発ガイドライン |
| [FOLDER_STRUCTURE.md](FOLDER_STRUCTURE.md) | フォルダ構造詳細 |
| [WORKFLOW_EXAMPLES.md](WORKFLOW_EXAMPLES.md) | ワークフロー例 |

## 🆘 困ったときは

1. このドキュメントを確認
2. 各ドキュメントの該当セクションを参照
3. チームのコミュニケーションツールで質問
4. 先輩開発者に相談

## 🎯 開発開始チェックリスト

作業開始前に確認:

- [ ] Unity 6000.3.8f1 をインストール済み
- [ ] Git LFS をセットアップ済み（推奨）
- [ ] リポジトリをクローン済み
- [ ] Unity エディタ設定を確認済み
  - [ ] Asset Serialization Mode: Force Text
  - [ ] Version Control Mode: Visible Meta Files
- [ ] developブランチから作業ブランチを作成
- [ ] 自分の担当フォルダを確認
- [ ] テストシーンを作成

## 📊 開発フロー（簡易版）

```
1. git checkout -b feature/機能名
   ↓
2. 開発・テスト（自分のフォルダ内）
   ↓
3. git commit -m "説明"
   ↓
4. git push origin feature/機能名
   ↓
5. Pull Request作成
   ↓
6. レビュー → マージ
```

---

**覚えておくこと**: 
- **1機能 = 1ブランチ**
- **自分の担当フォルダで作業**
- **こまめにコミット・プッシュ**
- **わからないことは質問する**
