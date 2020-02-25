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
using Oxide.Core.Libraries;

namespace Oxide.Plugins
{
    [Info("IPCManager", "noname", "0.0.1")]
    [Description("Manage UIscale Data")]
    class IPCManager : CovalencePlugin
    {
        private void OnServerInitialized()
        {
            GetRequest();
        }

        private void GetRequest()
        {
            // Set a custom timeout (in milliseconds)
            float timeout = 200f;

            // Set some custom request headers (eg. for HTTP Basic Auth)
            Dictionary<string, string> headers = new Dictionary<string, string> { { "header", "value" } };

            webrequest.Enqueue("http://www.google.com/search?q=umod", null, (code, response) =>
                GetCallback(code, response), this, RequestMethod.GET, headers, timeout);
        }

        private void GetCallback(int code, string response)
        {
            if (response == null || code != 200)
            {
                Puts($"Error: {code} - Couldn't get an answer from Google for");
                return;
            }

            Puts($"Google answered: {response}");
        }
    }
}