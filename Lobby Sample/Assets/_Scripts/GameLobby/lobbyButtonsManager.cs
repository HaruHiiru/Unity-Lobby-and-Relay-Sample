using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameLobby
{
    public class lobbyButtonsManager : MonoBehaviour
    {
        [SerializeField] private Button joinLobbyButon;
        [SerializeField] private Button createLobbyButon;

        [SerializeField] private TMP_InputField lobbyInputField;
        [SerializeField] private TextMeshProUGUI lobbyCode;
        [SerializeField] private TextMeshProUGUI playerNameInputField;

        [SerializeField] private LobbyUI lobbyUIManager;
        void Start()
        {
            createLobbyButon.onClick.AddListener(onCreateLobbyClicked);
            joinLobbyButon.onClick.AddListener(onSubmitCodeClicked);
        }

        void OnDisabled()
        {
            createLobbyButon.onClick.RemoveListener(onCreateLobbyClicked);
            joinLobbyButon.onClick.RemoveListener(onSubmitCodeClicked);
        }

        private async void onCreateLobbyClicked()
        {
            print("Creating Lobby");
            bool succeded = await GameLobbyManager.Instance.createLobby(playerNameInputField.text);
            
            if (succeded) 
            {
                lobbyCode.text = "Lobby Code: " + GameLobbyManager.Instance.getLobbyCode();
                lobbyUIManager.removeInitialCanvas();
                lobbyUIManager.addLobbyCanvas();
            }
        }

        private async void onSubmitCodeClicked()
        {
            string code = lobbyInputField.text.ToUpper();
            print("Joining Lobby");
            bool succeded = await GameLobbyManager.Instance.joinlobby(code, playerNameInputField.text);
            
            if (succeded) 
            {
                lobbyCode.text = "Lobby Code: " + code;
                lobbyUIManager.removeInitialCanvas();
                lobbyUIManager.addLobbyCanvas();
            }
        }
    }

}
