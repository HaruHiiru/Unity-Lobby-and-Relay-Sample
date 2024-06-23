using GameFramework.Core.Data;
using GameLobby.Events;
using System.Collections.Generic;
using Unity.Services.Authentication;
using UnityEngine;

namespace GameLobby
{
    public class LobbySpawner : MonoBehaviour
    {
        [SerializeField] public List<LobbyPlayer> _players;
        private LobbyPlayer _player;

        private void OnEnable()
        {
            LobbyEvents.onLobbyUpdated += OnLobbyUpdated;
        }

        private void OnDisable()
        {
            LobbyEvents.onLobbyUpdated -= OnLobbyUpdated;
        }


        private void OnLobbyUpdated()
        {
            
            List<lobbyPlayerData> playerDatas = GameLobbyManager.Instance.GetPlayers();

            for (int i = 0; i < playerDatas.Count; i++)
            {
                lobbyPlayerData data = playerDatas[i];
                _players[i].setData(data);
            }
        }

        public LobbyPlayer getPlayerById(string Id)
        {
            List<lobbyPlayerData> playerDatas = GameLobbyManager.Instance.GetPlayers();

            for (int i = 0; i < playerDatas.Count; i++)
            {
                lobbyPlayerData data = playerDatas[i];
                if (data.Id == AuthenticationService.Instance.PlayerId)
                {
                    return _players[i];
                }
            }
            
            return null;

        }

        public List<LobbyPlayer> getAllPlayers()
        {
            return _players;

        }
    }
}