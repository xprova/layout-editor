using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Tao.OpenGl;
using System.Drawing.Imaging;

namespace LayoutEditor
{
    public class OpenGL
    {

        private static void setColor(Color c) {

            Gl.glColor3f(c.R / 255f, c.G / 255f, c.B / 255f);
        }

        public static int[] makeGroupTextures(Bitmap[] bmps) {

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

        public static void drawTexture(int index, float x, float y, float w, float h, float alpha) {

            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, index);

            Gl.glColor4f(1, 1, 1, alpha);

            Gl.glBegin(Gl.GL_POLYGON);

            Gl.glTexCoord2f(0, 1); Gl.glVertex2f(x, y);
            Gl.glTexCoord2f(0, 0); Gl.glVertex2f(x, y+h);
            Gl.glTexCoord2f(1, 0); Gl.glVertex2f(x+w, y+h);
            Gl.glTexCoord2f(1, 1); Gl.glVertex2f(x+w, y);

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
