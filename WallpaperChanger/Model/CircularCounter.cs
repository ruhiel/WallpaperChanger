using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallpaperChanger.Model
{
    public class CircularCounter
    {
        private int maxValue;
        private int currentValue;

        public CircularCounter(int maxValue)
        {
            if (maxValue <= 0)
            {
                throw new ArgumentException("The maximum value must be greater than zero.");
            }

            this.maxValue = maxValue;
            this.currentValue = 0;
        }

        public int GetCounter()
        {
            int result = currentValue;
            currentValue = (currentValue + 1) % maxValue;
            return result;
        }
    }
}
