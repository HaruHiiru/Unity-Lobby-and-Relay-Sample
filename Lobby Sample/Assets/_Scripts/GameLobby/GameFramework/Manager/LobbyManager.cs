using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using GameFramework.Core;
using Unity.Services.Core;
using GameFramework.Core.Events;

public class LobbyManager : Singleton<LobbyManager>
{
    Lobby lobby;

    public Lobby getLobby() => lobby;
    public string getLobbyCode()
    {
        return lobby?.LobbyCode;
    }

    public async Task<bool> createLobby(int maxPlayers, bool isPrivate, Dictionary<string, string> data, Dictionary<string, string> lobbyData)
    {

        Dictionary<string, PlayerDataObject> playerData = SerializePlayerData(data);
        Player player = new Player(AuthenticationService.Instance.PlayerId, null, playerData);

        CreateLobbyOptions options = new CreateLobbyOptions()
        {
            Data = SerializeLobbyData(lobbyData), 
            IsPrivate = isPrivate,
            Player = player
        };

        try
        {
            lobby = await LobbyService.Instance.CreateLobbyAsync("Lobby", maxPlayers, options);
        }
        catch(System.Exception ex)
        {
            print(ex);
            return false;
        }

        Debug.Log("Lobby created with ID: " +  lobby.Id);
        Debug.Log("Lobby created with Code: " + lobby.LobbyCode);

        StartCoroutine(HeartBeatlobby(lobby.Id, 5f));
        StartCoroutine(RefreshLobby(lobby.Id, 1f));
        return true;
    }

    public async Task<bool> joinLobby(string code, Dictionary<string, string> data)
    {
        JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions();
        Player player = new Player(AuthenticationService.Instance.PlayerId, null, SerializePlayerData(data));

        options.Player = player;

        if(code == "")
        {
            // add invalid lobby animation here
        }

        try
        {
            lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code, options);
        }
        catch( System.Exception ex)
        {
            print(ex);
            return false;
        }
        StartCoroutine(RefreshLobby(lobby.Id, 1f));

        return true;
    }


    private IEnumerator HeartBeatlobby(string lobbyId, float waitTime)
    {
        while (true)
        {
            Debug.Log("Heartbeat Sent");
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return new WaitForSecondsRealtime(waitTime);
        }
    }

    private IEnumerator RefreshLobby(string lobbyId, float waitTime)
    {
        while (true)
        {
            Task<Lobby> task = LobbyService.Instance.GetLobbyAsync(lobbyId);
            yield return new WaitUntil(() => task.IsCompleted);
            Lobby newLobby = task.Result;

            if (newLobby.LastUpdated > lobby.LastUpdated)
            {
                lobby = newLobby;
                LobbyEvents.onLobbyUpdated?.Invoke(newLobby);
            }
            yield return new WaitForSecondsRealtime(waitTime);
        }
    }

    private Dictionary<string, PlayerDataObject> SerializePlayerData(Dictionary<string, string> date)
    {
        Dictionary<string, PlayerDataObject> playerData = new Dictionary<string, PlayerDataObject>();
        foreach (var (key, value) in date)
        {
            playerData.Add(key, new PlayerDataObject(
                visibility: PlayerDataObject.VisibilityOptions.Member,
                value: value));
        }

        return playerData;
    }

    private Dictionary<string, DataObject> SerializeLobbyData(Dictionary<string, string> date)
    {
        Dictionary<string, DataObject> lobbyData = new Dictionary<string, DataObject>();
        foreach (var (key, value) in date)
        {
            lobbyData.Add(key, new DataObject(
                visibility: DataObject.VisibilityOptions.Member,
                value: value));
        }

        return lobbyData;
    }

    public void OnApplicationQuit()
    {
        if (lobby != null && lobby.HostId == AuthenticationService.Instance.PlayerId) 
        {
            LobbyService.Instance.DeleteLobbyAsync(lobby.Id);        
        }
    }

    public List<Dictionary<string, PlayerDataObject>> GetPlayerData()
    {
        List<Dictionary<string, PlayerDataObject>> data = new List<Dictionary<string, PlayerDataObject>>();

        foreach (Player p in lobby.Players) 
        {
            data.Add(p.Data);
        }

        return data;
    }

    public async Task<bool> UpdatePlayerData(string playerId, Dictionary<string, string> data, string allocaltionId = default, string connectionData = default)
    {
        Dictionary<string, PlayerDataObject> playerData = SerializePlayerData(data);
        UpdatePlayerOptions options = new UpdatePlayerOptions()
        {
            Data = playerData,
            AllocationId = allocaltionId,
            ConnectionInfo = connectionData
        };

        try
        {
            lobby = await LobbyService.Instance.UpdatePlayerAsync(lobby.Id, playerId, options);
        }
        catch (System.Exception ex) 
        {
            print(ex.ToString());
            return false;
        }

        LobbyEvents.onLobbyUpdated(lobby);

        return true;
    }

    public async Task<bool> UpdateLobbyData(Dictionary<string, string> data)
    {
        Dictionary<string, DataObject> lobbyData = SerializeLobbyData(data);
        UpdateLobbyOptions options = new UpdateLobbyOptions()
        { 
            Data = lobbyData
        };

        try
        {
            lobby = await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, options);
        }
        catch (System.Exception ex)
        {
            print(ex);
            return false;
        }

        LobbyEvents.onLobbyUpdated(lobby);

        return true;
    }

    public string GetHostId()
    {
        return lobby.HostId;
    }
}
