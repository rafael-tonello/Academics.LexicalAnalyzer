using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using NShared;

namespace LexicalAnalyzer
{
    class App
    {
        GUI.GUI ui = new GUI.GUI();


        public App()
        {
        
        }

        private void toUI(string message, object arguments = null)
        {
            ui.sendMessage(message, arguments);
        }


    }

    class MainClass
    {
        public static void Main(string[] args)
        {
            App app = new App();
        }
    }
}
