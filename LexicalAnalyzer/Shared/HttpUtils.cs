using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Libs
{
    class HttpUtils
    {
        public string httpRequest(string url, string method = "GET", string postData = "", Dictionary<string, string> headers = null)
        {
            string res = "";
            try
            {
                res = httpRequestDotNet(url, method, postData, headers);
            }
            catch
            {
				res = httpRequestRaw(url, method, postData, headers);
            }

            

            return res;
        }

        public string httpRequestRaw(string url, string method = "GET", string postData = "", Dictionary<string, string> headers = null)
        {
            if (url.StartsWith("https://"))
                return httpRequestDotNet(url, method, postData, headers);

            int port = 80;
            string host = url.Substring(7);
            string resource = "/";

            if (host.Contains('/'))
            {
                resource = host.Substring(host.IndexOf('/'));
                host = host.Substring(0, host.IndexOf('/'));
            }

            if (host.Contains(':'))
            {
                port = int.Parse(host.Substring(host.IndexOf(':') + 1));
                host = host.Substring(0, host.IndexOf(':'));
            }

            var cli = new TcpClient(host, port);
            if (cli.Connected)
            {
                string header = method.ToUpper() + " " + resource + " HTTP/1.1\r\n";
                header += "Host: " + host + "\r\n";
                if (headers != null)
                    foreach (var curr in headers)
                        header += curr.Key + ": " + curr.Value + "\r\n";

                if (postData != "")
                {
                    header += "Content-Length: " + Encoding.UTF8.GetByteCount(postData) + "\r\n";
                }


                header += "\r\n" +postData;

                //mound http header
                cli.Client.Send(Encoding.UTF8.GetBytes(header));

                cli.Client.ReceiveTimeout = 120000;

                string state = "ReceivingHeader";
                byte[] bufferRec;
                List<byte> HeaderBytes = new List<byte>();
                string[] headersStrings;
                string[] tempArray;
                Dictionary<string, string> headersList = new Dictionary<string, string>();
                byte[] content = new byte[0];
                int readedContent = 0;

                DateTime timeoutStart = DateTime.Now;
                while (cli.Client.Connected)
                {
                    int toRead = cli.Client.Available;
                    if (toRead > 0)
                    {
                        timeoutStart = DateTime.Now;
                        bufferRec = new byte[toRead];
                        int readed = cli.Client.Receive(bufferRec, toRead, SocketFlags.None);

                        int cont = 0;
                        bool stop = false;
                        while (!stop)
                        {
                            switch (state)
                            {
                                case "ReceivingHeader":
                                    if ((cont < bufferRec.Length) && bufferRec[cont] != '\r')
                                    {
                                        HeaderBytes.Add(bufferRec[cont]);
                                        if ((HeaderBytes.Count > 1) && (HeaderBytes[HeaderBytes.Count - 1] == '\n') && (HeaderBytes[HeaderBytes.Count - 2] == '\n'))
                                        {
                                            state = "PasingHeader";
                                            cont++;
                                            continue;
                                        }
                                    }
                                    cont++;

                                    if (cont >= readed)
                                        stop = true;
                                    break;
                                case "PasingHeader":
                                    headersStrings = Encoding.UTF8.GetString(HeaderBytes.ToArray()).Split('\n');
                                    HeaderBytes.Clear();
                                    foreach (var curr in headersStrings)
                                    {
                                        if (curr.Contains(':'))
                                        {
                                            tempArray = curr.ToLower().Split(':');
                                            tempArray[0] = tempArray[0].TrimStart().TrimEnd().Trim();
                                            tempArray[1] = tempArray[1].TrimStart().TrimEnd().Trim();
                                            headersList[tempArray[0]] = tempArray[1];
                                        }
                                    }
                                    if (headersList.ContainsKey("content-length"))
                                        content = new byte[int.Parse(headersList["content-length"])];
                                    else
                                        content = new byte[0];

                                    state = "ReadingContent";
                                    break;
                                case "ReadingContent":
                                    Array.Copy(bufferRec, cont, content, readedContent, readed - cont);
                                    readedContent += readed - cont;
                                    cont = readed;
                                    if (readedContent >= content.Length)
                                    {
                                        state = "ProcessingResponse";
                                    }
                                    else
                                        stop = true;
                                    break;
                                case "ProcessingResponse":
                                    string ret = Encoding.UTF8.GetString(content);
                                    content = new byte[0];
                                    cli.Client.Disconnect(true);
                                    return ret;
                            }
                        }
                    }

                    Thread.Sleep(1);
                    if (DateTime.Now.Subtract(timeoutStart).TotalSeconds >= 120)
                    {
                        try { cli.Client.Disconnect(false); } catch { }
                        break;
                    }
                }
            }

            return "";
        }

        public string httpRequestDotNet(string url, string method = "GET", string postData = "", Dictionary<string, string> headers = null)
        {

            HttpClient client = new HttpClient();

            method = method.ToUpper();


            string mimeType = "text/plain";

            if (headers != null)
            {
                foreach (var c in headers)
                {
                    if ((c.Key.ToLower() == "content-type") || (c.Key.ToLower() == "accept"))
                    {
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(c.Value));
                        mimeType = c.Value;
                    }
                    else
                        client.DefaultRequestHeaders.TryAddWithoutValidation(c.Key, c.Value);

                }
            }


            

            Task<HttpResponseMessage> wait1 = null;
            if (method == "GET")
                wait1 = client.GetAsync(url);
            if (method == "POST")
                wait1 = client.PostAsync(url, new StringContent(postData, Encoding.UTF8, mimeType));
            if (method == "DELETE")
                wait1 = client.DeleteAsync(url);
            if (method == "PUT")
                wait1 = client.PutAsync(url, new StringContent(postData));


            wait1.Wait();
            var response = wait1.Result;
            

            //takes the server response
            var wait2 = response.Content.ReadAsStringAsync();
            wait2.Wait();
            string result = wait2.Result;

            return result;

        }

        public string getInnerText(string source)
        {
            StringBuilder sb = new StringBuilder();
            string st = "finding >";

            foreach (var c in source)
            {
                switch (st)
                {
                    case "finding >":
                        if (c == '>')
                            st = "copy";
                        else if ((c == '"') || (c == '\''))
                            st = "skipingProperty";
                        break;
                    case "copy":
                        if (c != '<')
                            sb.Append(c);
                        else
                            st = "finding >";
                        break;
                    case "skipingProperty":
                        if ((c == '"') || (c == '\''))
                            st = "finding >";
                        break;
                }
            }

            return sb.ToString();
        }
		
		public static string getHttpField(string urlTemplate, string urlSource, string field, string defaultValue = null)
        {
            //determin the position of field in urlTemplate
            string[] templateData = urlTemplate.Split('/');
            string[] urlSourceData = urlSource.Split('/');
            int indexOfField = -1;
            for (int c = 0; c < templateData.Length; c++)
            {
                if (templateData[c].ToLower() == field.ToLower())
                {
                    indexOfField = c;
                    break;
                }
            }

            if ((indexOfField > -1) && (indexOfField < urlSourceData.Length))
                return urlSourceData[indexOfField];

            return defaultValue;
        }

    }
}