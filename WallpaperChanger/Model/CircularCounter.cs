using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallpaperChanger.Model
{
    public class CircularCounter
    {
        public int MaxValue { get; set; }
        public bool Shuffle { get; set; }

        private int _CurrentValue;

        private Random _Random = new Random();
        public CircularCounter(int maxValue)
        {
            if (maxValue < 0)
            {
                throw new ArgumentException("The maximum value must be greater than zero.");
            }

            MaxValue = maxValue;
            _CurrentValue = 0;
        }

        public int GetCounter()
        {
            if (Shuffle)
            {
                return _Random.Next(0, MaxValue);
            }

            int result = _CurrentValue;
            _CurrentValue = (_CurrentValue + 1) % MaxValue;
            return result;
        }
    }
}
