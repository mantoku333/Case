using System.ComponentModel;
using UnityEngine;

public partial class SROptions
{
    [Category("Camera")]
    [DisplayName("カメラ切り替え (Follow ⇔ DirectFollow)")]
    [Sort(0)]
    public void ToggleCameraPriority()
    {
        if (CameraManager.Instance != null)
        {
            CameraManager.Instance.ToggleCamera();
        }
        else
        {
            Debug.LogWarning("[SROptions] CameraManagerが見つかりません。シーンに存在するか確認してください。");
        }
    }

    [Category("Camera")]
    [DisplayName("現在のカメラ")]
    [Sort(1)]
    public string ActiveCameraName 
    {
        get 
        {
            if (CameraManager.Instance != null)
            {
                return CameraManager.Instance.GetActiveCameraName();
            }
            return "CameraManagerなし";
        }
    }

    // --- インパルス（シェイク）テスト ---
    [Category("Camera")]
    [DisplayName("画面揺れ (小)")]
    [Sort(2)]
    public void TestShakeSmall()
    {
        if (CameraManager.Instance != null) CameraManager.Instance.PlayShake(0.5f);
    }

    [Category("Camera")]
    [DisplayName("画面揺れ (中)")]
    [Sort(3)]
    public void TestShakeMedium()
    {
        if (CameraManager.Instance != null) CameraManager.Instance.PlayShake(1.0f);
    }

    [Category("Camera")]
    [DisplayName("画面揺れ (大)")]
    [Sort(4)]
    public void TestShakeLarge()
    {
        if (CameraManager.Instance != null) CameraManager.Instance.PlayShake(2.0f);
    }
}
