using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Reflection;
using KW;
using Newtonsoft.Json;

namespace LexicalAnalyzer.GUI
{
    public delegate void OnMessage(string message, object arguments);
    public class GUI
    {
        KW.KWHttpServer server;
        int id = 0;
        Dictionary<string, OnMessage> MessagesAwaiting = new Dictionary<string, OnMessage>();
        public GUI()
        {
            string htmlDirectory = this.findHtmlFilesFolder();
            if (htmlDirectory != "")
            {
                server = new KW.KWHttpServer(
                    new int[] { 2030 },
                    true,
                    new List<string> { htmlDirectory }
                );

                server.onWebSocketData += delegate (
                    string resource, 
                    object wsId, 
                    string stringData, 
                    byte[] rawData, 
                    out byte[] opRawReturn)
                {
                    opRawReturn = new byte[0];
                    parseJsonMessage(stringData);
                    return null;
                };

                server.onClienteDataSend += delegate (HttpSessionData data)
                {
                    parseJsonMessage(data.sBody, data.resource);
                    return data;
                };
            }

            //try run google chrome
            tryRunChromium("http://127.0.0.1:2030");


        }

        /// <summary>
        /// Sends a message to the browser.
        /// </summary>
        /// <param name="message">The message to be sent</param>
        /// <param name="args">The arguments of the message</param>
        public void sendMessage(string message, object args, OnMessage onResponse = null)
        {
            string dt = "";
            //try convert args to an streamable object

            dynamic obj = new ExpandoObject();
            obj.message = message;
            obj.arguments = args;
            obj.id = "server_" + this.getId().ToString();
            obj.waitingResponse = false;


            dt = JsonConvert.SerializeObject(obj);

            if (onResponse != null)
            {
                obj.waitingResponse = true;
                this.MessagesAwaiting[obj.id] = onResponse;
            }
                
            server.sendWebSocketData(null, obj);
        }

        public event OnMessage OnMessage = null;

        /// <summary>
        /// This  message parse message comming from the browser. After parsing,
        /// the  event  'onCommand' is called.All messages must be the following
        /// structure:
        /// {
        ///     "id":"an unique message id"
        ///     "responseId":"if this message is an response, send original id 
        ///                 here"
        ///     "message":"the message",
        ///     "argument":"a  number,  text,  object  or null that contains the 
        ///                 'message' aditional arguments"
        /// }
        /// </summary>
        /// <param name="data">The json data that is came from brower</param>
        /// <param name="optionalCommand">An optional command to be used if it's
        /// not found in the json data</param>
        private void parseJsonMessage(string data, string optionalCommand = "")
        {
            dynamic obj = JsonConvert.DeserializeObject(data);
            string command = obj.message;
            if (command == null)
                command = optionalCommand;

            dynamic arguments = obj.arguments;

            this.OnMessage?.Invoke(command, arguments);
            obj = null;
        }

        /// <summary>
        /// Tries  the  run  chromium browser. If found, the browser will ru'''n in
        /// "app" mode.
        /// </summary>
        /// <returns><c>true</c>, if run chromium was found, <c>false</c> otherwise.</returns>
        /// <param name="url">URL.</param>
        private bool tryRunChromium(string url)
        {
            string[] possibleCommands = new string[] {
                "chrome",
                "chrome-browser",
                "google-chrome",
                "google-chrome-stable",
                "chromium",
                "chromium-browser"
            };

            foreach (var curr in possibleCommands)
            {
                try
                {
                    Process.Start(curr, "-app=\""+url+"\"");
                    return true;
                }
                catch { }
            }

            return false;
        }

        /// <summary>
        /// This function looks by html files folder in current app path. 
        /// If not found, try look at directory above, and repeat this process 
        /// until find an directory with an index.html file
        /// </summary>
        /// <returns>The html files folder.</returns>
        private string findHtmlFilesFolder(string currDirectory = "", List<string> checkedDirectories = null)
        {
            if (checkedDirectories == null)
                checkedDirectories = new List<string>();


            if (currDirectory == "")
                currDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            currDirectory = currDirectory.Replace('\\', '/');


            if (checkedDirectories.IndexOf(currDirectory) > -1)
                return "";

            checkedDirectories.Add(currDirectory);

            if (currDirectory == "/home")
                return "";

            if (File.Exists(currDirectory + "/index.html"))
                return currDirectory;


            //get all subfolder from current location
            var subs = Directory.GetDirectories(currDirectory);

            foreach (var c in subs)
            {
                var curSubFix = c.Replace("\\", "/");
                var found = findHtmlFilesFolder(curSubFix, checkedDirectories);
                if (found != "")
                    return found;
            }

            return findHtmlFilesFolder(currDirectory.Substring(0, currDirectory.LastIndexOf('/')), checkedDirectories);
        }
    
        private int getId()
        {
            return this.id++;

        }
    }
}
