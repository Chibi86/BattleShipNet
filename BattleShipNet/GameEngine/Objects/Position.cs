using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    public class Position
    {
        private int x;
        private int y;

        /// <summary>
        /// Properties for x - get and set
        /// </summary>
        public int X
        {
            get
            {
                return x;
            }
            set
            {
                if(value > 0 && value < 11)
                {
                    x = value;
                }
                else
                {
                    throw new Exception("Position x cannot be under 1 or over 10.");
                }
            }
        }

        /// <summary>
        /// Properties for y - get & set
        /// </summary>
        public int Y
        {
            get
            {
                return y;
            }
            set
            {
                if (value > 0 && value < 11)
                {
                    y = value;
                }
                else
                {
                    throw new Exception("Position y cannot be under 1 or over 10.");
                }
            }
        }

        /// <summary>
        /// Constructor with position x and y
        /// </summary>
        /// <param name="newX">Position x (int)</param>
        /// <param name="newY">Position y (int)</param>
        public Position(int newX, int newY)
        {
            X = newX;
            Y = newY;
        }
    }
}
