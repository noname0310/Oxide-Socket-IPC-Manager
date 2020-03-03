using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Oxide.Core;
using Oxide.Core.Configuration;
using Oxide.Core.Plugins;
using Oxide.Core.Libraries;
using Oxide.Core.Libraries.Covalence;

namespace Oxide.Plugins
{
    [Info("Socket IPC Manager", "noname", "0.1.0")]
    [Description("Manage Socket IPC IO")]
    class SocketIPCManager : CovalencePlugin
    {
        private JArray SendDataQueue;

        private string FullAddress;
        private bool Requesting;
        private RequestMethod CurrentRequstMethod;

        #region Hooks

        private new void LoadDefaultConfig()
        {
            PrintWarning("Creating a new configuration file");

            Config.WriteObject(GetDefaultConfig(), true);
        }

        private void Init()
        {
            LoadConfig();
        }

        private void OnServerInitialized()
        {
            SendDataQueue = new JArray();
            FullAddress = string.Format("http://{0}:{1}/", config.IPCAddress, config.IPCPort);

            Requesting = false;
            CurrentRequstMethod = RequestMethod.GET;
            //timer.Every(0.1f, TryRequest);
        }

        #endregion

        #region ConfigManage

        private PluginConfig config;

        private void LoadConfig()
        {
            config = Config.ReadObject<PluginConfig>();

            if (config == null)
                config = GetDefaultConfig();
        }

        private class PluginConfig
        {
            public string IPCAddress;
            public int IPCPort;
        }

        private PluginConfig GetDefaultConfig()
        {
            return new PluginConfig
            {
                IPCAddress = "127.0.0.1",
                IPCPort = 20311
            };
        }

        #endregion

        #region WebRequest

        private void TryRequest()
        {
            if (Requesting == true)
                return;

            if (CurrentRequstMethod == RequestMethod.GET)
            {
                webrequest.Enqueue(FullAddress, null, (code, response) => GetCallback(code, response), this, RequestMethod.GET);
                CurrentRequstMethod = RequestMethod.POST;
            }
            else if (CurrentRequstMethod == RequestMethod.POST)
            {
                webrequest.Enqueue(FullAddress, SendDataQueue.ToString(), (code, response) => GetCallback(code, response), this, RequestMethod.POST);
                CurrentRequstMethod = RequestMethod.GET;
            }
            Requesting = true;
        }

        private void GetCallback(int code, string response)
        {
            if (response == null || code != 200)
            {
                Puts($"Error: {code} - Couldn't get an answer");
                Requesting = false;
                return;
            }

            Puts($"answered: {response}");
            Requesting = false;
        }

        #endregion

        #region Command/API



        #endregion

        #region Helper



        #endregion
    }
}