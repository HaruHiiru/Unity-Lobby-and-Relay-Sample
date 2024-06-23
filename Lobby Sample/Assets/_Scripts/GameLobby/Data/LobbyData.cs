using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace GameFramework.Core.Data
{
    public class LobbyData
    {
        private string _gameMode;
        private string _relayJoinCode;
        private string _sceneName;

        public string GameMode
        {
            get => _gameMode ;
            set => _gameMode = value;
        }

        public string RelayJoinCode
        {
            get => _relayJoinCode;
            set => _relayJoinCode = value;

        }

        public string SceneName
        {
            get => _sceneName;
            set => _sceneName = value;
        }

        public void Initialze(string gameMode)
        {
            _gameMode = gameMode;
    }

        public void Initialze(Dictionary<string, DataObject> lobbyData)
        {
            UpdateState(lobbyData);
        }

        public void UpdateState(Dictionary<string, DataObject> lobbyData)
        {
            if(lobbyData.ContainsKey("GameMode"))
            {
                _gameMode = lobbyData["GameMode"].Value.ToString();
            }

            if (lobbyData.ContainsKey("RelayJoinCode"))
            {
                _relayJoinCode = lobbyData["RelayJoinCode"].Value;
            }

            if(lobbyData.ContainsKey("SceneName"))
            {
                _sceneName = lobbyData["SceneName"].Value;
            }
        }

        public Dictionary<string, string> Serialize()
        {
            return new Dictionary<string, string>()
            {
                {"GameMode", _gameMode.ToString()},
                {"RelayJoinCode", _relayJoinCode},
                {"SceneName", _sceneName}
            };
        }
    }
}

