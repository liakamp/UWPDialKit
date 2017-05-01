using System;
using Windows.UI;

namespace UWPDialKit.Controls.Helpers
{
    public static class Utilities
    {
        public static double ClampAngle(double currentAngle)
        {
            if (currentAngle > 360)
            {
                return currentAngle % 360;
            }
            else if (currentAngle < 0)
            {
                return 360 - (Math.Abs(currentAngle) % 360);
            }
            else
            {
                return currentAngle;
            }

        }

        public static Color ConvertHSV2RGB(float hue, float saturation, float value)
        {
            byte r = 0;
            byte g = 0;
            byte b = 0;
            hue /= 60;
            int i = (int)Math.Floor(hue);
            float f = hue - i;
            float p = value * (1 - saturation);
            float q = value * (1 - saturation * f);
            float t = value * (1 - saturation * (1 - f));
            switch (i)
            {
                case 0:
                    r = (byte)(255 * value);
                    g = (byte)(255 * t);
                    b = (byte)(255 * p);
                    break;
                case 1:
                    r = (byte)(255 * q);
                    g = (byte)(255 * value);
                    b = (byte)(255 * p);
                    break;
                case 2:
                    r = (byte)(255 * p);
                    g = (byte)(255 * value);
                    b = (byte)(255 * t);
                    break;
                case 3:
                    r = (byte)(255 * p);
                    g = (byte)(255 * q);
                    b = (byte)(255 * value);
                    break;
                case 4:
                    r = (byte)(255 * t);
                    g = (byte)(255 * p);
                    b = (byte)(255 * value);
                    break;
                default:
                    r = (byte)(255 * value);
                    g = (byte)(255 * p);
                    b = (byte)(255 * q);
                    break;
            }
            return Color.FromArgb(255, r, g, b);
        }
    }
}