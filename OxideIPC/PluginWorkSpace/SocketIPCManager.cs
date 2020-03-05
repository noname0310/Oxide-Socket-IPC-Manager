using Newtonsoft.Json.Linq;
using Oxide.Core;
using Oxide.Core.Libraries;

namespace Oxide.Plugins
{
    [Info("Socket IPC Manager", "noname", "0.1.0")]
    [Description("Manage Socket IPC IO")]
    class SocketIPCManager : CovalencePlugin
    {
        private JArray SendDataQueue;

        private string FullAddress;
        private string GetRequest;
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
            GetRequest = string.Format("{0}{1}", FullAddress, "json");

            Requesting = false;
            CurrentRequstMethod = RequestMethod.GET;
            timer.Every(0.1f, TryRequest);
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
                webrequest.Enqueue(GetRequest, null, (code, response) => GetCallback(RequestMethod.GET, code, response), this, RequestMethod.GET);
                CurrentRequstMethod = RequestMethod.POST;
            }
            else if (CurrentRequstMethod == RequestMethod.POST)
            {
                if (SendDataQueue.Count == 0)
                {
                    webrequest.Enqueue(GetRequest, null, (code, response) => GetCallback(RequestMethod.GET, code, response), this, RequestMethod.GET);
                }
                else
                {
                    webrequest.Enqueue(FullAddress, SendDataQueue.ToString(), (code, response) => GetCallback(RequestMethod.POST, code, response), this, RequestMethod.POST);
                    CurrentRequstMethod = RequestMethod.GET;

                    SendDataQueue.Clear();
                }
            }
            Requesting = true;
        }

        private void GetCallback(RequestMethod requestMethod, int code, string response)
        {
            if (response == null || code != 200)
            {
                Puts($"Error: {code} - Couldn't connect from Discord Linker");
                Requesting = false;
                return;
            }

            if (requestMethod == RequestMethod.GET)
                Interface.CallHook("OnIPCReceivedData", JArray.Parse(response));
            Requesting = false;
        }

        #endregion

        #region Command/API

        private void API_IPCEnqueue(JToken jToken)
        {
            SendDataQueue.Add(jToken);
        }

        #endregion
    }
}