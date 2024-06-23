using System.Threading.Tasks;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;

namespace GameFramework.Core.GameFramework.Manager
{
    public class RelayManager: Singleton<RelayManager>
    {

        private bool _isHost;
        private string _joinCode;
        private string _ip;
        private int _port;
        private byte[] _key;
        private byte[] _connectionData;
        private byte[] _HostConnectionData;
        private System.Guid _allocationId;
        private byte[] _allocationIdBytes;

        public bool IsHost
        {
            get { return _isHost; }
        }

        public string GetAllocationId()
        {
            return _allocationId.ToString();
        }

        public string GetConnectionData()
        {
            return _connectionData.ToString();
        }

        public async Task<string> CreateRelay(int maxConnections)
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
            _joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            RelayServerEndpoint dtlsEndpoint = allocation.ServerEndpoints.First(conn => conn.ConnectionType == "dtls");
            _ip = dtlsEndpoint.Host;
            _port = dtlsEndpoint.Port;

            _allocationId = allocation.AllocationId;
            _allocationIdBytes = allocation.AllocationIdBytes;
            _connectionData = allocation.ConnectionData;
            _key = allocation.Key;

            _isHost = true;

            return _joinCode;
        }

        public async Task<bool> JoinRelay(string joinCode)
        {
            _joinCode = joinCode;
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerEndpoint dtlsEndpoint = allocation.ServerEndpoints.First(conn => conn.ConnectionType == "dtls");
            _ip = dtlsEndpoint.Host;
            _port = dtlsEndpoint.Port;

            _allocationId = allocation.AllocationId;
            _allocationIdBytes = allocation.AllocationIdBytes;
            _connectionData = allocation.ConnectionData;
            _HostConnectionData = allocation.HostConnectionData;
            _key = allocation.Key;

            _isHost = false;

            return true;
        }

        public (byte[] AllocationId, byte[] key, byte[] ConnectionData,string _dtlsAddress, int _dtlsPort) GetHostConnectionInfo()
        {
            return (_allocationIdBytes, _key, _connectionData, _ip, _port);
        }
        public (byte[] AllocationId, byte[] key, byte[] ConnectionData, byte[] HostConnectionData,string _dtlsAddress, int _dtlsPort) GetClientConnectionInfo()
        {
            return (_allocationIdBytes, _key, _connectionData, _HostConnectionData,_ip, _port);
        }
    }
}