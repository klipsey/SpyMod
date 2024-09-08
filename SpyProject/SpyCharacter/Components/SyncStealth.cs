﻿using UnityEngine.Networking;
using R2API.Networking;
using R2API.Networking.Interfaces;
using UnityEngine;
using RoR2;

namespace SpyMod.Spy.Components
{
    public class SyncStealth : INetMessage
    {
        private NetworkInstanceId netId;
        private GameObject target;

        public SyncStealth()
        {
        }

        public SyncStealth(NetworkInstanceId netId, GameObject target)
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
            SpyController spyController = bodyObject.GetComponent<SpyController>();
            if (spyController) spyController.EnterStealth();
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(this.netId);
            writer.Write(this.target);
        }
    }
}