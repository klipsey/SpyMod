using UnityEngine.Networking;
using R2API.Networking;
using R2API.Networking.Interfaces;
using UnityEngine;
using RoR2;

namespace SpyMod.Spy.Components
{
    public class SyncResetStab : INetMessage
    {
        private NetworkInstanceId netId;

        public SyncResetStab()
        {
        }

        public SyncResetStab(NetworkInstanceId netId)
        {
            this.netId = netId;
        }

        public void Deserialize(NetworkReader reader)
        {
            this.netId = reader.ReadNetworkId();
        }

        public void OnReceived()
        {
            GameObject bodyObject = Util.FindNetworkObject(this.netId);
            if (!bodyObject) return;

            bodyObject.GetComponent<CharacterBody>().skillLocator.secondary.Reset();
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(this.netId);
        }
    }
}