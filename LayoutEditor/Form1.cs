using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Tao.OpenGl;

namespace LayoutEditor
{
    public partial class Form1 : Form
    {

        private Boolean is_dragging = false;

        private PointF drag_start = new PointF();

        private PointF vCenter0 = new PointF(); // vCentre at the beginning of a drag operation

        private Font footer_font = new Font(FontFamily.GenericSansSerif, 10);

        private Font module_font = new Font("Consolas", 16, FontStyle.Regular);

        public Form1() {

            InitializeComponent();

            label1.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;

            simpleOpenGlControl1.MouseWheel += SimpleOpenGlControl1_MouseWheel;

            this.Size = new Size(1300, 1000);

            this.CenterToScreen();

            this.Refresh();

        }

        private void SimpleOpenGlControl1_MouseWheel(object sender, MouseEventArgs e) {

            float factor = 1.1f;

            float ac_factor = e.Delta > 0 ? factor : 1/factor;

            OpenGL.zoom(ac_factor);

            float cx = simpleOpenGlControl1.Width * 0.5f;
            float cy = simpleOpenGlControl1.Height * 0.5f;

            //shift.X += cx * (1 - ac_factor);
            //shift.Y += cy * (1 - ac_factor);

            simpleOpenGlControl1.Invalidate();

        }

        private void Form1_Load(object sender, EventArgs e) {

            simpleOpenGlControl1.InitializeContexts();

            Gl.ReloadFunctions();

            simpleOpenGlControl1_Resize(null, null); // trigger initialization

            Bitmap[] bmps = {
                new Bitmap(@"d:\test2.jpg"),
                new Bitmap(@"d:\test5.png"),
                OpenGL.createLabelBmp("module1", module_font, false),
                OpenGL.createLabelBmp("module2", module_font, false)
            };

            string[] str_ids = {
                "img1",
                "img2",
                "module1",
                "module2"
            };

            OpenGL.uploadTextures(str_ids, bmps);

        }

        private void simpleOpenGlControl1_Paint(object sender, PaintEventArgs e) {

            float w = simpleOpenGlControl1.Width;
            float h = simpleOpenGlControl1.Height;

            Renderer.render(w, h);

            PointF vCentre = Renderer.getCentre();

            label1.Text = String.Format("({0}, {1})", vCentre.X, vCentre.Y);

            label1.Left = this.Width - label1.Width- 25;

        }

        private void simpleOpenGlControl1_MouseMove(object sender, MouseEventArgs e) {

            // mouse coordinates (relative)

            float mx = -e.X;
            float my = e.Y;

            if (e.Button == MouseButtons.Middle) {

                if (is_dragging) {

                    float dx = mx - drag_start.X;
                    float dy = my - drag_start.Y;

                    float x = vCenter0.X + dx;
                    float y = vCenter0.Y + dy;

                    Renderer.changeView(x, y);

                    // Now we do the same calculations in reverse to calculate
                    // drag_start coordinates. Normally (if changeView changed
                    // the view coordinates to x,y), the re-calculated values
                    // of drag_start will be the same. However, if changeView
                    // did not (say because it capped x or y at a limit) then
                    // the new anchor point will be calculated at this limit
                    // as opposed to beyond it. The upshot is that if dragging
                    // was bounded in any direction then moving the cursor in
                    // the opposite direction will drag in the opposite
                    // direction *immediately*.

                    PointF vCentre = Renderer.getCentre();

                    dx = vCentre.X - vCenter0.X;
                    dy = vCentre.Y - vCenter0.Y;

                    drag_start.X = mx - dx;
                    drag_start.Y = my - dy;

                    simpleOpenGlControl1.Invalidate();

                } else {

                    is_dragging = true;
                    drag_start.X = mx;
                    drag_start.Y = my;

                    PointF vCentre = Renderer.getCentre();

                    vCenter0.X = vCentre.X;
                    vCenter0.Y = vCentre.Y;

                    simpleOpenGlControl1.Cursor = Cursors.SizeAll;
                }

            } else {

                if (is_dragging) {

                    is_dragging = false;
                    simpleOpenGlControl1.Cursor = Cursors.Default;
                }
            }

        }

        private void simpleOpenGlControl1_Resize(object sender, EventArgs e) {

            int w = simpleOpenGlControl1.Width;

            int h = simpleOpenGlControl1.Height;

            OpenGL.init(w, h);
        }

    }
}
