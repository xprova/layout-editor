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

        private Pen grid_major_pen = new Pen(Color.FromArgb(255, 220, 220, 220), 1);

        private Pen grid_minor_pen = new Pen(Color.FromArgb(255 ,230, 230, 230), 1);

        private Color bg = Color.FromArgb(240, 240, 240);

        private Color module_fill = Color.White;

        private PointF shift = new PointF(0, 0);

        private Boolean is_dragging = false;

        private PointF drag_start = new PointF();

        private PointF shift_at_drag_start = new PointF();

        private Font footer_font = new Font(FontFamily.GenericSansSerif, 10);

        private Pen module_border = new Pen(Color.Black, 2);

        private Font module_font = new Font(FontFamily.GenericSansSerif, 24);

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

            this.CenterToScreen();

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
                //updateFPS();
                simpleOpenGlControl1.Refresh();
            }
            pictureBox1.Hide();
            simpleOpenGlControl1.Show();
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


        private void drawObjects_GL(PointF shift) {

            int shift_x = (int)shift.X;
            int shift_y = (int)shift.Y;

            OpenGL.translate(shift_x, shift_y);

            drawModule_GL(200, 200, 200, 200, "module1");
            drawModule_GL(500, 600, 200, 300, "module2");

            OpenGL.translate(-shift_x, -shift_y);
        }

        private void drawModule_GL(float cx, float cy, float w, float h, String name) {

            float x = cx - w * 0.5f;
            float y = cy - h * 0.5f;

            OpenGL.fillRectangle(x, y, w, h, module_fill);

            OpenGL.drawRectangle(x, y, w, h, module_border);

            //SizeF ms = g.MeasureString(name, module_font);

            //g.DrawString(name, module_font, Brushes.Black, cx - ms.Width * 0.5f, cy - ms.Height * 0.5f);
        }

        private void drawGrid_GL(float spacing, Pen p, PointF shift) {

            float w = simpleOpenGlControl1.Width;
            float h = simpleOpenGlControl1.Height;

            int grid_xlines_count = (int)(w / spacing) + 1;
            int grid_ylines_count = (int)(h / spacing) + 1;

            float shift_x = shift.X % spacing;
            float shift_y = shift.Y % spacing;

            OpenGL.translate(shift_x, shift_y);

            foreach (float ind in Enumerable.Range(0, grid_xlines_count + 1)) {

                float x = ind * spacing;

                OpenGL.drawLine(x, -spacing, x, h + spacing, p);

            }

            foreach (float ind in Enumerable.Range(0, grid_ylines_count + 1)) {

                float y = ind * spacing;

                OpenGL.drawLine(-spacing, y, w + spacing, y, p);
            }

            OpenGL.translate(-shift_x, -shift_y);

        }

        private void simpleOpenGlControl1_Paint(object sender, PaintEventArgs e) {

            int w = simpleOpenGlControl1.Width;
            int h = simpleOpenGlControl1.Height;

            OpenGL.init(w, h);
            OpenGL.clear(bg);

            drawGrid_GL(25, grid_minor_pen, shift);
            drawGrid_GL(100, grid_major_pen, shift);

            drawObjects_GL(shift);

            OpenGL.flush();

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

        }
    }
}
