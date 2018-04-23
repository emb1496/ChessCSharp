using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessServer
{
    /// <summary>
    /// This is a class with attributes for value, color and whether or not a piece has moved
    /// </summary>
    public class Piece
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

        /// <summary>
        /// Constructor for Piece which sets the value and color of the piece
        /// </summary>
        /// <param name="val">piece value</param>
        /// <param name="isWhite">piece color</param>
        public Piece(int val, bool isWhite)
        {
            value = val;
            white = isWhite;
        }
    }
}
