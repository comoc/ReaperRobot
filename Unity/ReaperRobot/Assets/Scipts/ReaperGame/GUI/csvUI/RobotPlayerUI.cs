using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace smart3tene.Reaper
{
    public class RobotPlayerUI : MonoBehaviour
    {
        #region Public Fields
        public bool ControllableRobot { get; private set; } = true;
        #endregion

        #region Serialized Private Fields
        [Header("CSV Player")]
        [SerializeField] private RobotReaperPlayer _robotReaperPlayer;

        [Header("UIs")]
        [SerializeField] private GameObject _RobotPlayerPanel;
        [SerializeField] private TMP_Text _fileNameText;
        [SerializeField] private TMP_Text _timeText;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Button _stopButton;
        #endregion

        #region Readonly Field
        readonly string defaultFileDirectory = Path.GetFullPath(Application.streamingAssetsPath + "/../../../InputLog");
        readonly string defaultFileNameText = "select csv file";
        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
            _RobotPlayerPanel.SetActive(false);
            _fileNameText.text = defaultFileNameText;

            _backButton.interactable = false;
            _playButton.interactable = false;
            _pauseButton.interactable = false;
            _stopButton.interactable = false;

            _robotReaperPlayer.EndCSVEvent += OnEndCSVEvent;
        }

        void Update()
        {
            _timeText.text = GameTimer.ConvertSecondsToString(_robotReaperPlayer.PlayTime, false);
        }

        private void OnDestroy()
        {
            _robotReaperPlayer.EndCSVEvent -= OnEndCSVEvent;
        }
        #endregion

        #region Public method
        public void SelectCSVFile()
        {
            if (_robotReaperPlayer.IsPlaying) _robotReaperPlayer.Pause();

            var path = OpenDialogUtility.OpenCSVFile("select csv file", defaultFileDirectory);

            if (path != "")
            {
                //ボタンの設定
                _backButton.interactable = false;
                _playButton.interactable = true;
                _pauseButton.interactable = false;
                _stopButton.interactable = true;

                //csvDataの取得
                _robotReaperPlayer.SetUp(path);

                //初期位置の設定
                ReaperEventManager.InvokeResetEvent();

                //FileNameTextの設定
                _fileNameText.text = Path.GetFileName(path);
            }
            else
            {
                Debug.LogWarning("パスが指定されませんでした");
            }
        }

        public void OnClickPlay()
        {
            //ボタンの設定
            _backButton.interactable = false;
            _playButton.interactable = false;
            _pauseButton.interactable = true;
            _stopButton.interactable = true;

            //プレイヤーの設定
            _robotReaperPlayer.Play();
        }

        public void OnClickBack()
        {
            //ボタンの設定
            _backButton.interactable = false;
            _playButton.interactable = true;
            _pauseButton.interactable = false;
            _stopButton.interactable = true;

            //初期位置の設定
            ReaperEventManager.InvokeResetEvent();

            //プレイヤーの設定
            _robotReaperPlayer.Back();
        }

        public void OnClickPause()
        {
            //ボタンの設定
            _backButton.interactable = true;
            _playButton.interactable = true;
            _pauseButton.interactable = false;
            _stopButton.interactable = true;

            //プレイヤーの設定
            _robotReaperPlayer.Pause();
        }

        public void OnClickStop()
        {
            //ボタンの設定
            _backButton.interactable = false;
            _playButton.interactable = false;
            _pauseButton.interactable = false;
            _stopButton.interactable = false;

            //プレイヤーの設定
            _robotReaperPlayer.Stop();

            //パネルの初期化
            _RobotPlayerPanel.SetActive(false);
            _fileNameText.text = defaultFileNameText;


            //コントローラの使用の許可
            ControllableRobot = true;
        }

        public void OnClickSelectRobotMode()
        {
            if (_RobotPlayerPanel.activeSelf)
            {
                OnClickStop();
            }
            else
            {
                _RobotPlayerPanel.SetActive(true);

                //コントローラの使用の禁止
                ControllableRobot = false;
            }
        }
        #endregion

        #region Private method
        private void OnEndCSVEvent()
        {
            _backButton.interactable = true;
            _playButton.interactable = false;
            _pauseButton.interactable = false;
            _stopButton.interactable = true;
        }
        #endregion
    }

}