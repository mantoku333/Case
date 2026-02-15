# セットアップガイド

このガイドでは、プロジェクトのセットアップと開発を開始するための手順を説明します。

## 必要な環境

- **Unity**: Unity 6000.3.8f1
- **Git**: 最新バージョン推奨
- **Git LFS**: 大きなバイナリファイル管理用（推奨）
- **IDE**: Visual Studio, Rider, または VS Code

## 初回セットアップ

### 1. リポジトリのクローン

```bash
git clone https://github.com/mantoku333/Case.git
cd Case
```

### 2. Git LFS のセットアップ（推奨）

大きなアセット（画像、音声、3Dモデル等）を効率的に管理するため、Git LFSの使用を推奨します。

```bash
# Git LFSをインストール（未インストールの場合）
# Windows: https://git-lfs.github.com/ からインストーラーをダウンロード
# Mac: brew install git-lfs
# Linux: sudo apt-get install git-lfs

# Git LFSを有効化
git lfs install

# LFS管理ファイルを取得
git lfs pull
```

### 3. Unity Hub で Unity 6000.3.8f1 をインストール

1. Unity Hub を開く
2. 「Installs」タブを選択
3. 「Install Editor」をクリック
4. バージョン **6000.3.8f1** を選択してインストール
5. 必要なモジュールを選択:
   - **WebGL Build Support**（Web公開する場合）
   - **Windows Build Support** または **Mac Build Support**
   - **Documentation**（推奨）

### 4. Unity Hub でプロジェクトを開く

1. Unity Hub の「Projects」タブを選択
2. 「Add」をクリック
3. クローンした `Case` フォルダを選択
4. プロジェクトをダブルクリックして開く

### 5. Unity エディタの設定確認

プロジェクトを開いたら、以下の設定を確認してください：

#### Asset Serialization Mode
1. `Edit > Project Settings > Editor`
2. `Asset Serialization Mode` が **Force Text** になっていることを確認
   - これにより、YAMLテキスト形式で保存され、Gitマージが容易になります

#### Version Control Mode
1. `Edit > Project Settings > Editor`
2. `Version Control Mode` が **Visible Meta Files** になっていることを確認
   - これにより、`.meta`ファイルが可視化され、Gitで管理しやすくなります

#### Line Ending
1. `Edit > Project Settings > Editor`
2. `Line Endings For New Scripts` を **Unix (LF)** に設定
   - チーム全体で統一された改行コードを使用

## 開発の開始

### ブランチの作成

作業を開始する前に、必ず作業用ブランチを作成してください。

```bash
# developブランチを最新にする
git checkout develop
git pull origin develop

# 新しい機能ブランチを作成
git checkout -b feature/あなたの機能名
```

例:
```bash
git checkout -b feature/player-movement
git checkout -b feature/enemy-slime
git checkout -b feature/ui-health-bar
```

### 作業フォルダの確認

`FOLDER_STRUCTURE.md` を参照して、あなたの担当フォルダを確認してください。

例：プレイヤー担当の場合
- `Assets/Scripts/Player/`
- `Assets/Prefabs/Player/`
- `Assets/Art/Sprites/Player/`
- `Assets/Art/Animations/Player/`
- `Assets/Data/Player/`

### テストシーンの作成

他の開発者とのコンフリクトを避けるため、自分専用のテストシーンを作成します。

1. `Assets/Scenes/TestScenes/` を右クリック
2. `Create > Scene` を選択
3. シーン名: `YourName_Test.unity` （例: `Tanaka_PlayerTest.unity`）
4. このシーンで開発・テストを行う

## 日常的なワークフロー

### 1. 作業開始前

```bash
# 最新の変更を取得
git fetch origin
git rebase origin/develop
```

### 2. 作業中

```bash
# 変更をこまめにコミット
git add .
git commit -m "feat: 実装内容を記述"

# リモートにプッシュ（バックアップ）
git push origin feature/あなたの機能名
```

### 3. 作業完了後

```bash
# 最新のdevelopを取り込む
git fetch origin
git rebase origin/develop

# Unityでビルドが通ることを確認
# テストを実行

# Pull Requestを作成
# GitHub上でPull Requestを作成し、レビューを依頼
```

## トラブルシューティング

### Unity が起動しない

1. Unity Hub でプロジェクトを削除
2. `Library/` フォルダを削除
3. Unity Hub から再度プロジェクトを追加

### .meta ファイルが消えた

```bash
# Unityエディタを閉じる
# 該当するアセットを削除して、再度インポート
```

### マージコンフリクトが発生

詳細は `CONTRIBUTING.md` の「トラブルシューティング」セクションを参照してください。

### 大きなファイルのプッシュに失敗

Git LFS を使用していない場合、大きなファイルのプッシュに失敗することがあります。

```bash
# Git LFSをセットアップ
git lfs install

# 該当ファイルをLFS管理に追加（.gitattributesで既に設定済み）
git lfs track "*.png"
git lfs track "*.mp3"

# 再度プッシュ
git push origin feature/あなたの機能名
```

## よくある質問

### Q: 他の人のブランチを見たい

```bash
# リモートブランチ一覧を確認
git branch -r

# 特定のブランチをチェックアウト
git checkout -b local-branch-name origin/remote-branch-name
```

### Q: 間違えてメインブランチで作業してしまった

```bash
# 変更を一時退避
git stash

# 新しいブランチを作成
git checkout -b feature/correct-branch-name

# 変更を適用
git stash pop
```

### Q: コミットメッセージを間違えた

```bash
# 直前のコミットメッセージを修正
git commit --amend -m "正しいメッセージ"

# リモートに反映（まだプッシュしていない場合のみ）
git push origin feature/あなたの機能名
```

## 参考資料

- [README.md](README.md) - プロジェクト概要とフォルダ構造
- [CONTRIBUTING.md](CONTRIBUTING.md) - 開発ガイドライン
- [FOLDER_STRUCTURE.md](FOLDER_STRUCTURE.md) - 詳細なフォルダ構造図
- [Unity Documentation](https://docs.unity3d.com/)
- [Git Documentation](https://git-scm.com/doc)

## サポート

質問や問題がある場合は、チームのコミュニケーションツールで相談してください。
