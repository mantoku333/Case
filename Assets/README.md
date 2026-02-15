# Assets フォルダ構造

このフォルダには、2Dメトロイドヴァニアゲームのすべてのアセットが含まれています。

## 📂 フォルダ概要

| フォルダ | 用途 | 担当例 |
|---------|------|--------|
| **Scripts/** | C#スクリプト | 機能別に分担 |
| **Scenes/** | シーンファイル | エリア別に分担 |
| **Prefabs/** | プレハブ | カテゴリ別に分担 |
| **Art/** | グラフィックアセット | アーティスト |
| **Audio/** | 音声ファイル | サウンド担当 |
| **Data/** | ScriptableObject | データ設計担当 |
| **Resources/** | 動的ロードアセット | 全員共有 |
| **Plugins/** | サードパーティ | システム担当 |
| **Settings/** | Unity設定 | システム担当 |

## 🎯 開発時の注意点

### 担当フォルダで作業
各開発者は自分の担当フォルダ内で作業することで、マージコンフリクトを回避できます。

**例: プレイヤー担当者**
- Scripts/Player/
- Prefabs/Player/
- Art/Sprites/Player/
- Art/Animations/Player/
- Data/Player/

### テストシーンを使用
メインシーンではなく、`Scenes/TestScenes/` に自分専用のテストシーンを作成して開発してください。

### Prefabを活用
シーン内に直接オブジェクトを配置するのではなく、Prefab化してから配置することで、変更管理が容易になります。

### ScriptableObjectでデータ管理
敵のステータス、アイテムデータなどは ScriptableObject で管理することで、プログラマーとデザイナーが並行作業できます。

## 📖 詳細ドキュメント

各フォルダには README.md があります。詳細はそちらを参照してください。

ルートディレクトリには以下のドキュメントがあります:
- **README.md** - プロジェクト概要
- **SETUP.md** - セットアップ手順
- **CONTRIBUTING.md** - 開発ガイドライン
- **FOLDER_STRUCTURE.md** - フォルダ構造詳細
- **WORKFLOW_EXAMPLES.md** - ワークフロー例
- **QUICK_REFERENCE.md** - クイックリファレンス

## 🚀 クイックスタート

1. `Scenes/TestScenes/` に自分のテストシーン作成
2. 自分の担当フォルダで開発
3. こまめにGitコミット
4. Pull Request作成

詳細は [SETUP.md](../SETUP.md) を参照してください。
