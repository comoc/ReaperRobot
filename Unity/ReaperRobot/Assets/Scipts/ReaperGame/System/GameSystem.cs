﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Photon.Pun;

namespace smart3tene.Reaper
{
    [DefaultExecutionOrder(-2)]
    public class GameSystem : MonoBehaviour
    {
        #region Event
        public event Action ResetEvent;
        #endregion

        #region public Fields
        public static GameSystem Instance = null;
        #endregion

        #region Enum
        public enum ViewMode
        {
            REAPER,
            TPV,
            FPV,
            VR,
        }
        public ReactiveProperty<ViewMode> NowViewMode { get; private set; } = new ReactiveProperty<ViewMode>(ViewMode.REAPER);        
        #endregion

        #region Serialized private Fields
        public GameObject ReaperInstance => _reaperInstance;
        [SerializeField, Tooltip("マルチプレイの時はnullにしておいてください")] private GameObject _reaperInstance = null;

        public GameObject PersonInstance => _personInstance;
        [SerializeField, Tooltip("マルチプレイの時はnullにしておいてください")] private GameObject _personInstance = null;

        [SerializeField] private ViewMode _defaultOperationMode = ViewMode.REAPER;

        [SerializeField] private List<Transform> _instantiatePos = new List<Transform>();
        #endregion

        #region private Fields
        public int AllGrassCount => _allGrassCount;
        private int _allGrassCount;

        public IReadOnlyReactiveProperty<int> CutGrassCount => _cutGrassCount;
        private ReactiveProperty<int> _cutGrassCount = new(0);

        public IReadOnlyReactiveProperty<float> GameTime => _gameTime;
        private ReactiveProperty<float> _gameTime = new(0);        

        private float _gameStartTime;
        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            NowViewMode.Value = _defaultOperationMode;

            if (!PhotonNetwork.IsConnected)
            {
                //オフラインとして参加する
                PhotonNetwork.OfflineMode = true;
                PhotonNetwork.JoinRandomRoom();
            }

            var posId = GameData.PlayerId - 1; 
            //草刈り機の生成
            if (_reaperInstance == null)
            {
                _reaperInstance = PhotonNetwork.Instantiate("ReaperCrawlerResource", _instantiatePos[posId].position, _instantiatePos[posId].rotation, 0);
            }

            //人モデルの生成
            //VRモードの時は人出さなくていい？
            if(NowViewMode.Value != ViewMode.VR &&　_personInstance == null)
            {
                var playerBackDistance = 3f;
                _personInstance = PhotonNetwork.Instantiate("PersonModel", _instantiatePos[posId].position + (-1 * _instantiatePos[posId].forward * playerBackDistance), _instantiatePos[posId].rotation, 0);
            }

            if(NowViewMode.Value == ViewMode.VR)
            {
                var manualXRControl = new ManualXRControl();
                StartCoroutine(manualXRControl.StartXRCoroutine());
            }

            //草の総数をカウント
            _allGrassCount = GameObject.FindGameObjectsWithTag("Grass").Length;

            //ゲーム時間の測定
            _gameStartTime = Time.time;
            this.UpdateAsObservable()
                .Subscribe(_ => _gameTime.Value = Time.time - _gameStartTime)
                .AddTo(this);           
        }

        private void OnDisable()
        {
            if (NowViewMode.Value == ViewMode.VR)
            {
                var manualXRControl = new ManualXRControl();
                manualXRControl.StopXR();
            }
        }
        #endregion

        #region Public Method
        public void AddCutGrassCount(int count)
        {
            _cutGrassCount.Value += count;
        }

        public void ResetGrasses()
        {
            ResetEvent?.Invoke();

            //時間のリセットはいるだろうか

            //スコアとかつけてるならそれもリセットするか？
        }

        public void ChangeViewMode()
        {
            switch (NowViewMode.Value)
            {
                case ViewMode.REAPER:
                    NowViewMode.Value = ViewMode.FPV;
                    break;
                case ViewMode.FPV:
                    NowViewMode.Value = ViewMode.TPV;
                    break;
                case ViewMode.TPV:
                    NowViewMode.Value = ViewMode.REAPER;
                    break;
                case ViewMode.VR:
                    //VRモードの時はモードを変えない
                    break;
                default:
                    break;
            }
        }
        #endregion

    }
}
