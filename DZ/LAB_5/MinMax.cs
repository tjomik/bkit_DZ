using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LAB_5
{
    class MinMax
    {
       public int Min
        {
            get;
            set;
        }
        public int Max
        {
            get ;
            set ;
        }
        public MinMax(int a, int b)
        {
            Min = a;
            Max = b;
        }


    }
}