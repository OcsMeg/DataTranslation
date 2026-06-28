using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleImageTint : MonoBehaviour
{
    [SerializeField] private Graphic targetGraphic;   // ここに RawImage をドラッグ
    [SerializeField] private Color onColor  = Color.white;
    [SerializeField] private Color offColor = new Color(1f, 1f, 1f, 0.4f); // 少し薄く

    private Toggle toggle;

    void Awake()
    {
        toggle = GetComponent<Toggle>();

        // 未設定なら自分自身の Graphic を使う
        if (targetGraphic == null)
            targetGraphic = GetComponent<Graphic>();

        // 初期状態を反映
        ApplyColor(toggle.isOn);

        // isOn が変わったときに呼ばれる
        toggle.onValueChanged.AddListener(ApplyColor);
    }

    private void ApplyColor(bool isOn)
    {
        if (targetGraphic == null) return;
        targetGraphic.color = isOn ? onColor : offColor;
    }
}