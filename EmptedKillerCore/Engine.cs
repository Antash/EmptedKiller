using System;
using System.Collections.Generic;
using System.Linq;

namespace EmptedKillerCore
{
    class Engine
    {
        public List<IPosition> lines = new List<IPosition>();

        private readonly IEvaluate _evaluationEngine;

        public Engine(IEvaluate evaluationEngine)
        {
            _evaluationEngine = evaluationEngine;
        }

        public void Analyze(IPosition position)
        {
            lines.AddRange(AnalyzeLines(position, 4));
        }

        private List<IPosition> AnalyzeLines(IPosition initialLine, int depth, float min = float.MaxValue, float max = float.MinValue)
        {
            if (depth == 0)
            {
                return new[] { initialLine }.ToList();
            }
            bool whiteToMove = initialLine.WhiteToMove;
            List<IPosition> bestLines = new List<IPosition>();
            var bestEvaluation = whiteToMove ? float.MinValue : float.MaxValue;
            var linesToConsider = initialLine.GetValidMoves().Select(m => initialLine.MakeMove(m));
           
            foreach (var line in linesToConsider)
            {
                var consideredLines = AnalyzeLines(line, depth - 1, min, max);
                foreach (var consideredLine in consideredLines)
                {
                    if (whiteToMove && bestEvaluation < _evaluationEngine.Evaluate(consideredLine) ||
                        !whiteToMove && bestEvaluation > _evaluationEngine.Evaluate(consideredLine))
                    {
                        bestLines.Clear();
                        bestLines.Add(consideredLine);
                        bestEvaluation = _evaluationEngine.Evaluate(consideredLine);
                    }
                    else if (bestEvaluation == _evaluationEngine.Evaluate(consideredLine))
                    {
                        bestLines.Add(consideredLine);
                    }

                    if (!whiteToMove)
                    {
                        min = Math.Min(min, bestEvaluation);
                    }
                    else
                    {
                        max = Math.Max(max, bestEvaluation);
                    }
                    if (min < max)
                    {
                        break;
                    }
                }
            }
            return bestLines;
        }
    }
}
