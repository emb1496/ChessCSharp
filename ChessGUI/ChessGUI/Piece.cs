using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGUI
{
    class Piece
    {
        private int value;
        private bool white;
        private bool hasMoved;

        public bool HasMoved
        {
            get
            {
                return hasMoved;
            }
            set
            {
                hasMoved = value;
            }
        }

        public int Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }
        public bool White
        {
            get
            {
                return white;
            }
            set
            {
                white = value;
            }
        }
        public Piece()
        {
            Value = 0;
            White = false;
        }
        public string toString()
        {
            string s = "";
            return s + (int)Value + White.ToString();
        }
    }
}
