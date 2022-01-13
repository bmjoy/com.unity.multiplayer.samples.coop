using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
namespace Unity.Multiplayer.Samples.BossRoom
{
    public class DebugCheatsMediator : NetworkBehaviour
    {

        public Action<ulong> SpawnEnemy;
        public Action<ulong> SpawnBoss;
        public Action<ulong> GoToPostGame;
        public Action<ulong> ToggleGodMode;

        Dictionary<ulong, bool> m_GodModeState;

        [ServerRpc(RequireOwnership = false)]
        public void SpawnEnemyServerRpc(ulong clientId)
        {
            SpawnEnemy?.Invoke(clientId);
            LogCheatUsedClientRPC(clientId, "SpawnEnemy");
        }

        [ServerRpc(RequireOwnership = false)]
        public void SpawnBossServerRpc(ulong clientId)
        {
            SpawnBoss?.Invoke(clientId);
            LogCheatUsedClientRPC(clientId, "SpawnBoss");
        }

        [ServerRpc(RequireOwnership = false)]
        public void GoToPostGameServerRpc(ulong clientId)
        {
            GoToPostGame?.Invoke(clientId);
            LogCheatUsedClientRPC(clientId, "GoToPostGame");
        }

        [ServerRpc(RequireOwnership = false)]
        public void ToggleGodModeServerRpc(ulong clientId)
        {
            ToggleGodMode?.Invoke(clientId);
            LogCheatUsedClientRPC(clientId, "ToggleGodMode");
        }

        [ClientRpc]
        public void LogCheatUsedClientRPC(ulong clientId, string cheatUsed)
        {
            Debug.Log($"Cheat {cheatUsed} used by client {clientId}");
        }
    }
}
#endif
