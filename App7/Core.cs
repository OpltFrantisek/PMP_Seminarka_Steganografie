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
    public class AndroidBmpUtil
    {
        private readonly int BMP_WIDTH_OF_TIMES = 4;
        private readonly int BYTE_PER_PIXEL = 3;

        /**
         * Android Bitmap Object to Window's v3 24bit Bmp Format File
         * @param orgBitmap
         * @param filePath
         * @return file saved result
         */

        public bool Save(Bitmap orgBitmap, string filePath)
        {
            if (orgBitmap == null)
            {
                return false;
            }

            if (filePath == null)
            {
                return false;
            }

            var isSaveSuccess = true;

            //image size
            var width = orgBitmap.Width;
            var height = orgBitmap.Height;

            //image dummy data size
            //reason : bmp file's width equals 4's multiple
            var dummySize = 0;
            byte[] dummyBytesPerRow = null;
            var hasDummy = false;
            if (isBmpWidth4Times(width))
            {
                hasDummy = true;
                dummySize = BMP_WIDTH_OF_TIMES - width % BMP_WIDTH_OF_TIMES;
                dummyBytesPerRow = new byte[dummySize * BYTE_PER_PIXEL];
                for (var i = 0; i < dummyBytesPerRow.Length; i++)
                {
                    dummyBytesPerRow[i] = 0xFF;
                }
            }

            var pixels = new int[width * height];
            var imageSize = pixels.Length * BYTE_PER_PIXEL + height * dummySize * BYTE_PER_PIXEL;
            var imageDataOffset = 0x36;
            var fileSize = imageSize + imageDataOffset;

            //Android Bitmap Image Data
            orgBitmap.GetPixels(pixels, 0, width, 0, 0, width, height);

            //ByteArrayOutputStream baos = new ByteArrayOutputStream(fileSize);
            var buffer = ByteBuffer.Allocate(fileSize);

            try
            {
                /**
                 * BITMAP FILE HEADER Write Start
                 **/
                buffer.Put(0x42);
                buffer.Put(0x4D);

                //size
                buffer.Put(WriteInt(fileSize));

                //reserved
                buffer.Put(WriteShort(0));
                buffer.Put(WriteShort(0));

                //image data start offset
                buffer.Put(WriteInt(imageDataOffset));

                /** BITMAP FILE HEADER Write End */

                //*******************************************

                /** BITMAP INFO HEADER Write Start */
                //size
                buffer.Put(WriteInt(0x28));

                //width, height
                buffer.Put(WriteInt(width));
                buffer.Put(WriteInt(height));

                //planes
                buffer.Put(WriteShort(1));

                //bit count
                buffer.Put(WriteShort(24));

                //bit compression
                buffer.Put(WriteInt(0));

                //image data size
                buffer.Put(WriteInt(imageSize));

                //horizontal resolution in pixels per meter
                buffer.Put(WriteInt(0));

                //vertical resolution in pixels per meter (unreliable)
                buffer.Put(WriteInt(0));

                buffer.Put(WriteInt(0));

                buffer.Put(WriteInt(0));

                /** BITMAP INFO HEADER Write End */

                var row = height;
                var col = width;
                var startPosition = 0;
                var endPosition = 0;

                while (row > 0)
                {
                    startPosition = (row - 1) * col;
                    endPosition = row * col;

                    for (var i = startPosition; i < endPosition; i++)
                    {
                        buffer.Put(Write24BitForPixcel(pixels[i]));

                        if (hasDummy)
                        {
                            if (isBitmapWidthLastPixcel(width, i))
                            {
                                buffer.Put(dummyBytesPerRow);
                            }
                        }
                    }
                    row--;
                }

                var fos = new FileOutputStream(filePath);
                //fos.Write(new byte[buffer.Remaining()]); 
                byte[] bmpData = new byte[buffer.Position()];
                buffer.Position(0); //or buffer.Flip();
                buffer.Get(bmpData, 0, buffer.Remaining());

                fos.Write(bmpData);
                fos.Close();
            }
            catch (Exception e1)
            {
                isSaveSuccess = false;
            }
            finally
            {
            }

            return isSaveSuccess;
        }

        /**
         * Is last pixel in Android Bitmap width  
         * @param width
         * @param i
         * @return
         */

        private bool isBitmapWidthLastPixcel(int width, int i)
        {
            return i > 0 && i % (width - 1) == 0;
        }

        /**
         * BMP file is a multiples of 4?
         * @param width
         * @return
         */

        private bool isBmpWidth4Times(int width)
        {
            return width % BMP_WIDTH_OF_TIMES > 0;
        }

        /**
         * Write integer to little-endian 
         * @param value
         * @return
         * @throws IOException
         */

        private byte[] WriteInt(int value)
        {
            var b = new byte[4];

            b[0] = (byte)(value & 0x000000FF);
            b[1] = (byte)((value & 0x0000FF00) >> 8);
            b[2] = (byte)((value & 0x00FF0000) >> 16);
            b[3] = (byte)((value & 0xFF000000) >> 24);

            return b;
        }

        /**
         * Write integer pixel to little-endian byte array
         * @param value
         * @return
         * @throws IOException
         */

        private byte[] Write24BitForPixcel(int value)
        {
            var
                b = new byte[3];

            b[0] = (byte)(value & 0x000000FF);
            b[1] = (byte)((value & 0x0000FF00) >> 8);
            b[2] = (byte)((value & 0x00FF0000) >> 16);

            return b;
        }

        /**
         * Write short to little-endian byte array
         * @param value
         * @return
         * @throws IOException
         */

        private byte[] WriteShort(short value)
        {
            var
                b = new byte[2];

            b[0] = (byte)(value & 0x00FF);
            b[1] = (byte)((value & 0xFF00) >> 8);

            return b;
        }
    }
    class Core
    {

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
                                if ((cisloPixelu - 1) % 3 < 2) // mozna by to šlo smazat ? 
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