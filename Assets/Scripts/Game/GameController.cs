using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Language { English, Spanish };

public class GameController : MonoBehaviour
{
    public bool Pause {  get; private set; }

    [SerializeField] public float GlobalVolume = 1;

    [SerializeField] public float MouseSensibilityMultiplier = 1;

    public Language SelectedLanguage;

    [SerializeField] private TMP_Dropdown _languageDropdownToggle;
    [SerializeField] private TMP_Dropdown _framelockDropdownToggle;
    [SerializeField] private Slider _mouseSensitivitySlider;
    [SerializeField] private Slider _globalVolumeSlider;
    [SerializeField] private Toggle _vsyncToggle;

    [SerializeField] private GameObject _menuScene;

    [SerializeField] private GameObject _newGameButton;
    [SerializeField] private GameObject _continueButton;

    [SerializeField] private AudioSource _menuMusicSource;
    [SerializeField] private float _menuMusicVolume;

    private bool _playMenuMusic = true;
    private GameObject _playerCamera;

    public static GameController Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else Instance = this;

        ChangeLanguage();
        FramelockChange();
        VSyncToggle();
        ChangeMouseSensitivity();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name == "Level_House")
        {
            _playerCamera = PlayerController.Instance.PlayerData.Camera.gameObject;
            Pause = !Pause;
            PauseControl();
        }

        MenuMusic();
    }

    private void MenuMusic()
    {
        if(_playMenuMusic)
        {
            if (!_menuMusicSource.isPlaying)
            {
                _menuMusicSource.volume = _menuMusicVolume * GlobalVolume;
                _menuMusicSource.Play();
            }
        }
    }

    private void PauseControl()
    {
        if (Pause)
        {
            _menuScene.SetActive(true);
            _playerCamera.SetActive(false);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0;
        }
        else
        {
            _playerCamera.SetActive(true);
            _menuScene.SetActive(false);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1;
        }
    }

    public void ContinueGame()
    {
        Pause = false;
        PauseControl();
    }

    public void ChangeLanguage()
    {
        if(_languageDropdownToggle.value == 0)
        {
            SelectedLanguage = Language.English;
        }
        else if (_languageDropdownToggle.value == 1)
        {
            SelectedLanguage = Language.Spanish;
        }

        if(UIManager.Instance != null)
        {
            UIManager.Instance.Subtitles.ParseText();
            UIManager.Instance.Language.SetLanguage(SelectedLanguage);
        }
    }

    public void FramelockChange()
    {
        int targetFrameRate = 0;

        if (_framelockDropdownToggle.value == 0)
        {
            targetFrameRate = 0;
        }
        else if(_framelockDropdownToggle.value == 1)
        {
            targetFrameRate = 30;
        }
        else if (_framelockDropdownToggle.value == 2)
        {
            targetFrameRate = 60;
        }
        else if (_framelockDropdownToggle.value == 3)
        {
            targetFrameRate = 144;
        }

        Application.targetFrameRate = targetFrameRate;
    }

    public void VSyncToggle()
    {
        if(_vsyncToggle.isOn)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }
    }

    public void ChangeMouseSensitivity()
    {
        MouseSensibilityMultiplier = _mouseSensitivitySlider.value;
    }

    public void ChangeGlobalVolume()
    {
        GlobalVolume = _globalVolumeSlider.value;
        _menuMusicSource.volume = _menuMusicVolume * GlobalVolume;
    }

    public void StartGame()
    {
        _newGameButton.SetActive(false);
        _continueButton.SetActive(true);
        _playMenuMusic = false;
        _menuMusicSource.Stop();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        UIManager.Instance.CanvasControl.OnSceneChanged(true);
        SceneManager.LoadScene("Level_House");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
