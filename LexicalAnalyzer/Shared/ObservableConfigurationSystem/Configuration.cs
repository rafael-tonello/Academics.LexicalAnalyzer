﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NShared.Configuration
{
    public delegate void OnConfChange(ConfItem value);
    public class Configuration
    {
        string filename;
        public Configuration(string filename )
        {
            this.filename = filename;

            if (!Directory.Exists(Path.GetDirectoryName(filename)))
                Directory.CreateDirectory(Path.GetDirectoryName(filename));
        }

        Dictionary<string, ConfItem> confs = new Dictionary<string, ConfItem>();
        Dictionary<string, List<OnConfChange>> confsObservers = new Dictionary<string, List<OnConfChange>>();
        DateTime lastFileChangeTime = DateTime.Now;

        Timer fileMonitorTimer = null;

        /// <summary>
        /// Set an configuration
        /// </summary>
        /// <typeparam name="T">Enum type of configuration</typeparam>
        /// <param name="enumItem">Enum value that represents the desired configuration</param>
        /// <param name="value">The new value of configuratio. The allowed types are string, bool, int and double</param>
        public void set<T>(T enumItem, object value, bool justMemory = false)
        {
            //chekcs if the configuration is an enum
            string name = GetConfName<T>(enumItem);
            this.set(name, value, justMemory);
        }

        public void set(string name, object value, bool justMemory = false)
        {
            name = name.ToLower();
            //chekcs if the configuration is an enum
            ConfItem conf = new ConfItem();
            conf.Set(value);
            conf.justMemory = justMemory;
            confs[name] = conf;
            WriteFile();
            NotifyConfObservers(name);
        }

        /// <summary>
        /// Get a configuration
        /// </summary>
        /// <typeparam name="T">Enum type of the configuration</typeparam>
        /// <param name="enumItem">Enum value that represents the desired configuration</param>
        /// <param name="defaultValue">The value to be returned when the desired configutaion is not found</param>
        /// <returns>A object with the value that can be accessed as string, bool int or double</returns>
        public ConfItem get<T>(T enumItem, object defaultValue)
        {
            string name = GetConfName<T>(enumItem);

            return this.get(name, defaultValue);
        }

        public ConfItem get(string name, object defaultValue)
        {
            name = name.ToLower();
            //verify if the ram contains the variable
            if (confs.ContainsKey(name))
                return confs[name];
            else
            {
                //load file to ram
                ReadFile();

                //recheck the ram
                if (!confs.ContainsKey(name))
                {
                    ConfItem newConf = new ConfItem();
                    newConf.Set(defaultValue);
                    confs[name] = newConf;
                    WriteFile();
                }
                return confs[name];
            }
        }

        public T get<T>(string name, object defaultValue = null)
        {
            name = name.ToLower();
            //verify if the ram contains the variable
            if (confs.ContainsKey(name))
                return confs[name].AsT<T>();
            else
            {
                //load file to ram
                ReadFile();

                //recheck the ram
                if (!confs.ContainsKey(name))
                {
                    ConfItem newConf = new ConfItem();
                    newConf.Set(defaultValue);
                    confs[name] = newConf;
                    WriteFile();
                }
                return confs[name].AsT<T>();
            }
        }


        Semaphore WriteFileSempahore = new Semaphore(1, int.MaxValue);
        private void WriteFile()
        {
            Thread th = new Thread(delegate ()
            {
                WriteFileSempahore.WaitOne();
                List<string> lines = new List<string>();
                if (File.Exists(this.filename))
                    lines = File.ReadAllLines(this.filename).ToList();

                for (int counter = 0; counter < this.confs.Count; counter++)
                {
                    var c = this.confs.ElementAt(counter);
                    var found = false;
                    for (var count = 0; count < lines.Count; count++)
                    {
                        if (lines[count].Contains('=') && lines[count].Substring(0, lines[count].IndexOf('=')).ToLower() == c.Key.ToLower())
                        {
                            found = true;
                            lines[count] = lines[count].Substring(0, lines[count].IndexOf('=') + 1) + c.Value.AsString;
                        }
                    }

                    if (!found)
                        lines.Add(c.Key + '=' + c.Value.AsString);

                }
                File.WriteAllLines(this.filename, lines.ToArray());
                WriteFileSempahore.Release();
            });
            th.Start();
        }

        private void ReadFile()
        {

            WriteFileSempahore.WaitOne();

            var attemps = 0;
            List<string> lines = new List<string>();
            while (attemps < 10)
            {
                try
                {
                    lines = File.ReadAllLines(this.filename).ToList();
                    break;
                }
                catch {
                    attemps++;
                    Thread.Sleep(50);
                }
            }

            for (int cLines = 0; cLines < lines.Count; cLines++)
            {
                //check if line is not an comment or empty
                if ((lines[cLines] != "") && (!";#/\\".Contains(lines[cLines][0])))
                {
                    //checks if the line is a valid configuration
                    if (lines[cLines].Contains("="))
                    {
                        //take the name and the value of variable
                        string name = lines[cLines].Substring(0, lines[cLines].IndexOf('=')).TrimStart().TrimEnd().Trim();
                        name = name.Replace('_', '.').ToLower();

                        //take the value of variable
                        string value = lines[cLines].Substring(lines[cLines].IndexOf('=') + 1).TrimStart().TrimEnd().Trim();

                        //checks if has change in variable
                        if (this.confs.ContainsKey(name))
                        {
                            if (this.confs[name].AsString != value)
                            {
                                //store the value to ram
                                this.confs[name] = new ConfItem { AsString = value };

                                //notify observers about the change of variable
                                NotifyConfObservers(name);
                            }
                        }
                        else
                        {
                            //store the value to ram
                            this.confs[name] = new ConfItem { AsString = value };
                        }

                    }
                }
            }
            
            //write the file
            WriteFileSempahore.Release();
        }

        private string GetConfName<T>(T enumItem)
        {
            //chekcs if the configuration is an enum
            if (!enumItem.GetType().IsEnum)
            {
                throw new InvalidCastException("Excpected enum");
            }

            //convert the enum to string (is used as filename)
            string name = enumItem.GetType().Namespace + "." + enumItem.GetType().Name.ToString() + "." + enumItem.ToString();
            name = name.Replace("_", ".").ToLower();

            return name;
        }

        public OnConfChange ObservateConfiguration<T>(T enumItem, OnConfChange onChange, bool invokeFirstTime = true, object defaultValueToFirstTimeInvoke = null)
        {
            if (defaultValueToFirstTimeInvoke == null)
                defaultValueToFirstTimeInvoke = "";

            string name = GetConfName(enumItem).ToLower();

            if (!confsObservers.ContainsKey(name))
                confsObservers[name] = new List<OnConfChange>();


            confsObservers[name].Add(onChange);

            if (invokeFirstTime)
                onChange(this.get(name, defaultValueToFirstTimeInvoke));


            StartFileMonitor();

            return onChange;
                
        }

        public OnConfChange ObservateConfiguration(string confName, OnConfChange onChange, bool invokeFirstTime = true, object defaultValueToFirstTimeInvoke = null)
        {
            confName = confName.ToLower();
            if (defaultValueToFirstTimeInvoke == null)
                defaultValueToFirstTimeInvoke = "";
            

            if (!confsObservers.ContainsKey(confName))
                confsObservers[confName] = new List<OnConfChange>();


            confsObservers[confName].Add(onChange);

            if (invokeFirstTime)
                onChange(this.get(confName, defaultValueToFirstTimeInvoke));


            StartFileMonitor();

            return onChange;

        }


        Semaphore monitorSemaphore1 = new Semaphore(1, 10);
        Semaphore monitorSemaphore2 = new Semaphore(1, 10);
        int startedReaders = 0;
        private void StartFileMonitor()
        {
            monitorSemaphore1.WaitOne();
            if (fileMonitorTimer == null)
            {
                object state = new object();
                lastFileChangeTime = File.GetLastWriteTime(this.filename);
                startedReaders++;
                fileMonitorTimer = new Timer(delegate (object state2)
                {
                    monitorSemaphore2.WaitOne();
                    if (File.GetLastWriteTime(this.filename).ToString() != lastFileChangeTime.ToString())
                    {
                        ReadFile();
                        lastFileChangeTime = File.GetLastWriteTime(this.filename);
                    }
                    monitorSemaphore2.Release();

                }, state, 0, 50);

            }
            monitorSemaphore1.Release();
        }


        private void NotifyConfObservers(string name)
        {
            if (confsObservers.ContainsKey(name))
            {
                ConfItem conf = this.confs[name];
                foreach (var c in confsObservers[name])
                    c(conf);
            }
        }

        public List<string> GetConfsNames()
        {
            List<string> ret = new List<string>();
            foreach (var c in this.confs)
                ret.Add(c.Key);

            return ret;
        }
    }
}
