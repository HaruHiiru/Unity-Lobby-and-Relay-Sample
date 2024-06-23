using UnityEngine;
using TMPro;
using UnityEngine.UI;
using GameFramework.Core.Data;
using GameLobby.Events;


namespace GameLobby
{
    public class LobbyUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI lobbyCode;
        [SerializeField] GameObject initialCanvas;
        [SerializeField] GameObject lobbyCanvas;
        [SerializeField] Button _readyButton;
        [SerializeField] Button _startButton;
        [SerializeField] TMP_Dropdown mode;
        [SerializeField] TextMeshProUGUI gameModeText;
        [SerializeField] ModeSelectionData mapSelectionData;

        string gameMode;

        private void OnEnable()
        {
            _readyButton.onClick.AddListener(onReadyPressed);
            _startButton.onClick.AddListener(onStartButtonClicked);

            LobbyEvents.onLobbyUpdated += OnLobbyUpdated;
        }

       

        private void OnDisable()
        {
            _readyButton.onClick.RemoveAllListeners();
            _startButton.onClick.RemoveAllListeners();

            LobbyEvents.onLobbyUpdated -= OnLobbyUpdated;
            LobbyEvents.onLobbyReady -= onLobbyReady;
        }

        private void Start()
        {
            mode.onValueChanged.AddListener(delegate {
                modeSelected(mode);
            });
        }

        private async void modeSelected(TMP_Dropdown mode)
        {
            updateLobby(mode.options[mode.value].text);
            await GameLobbyManager.Instance.SetGameMode(mode.options[mode.value].text, mapSelectionData.Maps[mode.value].sceneName);
        }
        private async void onReadyPressed()
        {
            bool succeed = await GameLobbyManager.Instance.SetPlayerReady();
            
            if (succeed)
            {
                _readyButton.gameObject.SetActive(false);
            }
        }

        private async void onStartButtonClicked()
        {
            await GameLobbyManager.Instance.StartGame();
        }

        private void onLobbyReady()
        {
            _startButton.gameObject.SetActive(true);
        }

        void displayLobbyCode()
        {
            lobbyCode.text = "Lobby Code:" + GameLobbyManager.Instance.getLobbyCode();
        }

        public void removeInitialCanvas()
        {
            initialCanvas.SetActive(false);
        }
        
        public void removeLobbyCanvas()
        {
            lobbyCanvas.SetActive(false);
        }

        public void addInitialCanvas()
        {
            initialCanvas.SetActive(true);
        }
        
        public async void addLobbyCanvas()
        {
            if (LobbyManager.Instance.getLobby() != null && GameLobbyManager.Instance.isHost == false)
            {
                mode.gameObject.SetActive(false);
            }
            else
            {
                mode.gameObject.SetActive(true);
                await GameLobbyManager.Instance.SetGameMode(mode.options[mode.value].text, mapSelectionData.Maps[mode.value].sceneName);
                LobbyEvents.onLobbyReady += onLobbyReady;
            }
            lobbyCanvas.SetActive(true);
        }

        private void updateLobby(string currentGameMode)
        {
            gameModeText.text = "Game Mode: " + currentGameMode;
        }
        private void OnLobbyUpdated()
        {
            gameMode = GameLobbyManager.Instance.GetGameMode();
            updateLobby(gameMode);
        }
    }

}
