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



namespace PrototipoPelotaTerapeuticaBase
{
    public partial class Program
    {

        private uint _axisX = 20;
        private uint _axisY = 20;

        private bool _modeUp = true;
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

            
            /*
             * Algoritmo para generar el hmac necesarion para usar el api de twitter
             **/ 
            KeyedHashAlgorithm kha = new KeyedHashAlgorithm(KeyedHashAlgorithmType.HMACSHA1, "key");

            byte[] res = kha.ComputeHash(Encoding.UTF8.GetBytes("HOlassss"));

            Debug.Print(Encoding.UTF8.GetChars(res).ToString());




            POSTContent post_content;
            String twitter_url;
            String post_string;

            twitter_url = "";
            post_string = "";
            post_content = POSTContent.CreateTextBasedContent(post_string);

            HttpRequest request = HttpHelper.CreateHttpPostRequest(twitter_url, post_content, null);

            request.ResponseReceived += new HttpRequest.ResponseHandler(ResponseReceived);
            request.AddHeaderField("", "");
            request.SendRequest();

        }

        void ResponseReceived(HttpRequest sender, HttpResponse response) 
        {

            Debug.Print("Response received handler!!!");

            if (response.StatusCode != "200") 
            {
                Debug.Print("Ha ocurrido un error: " + response.StatusCode);
            }


        }


        void ListNetworkInterfaces()
        {

            var settings = ethernet.NetworkSettings;

            Debug.Print("------------------------------------------------");
            Debug.Print("MAC: " + ByteExtensions.ToHexString(settings.PhysicalAddress, "-"));
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
            proceso();
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
