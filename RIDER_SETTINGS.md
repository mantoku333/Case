# Rider Cleanup Settings

このリポジトリでは、Rider のコード整形/クリーンアップを次の共有設定で統一します。

- `.editorconfig`
- `CaseStudy.sln.DotSettings`

## 1. Rider で `.editorconfig` を使う

1. `Settings > Editor > Code Style > Enable EditorConfig support` を有効化
2. `Settings > Editor > Code Style > C#` で `Use indents from .editorconfig` が有効であることを確認

## 2. サイレントクリーンアップの既定を確認

`CaseStudy.sln.DotSettings` で `Silent Cleanup Profile` を `Built-in: Full Cleanup` に設定しています。

- `Ctrl+R, G` (Silent Code Cleanup) で適用
- `Code > Reformat and Cleanup...` でも同じプロファイルを使用可能

## 3. 推奨運用

- コミット前に変更した C# ファイルへクリーンアップを実行
- 大量変更になるため、初回は機能追加PRとは分けて整形専用コミットにする
