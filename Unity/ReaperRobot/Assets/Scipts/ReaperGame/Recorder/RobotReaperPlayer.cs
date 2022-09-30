using System.Collections.Generic;
using UnityEngine;

namespace smart3tene.Reaper
{
    public class RobotReaperPlayer : BaseCSVPlayer
    { 
        #region Serialized Private Fields
        [SerializeField] private ReaperManager _reaperManager;
        [SerializeField] private Transform _reaperTransform;
        [SerializeField] private Material _pathMaterial;
        #endregion

        #region Private Fields
        private int _flameCount = 0;
        private List<GameObject> _pathObjects = new();
        #endregion

        #region MonoBehaviour Callbacks
        private void Update()
        {
            if (!_isPlaying) return;
            if (PlayTime > ExtractSeconds(_csvData, _csvData.Count - 1)) return;

            PlayTime += Time.deltaTime;

            OneFlameMove(_csvData, PlayTime);

            if (_flameCount % 10 == 0)
            {
                var obj = MeshCreator.CreateCubeMesh(_reaperTransform.position, _pathMaterial, 0.05f);
                _pathObjects.Add(obj);
            }
            _flameCount++;

            if (PlayTime > ExtractSeconds(_csvData, _csvData.Count - 1))
            {
                //再生が終わった処理
                Pause();
                EndCSVEvent?.Invoke();
            }
        }
        #endregion

        #region Public method
        public override void SetUp(string filePath)
        {
            _csvData.Clear();
            _csvData.AddRange(GetCSVData(filePath));

            if (_csvData.Count == 0)
            {
                return;
            }

            //プレイタイムの初期化
            PlayTime = 0;

            //フレームカウントの初期化
            _flameCount = 0;

            //リフト・カッターの初期設定
            _reaperManager.MoveLift(ExtractLift(_csvData, PlayTime));
            _reaperManager.RotateCutter(ExtractCutter(_csvData, PlayTime));
        }

        public override void Play()
        {
            _isPlaying = true;

            StartPlayEvent?.Invoke();
        }

        public override void Pause()
        {
            _isPlaying = false;
            _reaperManager.Move(0, 0);

            PausePlayEvent?.Invoke();
        }

        public override void Stop()
        {
            _isPlaying = false;
            _reaperManager.Move(0, 0);

            PlayTime = 0;
            _csvData.Clear();

            //pathの初期化
            foreach (var obj in _pathObjects)
            {
                Destroy(obj);
            }
            _pathObjects.Clear();
            _flameCount = 0;

            StopEvent?.Invoke();
        }

        public override void Back()
        {
            _isPlaying = false;

            PlayTime = 0;

            //初期位置の設定
            ReaperEventManager.InvokeResetEvent();

            //リフト・カッターの初期設定
            _reaperManager.MoveLift(ExtractLift(_csvData, PlayTime));
            _reaperManager.RotateCutter(ExtractCutter(_csvData, PlayTime));

            //pathの初期化
            foreach (var obj in _pathObjects)
            {
                Destroy(obj);
            }
            _pathObjects.Clear();
            _flameCount = 0;
        }
        #endregion

        #region Private method
        protected override void OneFlameMove(List<string[]> data, float seconds)
        {
            var input = ExtractInput(data, seconds);
            _reaperManager.Move(input.x, input.y);

            //リフトの上下
            _reaperManager.MoveLift(ExtractLift(data, seconds));

            //カッターの静動
            _reaperManager.RotateCutter(ExtractCutter(data, seconds));
        }
        #endregion
    }

}
