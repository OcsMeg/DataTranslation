using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardTest : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _textMeshProUGUI;

    private TouchScreenKeyboard _overlayKeyboard;

    private void Start()
    {
        _button.onClick.AddListener(OnClickButton);
    }

    private void Update()
    {
        if (_textMeshProUGUI.text == _overlayKeyboard.text) return;
        _textMeshProUGUI.text = _overlayKeyboard.text;
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }

    void OnClickButton()
    {
        _overlayKeyboard = TouchScreenKeyboard.Open(_textMeshProUGUI.text, TouchScreenKeyboardType.Default);
    }
}