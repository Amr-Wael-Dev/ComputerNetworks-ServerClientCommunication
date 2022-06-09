using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.LoadRedirectionRules(redirectionMatrixPath);

            //TODO: initialize this.serverSocket
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, portNumber));
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            // largest backlog = maximun integer value تقريبا و الله اعلم ¯\_(ツ)_/¯
            serverSocket.Listen(int.MaxValue);

            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket client = serverSocket.Accept();
                Thread newThread = new Thread(new ParameterizedThreadStart(HandleConnection));
                newThread.Start(client);
            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            Socket client = (Socket)obj;

            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            client.ReceiveTimeout = 0;

            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {
                    // TODO: Receive request

                    // مفيش ريكويست هتبقى اكبر من كيلوبايت كده كده 
                    byte[] data = new byte[1024];
                    int receivedLength = client.Receive(data);

                    // TODO: break the while loop if receivedLen==0
                    if (receivedLength == 0)
                    {
                        break;
                    }

                    // TODO: Create a Request object using received request string
                    Request request = new Request(Encoding.ASCII.GetString(data));

                    // TODO: Call HandleRequest Method that returns the response
                    Response response = HandleRequest(request);

                    // TODO: Send Response back to client
                    client.Send(Encoding.ASCII.GetBytes(response.ResponseString));
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }

            // TODO: close client socket
            client.Close();
        }

        Response HandleRequest(Request request)
        {
            // throw new NotImplementedException();

            // بتاخد 4 باراميترز Response
            // 1: code
            // 2: content type (دائما text\html في حالتنا)
            // 3: content
            // 4: redirection path (يعني يعمل ريدايركت على انهي صفحة)
            // دايما 4 في الاغلب بتبقى فاضية عشان ماعندناش غير ريدايركت واحد

            string content;
            try
            {
                //TODO: check for bad request 
                if (!request.ParseRequest())
                {
                    content = LoadDefaultPage(Configuration.BadRequestDefaultPageName);
                    return new Response(StatusCode.BadRequest, "text/html", content, GetRedirectionPagePathIFExist(Configuration.BadRequestDefaultPageName));
                }

                //TODO: map the relativeURI in request to get the physical path of the resource.
                string fullPath = Configuration.RootPath + request.relativeURI;

                //TODO: check for redirect

                // !GetRedirectionPagePathIFExist(request.relativeURI).Equals(string.Empty) = redirection path is not empty
                // يعني ريدايركت
                if (!GetRedirectionPagePathIFExist(request.relativeURI).Equals(string.Empty))
                {
                    content = LoadDefaultPage(Configuration.RedirectionDefaultPageName);
                    fullPath = Configuration.RootPath + "/" + GetRedirectionPagePathIFExist(request.relativeURI);

                    return new Response(StatusCode.Redirect, "text/html", content, GetRedirectionPagePathIFExist(request.relativeURI));
                }

                //TODO: check file exists
                if (!File.Exists(fullPath))
                {
                    content = LoadDefaultPage(Configuration.NotFoundDefaultPageName);
                    return new Response(StatusCode.NotFound, "text/html", content, GetRedirectionPagePathIFExist(Configuration.NotFoundDefaultPageName));
                }

                //TODO: read the physical file
                content = File.ReadAllText(fullPath);

                // Create OK response
                return new Response(StatusCode.OK, "text/html", content, GetRedirectionPagePathIFExist(fullPath));
            }

            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);

                // TODO: in case of exception, return Internal Server Error. 
                content = LoadDefaultPage(Configuration.InternalErrorDefaultPageName);
                return new Response(StatusCode.InternalServerError, "text/html", content, GetRedirectionPagePathIFExist(Configuration.InternalErrorDefaultPageName));
            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            for (int i = 0; i < Configuration.RedirectionRules.Count; i++)
            {
                // Configuration.RedirectionRules = key: value
                // (Ex) key: aboutus , value: aboutus2
                // valueرجع ال ,keyلو لقيت ال
                if (Configuration.RedirectionRules.Keys.ElementAt(i) == relativePath)
                {
                    return Configuration.RedirectionRules.Values.ElementAt(i);
                }
            }

            return string.Empty;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);

            // TODO: check if filepath not exist log exception using Logger class and return empty string
            if (!File.Exists(filePath))
            {
                Logger.LogException(new Exception(defaultPageName + " doesn't exist"));
                return string.Empty;
            }

            // else read file and return its content
            // File.ReadAllText(filePath) returns string of all file content
            return File.ReadAllText(filePath);
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 

                // File.ReadAllLines(filePath) returns array of string (array element = line in file)
                // File.ReadAllText(filePath) و File.ReadAllLines(filePath) ماتنساش\ماتنسيش الفرق بين
                string[] rules = File.ReadAllLines(filePath);
                Configuration.RedirectionRules = new Dictionary<string, string>();

                // then fill Configuration.RedirectionRules dictionary 
                for (int i = 0; i < rules.Length; i++)
                {
                    Configuration.RedirectionRules.Add(rules[i].Split(',')[0], rules[i].Split(',')[1]);
                }
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}
