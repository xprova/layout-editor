using System;
using System.Collections.Generic;
using System.Collections;
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

        private static int zoom_level = 0;

        // styles

        private static Pen module_border = new Pen(Color.Black, 2);

        private static Color module_fill = Color.White;

        private static Font module_font = new Font("Consolas", 16, FontStyle.Regular);

        private static Pen grid_major_pen = new Pen(Color.FromArgb(255, 220, 220, 220), 1);

        private static Pen grid_minor_pen = new Pen(Color.FromArgb(255, 230, 230, 230), 1);

        private static Color bg = Color.FromArgb(240, 240, 240);

        private static Dictionary<String, Bitmap> bitmaps = new Dictionary<string, Bitmap>();

        public static List<Module> modules = new List<Module>();

        // body

        private static void init(SizeF canvas_size) {

            float cw = canvas_size.Width;
            float ch = canvas_size.Height;

            canvas = new RectangleF(0, 0, cw, ch);
        }

        public static void drawGrid(float spacing, Pen p, float w, float h) {

            float scale = getScale();

            int grid_xlines_count = (int)(w / scale / spacing) + 1;
            int grid_ylines_count = (int)(h / scale / spacing) + 1;

            float shift_x = (-vCentre.X * scale + w * 0.5f) % (spacing * scale);
            float shift_y = (-vCentre.Y * scale + h * 0.5f) % (spacing * scale);

            OpenGL.translate(shift_x, shift_y);

            OpenGL.setScale(scale);

            foreach (float ind in Enumerable.Range(0, grid_xlines_count + 1)) {

                float x = ind * spacing;

                OpenGL.drawLine(x, -spacing, x, h / scale + spacing, p, false);

            }

            foreach (float ind in Enumerable.Range(0, grid_ylines_count + 1)) {

                float y = ind * spacing;

                OpenGL.drawLine(-spacing, y, w / scale + spacing, y, p, false);
            }

            OpenGL.setScale(1/scale);

            OpenGL.translate(-shift_x, -shift_y);
        }

        private static T crop<T>(T v, T min, T max) where T : System.IComparable {

            v = v.CompareTo(min) > 0 ? v : min;
            v = v.CompareTo(max) < 0 ? v : max;

            return v;
        }

        public static void changeView(float cx, float cy) {

            float b = 500;

            cx = crop(cx, -b, b);
            cy = crop(cy, -b, b);

            vCentre.X = cx;
            vCentre.Y = cy;
        }

        public static void drawObjects(float w, float h) {

            float scale = getScale();

            float shift_x = -vCentre.X * scale + w * 0.5f;
            float shift_y = -vCentre.Y * scale + h * 0.5f;

            OpenGL.translate(shift_x, shift_y);

            OpenGL.setScale(scale);

            foreach (Module mod in modules)
                drawModule(mod.cx, mod.cy, mod.width, mod.height, mod.name);

            //OpenGL.drawTestTriangle(300, 300, 400, 300, 400, 400);

            OpenGL.drawTexture("img1", 150, 150, 1f);
            OpenGL.drawTexture("img2", 800, 200, 1f);

            OpenGL.setScale(1 / scale);

            OpenGL.translate(-shift_x, -shift_y);
        }

        public static void zoom(int direction) {

            zoom_level = crop(zoom_level + direction, -3, 3);

            prepareTextures();

        }

        public static float getScale() {

            // returns drawing scale (ratio of screen unit to view port unit)

            float factor = 1.2f;

            return (float) Math.Pow(factor, zoom_level);
        }

        private static void drawModule(float cx, float cy, float w, float h, String name) {

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

            drawGrid(25, grid_minor_pen, w, h);

            drawGrid(100, grid_major_pen, w, h);

            drawObjects(w, h);

            OpenGL.flush();
        }

        public static void resize(int w, int h) {

            OpenGL.init(w, h);

        }

        public static void init() {

            prepareTextures();
        }

        public static Bitmap testBmp() {
            string str = "module1";
            return OpenGL.createLabelBmp(str, module_font, false, getScale());
        }

        public static void prepareTextures() {

            List<Bitmap> bmps = new List<Bitmap>();

            List<string> ids = new List<string>();

            bmps.Add(new Bitmap(@"d:\test2.jpg"));
            bmps.Add(new Bitmap(@"d:\test5.png"));

            ids.Add("img1");
            ids.Add("img2");

            int n = modules.Count;

            foreach (int i in Enumerable.Range(0, n)) {

                String str = modules[i].name;

                bmps.Add(OpenGL.createLabelBmp(str, module_font, false, getScale()));

                ids.Add(str);

            }

            OpenGL.uploadTextures(ids, bmps);
        }
    }
}
