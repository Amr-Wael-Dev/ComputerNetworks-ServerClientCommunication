using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            // throw new NotImplementedException();
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            this.code = code;
            responseString = GetStatusLine(this.code);

            headerLines.Add("Content-Type: " + contentType + "\r\n");
            headerLines.Add("Content-Length: " + content.Length + "\r\n");
            headerLines.Add("Date: " + DateTime.Now + "\r\n");
            if (code == StatusCode.Redirect)
            {
                headerLines.Add("Location: " + redirectoinPath + "\r\n");
            }

            // TODO: Create the request string
            for (int i = 0; i < headerLines.Count; i++)
            {
                responseString += headerLines[i];
            }
            responseString += "\r\n";
            responseString += content;
        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string statusLine = string.Empty;

            switch (code)
            {
                case StatusCode.OK:
                    statusLine = "200 OK\r\n";

                    break;
                case StatusCode.BadRequest:
                    statusLine = "400 Bad Request\r\n";

                    break;
                case StatusCode.InternalServerError:
                    statusLine = "500 Internal Server Error\r\n";

                    break;
                case StatusCode.NotFound:
                    statusLine = "404 Not Found\r\n";

                    break;
                case StatusCode.Redirect:
                    statusLine = "301 Redirect\r\n";

                    break;
            }

            statusLine = "HTTP/1.1 " + statusLine;

            return statusLine;
        }
    }
}
