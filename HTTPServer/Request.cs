using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>

        string[] firstLine;

        public bool ParseRequest()
        {
            // throw new NotImplementedException();

            bool result = true;

            //TODO: parse the receivedRequest using the \r\n delimeter   
            requestLines = requestString.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (requestLines.Length < 3)
            {
                return false;
            }

            // Parse Request line
            // firstLine (the request line) = "Method URI HTTP/Version"
            firstLine = requestLines[0].Split(' ');
            result &= ParseRequestLine();

            // Validate blank line exists
            result &= ValidateBlankLine();

            // Load header lines into HeaderLines dictionary
            result &= LoadHeaderLines();

            return result;
        }

        private bool ParseRequestLine()
        {
            // throw new NotImplementedException();

            switch (firstLine[0].ToLower())
            {
                case "get":
                    method = RequestMethod.GET;

                    break;
                case "post":
                    method = RequestMethod.POST;

                    break;
                case "head":
                    method = RequestMethod.HEAD;

                    break;
            }

            relativeURI = firstLine[1];

            return ValidateIsURI(firstLine[1]);
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
            // throw new NotImplementedException();

            headerLines = new Dictionary<string, string>();

            // starting from 1 because index 0 is for request line
            // i < requestLines.Length - 2 because last two lines are blank line and content
            for (int i = 1; i < requestLines.Length - 2; i++)
            {
                string[] line = requestLines[i].Split(new string[] { ": " }, StringSplitOptions.None);
                headerLines.Add(line[0], line[1]);
            }

            return true;
        }

        private bool ValidateBlankLine()
        {
            // throw new NotImplementedException();

            // requestLines.Length - 2 = index of Blank line
            if (requestLines[(requestLines.Length - 2)] == string.Empty)
            {
                return true;
            }

            return false;
        }
    }
}
