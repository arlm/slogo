using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace TurtleRuntime
{
    public class Turtle
    {
        Graphics canvas;
        Color background;
        Color foreground;
        Font font;
        float fontSize = 10;
        float penSize;
        Pen drawingPen;

        double currentX, currentY;
        bool penDown = true;
        bool showTurtle = true;
        double direction = 90;

        public Turtle(Graphics canvas)
        {
            this.canvas = canvas;
            drawingPen = new Pen(foreground, penSize);
            font = new Font("Courier", fontSize);
            Initialize();
        }

        public void PenUp()
        {
            penDown = false;
        }

        public void PenDown()
        {
            penDown = true;
        }

        public void Forward(double steps)
        {
            int oldX = (int) currentX;
            int oldY = (int) currentY;

            currentX += steps * Math.Cos(DegToRad(direction));
            currentY -= steps * Math.Sin(DegToRad(direction));

            if(penDown)
            {
                canvas.DrawLine(drawingPen, oldX, oldY, 
                    (int) currentX, (int) currentY);
            }
        }

        public void Backward(double steps)
        {
            Forward(-1 * steps);
        }

        public void Left(double degrees)
        {
            direction = (direction + degrees) % 360;
        }

        public void Right(double degrees)
        {
            direction = (direction - degrees + 360) % 360;
        }

        public void Hide()
        {
            showTurtle = false;
        }

        public void Show()
        {
            showTurtle = true;
        }

        public void SetFontSize(double size)
        {
            fontSize = (float)size;
            font = new Font("Courier", fontSize);
        }

        public void DrawText(string txt)
        {
            System.Drawing.Drawing2D.GraphicsState canvasState = canvas.Save();
            canvas.TranslateTransform((float)currentX, (float)currentY);
            canvas.RotateTransform((float)(90 - direction));
            canvas.DrawString(txt, font, new SolidBrush(foreground), (float)0, (float)0);

            canvas.Restore(canvasState);
        }

        public void Draw()
        {
            if (showTurtle)
            {
                System.Drawing.Drawing2D.GraphicsState canvasState = canvas.Save();
                canvas.TranslateTransform((float) currentX, (float) currentY);
                canvas.RotateTransform((float)(90 - direction));

                canvas.DrawLine(drawingPen, -4, 4, 0, -8);
                canvas.DrawLine(drawingPen, 0, -8, 4, 4);
                canvas.DrawLine(drawingPen, -4, 4, 4, 4);

                canvas.Restore(canvasState);
            }
        }

        public void Reset()
        {
            Initialize();
            drawingPen = new Pen(foreground, penSize);
            canvas.Clear(background);
        }

        public void Stop()
        {
            throw new TurtleStopException();
        }

        public void SetPenSize(double size)
        {
            penSize = (float)size;
            drawingPen = new Pen(foreground, penSize);
        }

        public void SetPenColor(Color clr)
        {
            foreground = clr;
            drawingPen = new Pen(foreground, penSize);
        }

        public void PenErase()
        { 
            SetPenColor(background);
        }

        public void Home()
        {
            currentX = canvas.VisibleClipBounds.Width / 2.0;
            currentY = canvas.VisibleClipBounds.Height / 2.0;
            direction = 90;
        }

        private void Initialize()
        {
            currentX = canvas.VisibleClipBounds.Width / 2.0;
            currentY = canvas.VisibleClipBounds.Height / 2.0;

            penDown = true;
            showTurtle = true;
            direction = 90;

            background = Color.White;
            foreground = Color.Black;
            penSize = 1.0f;

        }

        private double DegToRad(double degrees)
        {
            return (Math.PI * (double) degrees / 180.0);
        }
    }
}
