using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Tao.OpenGl;


namespace LayoutEditor
{
    public class OpenGL
    {
        private static void setColor(Color c) {
            Gl.glColor3f(c.R / 255f, c.G / 255f, c.B / 255f);
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
            Gl.glRectf(x, y, x+w, y+h);

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
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
        }

        public static void flush() {
            Gl.glFlush();
        }
    }
}
