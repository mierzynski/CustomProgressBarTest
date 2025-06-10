using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace progressBar
{
    public partial class cRoundedProgressBar : UserControl
    {
        private int percent = 97;
        private int rectHeight = 50;
        private int distanceToRight = 0;
        private int distanceToLeft = 0;

        private NumericUpDown editBox;
        private bool editing = false;

        private Color barColor = Color.SteelBlue;

        public int Percent
        {
            get => percent;
            set
            {
                if (value < -100) percent = -100;
                else if (value > 200) percent = 200;
                else percent = value;
                Invalidate();
            }
        }

        public Color BarColor
        {
            get => barColor;
            set
            {
                barColor = value;
                Invalidate();
            }
        }

        public cRoundedProgressBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);

            this.Size = new Size(440, 350);

            InitializeEditBox();
        }

        private void InitializeEditBox()
        {
            editBox = new NumericUpDown
            {
                Visible = false,
                BorderStyle = BorderStyle.None,
                Font = this.Font,
                TextAlign = HorizontalAlignment.Center,
                Minimum = -100,
                Maximum = 200,
                DecimalPlaces = 0,
                Increment = 1
            };

            editBox.Leave += (s, ev) => FinishEdit();
            editBox.KeyDown += (s, ev) =>
            {
                if (ev.KeyCode == Keys.Enter)
                    FinishEdit();
                else if (ev.KeyCode == Keys.Escape)
                    CancelEdit();
            };

            this.Controls.Add(editBox);

            this.MouseClick += RoundedProgressBar_MouseClick;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle fullRect = new Rectangle(50, 150, 340, rectHeight);

            int maxWidth = fullRect.Width;
            int currentWidth = (int)(maxWidth * (Math.Abs(percent) / 100.0));
            currentWidth = Math.Min(currentWidth, maxWidth);

            if (percent < 0)
                DrawNegativeBar(g, fullRect);

            DrawBackgroundBar(g, fullRect);

            if (percent >= 0 && currentWidth > 0)
                DrawPositiveBar(g, fullRect, percent);

            DrawCenteredText(g, fullRect, currentWidth);
        }

        private void DrawBackgroundBar(Graphics g, Rectangle rect)
        {
            using (GraphicsPath backgroundPath = CreateRoundedRectangle(rect, 100, true))
            using (SolidBrush bgBrush = new SolidBrush(Color.LightGray))
            {
                g.FillPath(bgBrush, backgroundPath);
            }
        }

        private void DrawPositiveBar(Graphics g, Rectangle rect, int percent)
        {
            bool roundRight = percent <= 100;

            using (GraphicsPath progressPath = CreateRoundedRectangle(rect, percent, false, roundRight))
            using (SolidBrush progressBrush = new SolidBrush(barColor))
            {
                g.FillPath(progressBrush, progressPath);
            }
        }

        private void DrawNegativeBar(Graphics g, Rectangle fullRect)
        {
            int rectWidth = fullRect.Height;
            int barHeight = rectHeight - 2;
            int yOffset = fullRect.Y + 1;
            Rectangle negativeRect = new Rectangle(fullRect.X, yOffset, rectWidth, barHeight);

            using (SolidBrush brush = new SolidBrush(barColor))
            {
                g.FillRectangle(brush, negativeRect);
            }
        }

        private void DrawCenteredText(Graphics g, Rectangle fullRect, int currentWidth)
        {
            string text = $"{percent}%";
            RectangleF textRect;
            Color textColor;

            if (percent >= 0 && currentWidth >= rectHeight / 2)
            {
                textRect = new RectangleF(fullRect.X, fullRect.Y, currentWidth, fullRect.Height);
                textColor = Color.LightGray;
            }
            else
            {
                textRect = fullRect;
                textColor = barColor;
            }

            using (StringFormat sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            using (Brush textBrush = new SolidBrush(textColor))
            {
                g.DrawString(text, this.Font, textBrush, textRect, sf);
            }
        }

        private void RoundedProgressBar_MouseClick(object sender, MouseEventArgs e)
        {
            Rectangle fullRect = new Rectangle(50, 150, 340, rectHeight);

            if (editing)
            {
                if (!editBox.Bounds.Contains(e.Location))
                {
                    FinishEdit();
                }
                return;
            }

            if (fullRect.Contains(e.Location))
            {
                ShowEditBox(fullRect);
            }
        }

        private void ShowEditBox(Rectangle rect)
        {
            editBox.Bounds = new Rectangle(
                rect.X + rect.Width / 2 - 30,
                rect.Y + rect.Height / 2 - 10,
                60, 20
            );

            editBox.Value = percent;
            editBox.Visible = true;
            editBox.BackColor = Color.LightGray;
            editBox.ForeColor = barColor;

            editBox.Focus();
            editing = true;
        }

        private void FinishEdit()
        {
            percent = (int)editBox.Value;
            editBox.Visible = false;
            editing = false;
            Invalidate();
        }

        private void CancelEdit()
        {
            editBox.Visible = false;
            editing = false;
            Invalidate();
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int percent, bool background, bool roundRightSide = true)
        {
            GraphicsPath path = new GraphicsPath();
            int newWidth = 0;
            int radius = rect.Height / 2;

            if (roundRightSide)
            {
                newWidth = (int)(rect.Width * (percent / 100.0));
                if (newWidth < 1) newWidth = 1;
            }
            else
            {
                newWidth = rect.Width;
            }

            Rectangle newRect = new Rectangle(rect.X, rect.Y, newWidth, rect.Height);
            distanceToRight = rect.Right - newRect.Right;

            path.StartFigure();

            AddFullLeftArc(path, newRect, radius);

            if (newRect.Width > radius)
            {
                path.AddLine(newRect.X + radius, newRect.Y, newRect.Right - radius * 2, newRect.Y);
                if (roundRightSide)
                {
                    AddTopRightArc(path, newRect, radius, background);
                    path.AddLine(newRect.Right, newRect.Y + radius, newRect.Right, newRect.Bottom - radius * 2);
                    AddBottomRightArc(path, newRect, radius, background);
                }
                else
                {
                    path.AddLine(newRect.Right, newRect.Y, newRect.Right, newRect.Bottom);
                }
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
