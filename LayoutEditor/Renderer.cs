using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutEditor
{
    class Renderer
    {
        // This class manages rendering by maintaining three rectangles:
        // canvas, view port and screen. Canvas is the entire drawing
        // workspace and its dimensions are usually (much) larger than the
        // screen. The view port is the portion of canvas that is rendered and
        // screen is the window/control onto which the view port is rendered.
        // All three have pixel coordinate systems.

        private static RectangleF canvas;

        private static PointF vCentre; // view centre

        private static float scale = 1; // zoom factor

        // styles

        private static Pen module_border = new Pen(Color.Black, 2);

        private static Color module_fill = Color.White;

        private static Pen grid_major_pen = new Pen(Color.FromArgb(255, 220, 220, 220), 1);

        private static Pen grid_minor_pen = new Pen(Color.FromArgb(255, 230, 230, 230), 1);

        private static Color bg = Color.FromArgb(240, 240, 240);

        // body

        private static void init(SizeF canvas_size) {

            float cw = canvas_size.Width;
            float ch = canvas_size.Height;

            canvas = new RectangleF(0, 0, cw, ch);

        }

        public static void drawGrid_GL(float spacing, Pen p, float w, float h) {

            int grid_xlines_count = (int)(w / spacing) + 1;
            int grid_ylines_count = (int)(h / spacing) + 1;

            float shift_x = vCentre.X % spacing;
            float shift_y = vCentre.Y % spacing;

            OpenGL.translate(shift_x, shift_y);

            foreach (float ind in Enumerable.Range(0, grid_xlines_count + 1)) {

                float x = ind * spacing;

                OpenGL.drawLine(x, -spacing, x, h + spacing, p, false);

            }

            foreach (float ind in Enumerable.Range(0, grid_ylines_count + 1)) {

                float y = ind * spacing;

                OpenGL.drawLine(-spacing, y, w + spacing, y, p, false);
            }

            OpenGL.translate(-shift_x, -shift_y);

        }

        public static void changeView(float cx, float cy) {

        	vCentre.X = cx;
        	vCentre.Y = cy;

        }

        public static void drawObjects_GL() {

            OpenGL.translate(vCentre.X, vCentre.Y);

            drawModule_GL(200, 200, 200, 200, "module1");

            drawModule_GL(500, 600, 200, 300, "module2");

            OpenGL.translate(-vCentre.X, -vCentre.Y);
        }

        private static void drawModule_GL(float cx, float cy, float w, float h, String name) {

            float x = cx - w * 0.5f;
            float y = cy - h * 0.5f;

            OpenGL.fillRectangle(x, y, w, h, module_fill);

            OpenGL.drawRectangle(x, y, w, h, module_border);

            OpenGL.drawTexture(name, cx, cy - (h/2) - 25, 1f);

        }

        public static PointF getCentre() {

        	return new PointF(vCentre.X, vCentre.Y);
        }

        public static void render(float w, float h) {

        	OpenGL.clear(bg);

        	drawGrid_GL(25, grid_minor_pen, w, h);

        	drawGrid_GL(100, grid_major_pen, w, h);

        	drawObjects_GL();

        	// OpenGL.drawTestTriangle(500, 500, 600, 500, 600, 600);

        	// OpenGL.drawTexture("img1", 150, 150, 1f);
        	// OpenGL.drawTexture("img2", 800, 200, 1f);
        	//OpenGL.drawTexture("mod", 500, 200, 1f);

        	OpenGL.flush();
        }

    }
}
