using System;
using System.Drawing;
using System.Windows.Forms;
using Tao.OpenGl;

namespace LayoutEditor
{
    public partial class Form1 : Form
    {
        private Boolean is_dragging = false;

        private PointF drag_start = new PointF();

        private PointF vCenter0 = new PointF(); // vCentre at the beginning of a drag operation

        public Form1() {

            InitializeComponent();

            label1.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;

            simpleOpenGlControl1.MouseWheel += SimpleOpenGlControl1_MouseWheel;

            CenterToScreen();

            Refresh();

            pictureBox1.BackgroundImage = Renderer.testBmp();

        }

        private void SimpleOpenGlControl1_MouseWheel(object sender, MouseEventArgs e) {

            simpleOpenGlControl1.Invalidate();

            Renderer.zoom(Math.Sign(e.Delta));

            simpleOpenGlControl1.Invalidate();

            pictureBox1.BackgroundImage = Renderer.testBmp();

            pictureBox1.Invalidate();

        }

        private void Form1_Load(object sender, EventArgs e) {

            simpleOpenGlControl1.InitializeContexts();

            Gl.ReloadFunctions();

            simpleOpenGlControl1_Resize(null, null); // trigger initialization

            Renderer.modules.Add(new Module(0, 0, 200, 200, "module1"));

            Renderer.modules.Add(new Module(500, 600, 200, 300, "module2"));

            Renderer.modules.Add(new Module(-200, 600, 200, 300, "norGate1"));

            Renderer.init();

        }

        private void simpleOpenGlControl1_Paint(object sender, PaintEventArgs e) {

            float w = simpleOpenGlControl1.Width;
            float h = simpleOpenGlControl1.Height;

            Renderer.render(w, h);

            PointF vCentre = Renderer.getCentre();

            double x = Math.Round(vCentre.X);
            double y = Math.Round(vCentre.Y);

            double z = Math.Round(Renderer.getScale() * 100);

            label1.Text = String.Format("({0}, {1}) at {2}%", x, y, z);

            label1.Left = this.Width - label1.Width- 25;

        }

        private void simpleOpenGlControl1_MouseMove(object sender, MouseEventArgs e) {

            // mouse coordinates (relative)

            float mx = -e.X / Renderer.getScale();
            float my = e.Y / Renderer.getScale();

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

            Renderer.resize(w, h);

        }

    }
}
