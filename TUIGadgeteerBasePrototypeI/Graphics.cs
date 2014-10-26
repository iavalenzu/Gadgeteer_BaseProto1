using System;
using System.IO;
using System.Collections;
using System.Threading;

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
    class Graphics
    {

        private Gadgeteer.Modules.GHIElectronics.Display_T35 display;

        public Color[] colors = new Color[4] { Colors.Green, Colors.Orange, Colors.Red, Colors.Blue };


        public Graphics(Gadgeteer.Modules.GHIElectronics.Display_T35 _display) { 
            display = _display;
        }


        public Bitmap createFrecuenciesChart(int[] frequencies)
        {

            Bitmap chart;
            int width;
            int height;
            int total;
            int radius;

            height = (int)display.Height;
            width = (int)display.Width;
            radius = 90;
            total = 0;

            for (int i = 0; i < frequencies.Length; i++)
            {
                total += frequencies[i];
            }

            int[] percents = new int[frequencies.Length];
            for (int i = 0; i < percents.Length; i++)
            {
                percents[i] = (int)((frequencies[i] * 360) / total);
            }

            chart = new Bitmap(width, height);
            chart.DrawRectangle(Colors.White, 1, 0, 0, width, height, 0, 0, Colors.White, 0, 0, Colors.White, 0, 0, Bitmap.OpacityOpaque);



            int ini = 0;
            for (int i = 0; i < percents.Length; i++)
            {
                drawSection(chart, 0, radius, ini, ini + percents[i], (int)(width / 2), (int)(height / 2), colors[i]);
                ini += percents[i];
            }

            return chart;
        }

        public Bitmap createFrecuenciesChart2(int[] frequencies)
        {

            Bitmap chart;
            int width;
            int height;
            int[] frecuencies_height = new int[frequencies.Length];

            height = (int)display.Height;
            width = (int)display.Width;

            int bar_width = 50;

            int top_margin = 20;
            int bottom_margin = 20;
            int left_margin = 20;
            int right_margin = 20;

            int grid_height = height - top_margin - bottom_margin;
            int grid_width = width - left_margin - right_margin;

            int grid_top_padding = 20;
            int grid_left_padding = 20;

            /*
             * Se obtiene la mayor de las frecuencias
             */

            int max_frecuency = 0;
            for (int i = 0; i < frequencies.Length; i++)
            {
                if (frequencies[i] > max_frecuency)
                {
                    max_frecuency = frequencies[i];
                }
            }

            for (int i = 0; i < frecuencies_height.Length; i++)
            {
                frecuencies_height[i] = (frequencies[i] * (grid_height - grid_top_padding)) / max_frecuency;
            }

            chart = new Bitmap(width, height);
            chart.DrawRectangle(Colors.White, 1, 0, 0, width, height, 0, 0, Colors.White, 0, 0, Colors.White, 0, 0, Bitmap.OpacityOpaque);
            chart.DrawRectangle(Colors.Black, 1, left_margin, top_margin, grid_width, grid_height, 0, 0, Colors.White, 0, 0, Colors.White, 0, 0, Bitmap.OpacityOpaque);

            Color[] colors = new Color[4] { Colors.Green, Colors.Orange, Colors.Red, Colors.Blue };

            for (int i = 0; i < frecuencies_height.Length; i++)
            {
                chart.DrawRectangle(colors[i % colors.Length], 1, i * (bar_width + 10) + left_margin + grid_left_padding, height - bottom_margin - frecuencies_height[i], bar_width, frecuencies_height[i], 0, 0, colors[i % colors.Length], 0, 0, colors[i % colors.Length], 0, 0, Bitmap.OpacityOpaque);
            }

            return chart;
        }

        void drawSection(Bitmap chart, int radius_ini, int radius_end, int degrees_ini, int degrees_end, int center_x, int center_y, Color color)
        {

            for (int i = radius_ini; i < radius_end; i += 1)
            {
                drawArc2(chart, degrees_ini, degrees_end, color, center_x, center_y, i);
            }

        }

        void drawArc2(Bitmap chart, int degrees_ini, int degrees_end, Color color, int center_x, int center_y, int radius)
        {
            // Standard Midpoint Circle algorithm
            int p = (5 - radius * 4) / 4;
            int x = 0;
            int y = radius;

            drawCirclePoints(chart, center_x, center_y, x, y, degrees_ini, degrees_end, color);

            while (x <= y)
            {
                x++;
                if (p < 0)
                {
                    p += 2 * x + 1;
                }
                else
                {
                    y--;
                    p += 2 * (x - y) + 1;
                }
                drawCirclePoints(chart, center_x, center_y, x, y, degrees_ini, degrees_end, color);
            }
        }


        void drawCirclePoints(Bitmap chart, int centerX, int centerY, int x, int y, int startAngle, int endAngle, Color color)
        {

            // Calculate the angle the current point makes with the circle center
            int angle = (int)(System.Math.Atan2(y, x) * (180 / System.Math.PI));


            // draw the circle points as long as they lie in the range specified
            if (x < y)
            {
                // draw point in range 0 to 45 degrees
                if (90 - angle >= startAngle && 90 - angle <= endAngle)
                {
                    chart.SetPixel(centerX - y, centerY - x, color);
                }

                // draw point in range 45 to 90 degrees
                if (angle >= startAngle && angle <= endAngle)
                {
                    chart.SetPixel(centerX - x, centerY - y, color);
                }

                // draw point in range 90 to 135 degrees
                if (180 - angle >= startAngle && 180 - angle <= endAngle)
                {
                    chart.SetPixel(centerX + x, centerY - y, color);
                }

                // draw point in range 135 to 180 degrees
                if (angle + 90 >= startAngle && angle + 90 <= endAngle)
                {
                    chart.SetPixel(centerX + y, centerY - x, color);
                }

                // draw point in range 180 to 225 degrees
                if (270 - angle >= startAngle && 270 - angle <= endAngle)
                {
                    chart.SetPixel(centerX + y, centerY + x, color);
                }

                // draw point in range 225 to 270 degrees
                if (angle + 180 >= startAngle && angle + 180 <= endAngle)
                {
                    chart.SetPixel(centerX + x, centerY + y, color);
                }

                // draw point in range 270 to 315 degrees
                if (360 - angle >= startAngle && 360 - angle <= endAngle)
                {
                    chart.SetPixel(centerX - x, centerY + y, color);
                }

                // draw point in range 315 to 360 degrees
                if (angle + 270 >= startAngle && angle + 270 <= endAngle)
                {
                    chart.SetPixel(centerX - y, centerY + x, color);
                }
            }
        }
/*
        void drawArc(Bitmap chart, double radians_ini, double radians_end, Color color, int center_x, int center_y, double radius)
        {

            int x;
            int y;

            int ini = (int)(radians_ini / interval);
            int end = (int)(radians_end / interval);

            for (int i = ini; i < end; i++)
            {
                x = center_x + (int)(radius * cos_values[i]);
                y = center_y - (int)(radius * sin_values[i]);

                chart.SetPixel(x, y, color);
            }

        }
*/
        public void saveChart(GT.StorageDevice storage, Bitmap chart, String pathGraphName)
        {

            byte[] bytes = new byte[chart.Width * chart.Height * 3 + 54];

            Util.BitmapToBMPFile(chart.GetBitmap(), chart.Width, chart.Height, bytes);

            GT.Picture picture = new GT.Picture(bytes, GT.Picture.PictureEncoding.BMP);

            showMsgOnScreen("Guardando el grafico en la SD en: " + pathGraphName);
            Debug.Print("Guardando el grafico en la SD en: " + pathGraphName);

            storage.WriteFile(pathGraphName, picture.PictureData);

        }

        public void showMsgOnScreen(String msg)
        {

            display.SimpleGraphics.Clear();
            display.SimpleGraphics.DisplayTextInRectangle(
                            msg,
                            50, 50, 250, 200, GT.Color.Green, Resources.GetFont(Resources.FontResources.NinaB),
                            GTM.Module.DisplayModule.SimpleGraphicsInterface.TextAlign.Left,
                            GTM.Module.DisplayModule.SimpleGraphicsInterface.WordWrap.Wrap,
                            GTM.Module.DisplayModule.SimpleGraphicsInterface.Trimming.WordEllipsis,
                            GTM.Module.DisplayModule.SimpleGraphicsInterface.ScaleText.ScaleToFit);


        }

        public void showChart(GT.StorageDevice storage, String pathGraphName)
        {

            Debug.Print("Leo el grafico desde la SD y lo muestro en pantalla...");

            showMsgOnScreen("Leo el grafico desde la SD...");

            Bitmap bitmap = storage.LoadBitmap(pathGraphName, Bitmap.BitmapImageType.Bmp);
            display.SimpleGraphics.Clear();
            display.SimpleGraphics.DisplayImage(bitmap, 0, 0);
        
        
        }


    }
}
