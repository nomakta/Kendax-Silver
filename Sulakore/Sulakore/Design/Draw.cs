using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;

namespace Sulakore.Design
{
    public static class Draw
    {
        public const int MState_None = 0, MState_Over = 1, MState_Down = 2;

        public static Image Scale(this Image I, int Height, int Width)
        {
            if (I == null || Height <= 0 || Width <= 0) return null;

            Rectangle ScaledR = new Rectangle(0, 0, (I.Width * Height) / (I.Height), (I.Height * Width) / (I.Width));
            Bitmap ScaledB = new Bitmap(Width, Height);

            using (Graphics G = Graphics.FromImage(ScaledB))
            {
                G.InterpolationMode = InterpolationMode.HighQualityBilinear;
                if (ScaledR.Width > Width)
                {
                    ScaledR.X = (ScaledB.Width - Width) / 2;
                    ScaledR.Y = (ScaledB.Height - ScaledR.Height) / 2;
                    G.DrawImage(I, ScaledR.X, ScaledR.Y, Width, ScaledR.Height);
                }
                else
                {
                    ScaledR.X = (ScaledB.Width / 2) - (ScaledR.Width / 2);
                    ScaledR.Y = (ScaledB.Height / 2) - (Height / 2);
                    G.DrawImage(I, ScaledR.X, ScaledR.Y, ScaledR.Width, Height);
                }
            }
            return ScaledB;
        }

        public static GraphicsPath Round(this Rectangle R, int Curve)
        {
            GraphicsPath GP = new GraphicsPath();
            if (Curve <= 0)
            {
                GP.AddRectangle(R);
                return GP;
            }
            else
            {
                int ARW = Curve * 2;
                GP.AddArc(new Rectangle(R.X, R.Y, ARW, ARW), -180, 90);
                GP.AddArc(new Rectangle(R.Width - ARW + R.X, R.Y, ARW, ARW), -90, 90);
                GP.AddArc(new Rectangle(R.Width - ARW + R.X, R.Height - ARW + R.Y, ARW, ARW), 0, 90);
                GP.AddArc(new Rectangle(R.X, R.Height - ARW + R.Y, ARW, ARW), 90, 90);
                GP.AddLine(new Point(R.X, R.Height - ARW + R.Y), new Point(R.X, Curve + R.Y));
                return GP;
            }
        }
        public static GraphicsPath Triangle(this Rectangle R, float Angle = 90)
        {
            using (Matrix M = new Matrix())
            {
                M.RotateAt(Angle, new Point(R.X + (R.Width / 2), R.Y + (R.Height / 2)));
                GraphicsPath GP = new GraphicsPath();
                GP.AddLine(new Point(R.X, R.Y + (R.Height / 2)), new Point(R.X + R.Width, R.Y + R.Height));
                GP.AddLine(new Point(R.X + R.Width, R.Y + R.Height), new Point(R.X + R.Width, R.Y));
                GP.CloseAllFigures();
                GP.Transform(M);
                return GP;
            }
        }

        public static LinearGradientBrush CreateGradient(this Color C1, Color C2, int X, int Y, int Width, int Height, float Angle = 90)
        {
            return C1.CreateGradient(C2, new Rectangle(X, Y, Width, Height), Angle);
        }
        public static LinearGradientBrush CreateGradient(this Color C1, Color C2, Rectangle R, float Angle = 90)
        {
            return new LinearGradientBrush(R, C1, C2, Angle);
        }

        public static void DrawLine(this Graphics G, Color C1, int X1, int Y1, int X2, int Y2)
        {
            G.DrawLine(C1, new Point(X1, Y1), new Point(X2, Y2));
        }
        public static void DrawLine(this Graphics G, Color C1, Point P, int X, int Y)
        {
            G.DrawLine(C1, P, new Point(X, Y));
        }
        public static void DrawLine(this Graphics G, Color C1, int X, int Y, Point P)
        {
            G.DrawLine(C1, new Point(X, Y), P);
        }
        public static void DrawLine(this Graphics G, Color C1, Point P1, Point P2)
        {
            using (Pen P = new Pen(C1))
                G.DrawLine(P, P1, P2);
        }

        public static void DrawString(this Graphics G, string Text, Font Font, Color C1, int X, int Y, int Width, int Height)
        {
            G.DrawString(Text, Font, C1, new Rectangle(X, Y, Width, Height));
        }
        public static void DrawString(this Graphics G, string Text, Font Font, Color C1, Rectangle R)
        {
            using (SolidBrush SB = new SolidBrush(C1))
                G.DrawString(Text, Font, SB, R);
        }
        public static void DrawString(this Graphics G, string Text, Font Font, Color C1, int X, int Y, int Width, int Height, StringAlignment SA)
        {
            G.DrawString(Text, Font, C1, new Rectangle(X, Y, Width, Height), SA);
        }
        public static void DrawString(this Graphics G, string Text, Font Font, Color C1, Rectangle R, StringAlignment SA)
        {
            using (SolidBrush SB = new SolidBrush(C1))
            using (StringFormat SF = new StringFormat() { Alignment = SA, LineAlignment = SA })
                G.DrawString(Text, Font, SB, R, SF);
        }

        public static void DrawString(this Graphics G, string Text, Font Font, Color C1, int X, int Y, int Width, int Height, StringAlignment Alignment, StringAlignment LineAlignment)
        {
            G.DrawString(Text, Font, C1, new Rectangle(X, Y, Width, Height), Alignment, LineAlignment);
        }
        public static void DrawString(this Graphics G, string Text, Font Font, Color C1, Rectangle R, StringAlignment Alignment, StringAlignment LineAlignment)
        {
            using (SolidBrush SB = new SolidBrush(C1))
            using (StringFormat SF = new StringFormat() { Alignment = Alignment, LineAlignment = LineAlignment })
                G.DrawString(Text, Font, SB, R, SF);
        }

        public static void FillGradient(this Graphics G, Color C1, Color C2, int X, int Y, int Width, int Height, float Angle = 90)
        {
            G.FillGradient(C1, C2, new Rectangle(X, Y, Width, Height), Angle);
        }
        public static void FillGradient(this Graphics G, Color C1, Color C2, Rectangle R, float Angle = 90)
        {
            using (LinearGradientBrush LGB = new LinearGradientBrush(R, C1, C2, Angle))
                G.FillRectangle(LGB, R);
        }

        public static void FillRoundGradient(this Graphics G, Color C1, Color C2, int X, int Y, int Width, int Height, float Angle, int Curve)
        {
            G.FillRoundGradient(C1, C2, new Rectangle(X, Y, Width, Height), Angle, Curve);
        }
        public static void FillRoundGradient(this Graphics G, Color C1, Color C2, Rectangle R, float Angle, int Curve)
        {
            using (LinearGradientBrush LGB = new LinearGradientBrush(R, C1, C2, Angle))
            using (GraphicsPath GP = R.Round(Curve))
                G.FillPath(LGB, GP);
        }

        public static void FillRound(this Graphics G, Color C1, int X, int Y, int Width, int Height, int Curve)
        {
            G.FillRound(C1, new Rectangle(X, Y, Width, Height), Curve);
        }
        public static void FillRound(this Graphics G, Color C1, Rectangle R, int Curve)
        {
            using (SolidBrush SB = new SolidBrush(C1))
            using (GraphicsPath GP = R.Round(Curve))
                G.FillPath(SB, GP);
        }

        public static void DrawRound(this Graphics G, Color C1, int X, int Y, int Width, int Height, int Curve)
        {
            G.DrawRound(C1, new Rectangle(X, Y, Width, Height), Curve);
        }
        public static void DrawRound(this Graphics G, Color C1, Rectangle R, int Curve)
        {
            using (Pen P = new Pen(C1))
            using (GraphicsPath GP = R.Round(Curve))
                G.DrawPath(P, GP);
        }

        public static void FillTriangle(this Graphics G, Color C1, int X, int Y, int Width, int Height, float Angle = 90)
        {
            G.FillTriangle(C1, new Rectangle(X, Y, Width, Height), Angle);
        }
        public static void FillTriangle(this Graphics G, Color C1, Rectangle R, float Angle = 90)
        {
            using (GraphicsPath GP = R.Triangle(Angle))
            using (SolidBrush SB = new SolidBrush(C1))
                G.FillPath(SB, GP);
        }

        public static void FillRectangle(this Graphics G, Color C1, int X, int Y, int Width, int Height)
        {
            G.FillRectangle(C1, new Rectangle(X, Y, Width, Height));
        }
        public static void FillRectangle(this Graphics G, Color C1, Rectangle R)
        {
            using (SolidBrush SB = new SolidBrush(C1))
                G.FillRectangle(SB, R);
        }

        public static void DrawRectangle(this Graphics G, Color C1, int X, int Y, int Width, int Height)
        {
            G.DrawRectangle(C1, new Rectangle(X, Y, Width, Height));
        }
        public static void DrawRectangle(this Graphics G, Color C1, Rectangle R)
        {
            using (Pen P = new Pen(C1))
                G.DrawRectangle(P, R);
        }

        public static void DrawPixel(this Graphics G, Color C, int X, int Y)
        {
            G.DrawPixel(C, new Point(X, Y));
        }
        public static void DrawPixel(this Graphics G, Color C, Point P)
        {
            using (SolidBrush SB = new SolidBrush(C))
                G.FillRectangle(SB, P.X, P.Y, 1, 1);
        }

        public static void DrawBorders(this Graphics G, Color C, int X, int Y, int Width, int Height, int Offset = 0)
        {
            G.DrawBorders(C, new Rectangle(X, Y, Width, Height), Offset);
        }
        public static void DrawBorders(this Graphics G, Color C, Rectangle R, int Offset = 0)
        {
            using (Pen P = new Pen(C))
                G.DrawRectangle(P, R.X + Offset, R.Y + Offset, (R.Width - (Offset * 2)) - 1, (R.Height - (Offset * 2) - 1));
        }
    }
}