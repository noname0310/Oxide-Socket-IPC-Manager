using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Oxide.Core;
using Oxide.Core.Configuration;
using Oxide.Core.Plugins;
using Oxide.Core.Libraries.Covalence;
using Rust;

namespace Oxide.Plugins
{
    [Info("Socket IPC Manager", "noname", "0.0.1")]
    [Description("Manage IPC")]
    class SocketIPCManager : CovalencePlugin
    {
        private JArray SendDataQueue;
        private GameObject CoroutineObject;

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

        }

        #endregion

        #region PluginIO

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
            public int GuardDuration;
            public int BetweenReadDelay;
        }

        private PluginConfig GetDefaultConfig()
        {
            return new PluginConfig
            {
                GuardDuration = 400,//0.4sec
                BetweenReadDelay = 100,//0.1sec
                // [guard add][0.4 waiting..][ReadAndWriteFile][guard remove][0.1 waiting..][guard add][0.4 waiting..]
            };
        }

        #endregion

        #region DataManage

        DynamicConfigFile ServerWritingDataFile;
        DynamicConfigFile ServerWriteEndDataFile;

        private void InitData()
        {
            ServerWritingDataFile = Interface.Oxide.DataFileSystem.GetDatafile("IPCManager/ServerWriting.json");
            ServerWriteEndDataFile = Interface.Oxide.DataFileSystem.GetDatafile("IPCManager/ServerWriteEnd.json");
        }

        private void CreateIPCGuard()
        {
            ServerWritingDataFile.Save();
        }

        private void CreateIPCEnd()
        {
            ServerWriteEndDataFile.Save();
        }

        #endregion

        #endregion

        #region Coroutine

        private void LoadCoroutineObject()
        {
            CoroutineObject = new GameObject();

        }

        private void UnloadCoroutineObject()
        {

        }

        public class IPCCoroutineObject : MonoBehaviour
        {
            IEnumerator Start()
            {
                StartCoroutine("DoSomething", 2.0F);
                yield return new WaitForSeconds(1);
                StopCoroutine("DoSomething");
            }

            IEnumerator DoSomething(float someParameter)
            {
                while (true)
                {
                    print("DoSomething Loop");
                    yield return null;
                }
            }
        }

        private IEnumerator IPCRoutine()
        {

        }

        #endregion

        #region Command/API



        #endregion

        #region Helper



        #endregion
    }
}