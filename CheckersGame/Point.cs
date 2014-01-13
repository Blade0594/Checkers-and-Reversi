using System;
using System.Collections.Generic;
using System.Text;


namespace CheckersGame
{
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Point(){}
        public Point(int x,int y)
        {
            this.X = x;
            this.Y = y;
        }
        public bool IsEqual(Point secondPoint)
        {

            return this.X == secondPoint.X && this.Y == secondPoint.Y;
        }
        public bool IsEqual(int X, int Y)
        {
            return this.X == X && this.Y == Y;
        }

       
       
       
    }
}
