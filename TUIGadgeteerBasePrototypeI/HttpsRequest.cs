using System;
using System.Text;
using Microsoft.SPOT;
using Microsoft.SPOT;
using Microsoft.SPOT.Net.Security;


using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace TUIGadgeteerBasePrototypeI
{
    class HttpsRequest
    {

        public HttpsRequest()
        {

                String server_name = "https://api.twitter.com";
                int port = 443;

                IPHostEntry host_entry = Dns.GetHostEntry(server_name);

                IPAddress[] addresses = host_entry.AddressList;

                IPEndPoint server_endpoint = new IPEndPoint(addresses[0], port);

                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                socket.Connect(server_endpoint);

                SslStream ssl_stream = new SslStream(socket);

            try
            {

                ssl_stream.AuthenticateAsClient(server_name);


                // Encode a test message into a byte array. 
                // Signal the end of the message using the "<EOF>".
                byte[] messsage = Encoding.UTF8.GetBytes("Hello from the client.<EOF>");
                // Send hello message to the server. 
                ssl_stream.Write(messsage, 0, messsage.Length);
                ssl_stream.Flush();
                // Read message from the server. 
                string serverMessage = ReadMessage(ssl_stream);

                Debug.Print("Server says: " + serverMessage);

                // Close the client connection.
                ssl_stream.Close();
                socket.Close();
            
            } 
            catch (Exception e)
            {
                if (e.InnerException != null)
                {
                    Debug.Print("Inner exception: " + e.InnerException.Message);
                }

                ssl_stream.Close();
                socket.Close();

            }


        }

        static string ReadMessage(SslStream sslStream)
        {
            // Read the  message sent by the server. 
            // The end of the message is signaled using the 
            // "<EOF>" marker.
            byte[] buffer = new byte[2048];
            StringBuilder messageData = new StringBuilder();
            int bytes = -1;
            do
            {
                bytes = sslStream.Read(buffer, 0, buffer.Length);

                // Use Decoder class to convert from bytes to UTF8 
                // in case a character spans two buffers.
                Decoder decoder = Encoding.UTF8.GetDecoder();


                char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                decoder.GetChars(buffer, 0, bytes, chars, 0);

                messageData.Append(chars);
                // Check for EOF. 
                if (messageData.ToString().IndexOf("<EOF>") != -1)
                {
                    break;
                }
            } while (bytes != 0);

            return messageData.ToString();
        }


        /*
        Microsoft.SPOT.Net.Security.SslStream ssl = new SslStream(

            SslStream sslStream = new SslStream(
                client.GetStream(), 
                false, 
                new System.Net. RemoteCertificateValidationCallback (ValidateServerCertificate), 
                null
                );
            // The server name must match the name on the server certificate. 
            try 
            {
                sslStream.AuthenticateAsClient(serverName);
            } 
            catch (AuthenticationException e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
                if (e.InnerException != null)
                {
                    Console.WriteLine("Inner exception: {0}", e.InnerException.Message);
                }
                Console.WriteLine ("Authentication failed - closing the connection.");
                client.Close();
                return;
            }
            // Encode a test message into a byte array. 
            // Signal the end of the message using the "<EOF>".
            byte[] messsage = Encoding.UTF8.GetBytes("Hello from the client.<EOF>");
            // Send hello message to the server. 
            sslStream.Write(messsage);
            sslStream.Flush();
            // Read message from the server. 
            string serverMessage = ReadMessage(sslStream);
            Console.WriteLine("Server says: {0}", serverMessage);
        */
    }
}
