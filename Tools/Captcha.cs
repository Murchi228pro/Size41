using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Identity.Client;

namespace _41Size.Tools
{
    class Captcha
    {
        private string _text;

        private static readonly int MIN_SYMBOLS = 5;

        private static readonly int MAX_SYMBOLS = 8;

        private static readonly int NOISE_ITERS_COUNT = 5;

        private static readonly int SYMBOL_NOISE_MAX_ITERS = 5;


        private static readonly int ascii_a = (int)'a';

        private static readonly int ascii_A = (int)'A';

        private static readonly int ascii_Z = (int)'Z';


        private static readonly int ascii_0 = (int)'0';

        private static readonly int ascii_9 = (int)'9';


        private static readonly int ascii_z = (int)'z';

        private static readonly System.Windows.Media.Color[] randColors = {
            Colors.Red, Colors.Green, Colors.Black, Colors.Blue, Colors.Purple, Colors.Orange, Colors.Indigo
        };

        private static readonly FontFamily[] randFontFamilies = {
            new FontFamily("Arial"), new FontFamily("Papyrus"), new FontFamily("Papyrus")
        };

        private static readonly int lineWidth = 2;

        public Captcha() {
            int symbolsCount = Random.Shared.Next(MIN_SYMBOLS, MAX_SYMBOLS);

            for (int i = 0; i < symbolsCount; i++)
            {
                int symbolValue = Random.Shared.Next(ascii_0, ascii_z + 1);

                if ((symbolValue < ascii_A && symbolValue > ascii_9) || (symbolValue < ascii_a && symbolValue > ascii_Z))
                {
                    symbolValue += 8;
                }

                _text += (char)symbolValue;
            }

        }

        public void Create(Image image)
        {

            DrawingImage captchaImage = new DrawingImage();

            captchaImage.Drawing = GenerateCaptchaWithNoise();

            image.Source = captchaImage;


        }

        public DrawingGroup GenerateCaptchaWithNoise()
        {
            DrawingGroup group = new DrawingGroup();

            StringBuilder spacesAdder = new StringBuilder(_text);

            spacesAdder.Insert(0, "    ");

            for (int i = 0; i < NOISE_ITERS_COUNT; i++)
            {
                spacesAdder.Insert(Random.Shared.Next(0, spacesAdder.Length), ' ');
            }

            string displayingText = spacesAdder.ToString();

            FormattedText captchaValue = new FormattedText(
                displayingText,
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface("Adler"),
                32,
                Brushes.Black
                );

            for (int i = 0; i < NOISE_ITERS_COUNT; i++)
            {
                int symbolIndex = Random.Shared.Next(displayingText.Length);
                SetNoiseFormatToSymbol( captchaValue, symbolIndex );
            }
            
            Geometry geometry = captchaValue.BuildGeometry(new(0, 100));
            TransformGroup transfromation = new TransformGroup();
            transfromation.Children.Add(new RotateTransform(100));
            transfromation.Children.Add(new ScaleTransform(20, 50));
            geometry.Transform = new TransformGroup();


            DrawingContext draw = group.Open();

            var color = randColors[Random.Shared.Next(randColors.Length)];

            Brush brush = new SolidColorBrush(color);

            (int X, int Y) startPoint = new(Random.Shared.Next(0, 20), Random.Shared.Next(0, 60));
            (int X, int Y) endPoint = new(Random.Shared.Next(180, 200), Random.Shared.Next(0, 60));

            draw.DrawLine(new Pen(brush, lineWidth), new(startPoint.X, startPoint.Y), new(endPoint.X, endPoint.Y));
            draw.DrawText(captchaValue, new System.Windows.Point(0, 0));

            for (int i = 0; i < NOISE_ITERS_COUNT; i++)
            {
                brush = new SolidColorBrush(randColors[Random.Shared.Next(randColors.Length)]);
                brush.Opacity = 0.2;
                draw.DrawEllipse(brush, new Pen(brush, 2), new(Random.Shared.Next(0, 200), Random.Shared.Next(0, 40)), 10, 10);
            }

            draw.Close();

            return group;
        }

        private void SetNoiseFormatToSymbol(FormattedText text, int symbolIndex)
        {
            int iters_count = Random.Shared.Next(SYMBOL_NOISE_MAX_ITERS);
            for (int i = 0; i < iters_count; i++)
            {
                switch (i)
                {
                    case 0:
                        text.SetFontSize(Random.Shared.Next(20, 30), symbolIndex, 1);
                        text.SetFontFamily(randFontFamilies[Random.Shared.Next(randFontFamilies.Length)], symbolIndex, 1);
                        break;
                    case 1:
                        text.SetFontStyle(FontStyles.Italic, symbolIndex, 1);
                        text.SetFontWeight(FontWeight.FromOpenTypeWeight(Random.Shared.Next(20, 40)), symbolIndex, 1);
                        text.SetFontSize(Random.Shared.Next(20, 30), symbolIndex, 1);
                        break;
                    case 2:
                        text.SetFontStretch(FontStretch.FromOpenTypeStretch(Random.Shared.Next(5, 9)));
                        text.SetFontWeight(FontWeight.FromOpenTypeWeight(Random.Shared.Next(50, 100)), symbolIndex, 1);
                        break;
                    case 3:
                        text.SetFontStretch(FontStretch.FromOpenTypeStretch(Random.Shared.Next(5, 9)));
                        text.SetFontStyle(FontStyles.Italic, symbolIndex, 1);

                        break;
                }
            }
        }

        public bool IsCaptchaValid(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;
            if (value == _text) return true;
            return false;
        }
    }
}
