﻿using System;
using System.Drawing;
using System.IO;
using IronOcr;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

namespace TiqSoft.ScreenAssistant.ScreenInfoRecognition
{
    public static class WeaponTypeScreenRecognizer
    {
        
        private static readonly Random Rnd = new Random();
        

        public static bool IsFirstWeaponActive()
        {
            var image1 = ScreenCapture.CaptureScreenRelatively(82f, 84f, 95.5f, 96);
            var image2 = ScreenCapture.CaptureScreenRelatively(90f, 92f, 95.5f, 96);

            //image1.SaveTestImage();
            //image2.SaveTestImage();

            return image1.GetAverageBrightness() > image2.GetAverageBrightness();
        }

        private static float GetAverageBrightness(this Image img)
        {
            var brightnessTotal = 0f;

            if (img.Height == 0 || img.Width == 0)
            {
                return 0;
            }
            using (var db = new DirectBitmap(img.Width, img.Height))
            {
                using (var graphics = Graphics.FromImage(db.Bitmap))
                {
                    graphics.DrawImage(img, Point.Empty);
                }

                for (var i = 0; i < img.Width; i++)
                {
                    for (var j = 0; j < img.Height; j++)
                    {
                        var currentPixel = db.GetPixel(i, j);
                        brightnessTotal = currentPixel.GetBrightness();
                    }
                }
            }

            return brightnessTotal / (img.Width * img.Height);
        }

        private static Image GetWeapon1Image()
        {
            var image = ScreenCapture.CaptureScreenRelatively(81f, 87f, 95.5f, 98);//77, 97, 95, 99);
            return image;
        }

        private static Image GetWeapon2Image()
        {
            var image = ScreenCapture.CaptureScreenRelatively(88.3f, 95f, 95.5f, 98);//77, 97, 95, 99);
            return image;
        }

        private static void SaveTestImage(this Image image)
        {
            var fileName = $"{Rnd.Next(0, 999999999)}.tif";
            Directory.CreateDirectory("TestSC");
            image.Save(@"\TestSC\" + fileName, ImageFormat.Tiff);
        }

        private static Image AdjustImageForRecognition(Image img)
        {
            Image result;
            using (var db = new DirectBitmap(img.Width, img.Height))
            {
                using (var graphics = Graphics.FromImage(db.Bitmap))
                {
                    graphics.DrawImage(img, Point.Empty);
                }

                for (var i = 0; i < img.Width; i++)
                {
                    for (var j = 0; j < img.Height; j++)
                    {
                        var currentPixel = db.GetPixel(i, j);
                        var brightness = currentPixel.GetBrightness();
                        if (brightness < 0.85f)
                        {
                            db.SetPixel(i, j, Color.White);
                        }
                        else
                        {
                            db.SetPixel(i,j, Color.Black);
                        }
                    }
                }

                result = (Bitmap) db.Bitmap.Clone();
            }

            return result;
        }

        public static string TestWeapons()
        {
            var w1Img = GetWeapon1Image();
            var w2Img = GetWeapon2Image();
            SaveTestImage(w1Img);
            SaveTestImage(AdjustImageForRecognition(w1Img));
            SaveTestImage(w2Img);
            SaveTestImage(AdjustImageForRecognition(w2Img));
            return WeaponImageToString(w1Img) + WeaponImageToString(w2Img);
        }

        public static string GetWeapon1FromScreen()
        {
            return WeaponImageToString(GetWeapon1Image());
        }

        public static string GetWeapon2FromScreen()
        {
            return WeaponImageToString(GetWeapon2Image());
        }

        private static string WeaponImageToString(Image img)
        {
            var result = "";
            try
            {
                var img2 = AdjustImageForRecognition(img);
                var ocr = new AdvancedOcr
                {
                    CleanBackgroundNoise = true,
                    EnhanceContrast = true,
                    EnhanceResolution = true,
                    Language = IronOcr.Languages.English.OcrLanguagePack,
                    Strategy = AdvancedOcr.OcrStrategy.Advanced,
                    ColorSpace = AdvancedOcr.OcrColorSpace.GrayScale,
                    DetectWhiteTextOnDarkBackgrounds = false,
                    InputImageType = AdvancedOcr.InputTypes.Snippet,
                    RotateAndStraighten = false,
                    ReadBarCodes = false,
                    ColorDepth = 8
                };
                var res = ocr.Read(img2);
                result = res.Text.ToUpper();
            }
            catch
            {
                //ignore
            }

            return result;
        }

        
    }
}