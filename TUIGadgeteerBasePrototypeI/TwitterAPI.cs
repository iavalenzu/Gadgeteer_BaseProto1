using System;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using System.Net;
using System.Net.Sockets;

using System.Collections;
 
using System.IO;
using System.Text;

using GHI.Premium.Net;
using GHI.Premium.System;

using Microsoft.SPOT;
using Microsoft.SPOT.Net.Security;

using Gadgeteer.Networking;

namespace TUIGadgeteerBasePrototypeI
{

    /**
    * Esta clase corresponde a un wrapper del API dado por Twitter, permite generar post
    */

    class TwitterAPI
    {
        private String http_uri = "";

        private Hashtable http_get = null;
        private Hashtable http_post = null;

        private String http_method = "";

        private String oauth_consumer_key = "";
        private String oauth_token = "";

        private String oauth_consumer_secret = "";
        private String oauth_token_secret = "";

        private String oauth_signature_method = "HMAC-SHA1";
        private String oauth_oauth_version = "1.0";


        public TwitterAPI(String _oauth_consumer_key,  String _oauth_token, String _oauth_consumer_secret, String _oauth_token_secret, String _http_method, String _http_uri, Hashtable _http_get, Hashtable _http_post) 
        {
            oauth_consumer_key = _oauth_consumer_key;
            oauth_token = _oauth_token;

            oauth_consumer_secret = _oauth_consumer_secret;
            oauth_token_secret = _oauth_token_secret;

            http_method = _http_method.ToUpper();
            http_uri = _http_uri;

            http_get = _http_get;
            http_post = _http_post;

        }

        public HttpRequest createSignedRequest() 
        {
            String oauth_nonce = getOauthNonce();
            String oauth_signature_method = getOauthSignatureMethod();
            String oauth_timestamp = getOauthTimestamp();
            String oauth_token = getOauthToken();
            String oauth_version = getOauthVersion();

            Debug.Print("OauthNonce: " + oauth_nonce);
            Debug.Print("OauthSignatureMethod: " + oauth_signature_method);
            Debug.Print("OauthTimestamp: " + oauth_timestamp);
            Debug.Print("OauthToken: " + oauth_token);
            Debug.Print("OauthVersion: " + oauth_version);

            /*
             * Se genera la cadena que luego se firmará
             */
            String signature_base_string = createSignatureBaseString(http_method, http_uri, http_get, http_post, oauth_nonce, oauth_timestamp, oauth_token, oauth_version);

            Debug.Print("SignatureBaseString: " + signature_base_string);


            /*
             * Se genera la firma a partir del signature_base_string
             */

            String signing_key = getSigningKey();

            Debug.Print("SigningKey: " + signing_key);


            String oauth_signature = getOauthSignature(signature_base_string, signing_key);

            Debug.Print("OauthSignature: " + oauth_signature);


            /*
             * Se crea el header de autorizacion
             */ 

            String authorization_header = createAuthorizationHeader(oauth_nonce, oauth_signature, oauth_signature_method, oauth_timestamp, oauth_token, oauth_version);

            Debug.Print("AuthorizationHeader: " + authorization_header);


            HttpRequest request;
            POSTContent post_content;
            String post_query;
            String get_query;

            post_query = createPostQuery(http_post);
            get_query = createGetQuery(http_get);

            Debug.Print("PostQuery: " + post_query);
            Debug.Print("GetQuery: " + get_query);


            if (http_method == "POST") {
                post_content = POSTContent.CreateTextBasedContent(post_query);
                request = HttpHelper.CreateHttpPostRequest(http_uri + get_query, post_content, null);
            }
            else if (http_method == "GET")
            {
                request = HttpHelper.CreateHttpGetRequest(http_uri + get_query);
            }
            else {
                Debug.Print("El metodo " + http_method + " no está soportado.");
                return null;
            }

            request.AddHeaderField("Authorization", authorization_header);

            Debug.Print("Request Url: " + request.URL);

            return request;

        }

        public String createPostQuery(Hashtable http_post) 
        {
            String query = "";

            if (http_post.Count > 0)
            {
                query += "?";

                foreach (DictionaryEntry entry in http_post)
                {
                    query += percent_encode((string)entry.Key) + "=" + percent_encode((string)entry.Value) + "&";
                }

                query = query.Substring(0, query.Length - 1);

            }

            return query;
        }

        public String createGetQuery(Hashtable http_get)
        {
            String query = "";

            if (http_get.Count > 0)
            {
                query += "?";

                foreach (DictionaryEntry entry in http_get)
                {
                    query += percent_encode((string)entry.Key) + "=" + percent_encode((string)entry.Value) + "&";
                }

                query = query.Substring(0, query.Length - 1);

            }

            return query;

        }

        public String getOauthSignatureMethod() 
        {
            return oauth_signature_method;
        }

        public String getOauthVersion() 
        {
            return oauth_oauth_version;
        }

        public String getOauthToken() 
        {
            return oauth_token;
        }

        public String getOauthTimestamp() 
        {
            return "1318622958";   
        }

        public String getOauthNonce() 
        {
            Random rnd;
            Byte[] random_bytes;

            rnd = new Random();
            random_bytes = new Byte[32];

            rnd.NextBytes(random_bytes);

            return Convert.ToBase64String(random_bytes);

            //return "kYjzVBB8Y0ZFabxSWbWovY3uYSQ2pTgmZeNu2VS4cg";

        }

        public String getOauthSignature(String _signature_base_string, String _signing_key)
        {
            byte[] message;
            byte[] key;

            message = Encoding.UTF8.GetBytes(_signature_base_string);
            key = Encoding.UTF8.GetBytes(_signing_key);

            byte[]hashed = HMACSHA1.hash(message, key);

            Debug.Print("HASH: " + HMACSHA1.arrayToHexString(hashed));

            return Convert.ToBase64String(hashed);

        }

        public String getSigningKey() 
        {
            String signing_key = "";

            signing_key += percent_encode(oauth_consumer_secret) + "&";
            signing_key += percent_encode(oauth_token_secret);

            return signing_key;
        }


        public static string RFC3986Encode(string s)
        {
            StringBuilder sb = new StringBuilder(s.Length * 2);
            byte[] arr = Encoding.UTF8.GetBytes(s);

            for (int i = 0; i < arr.Length; i++)
            {
                byte c = arr[i];

                if (c >= 0x41 && c <= 0x5A)//alpha
                    sb.Append((char)c);
                else if (c >= 0x61 && c <= 0x7A)//ALPHA
                    sb.Append((char)c);
                else if (c >= 0x30 && c <= 0x39)//123456789
                    sb.Append((char)c);
                else if (c == '-' || c == '.' || c == '_' || c == '~')
                    sb.Append((char)c);
                else
                {
                    sb.Append('%');
                    sb.Append(c.ToString("X"));
                }
            }
            return sb.ToString();
        }


        public String percent_encode(String value)
        {
            return RFC3986Encode(value);
        }


        public String createAuthorizationHeader(String _oauth_nonce, String _oauth_signature, String _oauth_signature_method, String _oauth_timestamp, String _oauth_token, String _oauth_version) 
        {
            /*
             * https://dev.twitter.com/oauth/overview/authorizing-requests
             */

            String authorization_header = "";

            authorization_header += "OAuth ";
            authorization_header += percent_encode("oauth_consumer_key") + "=\"" + percent_encode(oauth_consumer_key) + "\"" + ", ";
            authorization_header += percent_encode("oauth_nonce") + "=\"" + percent_encode(_oauth_nonce) + "\"" + ", ";
            authorization_header += percent_encode("oauth_signature") + "=\"" + percent_encode(_oauth_signature) + "\"" + ", ";
            authorization_header += percent_encode("oauth_signature_method") + "=\"" + percent_encode(_oauth_signature_method) + "\"" + ", ";
            authorization_header += percent_encode("oauth_timestamp") + "=\"" + percent_encode(_oauth_timestamp) + "\"" + ", ";
            authorization_header += percent_encode("oauth_token") + "=\"" + percent_encode(_oauth_token) + "\"" + ", ";
            authorization_header += percent_encode("oauth_version") + "=\"" + percent_encode(_oauth_version) + "\""; 

            return authorization_header;
        }

        public String getParametersString(Hashtable _http_get, Hashtable _http_post, String _oauth_nonce, String _oauth_timestamp, String _oauth_token, String _oauth_version) 
        {

            Hashtable parameters = new Hashtable();
            parameters.Add(percent_encode("oauth_consumer_key"), percent_encode(oauth_consumer_key));
            parameters.Add(percent_encode("oauth_nonce"), percent_encode(_oauth_nonce));
            parameters.Add(percent_encode("oauth_signature_method"), percent_encode(oauth_signature_method));
            parameters.Add(percent_encode("oauth_timestamp"), percent_encode(_oauth_timestamp));
            parameters.Add(percent_encode("oauth_token"), percent_encode(_oauth_token));
            parameters.Add(percent_encode("oauth_version"), percent_encode(_oauth_version));

            /*
             * Se recorre los valores get y post añadiendolos al hashtable
             */
            foreach (DictionaryEntry entry in _http_get)
            {
                parameters.Add(percent_encode((string)entry.Key), percent_encode((string)entry.Value));
            }

            foreach (DictionaryEntry entry in _http_post)
            {
                parameters.Add(percent_encode((string)entry.Key), percent_encode((string)entry.Value));
            }

            String[] ordered_keys = CollectionsUtilities.orderHashTableKeys(parameters);

            String parameter_string = "";

            for (int i = 0; i < ordered_keys.Length - 1; i++)
            {
                parameter_string += ordered_keys[i] + "=" + parameters[ordered_keys[i]] + "&";
            }

            parameter_string += ordered_keys[ordered_keys.Length - 1] + "=" + parameters[ordered_keys[ordered_keys.Length - 1]];

            return parameter_string;
        
        }

        public String createSignatureBaseString(String _http_method, String _http_url, Hashtable _http_get, Hashtable _http_post, String _oauth_nonce, String _oauth_timestamp, String _oauth_token, String _oauth_version) 
        {
            String base_string = "";
            String parameter_string; 

            parameter_string = getParametersString(_http_get, _http_post, _oauth_nonce, _oauth_timestamp, _oauth_token, _oauth_version);

            Debug.Print("ParameterString: " + parameter_string);

            base_string += percent_encode(_http_method) + "&";
            base_string += percent_encode(_http_url) + "&";
            base_string += percent_encode(parameter_string);

            return base_string;
        
        }



    }
}
