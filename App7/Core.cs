using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Nio;

namespace App7
{
   
    class Core
    {
        public static bool IsValid(string message)
        {
            Dictionary<char, int> znaky = new Dictionary<char, int>();
            foreach (var znak in message)
                if (znaky.Keys.Contains(znak))
                    znaky[znak]++;
                else
                    znaky.Add(znak, 1);
            int normalni_znaky = 0;
            int ostatni = 0;
            foreach(var neco in znaky)
            {
                if (neco.Key > 31 && neco.Key < 127)
                    normalni_znaky += neco.Value;
                else
                    ostatni += neco.Value;
            }
            if (ostatni >= (normalni_znaky + ostatni) / 10.0)
                return false;
            return true;
        }
    }
    class Steganografie
    {
        public static Bitmap SchovejText(string text, Bitmap bmp)
        {
            bmp = bmp.Copy(Bitmap.Config.Argb8888, true);
            
            bool vyplnNulami = false;

            // index znaku v textu ktery chceme schovat
            int indexZnaku = 0;

            // hodnota znaku
            int hodnotaZnaku = text[0];

            int pocetNul = 0;

            // R = 0, G = 1 ; B = 2;
            int barvaPixelu = 0;

            int cisloPixelu = 0;

            int R;
            int G;
            int B;
            bool konec = false;
            // for pres vsechny radky bitmapy
            for (int i = 0; i < bmp.Height; i++)
            {
                // projde vsechny sloupce bitmapy
                for (int j = 0; j < bmp.Width; j++)
                {
                    // aktualni pixel
                   
                    var pixel = bmp.GetPixel(j, i);
                    

                     R = (pixel >> 16) & 0xff;
                     G = (pixel >> 8) & 0xff;
                     B = pixel & 0xff;
                    
                    // smazani posledniho bitu (LSB)
                    R = R - R % 2;
                    G = G - G % 2;
                    B = B - B % 2;

                    //projde vsechny tri barvy v pixelu (R,G,B)
                    for (int n = 0; n < 3; n++)
                    {
                        // novych 8 bitu
                        if (cisloPixelu % 8 == 0)
                        {
                            // posledni byte plnim nulamy 
                           if (vyplnNulami && pocetNul == 8)
                            {
                                if ((cisloPixelu - 1) % 3 < 2) // mozna by to šlo smazat ?  - V žádném případě NEmazat!!!
                                {
                                    bmp.SetPixel(j, i, Color.Argb(255,R, G, B));
                                }
                            }
                            if (indexZnaku >= text.Length) {
                                vyplnNulami = true;
                                if(konec)
                                    return bmp;
                                konec = true;
                            }
                               
                            else
                                hodnotaZnaku = text[indexZnaku++];
                        }
                        switch (cisloPixelu % 3)
                        {
                            case 0:
                                if (!vyplnNulami)
                                {
                                    R += hodnotaZnaku % 2;
                                    hodnotaZnaku /= 2;
                                }
                                break;
                            case 1:
                                if (!vyplnNulami)
                                {
                                    G += hodnotaZnaku % 2;
                                    hodnotaZnaku /= 2;
                                }
                                break;
                            case 2:
                                if (!vyplnNulami)
                                {
                                    B += hodnotaZnaku % 2;
                                    hodnotaZnaku /= 2;
                                }
                                var c = Color.Argb(255,R,G,B);
                                bmp.SetPixel(j, i, c);
                                break;
                        }
                        cisloPixelu++;
                        if (vyplnNulami)
                            pocetNul++;
                    }

                }
            }
            return bmp;
        }
        public static string ZiskejText(Bitmap bmp)
        {
            int pixelIndex = 0;
            int hodnotaZnaku = 0;
            int R, G, B;
            StringBuilder sb = new StringBuilder();
            // for pres vsechny radky bitmapy
            for (int i = 0; i < bmp.Height; i++)
            {
                // projde vsechny sloupce bitmapy
                for (int j = 0; j < bmp.Width; j++)
                {
                    var pixel = bmp.GetPixel(j, i);
                    R = (pixel >> 16) & 0xff;
                    G = (pixel >> 8) & 0xff;
                    B = pixel & 0xff;

                    for (int n = 0; n < 3; n++)
                    {
                        switch (pixelIndex % 3)
                        {
                            case 0: hodnotaZnaku = hodnotaZnaku * 2 + R % 2; break;
                            case 1: hodnotaZnaku = hodnotaZnaku * 2 + G % 2; break;
                            case 2: hodnotaZnaku = hodnotaZnaku * 2 + B % 2; break;
                        }
                        pixelIndex++;
                        if (pixelIndex % 8 == 0)
                        {
                            hodnotaZnaku = PrevratBity(hodnotaZnaku);

                            if (hodnotaZnaku == 0)
                                return sb.ToString();

                            sb.Append((char)hodnotaZnaku);

                            hodnotaZnaku = 0;
                        }
                    }
                }
            }
            return sb.ToString();
        }

        private static int PrevratBity(int hodnotaZnaku)
        {
            int result = 0;
            for (int i = 0; i < 8; i++)
            {
                result = result * 2 + hodnotaZnaku % 2;
                hodnotaZnaku /= 2;
            }
            return result;
        }

    }
}