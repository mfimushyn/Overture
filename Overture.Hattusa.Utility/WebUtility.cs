using System.IO;
using System.Net;

namespace Overture.Hattusa.Utility
{

    /// <summary>
    /// Provides common utilities methods for web.
    /// </summary>
    public static class WebUtility
    {

        #region Methods

        public static string GetPage( string url )
        {
            HttpWebRequest webrequest;
            WebResponse response;
            StreamReader stream;
            string result;

            webrequest = ( HttpWebRequest ) HttpWebRequest.Create( url );
            webrequest.Method = "GET";
            webrequest.ContentLength = 0;

            stream = null;
            response = null;

            try
            {
                response = webrequest.GetResponse();
                stream = new StreamReader( response.GetResponseStream() );
                result = stream.ReadToEnd();

                return result;
            }
            finally
            {
                if ( stream != null )
                {
                    stream.Close();
                }
                if ( response != null )
                {
                    response.Close();
                }
            }

        }
        #endregion

    }
}
