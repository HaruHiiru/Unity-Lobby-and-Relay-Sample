using GameFramework.Core.Data;
using TMPro;
using UnityEngine;

namespace GameLobby
{
    public class LobbyPlayer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _playerName;
        private string playerStatus = "Not Ready";

        public lobbyPlayerData _data;
        public void setData(lobbyPlayerData data)
        {
            _data = data;
            _playerName.text = _data.gamerTag + ": " + playerStatus;

            if(_data.isReady) 
            {
                if(playerStatus == "Not Ready")
                {
                    playerStatus = "Ready";
                }
            }
            gameObject.SetActive(true);
        }

        public void removeData()
        {
            if (_data == null)
            {
                return;
            }

            _playerName.text = "";
            if (_data.isReady) 
            {
                if (playerStatus == "Ready")
                {
                    playerStatus = "Not Ready";
                }

                _data.isReady = false;
            }
            _data = null;
            print("player removed");
            gameObject.SetActive(false);
        }
    }
}
