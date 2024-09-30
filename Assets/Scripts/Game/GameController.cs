using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Language { English, Spanish };

public class GameController : MonoBehaviour
{
    public bool Pause { get; private set; }

    [NonSerialized] public bool ShowCursor;
    [NonSerialized] public bool IsInDream;
    [SerializeField] private Texture2D _cursor;

    [SerializeField] public float GlobalVolume = 1;

    [SerializeField] public float MouseSensibilityMultiplier = 1;

    [field: SerializeField] public AudioSource GeneralAudioSource { get; private set; }

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
    [SerializeField] private AudioSource _windAudioSource; 
    [SerializeField] private float _windVolume = 0.1f;
    [SerializeField] private AudioSource _ambienceAudioSource;
    [SerializeField] private float _ambienceVolume = 0.3f;

    private bool _playMenuMusic = true;
    private GameObject _playerCamera;

    public static GameController Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else Instance = this;

        //ChangeLanguage();
        //FramelockChange();
        //VSyncToggle();
        //ChangeMouseSensitivity();


    }

    private void Start()
    {
        Cursor.SetCursor(_cursor, new Vector2(_cursor.width / 2, _cursor.height / 2), CursorMode.Auto);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (ShowCursor)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        

        /*if(Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name == "Level_House")
        {
            _playerCamera = PlayerController.Instance.Camera.gameObject;
            Pause = !Pause;
            PauseControl();
        }

        MenuMusic();*/
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

    public void ActivateAmbienceSounds(bool activate)
    {
        if (activate)
        {
            _windAudioSource.volume = Mathf.MoveTowards(_windAudioSource.volume, _windVolume, 0.05f);
            _ambienceAudioSource.volume = Mathf.MoveTowards(_ambienceAudioSource.volume, _ambienceVolume, 0.05f);
        }
        else
        {
            _windAudioSource.volume = Mathf.MoveTowards(_windAudioSource.volume, 0, 0.05f);
            _ambienceAudioSource.volume = Mathf.MoveTowards(_ambienceAudioSource.volume, 0, 0.05f);
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
