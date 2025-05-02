using System;
using System.Collections.Generic;
using System.Globalization;

namespace SageFinancialAPI.Generators
{
    public static class ColorGenerator
    {
        private static string HslToHex(double h, double s, double l)
        {
            l /= 100;
            double a = s * Math.Min(l, 1 - l) / 100;

            string F(double n)
            {
                double k = (n + h / 30) % 12;
                double color = l - a * Math.Max(Math.Min(Math.Min(k - 3, 9 - k), 1), -1);
                int rgb = (int)Math.Round(255 * color);
                return rgb.ToString("X2"); // 2-digit hexadecimal
            }

            return $"#{F(0)}{F(8)}{F(4)}";
        }

        public static List<string> GenerateDistinctColors(int count)
        {
            double saturation = 70;
            double lightness = 50;
            List<string> colors = [];

            double hueStep = 360.0 / count;

            Random random = new();

            for (int i = 0; i < count; i++)
            {
                double randomOffset = (random.NextDouble() - 0.5) * hueStep * 0.5;
                double hue = (i * hueStep + randomOffset + 360) % 360;
                colors.Add(HslToHex(hue, saturation, lightness));
            }

            return colors;
        }

        public static string GenerateRandomColor()
        {
            Random random = new();
            double h = random.NextDouble() * 360;
            double s = 40 + random.NextDouble() * 40;
            double l = 40 + random.NextDouble() * 40;
            return HslToHex(h, s, l);
        }

        public static List<string> GenerateDarkModePalette(int count)
        {
            List<string> colors = [];
            double hueStep = 360.0 / count;

            Random random = new();

            for (int i = 0; i < count; i++)
            {
                double baseHue = i * hueStep;
                double saturation = 50 + random.NextDouble() * 30;
                double lightness = 20 + random.NextDouble() * 20;
                colors.Add(HslToHex(baseHue, saturation, lightness));
            }

            return colors;
        }

        public static string GenerateDarkModeColor()
        {
            Random random = new();
            double hue = random.NextDouble() * 360;
            double saturation = 50 + random.NextDouble() * 30;
            double lightness = 20 + random.NextDouble() * 20;
            return HslToHex(hue, saturation, lightness);
        }
    }
}