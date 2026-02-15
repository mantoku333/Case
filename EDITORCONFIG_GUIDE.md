# .editorconfig 説明書

このドキュメントは、現在の `.editorconfig` の設定意図を共有するための説明用ガイドです。

## 1. 全体方針

- 文字コードは `utf-8-bom`
- 改行コードは `LF`
- 末尾空白は削除
- ファイル末尾に改行を入れる
- インデントはスペース

`root = true` により、このリポジトリ配下ではこの設定を起点にします。

## 2. ファイル種別ごとのインデント

- 全ファイル (`[*]`): 4 スペース
- C# (`[*.cs]`): 4 スペース
- JSON (`[*.json]`): 2 スペース
- YAML (`[*.{yml,yaml}]`): 2 スペース
- XML 系 (`[*.{xml,asmdef,csproj,sln}]`): 2 スペース
- シェーダー (`[*.{shader,hlsl,cginc,compute}]`): 4 スペース

## 3. C# 命名規則

### 命名スタイル定義

- `pascal_case`: `PlayerController`
- `camel_case`: `playerController`
- `underscore_camel_case`: `_playerController`
- `i_pascal_case`: `IPlayerService`
- `upper_snake_case`: `MAX_HP`

### 対象シンボル

- `public_members`: namespace/class/struct/enum/delegate/event/method/property
- `interfaces`: interface
- `private_fields`: field（private/internal/protected 系）
- `locals_and_parameters`: local/parameter
- `local_functions`: local_function
- `constants`: const field
- `type_parameters`: ジェネリック型パラメータ

### 適用ルール（すべて `warning`）

- 定数: `PascalCase`
- インターフェース: `I + PascalCase`
- 型パラメータ: `PascalCase`
- public メンバー: `PascalCase`
- private フィールド: `camelCase`
- ローカル変数・引数: `camelCase`
- ローカル関数: `PascalCase`

補足:
- `_camelCase` スタイル定義はありますが、現行ルールでは private フィールドに適用していません（`camelCase` を適用）。

## 4. C# コードスタイル

- `var`:
- 組み込み型は明示型推奨（`false:suggestion`）
- 型が明白な場合は `var` 推奨（`true:suggestion`）
- それ以外は明示型推奨（`false:suggestion`）

- 式形式メンバー:
- メソッドは 1 行で書ける場合のみ式形式推奨
- プロパティとアクセサは式形式推奨

- ブレース:
- Allman スタイル（`{` は改行）
- `else/catch/finally` も改行開始

- using:
- `System` 名前空間を先頭
- using グループは分割しない

- `this.` 修飾:
- フィールド/プロパティ/メソッド/イベントいずれも不要（suggestion）

## 5. severity の見方

- `warning`: IDE 上で警告として扱う
- `suggestion`: 提案レベル（警告より弱い）

## 6. 運用メモ

- コードクリーンアップ実行時に、この `.editorconfig` に沿って整形・命名チェックが行われます。
- 既存コードへ一括適用すると差分が大きくなりやすいため、機能追加と整形コミットは分ける運用を推奨します。
