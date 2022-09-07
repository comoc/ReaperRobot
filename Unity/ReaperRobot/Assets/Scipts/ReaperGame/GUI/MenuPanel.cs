using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace smart3tene.Reaper
{
    public class MenuPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _menuPanel;

        private void Awake()
        {
            _menuPanel.SetActive(false);

            ReaperEventManager.MenuEvent += ShowAndHideMenu;
        }

        private void OnDestroy()
        {
            ReaperEventManager.MenuEvent -= ShowAndHideMenu;
        }

        private void ShowAndHideMenu()
        {
            _menuPanel.SetActive(!_menuPanel.activeSelf);
        }

        public void EndGameButtonClick()
        {
            if(SceneTransitionManager.Instance != null)
            {
                SceneTransitionManager.Instance.EndGame();
            }
            else
            {
                Debug.LogWarning("SceneTransitionManager がありません。");
            }
        }
    }

}
