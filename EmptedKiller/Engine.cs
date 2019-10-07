using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmptedKiller
{
    class Engine
    {
        public List<Line> lines = new List<Line>();

        public Engine()
        {
        }

        public void Analyze(Position position)
        {
            lines.Add(AnalyzeLines(position, 4));
        }

        private Line AnalyzeLines(Position line, int depth, float min = float.MaxValue, float max = float.MinValue)
        {
            if (depth == 0)
            {
                return line as Line;
            }
            var evaluation = line.whiteToMove ? float.MinValue : float.MaxValue;
            Line bestLine = null;
            var lines = line.GetValidMoves().Select(m => line.MakeMove(m, true));
            foreach (var l in lines)
            {
                var ll = AnalyzeLines(l, depth - 1, min, max);
                if (line.whiteToMove && evaluation < ll.GetEvaluation() ||
                    !line.whiteToMove && evaluation > ll.GetEvaluation())
                {
                    bestLine = ll;
                    evaluation = ll.GetEvaluation();
                }

                if (!line.whiteToMove)
                {
                    min = Math.Min(min, evaluation);
                }
                else
                {
                    max = Math.Max(max, evaluation);
                }
                if (min < max)
                {
                    break;
                }
            }
            return bestLine;
        }
    }
}
