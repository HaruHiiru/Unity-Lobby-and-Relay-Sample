using GameFramework.Core;
using GameFramework.Core.Data;
using GameFramework.Core.Events;
using GameFramework.Core.GameFramework.Manager;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameLobby
{

    public class GameLobbyManager : Singleton<GameLobbyManager>
    {
        [SerializeField] private LobbySpawner lobbySpawner;
        [SerializeField] private int _maxPlayers = 50;

        private bool _inGame = false;
        private List<lobbyPlayerData> _lobbyPlayerData = new List<lobbyPlayerData>();
        private lobbyPlayerData _LocalPlayerData;
        private LobbyPlayer _localPlayer;
        private LobbyData _lobbyData;

        public bool isHost => _LocalPlayerData.Id == LobbyManager.Instance.GetHostId();

        private void OnEnable()
        {
            LobbyEvents.onLobbyUpdated += onLobbyUpdated;
        }
        private void OnDisable()
        {
            LobbyEvents.onLobbyUpdated -= onLobbyUpdated;
        }

        public async Task<bool> createLobby(string playerName)
        {
            _LocalPlayerData = new lobbyPlayerData();
            _LocalPlayerData.Initialize(AuthenticationService.Instance.PlayerId, playerName);
            _lobbyData = new LobbyData();
            _lobbyData.Initialze("Casual Game");
            bool succeded = await LobbyManager.Instance.createLobby(_maxPlayers, true, _LocalPlayerData.Serialize(), _lobbyData.Serialize());
            return succeded;
        }

        public async Task<bool> joinlobby(string code, string playerName)
        {
            _LocalPlayerData = new lobbyPlayerData();
            _LocalPlayerData.Initialize(AuthenticationService.Instance.PlayerId, playerName);
            bool succeded = await LobbyManager.Instance.joinLobby(code, _LocalPlayerData.Serialize());
            return succeded;
        }

        public string getLobbyCode()
        {
            return LobbyManager.Instance.getLobbyCode();
        }

        private async void onLobbyUpdated(Lobby lobby)
        {
            List<Dictionary<string, PlayerDataObject>> playerData = LobbyManager.Instance.GetPlayerData();
            _lobbyPlayerData.Clear();

            int readyPlayers = 0;

            foreach (Dictionary<string, PlayerDataObject> data in playerData)
            {
                lobbyPlayerData lobbyPlayerData = new lobbyPlayerData();
                lobbyPlayerData.Initialize(data);

                if(lobbyPlayerData.isReady)
                {
                    readyPlayers++;
                }

                if (lobbyPlayerData.Id == AuthenticationService.Instance.PlayerId)
                {
                    _LocalPlayerData = lobbyPlayerData;
                }

                _lobbyPlayerData.Add(lobbyPlayerData);
            }

            _lobbyData = new LobbyData();
            _lobbyData.Initialze(lobby.Data);

            Events.LobbyEvents.onLobbyUpdated?.Invoke();

            if(readyPlayers == lobby.Players.Count)
            {
                Events.LobbyEvents.onLobbyReady?.Invoke();
            }

            if(_lobbyData.RelayJoinCode != default && !_inGame) 
            {
                await JoinRelayServer(_lobbyData.RelayJoinCode);
                SceneManager.LoadSceneAsync(_lobbyData.SceneName);
            }
        }


        public List<lobbyPlayerData> GetPlayers()
        {
            return _lobbyPlayerData;
        }

        public async Task<bool> SetPlayerReady()
        {
            _LocalPlayerData.isReady = true;
            return await LobbyManager.Instance.UpdatePlayerData(_LocalPlayerData.Id, _LocalPlayerData.Serialize());
        }
        public async Task<bool> SetPlayerNotReady()
        {
            _LocalPlayerData.isReady = false;
            return await LobbyManager.Instance.UpdatePlayerData(_LocalPlayerData.Id, _LocalPlayerData.Serialize());
        }

        public string GetGameMode()
        {
            return _lobbyData.GameMode;
        }

        public async Task<bool> SetGameMode(string gameMode, string sceneName)
        {
            _lobbyData.GameMode = gameMode;
            _lobbyData.SceneName = sceneName;
            return await LobbyManager.Instance.UpdateLobbyData(_lobbyData.Serialize());
        }


        public void OnDestroy()
        {
            try
            {
                StopAllCoroutines();
                if(LobbyManager.Instance.getLobby() != null) 
                {
                    if (LobbyManager.Instance.GetHostId() == _LocalPlayerData.Id) Lobbies.Instance.DeleteLobbyAsync(LobbyManager.Instance.getLobby().Id);
                    else
                    {
                        Lobbies.Instance.RemovePlayerAsync(LobbyManager.Instance.getLobby().Id, _LocalPlayerData.Id);
                        _lobbyPlayerData.Remove(_LocalPlayerData);
                    }
                }
            }
            catch(System.Exception ex)
            {
                print(ex);
            }
        }

        public async Task StartGame()
        {
            string relayJoinCode = await RelayManager.Instance.CreateRelay(_maxPlayers);
            _inGame = true;

            _lobbyData.RelayJoinCode = relayJoinCode;
            await LobbyManager.Instance.UpdateLobbyData(_lobbyData.Serialize());

            string allocationId = RelayManager.Instance.GetAllocationId();
            string connectionData = RelayManager.Instance.GetConnectionData();

            await LobbyManager.Instance.UpdatePlayerData(_LocalPlayerData.Id, _LocalPlayerData.Serialize(), allocationId, connectionData);

            SceneManager.LoadSceneAsync(_lobbyData.SceneName);
        }

        private async Task<bool> JoinRelayServer(string RelayJoinCode)
        {
            _inGame = true;
            await RelayManager.Instance.JoinRelay(RelayJoinCode);

            string allocationId = RelayManager.Instance.GetAllocationId();
            string connectionData = RelayManager.Instance.GetConnectionData();

            await LobbyManager.Instance.UpdatePlayerData(_LocalPlayerData.Id, _LocalPlayerData.Serialize(), allocationId, connectionData);

            return true;
        }
    }
}

