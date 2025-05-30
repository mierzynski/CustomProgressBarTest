using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows.Forms;
using System;

namespace progressBar
{
    public partial class Form1 : Form
    {
        private int percent = 97;
        private int radius = 25;
        private int rectHeight = 50;
        private int distanceToRight = 0;
        private int distanceToLeft = 0;

        private TrackBar trackBarPercent;
        private TrackBar trackBarRadius;
        private Label labelPercent;
        private Label labelRadius;
        private Label labelHorizontalRadiusRight;
        private Label labelVerticalRadiusRight;
        private Label labelDistanceToRight;
        private Label labelHorizontalRadiusLeft;
        private Label labelVerticalRadiusLeft;
        private Label labelDistanceToLeft;


        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;


            InitializeControls();
        }

        private void InitializeControls()
        {
            Label labelHeight = new Label
            {
                Text = $"Height: {rectHeight}",
                Location = new Point(320, 90),
                AutoSize = true
            };
            TrackBar trackBarHeight = new TrackBar
            {
                Minimum = 10,
                Maximum = 300,
                Value = rectHeight,
                TickFrequency = 10,
                Location = new Point(10, 90),
                Width = 300
            };
            trackBarHeight.Scroll += (s, e) =>
            {
                rectHeight = trackBarHeight.Value;
                labelHeight.Text = $"Height: {rectHeight}";
                Invalidate();
            };



            Controls.Add(trackBarHeight);
            Controls.Add(labelHeight);


            trackBarPercent = new TrackBar
            {
                Minimum = 1,
                Maximum = 100,
                Value = percent,
                TickFrequency = 5,
                Location = new Point(10, 10),
                Width = 300
            };
            trackBarPercent.Scroll += TrackBarPercent_Scroll;


            trackBarRadius = new TrackBar
            {
                Minimum = 5,
                Maximum = 100,
                Value = radius,
                TickFrequency = 5,
                Location = new Point(10, 50),
                Width = 300
            };
            trackBarRadius.Scroll += TrackBarRadius_Scroll;

            labelPercent = new Label
            {
                Text = $"Percent: {percent}%",
                Location = new Point(320, 10),
                AutoSize = true
            };

            labelRadius = new Label
            {
                Text = $"Radius: {radius}",
                Location = new Point(320, 50),
                AutoSize = true
            };

            labelHorizontalRadiusRight = new Label
            {
                Text = $"(right) Horizontal Radius: -",
                Location = new Point(400, 160),
                AutoSize = true
            };

            labelVerticalRadiusRight = new Label
            {
                Text = $"(right) Vertical Radius: -",
                Location = new Point(400, 180),
                AutoSize = true
            };

            labelDistanceToRight = new Label
            {
                Text = $"distanceToRight: {distanceToRight}",
                Location = new Point(400, 200),
                AutoSize = true
            };

            labelHorizontalRadiusLeft = new Label
            {
                Text = $"(left) Horizontal Radius: -",
                Location = new Point(50, 220),
                AutoSize = true
            };

            labelVerticalRadiusLeft = new Label
            {
                Text = $"(left) Vertical Radius: -",
                Location = new Point(50, 240),
                AutoSize = true
            };

            labelDistanceToLeft = new Label
            {
                Text = $"distanceToLeft: {distanceToLeft}",
                Location = new Point(50, 260),
                AutoSize = true
            };

            Controls.Add(trackBarPercent);
            Controls.Add(trackBarRadius);
            Controls.Add(labelPercent);
            Controls.Add(labelRadius);
            Controls.Add(labelHorizontalRadiusRight);
            Controls.Add(labelVerticalRadiusRight);
            Controls.Add(labelDistanceToRight);
            Controls.Add(labelHorizontalRadiusLeft);
            Controls.Add(labelVerticalRadiusLeft); 
            Controls.Add(labelDistanceToLeft);
        }

        // Obsługuje zmianę wartości trackbara dla percent
        private void TrackBarPercent_Scroll(object sender, EventArgs e)
        {
            percent = trackBarPercent.Value;
            labelPercent.Text = $"Percent: {percent}%";
            Invalidate(); // Przeładuj formularz, aby odświeżyć rysowanie
        }

        // Obsługuje zmianę wartości trackbara dla radius
        private void TrackBarRadius_Scroll(object sender, EventArgs e)
        {
            radius = trackBarRadius.Value;
            labelRadius.Text = $"Radius: {radius}";
            Invalidate(); // Przeładuj formularz, aby odświeżyć rysowanie
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Parametry prostokąta
            Rectangle fullRect = new Rectangle(50, 150, 340, rectHeight); // pełna szerokość tła

            // 1. Rysowanie tła (stałego)
            using (GraphicsPath backgroundPath = CreateRoundedRectangle(fullRect, radius, 100, true)) // 100% szerokości
            using (SolidBrush bgBrush = new SolidBrush(Color.LightGray))
            {
                g.FillPath(bgBrush, backgroundPath);
            }

            // 2. Rysowanie dynamicznego paska (progressu)
            int currentWidth = (int)(fullRect.Width * (percent / 100.0));
            if (currentWidth > 0)
            {
                Rectangle progressRect = new Rectangle(fullRect.X, fullRect.Y, currentWidth, fullRect.Height);

                using (GraphicsPath progressPath = CreateRoundedRectangle(fullRect, radius, percent, false))
                using (SolidBrush progressBrush = new SolidBrush(Color.SteelBlue))
                {
                    g.FillPath(progressBrush, progressPath);
                }
            }
        }


        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius, int percent, bool background)
        {
            GraphicsPath path = new GraphicsPath();

            int newWidth = (int)(rect.Width * (percent / 100.0));
            if (newWidth < 1) newWidth = 1;

            Rectangle newRect = new Rectangle(rect.X, rect.Y, newWidth, rect.Height);
            distanceToRight = rect.Right - newRect.Right;


            path.StartFigure();

            AddFullLeftArc(path, newRect, radius);

            if (newRect.Width > radius)
            {
                path.AddLine(newRect.X + radius, newRect.Y, newRect.Right - radius * 2, newRect.Y);
                AddTopRightArc(path, newRect, radius, background);
                path.AddLine(newRect.Right, newRect.Y + radius, newRect.Right, newRect.Bottom - radius * 2);
                AddBottomRightArc(path, newRect, radius, background);
                AddBottomLine(path, newRect, radius, background);
            }

            path.CloseFigure();
            return path;
        }


        private void AddFullLeftArc(GraphicsPath path, Rectangle rect, int radius)
        {
            distanceToLeft = rect.Width;

            float progress = Clamp(distanceToLeft / (float)radius, 0f, 1f);

            float easedWidth = (float)Math.Pow(progress, 2);
            float easedHeight = (float)Math.Pow(progress, 0.6);

            float maxArcWidth = radius * 2;
            float dynamicArcWidth = 4f + (maxArcWidth - 4f) * easedWidth;
            float dynamicArcHeight = 4f + (rect.Height - 4f) * easedHeight;

            // Wyśrodkowanie łuku w pionie
            float offsetY = rect.Y + (rect.Height - dynamicArcHeight) / 2f;

            path.AddArc(
                rect.X, offsetY,
                dynamicArcWidth, dynamicArcHeight,
                90,
                180
            );

        }

        private void AddTopRightArc(GraphicsPath path, Rectangle rect, int radius, bool background)
        {
            if (background)
            {
                path.AddArc(rect.Right - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);
            }
            else if (distanceToRight <= radius)
            {
                float progress = 1f - (distanceToRight / (float)radius);
                float eased = (float)Math.Pow(progress, 3);
                float arcHeight = 0.01f + (radius * 2 - 0.01f) * eased;

                path.AddArc(rect.Right - radius * 2, rect.Y, radius * 2, arcHeight, 270, 90);
            }
            else
            {
                path.AddLine(rect.Right, rect.Y, rect.Right, rect.Y);
            }
        }

        private void AddBottomRightArc(GraphicsPath path, Rectangle rect, int radius, bool background)
        {
            if (background)
            {
                path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            }
            else if (distanceToRight <= radius)
            {
                float progress = 1f - (distanceToRight / (float)radius);
                float eased = (float)Math.Pow(progress, 3);
                float arcHeight = 0.01f + (radius * 2 - 0.01f) * eased;
                float arcY = rect.Bottom - arcHeight;

                path.AddArc(rect.Right - radius * 2, arcY, radius * 2, arcHeight, 0, 90);
            }
            else
            {
                path.AddLine(rect.Right, rect.Bottom, rect.Right, rect.Bottom);
            }
        }

        private void AddBottomLine(GraphicsPath path, Rectangle rect, int radius, bool background)
        {
            float yOffset = 0f;
            if (!background && rect.Width <= radius)
            {
                float progress = rect.Width / (float)radius;
                float eased = (float)Math.Pow(1f - progress, 3);
                yOffset = radius * eased;
            }

            float y = rect.Bottom - yOffset;
            if (rect.Width > radius * 2)
            {
                path.AddLine(rect.Right - radius * 2, y, rect.X + radius * 2, y);
            }
        }

        private float Clamp(float value, float min, float max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

    }

}
