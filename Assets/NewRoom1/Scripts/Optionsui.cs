using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class OptionsUI : MonoBehaviour
{
    [Header("Volume Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Keybind Buttons")]
    public Button moveForwardBtn;
    public Button moveBackBtn;
    public Button moveLeftBtn;
    public Button moveRightBtn;
    public Button sprintBtn;
    public Button jumpBtn;
    public Button interactBtn;
    public Button reloadBtn;
    public Button inventoryBtn;
    public Button pauseBtn;
    public Button inspectBtn;
    public Button rotateLeftBtn;
    public Button rotateRightBtn;

    [Header("Keybind Labels")]
    public TextMeshProUGUI moveForwardLabel;
    public TextMeshProUGUI moveBackLabel;
    public TextMeshProUGUI moveLeftLabel;
    public TextMeshProUGUI moveRightLabel;
    public TextMeshProUGUI sprintLabel;
    public TextMeshProUGUI jumpLabel;
    public TextMeshProUGUI interactLabel;
    public TextMeshProUGUI reloadLabel;
    public TextMeshProUGUI inventoryLabel;
    public TextMeshProUGUI pauseLabel;
    public TextMeshProUGUI inspectLabel;
    public TextMeshProUGUI rotateLeftLabel;
    public TextMeshProUGUI rotateRightLabel;

    [Header("Press To Rebind Overlay")]
    public GameObject pressToRebindPanel;
    public TextMeshProUGUI pressToRebindText;

    [Header("Bottom Buttons")]
    public Button resetButton;
    public Button backButton;

    [Header("References")]
    public PauseMenu pauseMenu;

    private int waitingIndex = -1;
    private bool started = false;
    private bool mouseWasDown = false;
    private Button[] allKeybindButtons;

    private KeyCode[] allKeys = new KeyCode[] {
        KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F,
        KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L,
        KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R,
        KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X,
        KeyCode.Y, KeyCode.Z,
        KeyCode.Space, KeyCode.LeftShift, KeyCode.RightShift,
        KeyCode.LeftControl, KeyCode.RightControl,
        KeyCode.Tab, KeyCode.Backspace, KeyCode.Escape,
        KeyCode.Alpha0, KeyCode.Alpha1, KeyCode.Alpha2,
        KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5,
        KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9,
        KeyCode.F1, KeyCode.F2, KeyCode.F3, KeyCode.F4, KeyCode.F5,
        KeyCode.F6, KeyCode.F7, KeyCode.F8, KeyCode.F9, KeyCode.F10,
        KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow
    };

    void OnEnable()
    {
        if (!started) return;
        if (OptionsManager.Instance == null) return;
        masterSlider.value = OptionsManager.Instance.masterVolume;
        musicSlider.value = OptionsManager.Instance.musicVolume;
        sfxSlider.value = OptionsManager.Instance.sfxVolume;
        RefreshLabels();
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
    }

    void Start()
    {
        started = true;

        allKeybindButtons = new Button[] {
            moveForwardBtn, moveBackBtn, moveLeftBtn, moveRightBtn,
            sprintBtn, jumpBtn, interactBtn, reloadBtn,
            inventoryBtn, pauseBtn, inspectBtn, rotateLeftBtn, rotateRightBtn
        };

        masterSlider.onValueChanged.AddListener(v => { OptionsManager.Instance.masterVolume = v; OptionsManager.Instance.ApplyVolume(); OptionsManager.Instance.SaveSettings(); });
        musicSlider.onValueChanged.AddListener(v => { OptionsManager.Instance.musicVolume = v; OptionsManager.Instance.ApplyVolume(); OptionsManager.Instance.SaveSettings(); });
        sfxSlider.onValueChanged.AddListener(v => { OptionsManager.Instance.sfxVolume = v; OptionsManager.Instance.ApplyVolume(); OptionsManager.Instance.SaveSettings(); });

        moveForwardBtn.onClick.AddListener(() => StartRebind(0));
        moveBackBtn.onClick.AddListener(() => StartRebind(1));
        moveLeftBtn.onClick.AddListener(() => StartRebind(2));
        moveRightBtn.onClick.AddListener(() => StartRebind(3));
        sprintBtn.onClick.AddListener(() => StartRebind(4));
        jumpBtn.onClick.AddListener(() => StartRebind(5));
        interactBtn.onClick.AddListener(() => StartRebind(6));
        reloadBtn.onClick.AddListener(() => StartRebind(7));
        inventoryBtn.onClick.AddListener(() => StartRebind(8));
        pauseBtn.onClick.AddListener(() => StartRebind(9));
        inspectBtn.onClick.AddListener(() => StartRebind(10));
        if (rotateLeftBtn) rotateLeftBtn.onClick.AddListener(() => StartRebind(11));
        if (rotateRightBtn) rotateRightBtn.onClick.AddListener(() => StartRebind(12));

        resetButton.onClick.AddListener(OnReset);
        backButton.onClick.AddListener(OnBack);

        if (pressToRebindPanel != null) pressToRebindPanel.SetActive(false);

        if (OptionsManager.Instance != null)
        {
            masterSlider.value = OptionsManager.Instance.masterVolume;
            musicSlider.value = OptionsManager.Instance.musicVolume;
            sfxSlider.value = OptionsManager.Instance.sfxVolume;
            RefreshLabels();
        }

        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
    }

    void StartRebind(int index)
    {
        if (waitingIndex >= 0) return;
        waitingIndex = index;
        mouseWasDown = true;
        SetAllButtonsInteractable(false);
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
        if (pressToRebindPanel != null) pressToRebindPanel.SetActive(true);
        if (pressToRebindText != null) pressToRebindText.text = "Press any key...";
    }

    void SetAllButtonsInteractable(bool interactable)
    {
        foreach (var btn in allKeybindButtons)
            if (btn != null) btn.interactable = interactable;
        if (resetButton != null) resetButton.interactable = interactable;
        if (backButton != null) backButton.interactable = interactable;
    }

    void Update()
    {
        if (waitingIndex < 0) return;

        if (Input.GetMouseButton(0) || Input.GetMouseButton(1)) { mouseWasDown = true; return; }
        if (mouseWasDown) { if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1)) mouseWasDown = false; return; }

        foreach (KeyCode key in allKeys)
        {
            if (Input.GetKeyDown(key))
            {
                if (IsKeyAlreadyUsed(key, waitingIndex))
                {
                    if (pressToRebindText != null) pressToRebindText.text = key.ToString() + " is already in use!\nPress another key...";
                    return;
                }
                ApplyRebind(waitingIndex, key);
                RefreshLabels();
                OptionsManager.Instance.SaveSettings();
                waitingIndex = -1;
                mouseWasDown = false;
                SetAllButtonsInteractable(true);
                if (pressToRebindPanel != null) pressToRebindPanel.SetActive(false);
                if (pressToRebindText != null) pressToRebindText.text = "Press any key...";
                if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
                return;
            }
        }
    }

    bool IsKeyAlreadyUsed(KeyCode key, int excludeIndex)
    {
        KeyCode[] current = {
            OptionsManager.MoveForward, OptionsManager.MoveBack,
            OptionsManager.MoveLeft,    OptionsManager.MoveRight,
            OptionsManager.Sprint,      OptionsManager.Jump,
            OptionsManager.Interact,    OptionsManager.Reload,
            OptionsManager.Inventory,   OptionsManager.Pause,
            OptionsManager.Inspect,     OptionsManager.RotateLeft,
            OptionsManager.RotateRight
        };
        for (int i = 0; i < current.Length; i++)
        {
            if (i == excludeIndex) continue;
            if (current[i] == key) return true;
        }
        return false;
    }

    void ApplyRebind(int index, KeyCode key)
    {
        switch (index)
        {
            case 0: OptionsManager.MoveForward = key; break;
            case 1: OptionsManager.MoveBack = key; break;
            case 2: OptionsManager.MoveLeft = key; break;
            case 3: OptionsManager.MoveRight = key; break;
            case 4: OptionsManager.Sprint = key; break;
            case 5: OptionsManager.Jump = key; break;
            case 6: OptionsManager.Interact = key; break;
            case 7: OptionsManager.Reload = key; break;
            case 8: OptionsManager.Inventory = key; break;
            case 9: OptionsManager.Pause = key; break;
            case 10: OptionsManager.Inspect = key; break;
            case 11: OptionsManager.RotateLeft = key; break;
            case 12: OptionsManager.RotateRight = key; break;
        }
    }

    void RefreshLabels()
    {
        if (moveForwardLabel != null) moveForwardLabel.text = OptionsManager.MoveForward.ToString();
        if (moveBackLabel != null) moveBackLabel.text = OptionsManager.MoveBack.ToString();
        if (moveLeftLabel != null) moveLeftLabel.text = OptionsManager.MoveLeft.ToString();
        if (moveRightLabel != null) moveRightLabel.text = OptionsManager.MoveRight.ToString();
        if (sprintLabel != null) sprintLabel.text = OptionsManager.Sprint.ToString();
        if (jumpLabel != null) jumpLabel.text = OptionsManager.Jump.ToString();
        if (interactLabel != null) interactLabel.text = OptionsManager.Interact.ToString();
        if (reloadLabel != null) reloadLabel.text = OptionsManager.Reload.ToString();
        if (inventoryLabel != null) inventoryLabel.text = OptionsManager.Inventory.ToString();
        if (pauseLabel != null) pauseLabel.text = OptionsManager.Pause.ToString();
        if (inspectLabel != null) inspectLabel.text = OptionsManager.Inspect.ToString();
        if (rotateLeftLabel != null) rotateLeftLabel.text = OptionsManager.RotateLeft.ToString();
        if (rotateRightLabel != null) rotateRightLabel.text = OptionsManager.RotateRight.ToString();
    }

    void OnReset()
    {
        OptionsManager.Instance.ResetToDefaults();
        masterSlider.value = 1f;
        musicSlider.value = 1f;
        sfxSlider.value = 1f;
        RefreshLabels();
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
    }

    void OnBack()
    {
        gameObject.SetActive(false);
        if (pauseMenu != null) pauseMenu.CloseOptions();
    }
}