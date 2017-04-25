using System;
using System.Linq;
using System.Drawing;
using Tao.OpenGl;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System.Windows.Forms;

namespace LayoutEditor
{
    public class OpenGL
    {

        private static Bitmap dummy_bmp = new Bitmap(1, 1);

        private static Graphics dummy_graphics = Graphics.FromImage(dummy_bmp);

        private static Dictionary<string, int> texture_ids = new Dictionary<string, int>();

        private static Dictionary<string, SizeF> texture_sizes = new Dictionary<string, SizeF>();

        public static Bitmap createLabelBmp(String str, Font font, bool draw_border) {

            Size size = dummy_graphics.MeasureString(str, font).ToSize();

            Bitmap bmp = new Bitmap(size.Width, size.Height);

            RectangleF rectf = new RectangleF(0, 0, (int)(size.Width * 1.5f), size.Height);

            Graphics g = Graphics.FromImage(bmp);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            g.DrawString(str, font, Brushes.Black, rectf);

            if (draw_border)
                g.DrawRectangle(Pens.Black, 0, 0, size.Width, size.Height);

            g.Flush();

            return bmp;

        }

        public static void uploadTextures(string[] str_ids, Bitmap[] bmps) {

            int n = bmps.Length;

            var int_ids = makeGroupTextures(bmps);

            foreach (int i in Enumerable.Range(0, n)) {

                texture_ids.Add(str_ids[i], int_ids[i]);

                int w = bmps[i].Size.Width;
                int h = bmps[i].Size.Height;

                texture_sizes.Add(str_ids[i], new Size(w, h));
            }

        }

        private static void setColor(Color c) {

            Gl.glColor3f(c.R / 255f, c.G / 255f, c.B / 255f);
        }

        private static int[] makeGroupTextures(Bitmap[] bmps) {

            int n = bmps.Length;

            int[] indices = new int[n];

            Gl.glGenTextures(n, indices); //where 1 is the count, and index a new texture slot in the gpu

            foreach (int i in Enumerable.Range(0, n)) {

                Bitmap bmp = bmps[i];

                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

                var bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                Gl.glBindTexture(Gl.GL_TEXTURE_2D, indices[i]);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);

                Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, (int)Gl.GL_RGBA, bmp.Width, bmp.Height, 0, Gl.GL_BGRA, Gl.GL_UNSIGNED_BYTE, bmpData.Scan0);

            }

            return indices;

        }

        public static void drawTexture(String str_id, float cx, float cy, float alpha) {

            int int_id;

            texture_ids.TryGetValue(str_id, out int_id);

            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, int_id);

            Gl.glColor4f(1, 1, 1, alpha);

            Gl.glBegin(Gl.GL_POLYGON);

            SizeF sz;

            texture_sizes.TryGetValue(str_id, out sz);

            float w = sz.Width;
            float h = sz.Height;

            float x = cx - (int) (w * 0.5f);
            float y = cy - (int) (h * 0.5f);

            Gl.glTexCoord2f(0, 1); Gl.glVertex2f(x, y);
            Gl.glTexCoord2f(0, 0); Gl.glVertex2f(x, y + h);
            Gl.glTexCoord2f(1, 0); Gl.glVertex2f(x + w, y + h);
            Gl.glTexCoord2f(1, 1); Gl.glVertex2f(x + w, y);

            Gl.glEnd();

            Gl.glDisable(Gl.GL_TEXTURE_2D);
        }

        public static void drawLine(float x1, float y1, float x2, float y2, Pen p) {

            setColor(p.Color);
            Gl.glLineWidth(p.Width);
            Gl.glBegin(Gl.GL_LINES);
            Gl.glVertex2f(x1, y1);
            Gl.glVertex2f(x2, y2);
            Gl.glEnd();
        }

        public static void drawRectangle(float x, float y, float w, float h, Pen p) {

            OpenGL.drawLine(x, y, x + w, y, p);
            OpenGL.drawLine(x, y + h, x + w, y + h, p);
            OpenGL.drawLine(x, y, x, y + h, p);
            OpenGL.drawLine(x + w, y, x + w, y + h, p);
        }

        public static void fillRectangle(float x, float y, float w, float h, Color c) {

            Gl.glColor3f(c.R / 255f, c.G / 255f, c.B / 255f);
            Gl.glRectf(x, y, x + w, y + h);

        }

        public static void translate(float x, float y) {

            Gl.glTranslatef(x, y, 0);
        }

        public static void clear(Color c) {

            Gl.glClearColor(c.R / 255f, c.G / 255f, c.B / 255f, 0f);
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);
        }

        public static void init(int width, int height) {

            Gl.glViewport(0, 0, width, height);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Gl.glOrtho(0, width, 0, height, -1, 1);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);

        }

        public static void drawTestTriangle(int x1, int y1, int x2, int y2, int x3, int y3) {

            Gl.glBegin(Gl.GL_TRIANGLES);
            Gl.glColor4f(1, 0, 0, 1f); Gl.glVertex2f(x1, y1);
            Gl.glColor4f(0, 1, 0, 0f); Gl.glVertex2f(x2, y2);
            Gl.glColor4f(0, 0, 1, 1f); Gl.glVertex2f(x3, y3);
            Gl.glEnd();
        }

        public static void flush() {

            Gl.glFlush();
        }

    }

}
