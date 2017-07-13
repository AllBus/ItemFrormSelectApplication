using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ItemFrormSelectApplication
{
    public class AnimateItem
    {
        public int state;
        public double value;
        public long startTime;
        public Label element;

        public AnimateItem(Label label)
        {
            this.element = label;
            this.state = 0;
            this.value = 0;
            this.startTime = 0;
        }
    }

}
