using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tao.FreeGlut;
using Tao.OpenGl;
using Tao.Platform.Windows;

namespace LayoutEditor
{
    public partial class Form1 : Form
    {

        private Pen grid_minor_pen = new Pen(Color.FromArgb(10, Color.Gray), 1);

        private Pen grid_major_pen = new Pen(Color.FromArgb(50, Color.Gray), 1);

        private PointF shift = new PointF(0, 0);

        private Boolean is_dragging = false;

        private PointF drag_start = new PointF();

        private PointF shift_at_drag_start = new PointF();

        private Font footer_font = new Font(FontFamily.GenericSansSerif, 10);

        private Pen module_border = new Pen(Color.Black, 2);

        private Font module_font = new Font(FontFamily.GenericSansSerif, 24);

        private int n = 10;

        private int turn = 0;

        private long[] old_ticks = new long[10];

        Graphics g;

        Region r = new Region();

        public Form1() {

            InitializeComponent();

            g = pictureBox1.CreateGraphics();

            for (int i = 0; i < 10; i++) {
                old_ticks[i] = 0;
            }

            this.Size = new Size(1300, 1000);

            this.Refresh();




        }

        private void Form1_Load(object sender, EventArgs e) {
            this.Show();
            simpleOpenGlControl1.InitializeContexts();
            while (false) {
                shift.X += 1.5f;
                shift.Y += 1.5f;
                //  pictureBox1.Invalidate();
                System.Threading.Thread.Sleep(15);
                updateFPS();
                simpleOpenGlControl1.Refresh();
            }
            pictureBox1.Hide();
            simpleOpenGlControl1.Show();
        }

        private void pictureBox1_Click(object sender, EventArgs e) {

        }


        // private void test_texture() {
        //     Bitmap image = new Bitmap("d:\test.jpg");
        //     image.RotateFlip(RotateFlipType.RotateNoneFlipY);

        //     Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
        //     var bitmapdata = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
        //                     System.Drawing.Imaging.PixelFormat.Format24bppRgb);

        //     // storage for texture for one picture

        //     Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture[3]);
        //     Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, (int)Gl.GL_RGB8, image.Width, image.Height,
        //         0, Gl.GL_BGR_EXT, Gl.GL_UNSIGNED_BYTE, bitmapdata.Scan0);
        //     Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);       // Linear Filtering
        //     Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
        // }

        private void pictureBox1_Paint(object sender, PaintEventArgs e) {

            Graphics g = e.Graphics;

            //pictureBox1.SuspendLayout();

            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            //g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;

            //g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            //   drawBackground(g);

            drawGrid(g, 25, grid_minor_pen, shift);
            drawGrid(g, 100, grid_major_pen, shift);

            drawObjects(g, shift);

            drawFooter(g);

            turn = 0;

        }


        private void drawObjects(Graphics g, PointF shift) {

            g.TranslateTransform(shift.X, shift.Y);

            drawModule(g, 0, 0, 200, 200, "module1");
            drawModule(g, 300, 400, 200, 300, "module2");

            g.TranslateTransform(-shift.X, -shift.Y);
        }

        private void drawObjects_GL(PointF shift) {

            int shift_x = (int)shift.X;
            int shift_y = (int)shift.Y;

            Gl.glTranslatef(shift_x, shift_y, 0);

            drawModule_GL(0, 0, 200, 200, "module1");
            drawModule_GL(300, 400, 200, 300, "module2");

            Gl.glTranslatef(-shift_x, -shift_y, 0);
        }


        private void drawModule(Graphics g, float cx, float cy, float w, float h, String name) {

            float x = cx - w * 0.5f;
            float y = cy - h * 0.5f;

            g.FillRectangle(Brushes.White, x, y, w, h);
            g.DrawRectangle(module_border, x, y, w, h);

            SizeF ms = g.MeasureString(name, module_font);

            g.DrawString(name, module_font, Brushes.Black, cx - ms.Width * 0.5f, cy - ms.Height * 0.5f);
        }

        private void drawModule_GL(float cx, float cy, float w, float h, String name) {

            float x = cx - w * 0.5f;
            float y = cy - h * 0.5f;

            //Gl.glClearColor(0.1f, 0, 0, 1);
            Gl.glColor3f(1f, 1f, 1f);
            Gl.glRectf(x, y, x+w, y+h);

            Gl.glColor3f(0, 0, 0);

            drawLine(x, y, x + w, y, 0);
            drawLine(x, y+h, x + w, y+h, 0);




            //g.FillRectangle(Brushes.White, x, y, w, h);
            //g.DrawRectangle(module_border, x, y, w, h);

            //SizeF ms = g.MeasureString(name, module_font);

            //g.DrawString(name, module_font, Brushes.Black, cx - ms.Width * 0.5f, cy - ms.Height * 0.5f);
        }

        private void drawFooter(Graphics g) {

            float w = g.ClipBounds.Width;
            float h = g.ClipBounds.Height;

            String str_footer = String.Format("({0}, {1})", shift.X, shift.Y);

            SizeF ss = g.MeasureString(str_footer, footer_font);

            float margin = 10;

            g.DrawString(str_footer, footer_font, Brushes.Gray, w - ss.Width - margin, h - ss.Height - margin);
        }

        private void drawBackground(Graphics g) {

            float w = g.ClipBounds.Width;
            float h = g.ClipBounds.Height;

            g.FillRectangle(Brushes.White, 0, 0, w, h);
        }

        private void drawGrid_GL(float spacing, Pen p, PointF shift) {

            float w = simpleOpenGlControl1.Width;
            float h = simpleOpenGlControl1.Height;

            int grid_xlines_count = (int)(w / spacing) + 1;
            int grid_ylines_count = (int)(h / spacing) + 1;

            float shift_x = shift.X % spacing;
            float shift_y = shift.Y % spacing;

            Gl.glTranslatef(shift_x, shift_y, 0);

            foreach (float ind in Enumerable.Range(0, grid_xlines_count + 1)) {

                float x = ind * spacing;

                drawLine(x, -spacing, x, h + spacing, 0.9f);

            }

            foreach (float ind in Enumerable.Range(0, grid_ylines_count + 1)) {

                float y = ind * spacing;

                drawLine(-spacing, y, w + spacing, y, 0.9f);
            }

           Gl.glTranslatef(-shift_x, -shift_y, 0);

        }

        private void drawGrid(Graphics g, float spacing, Pen p, PointF shift) {

            float w = g.ClipBounds.Width;
            float h = g.ClipBounds.Height;

            int grid_xlines_count = (int)(w / spacing) + 1;
            int grid_ylines_count = (int)(h / spacing) + 1;

            float shift_x = shift.X % spacing;
            float shift_y = shift.Y % spacing;

            g.TranslateTransform(shift_x, shift_y);



            foreach (float ind in Enumerable.Range(0, grid_xlines_count + 1)) {

                float x = ind * spacing;

                g.DrawLine(p, x, -spacing, x, h + spacing);

            }

            foreach (float ind in Enumerable.Range(0, grid_ylines_count + 1)) {

                float y = ind * spacing;

                g.DrawLine(p, -spacing, y, w + spacing, y);
            }

            g.TranslateTransform(-shift_x, -shift_y);

        }

        private void invalidate_mod_area(float x, float y, float w, float h) {

            int ix = (int) (x - w * 0.5f + shift.X);
            int iy = (int) (y - h * 0.5f + shift.Y);

            pictureBox1.Invalidate(new Rectangle(ix - 1, iy - 1, (int) w+2, (int) h+2));
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e) {

            if (e.Button == MouseButtons.Middle) {

                if (is_dragging) {

                    float dx = e.X - drag_start.X;
                    float dy = e.Y - drag_start.Y;

                    shift.X = shift_at_drag_start.X + dx;
                    shift.Y = shift_at_drag_start.Y + dy;

                    //invalidate_mod_area(0, 0, 200, 200);
                    //invalidate_mod_area(300, 400, 200, 300);

                    pictureBox1.Invalidate();

                } else {

                    is_dragging = true;
                    drag_start.X = e.X;
                    drag_start.Y = e.Y;

                    shift_at_drag_start.X = shift.X;
                    shift_at_drag_start.Y = shift.Y;

                    pictureBox1.Cursor = Cursors.SizeAll;
                }

            } else {

                if (is_dragging) {

                    is_dragging = false;
                    pictureBox1.Cursor = Cursors.Default;
                }
            }
        }

        private void updateFPS() {

            long t = DateTime.UtcNow.Ticks;

            long dt = t - old_ticks[0];
            double dt_sec = dt / 1e7d;
            double fps = 1.0f / dt_sec * n;
            this.Text = String.Format("FPS = {0:f}", fps);

            for (int i = 0; i < n - 1; i++) {
                old_ticks[i] = old_ticks[i + 1];
            }
            old_ticks[n - 1] = t;
        }

        private void pictureBox1_Resize(object sender, EventArgs e) {

            pictureBox1.Refresh();
        }

        private void simpleOpenGlControl1_Paint(object sender, PaintEventArgs e) {

            int window_width = simpleOpenGlControl1.Width;
            int window_height = simpleOpenGlControl1.Height;
            Gl.glViewport(0, 0, window_width, window_height);

            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Gl.glOrtho(0, window_width, 0, window_height, -1, 1);

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();

            float bg = 1f;

            Gl.glClearColor(bg, bg, bg, 0f);
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);

            Gl.glLineWidth(1f);


            //Gl.glTranslatef(shift.X, -shift.Y, 0);

            //Gl.glScalef(1, -1, 1);

            // for (float i = 0; i < window_height; i += 50f) {

            //     drawLine(0, i, window_width, i, 0.9f);

            // }

            // for (float i = 0; i < window_width; i += 50f) {

            //     drawLine(i, 0, i, window_height, 0.9f);

            // }

            //Gl.glRectf(50, 550, 200, 700);
            //Gl.glRectf(50 + 200, 550, 200 + 200, 700);

            drawGrid_GL(25, grid_minor_pen, shift);
            drawGrid_GL(100, grid_major_pen, shift);

            //drawObjects(g, shift);
            drawObjects_GL(shift);

            Gl.glFlush();

        }

          private void drawLine(float x1, float y1, float x2, float y2, float c) {
            Gl.glColor3f(c, c, c);
            Gl.glBegin(Gl.GL_LINES);
            Gl.glVertex2f(x1, y1);
            Gl.glVertex2f(x2, y2);
            Gl.glEnd();
        }

        private void simpleOpenGlControl1_MouseMove(object sender, MouseEventArgs e) {

             if (e.Button == MouseButtons.Middle) {

                if (is_dragging) {

                    float dx = e.X - drag_start.X;
                    float dy = e.Y - drag_start.Y;

                    shift.X = shift_at_drag_start.X + dx;
                    shift.Y = shift_at_drag_start.Y - dy;

                    simpleOpenGlControl1.Invalidate();

                } else {

                    is_dragging = true;
                    drag_start.X = e.X;
                    drag_start.Y = e.Y;

                    shift_at_drag_start.X = shift.X;
                    shift_at_drag_start.Y = shift.Y;

                    simpleOpenGlControl1.Cursor = Cursors.SizeAll;
                }

            } else {

                if (is_dragging) {

                    is_dragging = false;
                    simpleOpenGlControl1.Cursor = Cursors.Default;
                }
            }

            //simpleOpenGlControl1.Invalidate(new Rectangle(0, 0, 500, 500));
        }
    }
}
