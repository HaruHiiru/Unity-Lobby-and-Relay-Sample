using System.Collections.Generic;
using Unity.Services.Lobbies.Models;

namespace GameLobby.Events
{
    public static class LobbyEvents
    {
        public delegate void LobbyUpdated();
        public static LobbyUpdated onLobbyUpdated;
        
        public delegate void LobbyReady();
        public static LobbyReady onLobbyReady;
    }
}