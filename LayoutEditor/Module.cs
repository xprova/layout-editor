using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutEditor
{
    class Module
    {
        public float width, height, cx, cy;

        public String name;

        public Module (float cx, float cy, float width, float height, String name) {

            this.width = width;
            this.height = height;
            this.cx = cx;
            this.cy = cy;
            this.name = name;
        }
    }
}
