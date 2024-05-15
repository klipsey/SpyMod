using UnityEngine.Networking;
using R2API.Networking;
using R2API.Networking.Interfaces;
using UnityEngine;
using RoR2;

namespace SpyMod.Spy.Components
{
    internal class SyncStabExplosion : INetMessage
    {
        private NetworkInstanceId netId;
        private GameObject target;

        public SyncStabExplosion()
        {
        }

        public SyncStabExplosion(NetworkInstanceId netId, GameObject target)
        {
            this.netId = netId;
            this.target = target;
        }

        public void Deserialize(NetworkReader reader)
        {
            this.netId = reader.ReadNetworkId();
            this.target = reader.ReadGameObject();
        }

        public void OnReceived()
        {
            GameObject bodyObject = Util.FindNetworkObject(this.netId);
            if (!bodyObject) return;

            bodyObject.AddComponent<StabExplosion>();
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(this.netId);
            writer.Write(this.target);
        }
    }
}