# Scripts Folder

このフォルダには、ゲームのすべてのC#スクリプトが格納されます。

## フォルダ構成

### Player/
プレイヤーキャラクターに関連するすべてのスクリプト
- 移動制御
- 入力処理
- アニメーション制御
- プレイヤーステータス
- アビリティシステム

### Enemy/
敵キャラクターのAIと動作
- 敵AI（巡回、追跡、攻撃等）
- 敵のステータス管理
- 各敵タイプの固有動作

### Environment/
環境オブジェクト・ギミック
- ドア、スイッチ
- 動くプラットフォーム
- トラップ
- インタラクティブオブジェクト

### Items/
アイテムと収集品
- アイテムの取得処理
- アイテム効果
- インベントリシステム

### UI/
ユーザーインターフェース
- メニュー
- HUD（体力、スタミナ表示等）
- ダイアログシステム
- マップUI

### Managers/
ゲーム全体を管理するシングルトン等
- GameManager
- SaveManager
- SceneManager
- AudioManager
- InputManager

### Utilities/
汎用ヘルパークラス
- 拡張メソッド
- カスタムエディタツール
- デバッグツール
- 数学ユーティリティ

### Combat/
戦闘システム
- ダメージ計算
- ヒットボックス/ハートボックス
- 武器システム
- コンボシステム

### Movement/
移動・物理関連
- ジャンプ、ダッシュ等の移動能力
- 壁張り付き、壁ジャンプ
- 物理挙動

### Audio/
オーディオシステム
- サウンド再生管理
- BGM管理
- 効果音管理

## コーディング規約

- 1クラス1ファイルの原則
- ファイル名はクラス名と一致させる
- 名前空間を使用: `namespace GameName.Player`, `namespace GameName.Enemy` 等
- コメントは日本語でも英語でもOK
- 公開メソッド・プロパティにはXMLコメントを推奨

## 例

```csharp
using UnityEngine;

namespace Metroidvania.Player
{
    /// <summary>
    /// プレイヤーの移動を管理するクラス
    /// </summary>
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float jumpForce = 10f;
        
        // Implementation...
    }
}
```
