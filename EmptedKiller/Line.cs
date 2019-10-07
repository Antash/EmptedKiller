using System.Collections.Generic;

namespace EmptedKiller
{
    class Line : Position
    {
        public Line(Position position, string moveCode) : base(position)
        {
            if (position as Line != null)
            {
                moves = new List<string>((position as Line).moves);
            }
            else
            {
                moves = new List<string>();
            }
            moves.Add(moveCode);
        }

        public List<string> moves;

        public override string ToString()
        {
            return string.Join(",", moves);
        }
    }
}
