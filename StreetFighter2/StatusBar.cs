using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreetFighter2
{
    class StatusBar
    {
        private int currentValue;
        private int maxValue;
        private int barWidth;
        private char fillChar;
        private char emptyChar;

        public StatusBar(int currentValue, int maxValue, int barWidth = 20, char fillChar = '█', char emptyChar = '░')
        {
            this.currentValue = currentValue;
            this.maxValue = maxValue;
            this.barWidth = barWidth;
            this.fillChar = fillChar;
            this.emptyChar = emptyChar;
        }

        public void UpdateValue(int newValue)
        {
            currentValue = Math.Clamp(newValue, 0, maxValue);
        }

        public int CurrentValue => currentValue;
        public int MaxValue => maxValue;
        public double Percentage => maxValue > 0 ? (double)currentValue / maxValue : 0;

        public string BuildBar()
        {
            if (maxValue <= 0) return new string(emptyChar, barWidth);

            int filledBlocks = (int)Math.Ceiling((double)currentValue / maxValue * barWidth);
            filledBlocks = Math.Clamp(filledBlocks, 0, barWidth);

            string filled = new string(fillChar, filledBlocks);
            string empty = new string(emptyChar, barWidth - filledBlocks);

            return filled + empty;
        }

       
        public bool IsReady(int threshold)
        {
            return currentValue >= threshold;
        }
    }
}