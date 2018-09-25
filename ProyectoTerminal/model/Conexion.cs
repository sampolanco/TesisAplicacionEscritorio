using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectoTerminal2.controller
{
    class Conexion
    {
        
        public bool peticionPost(string url, string body)
        {
            try
            {
                // Create a request using a URL that can receive a post.   
                // WebRequest request = WebRequest.Create("http://localhost:3000" + url);
                WebRequest request = WebRequest.Create("http://servidortesisv3-samyckl.rhcloud.com" + url);
                // Set the Method property of the request to POST.  
                request.Method = "POST";
                // Create POST data and convert it to a byte array.  
                // string postData = "email=email4@hotmail.com&password=prueba";
                string postData = body;
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                // Set the ContentType property of the WebRequest.  
                request.ContentType = "application/x-www-form-urlencoded";
                // Set the ContentLength property of the WebRequest.  
                request.ContentLength = byteArray.Length;
                // Get the request stream.  
                Stream dataStream = request.GetRequestStream();
                // Write the data to the request stream.  
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.  
                dataStream.Close();
                // Get the response.  
                WebResponse response = request.GetResponse();
                // Get the stream containing content returned by the server.  
                dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.  
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.  
                string responseFromServer = reader.ReadToEnd();
                // Clean up the streams.  
                reader.Close();
                dataStream.Close();
                response.Close();
                if (responseFromServer == "true")
                    return true;
                else
                    return false;

            }
            catch (Exception x)
            {
                return false;
            }
        }
    }
}
