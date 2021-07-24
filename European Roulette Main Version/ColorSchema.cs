using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace European_Roulette_Main_Version
{
    public class ColorSchema
    {
        public ColorSchema(int count, int green, int yellow, int red)
        {
            Count = count;
            Green = green;
            Yellow = yellow;
            Red = red;
        }

        public int Count { get; set; }
        public int Green { get; set; }
        public int Yellow { get; set; }
        public int Red { get; set; }
    }
}
