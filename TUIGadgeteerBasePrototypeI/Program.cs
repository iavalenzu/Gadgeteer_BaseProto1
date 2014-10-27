using System;
using System.IO;
using System.Collections;
using System.Threading;
using System.Security.Cryptography;
using System.Text;


using GHI.Premium.System;

using GHIElectronics.Gadgeteer;

using Microsoft.SPOT;
using Microsoft.SPOT.IO;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Shapes;
using Microsoft.SPOT.Touch;


using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;

using Gadgeteer.Modules.GHIElectronics;
using Gadgeteer.Networking;



namespace TUIGadgeteerBasePrototypeI
{
    public partial class Program
    {

        private Window mainWindow;

        private string rootDirectory;

        private string pathGraphName = @"\images\" + "graph.bmp";

        private Graphics graphics;

        void ProgramStarted()
        {

            SetupDisplay();
            SetupEthernet();

            graphics = new Graphics(display);


            sdCard.SDCardMounted += new SDCard.SDCardMountedEventHandler(sdCard_SDCardMounted);

            Debug.Print("Program Started");
        }

        void OnNetworkDown(GTM.Module.NetworkModule sender, GTM.Module.NetworkModule.NetworkState state)
        {
            Debug.Print("Network down.");
        }

        void OnNetworkUp(GTM.Module.NetworkModule sender, GTM.Module.NetworkModule.NetworkState state)
        {
            Debug.Print("Network up.");

            ListNetworkInterfaces();

            String _oauth_consumer_key = "B7Iv0UO3POq9Wmq8wqTBRJU4P";
            String _oauth_token = "2263254559-bf9KdsZIXFLRftXVtzcHFaFhrCyXC2DnR49SMJ2";
            String _oauth_consumer_secret = "ZX3XtPezAmKPDg3JoHsxNgbK0ChzokkkaQ7Ub05docptodFwWW";
            String _oauth_token_secret = "GiDBFi7hpnHpsPPvfOIzo9WEBEM9PBrjp8iJMeQghLQCD";

            String http_method = "POST";
            String http_uri = "http://api.twitter.com/1.1/statuses/update.json";

            Hashtable http_get = new Hashtable();
            //http_get.Add("include_entities", "true");

            Hashtable http_post = new Hashtable();
            http_post.Add("status", "Hello Ladies + Gentlemen, a signed OAuth request!");

            TwitterAPI twitter = new TwitterAPI(_oauth_consumer_key, _oauth_token, _oauth_consumer_secret, _oauth_token_secret, http_method, http_uri, http_get, http_post);

            HttpRequest request = twitter.createSignedRequest();
            request.ResponseReceived += new HttpRequest.ResponseHandler(ResponseReceived);
            request.SendRequest();

        }

        void ResponseReceived(HttpRequest sender, HttpResponse response) 
        {

            Debug.Print("Response received handler!!!");

            if (response.StatusCode != "200")
            {
                Debug.Print("Ha ocurrido un error: " + response.StatusCode);
            }
            else 
            {
                StreamReader sr = new StreamReader(response.Stream);
                String content;

                if (sr == null) 
                {
                    Debug.Print("El streamreader es NULO!!");
                    return;
                }

                content = sr.ReadToEnd();

                if (content == null) 
                {
                    Debug.Print("El content es NULO!!");
                    return;
                }

                Debug.Print(content);
            }


        }


        void ListNetworkInterfaces()
        {

            var settings = ethernet.NetworkSettings;

            Debug.Print("------------------------------------------------");
            Debug.Print("MAC: " + Utilities.ToHexString(settings.PhysicalAddress, "-"));
            Debug.Print("IP Address:   " + settings.IPAddress);
            Debug.Print("DHCP Enabled: " + settings.IsDhcpEnabled);
            Debug.Print("Subnet Mask:  " + settings.SubnetMask);
            Debug.Print("Gateway:      " + settings.GatewayAddress);
            Debug.Print("------------------------------------------------");
        }

        void SetupEthernet()
        {

            if (!ethernet.IsNetworkConnected) 
            {
                Debug.Print("El cable de red no esta conectado!!");
                return;
            }


            ethernet.UseDHCP();
            //ethernet.UseStaticIP(
            //    "192.168.1.222",
            //    "255.255.254.0",
            //    "192.168.1.1");

            ethernet.NetworkUp += OnNetworkUp;
            ethernet.NetworkDown += OnNetworkDown;



        }

       

        bool VerifySDCard()
        {
            if (!sdCard.IsCardInserted || !sdCard.IsCardMounted)
            {
                graphics.showMsgOnScreen("Inserta la tarjeta SD!!");

                System.Threading.Thread.Sleep(2000);
                display.SimpleGraphics.ClearNoRedraw();

                /*
                 * Se monta la tarjeta SD
                 */ 

                sdCard.MountSDCard();

                return false;
            }
            else {

                //led.TurnOff();
                return true;
            }

        }

        void SetupDisplay()
        {
            mainWindow = display.WPFWindow;
            mainWindow.TouchDown += new Microsoft.SPOT.Input.TouchEventHandler(TouchDown);

        }

        void TouchDown(object sender, Microsoft.SPOT.Input.TouchEventArgs e)
        {
            //proceso();
            Debug.Print("TouchDown!!");



/*
            POSTContent post_content;
            String twitter_url;
            String post_string;

            twitter_url = "http://www.google.cl";
            post_string = "";
            //post_content = POSTContent.CreateTextBasedContent(post_string);
            post_content = new POSTContent();

            //HttpRequest request = HttpHelper.CreateHttpPostRequest(twitter_url, post_content, null);
            HttpRequest request = HttpHelper.CreateHttpGetRequest(twitter_url);

            request.ResponseReceived += new HttpRequest.ResponseHandler(ResponseReceived);
            //request.AddHeaderField("", "");
            request.SendRequest();
*/

        }

        void proceso() 
        {
            try
            {

                if (VerifySDCard())
                {

                    GT.StorageDevice storage = sdCard.GetStorageDevice();

                    rootDirectory = storage.RootDirectory;

                    String fileName = DataManager.getCurrentDataFileName(rootDirectory, graphics);

                    if (fileName == null)
                    {
                        DataManager.createRandomDataFileName(rootDirectory, 100);
                    }
                    else
                    {
                        Debug.Print("Procesando: " + fileName);
                        graphics.showMsgOnScreen("Procesando: " + fileName);

                        /*
                         * Se procesa el archivo obtenido y se obtiene la data a graficar
                         */

                        int[] frequencies;

                        string[] names = new string[4] { "SMILE_GREEN", "SMILE_YELLOW", "FROWN_YELLOW", "FROW_RED" };

                        frequencies = DataManager.processData(fileName, names, 0);

                        Debug.Print("Frecuencias!!");
                        for (int i = 0; i < frequencies.Length; i++)
                        {
                            Debug.Print(names[i] + ": " + frequencies[i]);
                        }


                        Bitmap chart = graphics.createFrecuenciesChart2(frequencies);

                        Debug.Print("Creamos un grafico a partir de las frecuencias");

                        /*
                         * Se guarda el bitmap en la SD
                         */

                        graphics.saveChart(storage, chart, pathGraphName);

                        if (File.Exists(rootDirectory + pathGraphName))
                        {
                            /*
                             * Leo el archivo de la SD y lo muestro en la pantalla
                             */

                            graphics.showChart(storage, pathGraphName);


                            /*
                             * Subo el grafico a Twitter
                             */


                        }
                        else
                        {

                            Debug.Print("Hubo un error al guardar el grafico en la SD, no es posible leer el archivo...");
                            graphics.showMsgOnScreen("Hubo un error al guardar el grafico en la SD, no es posible leer el archivo...");

                        }

                    }

                }

            }
            catch (Exception ex)
            {

                Debug.Print("Message: " + ex.Message + "  Inner Exception: " + ex.InnerException);
            }
        
        }

        void sdCard_SDCardMounted(SDCard sender, GT.StorageDevice SDCard)
        {
            Debug.Print("SD card has been successfully mounted.");
        }



    }
}
