using System;
using System.IO;
using Microsoft.SPOT;

namespace TUIGadgeteerBasePrototypeI
{
    class DataManager
    {
        public DataManager() { 
        
        }

        static public int[] processData(String dataFile, string[] names, int index)
        {

            StreamReader reader;
            int[] frequencies;
            string line;

            frequencies = new int[names.Length];

            /*
             * Se inicializan las frecuencias
             */

            for (int i = 0; i < frequencies.Length; i++)
            {
                frequencies[i] = 0;
            }

            /*
             * Se busca en el archivo los valores dados en names para obtener las frecuencias de aparicion
             */

            reader = new StreamReader(dataFile);


            while ((line = reader.ReadLine()) != null)
            {
                /*
                 * Dividimos la linea 
                 */

                string[] parts = line.Split(',');

                if (index < parts.Length)
                {
                    string seed = parts[index];

                    for (int j = 0; j < names.Length; j++)
                    {
                        if (String.Compare(seed.ToUpper().Trim(), names[j].ToUpper().Trim()) == 0)
                        {
                            frequencies[j]++;
                            break;
                        }
                    }

                }

            }

            reader.Close();

            return frequencies;

        }



        static public String getCurrentDataFileName(String rootDirectory, Graphics graphics)
        {
            String currentFileName;
            String path;
            StreamReader currentReader;

            currentFileName = "";
            path = rootDirectory + @"\current.cfg";

            if (File.Exists(path))
            {

                currentReader = new StreamReader(path);

                string line = currentReader.ReadLine();

                if (line == String.Empty)
                {
                    currentReader.Close();
                    Debug.Print("Ha ocurrido un error, el nombre de archivo a procesar no existe");
                    graphics.showMsgOnScreen("Ha ocurrido un error, el nombre de archivo a procesar no existe");
                    return null;

                }
                else
                {

                    Debug.Print("El nombre del archivo a procesar es: " + line);
                    graphics.showMsgOnScreen("El nombre del archivo a procesar es: " + line);

                    currentFileName = line.Trim();
                    currentReader.Close();

                    if (!File.Exists(currentFileName))
                    {
                        Debug.Print("Ha ocurrido un error, el archivo a procesar no existe!!");
                        graphics.showMsgOnScreen("Ha ocurrido un error, el archivo a procesar no existe!!");
                        return null;
                    }

                    return currentFileName;
                }

            }
            else
            {

                /*
                 * El archivo no existe, lo creamos y guardamos el nombre del archivo con la data
                 */

                Debug.Print("Ha ocurrido un error, el archivo 'current.cfg' no existe!!");
                graphics.showMsgOnScreen("Ha ocurrido un error, el archivo 'current.cfg' no existe!!");
                return null;
            }


        }

        static public void createRandomDataFileName(String rootDirectory, int max_values){

            string[] faces = new string[7] { "SMILE_GREEN", "SMILE_YELLOW", "FROWN_YELLOW", "FROW_RED", "FROW_RED", "FROW_RED", "FROWN_YELLOW" };

            String currentFileName;
            String path;
            StreamReader currentReader;
            StreamWriter currentWriter;

            currentFileName = rootDirectory + @"\data\" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-f") + ".dat";

            Debug.Print(currentFileName);

            path = rootDirectory + @"\current.cfg";

            Debug.Print("Path: " + path);

            if (File.Exists(path))
            {

                Debug.Print("El archivo existe!!");

                currentReader = new StreamReader(path);

                string line = currentReader.ReadLine();

                if (line == String.Empty)
                {

                    Debug.Print("No hay nombre de archivo guardado, lo creamos con el valor: " + currentFileName);

                    currentReader.Close();

                    currentWriter = new StreamWriter(path, false);
                    currentWriter.WriteLine(currentFileName);
                    currentWriter.Close();

                    Debug.Print("Creamos el archivo: " + currentFileName);

                    currentWriter = new StreamWriter(currentFileName, false);

                    Debug.Print("Creamos el archivo aleatorio!!");

                    Random rnd = new Random();

                    for (int i = 0; i < max_values; i++)
                    {
                        string tmp = faces[rnd.Next(faces.Length)] + "," + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt");
                        Debug.Print(tmp);
                        currentWriter.WriteLine(tmp);
                    }


                    currentWriter.Close();


                }
                else
                {

                    Debug.Print("Hay nombre guardado: " + line);

                    currentFileName = line.Trim();
                    currentReader.Close();
                }


            }
            else
            {

                /*
                 * El archivo no existe, lo creamos y guardamos el nombre del archivo con la data
                 */

                Debug.Print("El archivo no existe, lo creamos con el valor: " + currentFileName);

                currentWriter = new StreamWriter(path, false);
                currentWriter.WriteLine(currentFileName);
                currentWriter.Close();

                Debug.Print("Creamos el archivo: " + currentFileName);

                currentWriter = new StreamWriter(currentFileName, false);
                currentWriter.WriteLine("INIT");
                currentWriter.Close();

            }

            Debug.Print("FileName: " + currentFileName);


        }

        static public void findDataFileName(String rootDirectory)
        {
            String currentFileName;
            String path;
            StreamReader currentReader;
            StreamWriter currentWriter;

            currentFileName = rootDirectory + @"\data\" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-f") + ".dat";

            Debug.Print(currentFileName);

            path = rootDirectory + @"\current.cfg";

            Debug.Print("Path: " + path);

            if (File.Exists(path))
            {

                Debug.Print("El archivo existe!!");

                currentReader = new StreamReader(path);

                string line = currentReader.ReadLine();

                if (line == String.Empty)
                {

                    Debug.Print("No hay nombre de archivo guardado, lo creamos con el valor: " + currentFileName);

                    currentReader.Close();

                    currentWriter = new StreamWriter(path, false);
                    currentWriter.WriteLine(currentFileName);
                    currentWriter.Close();

                    Debug.Print("Creamos el archivo: " + currentFileName);

                    currentWriter = new StreamWriter(currentFileName, false);
                    currentWriter.WriteLine("INIT");
                    currentWriter.Close();


                }
                else
                {

                    Debug.Print("Hay nombre guardado: " + line);

                    currentFileName = line.Trim();
                    currentReader.Close();
                }


            }
            else
            {

                /*
                 * El archivo no existe, lo creamos y guardamos el nombre del archivo con la data
                 */

                Debug.Print("El archivo no existe, lo creamos con el valor: " + currentFileName);

                currentWriter = new StreamWriter(path, false);
                currentWriter.WriteLine(currentFileName);
                currentWriter.Close();

                Debug.Print("Creamos el archivo: " + currentFileName);

                currentWriter = new StreamWriter(currentFileName, false);
                currentWriter.WriteLine("INIT");
                currentWriter.Close();

            }

            Debug.Print("FileName: " + currentFileName);

        }




    }
}
