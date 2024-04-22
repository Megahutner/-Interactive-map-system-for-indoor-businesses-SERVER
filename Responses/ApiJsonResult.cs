using System;
using System.Collections.Generic;
using System.Text;

namespace UWEServer.Responses
{
    public class ApiJsonResult
    {

        /// <summary>
        /// 
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public object data { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public object error { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }


        public ApiJsonResult()
        {
            code = 200;
            message = string.Empty;
        }
    }
}
