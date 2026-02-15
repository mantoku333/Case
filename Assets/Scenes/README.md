# Scenes Folder

シーンファイルを管理するフォルダです。

## フォルダ構成

### MainScenes/
ゲームのメインシーン
- `TitleScreen.unity` - タイトル画面
- `MainMenu.unity` - メインメニュー
- `GameCore.unity` - ゲーム本編のコアシステム
- `UI.unity` - 常駐UIシーン（Additive Loadingで使用）

### AreaScenes/
ゲーム内の各エリア・マップ
- `Area01_Forest.unity` - 森エリア
- `Area02_Cave.unity` - 洞窟エリア
- `Area03_Castle.unity` - 城エリア
- など、エリアごとに個別のシーンを作成

### TestScenes/
開発・テスト用シーン
- `PlayerTest.unity` - プレイヤー機能テスト
- `EnemyTest_Slime.unity` - スライム敵のテスト
- `UITest.unity` - UI機能テスト
- 各開発者が自由に作成・編集可能

## Additive Scene Loading の活用

複数人で同じシーンを編集するとコンフリクトが起きやすいため、以下のような構成を推奨:

```
GameCore (Persistent Scene)
  ├─ UI (Additive)
  ├─ Audio (Additive)
  └─ Area01_Forest (Additive)
```

### 例: シーンのロード

```csharp
using UnityEngine.SceneManagement;

// UIシーンを追加ロード
SceneManager.LoadScene("UI", LoadSceneMode.Additive);

// エリアシーンをロード
SceneManager.LoadScene("Area01_Forest", LoadSceneMode.Additive);
```

## コンフリクト回避のルール

1. **個人専用のテストシーンを使用**
   - 他の人のテストシーンは編集しない
   - 自分専用のテストシーンを作成

2. **MainScenesは最小限の編集**
   - 大きな変更の前にチームに共有
   - できるだけManagerスクリプトで制御

3. **AreaScenesはエリア担当者が編集**
   - 1エリア1担当の原則
   - 複数人で編集する場合は事前調整

4. **Prefabを活用**
   - シーン内で直接編集せず、Prefabを編集
   - シーンには配置のみを行う

## Scene Template の活用

Unity 6では Scene Template を活用して、統一されたシーン構成を作成できます。

`Settings/Scenes/` にテンプレートを配置しています。
