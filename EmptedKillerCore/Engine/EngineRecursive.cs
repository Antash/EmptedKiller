using EmptedKillerCore.Evaluation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EmptedKillerCore.Engine
{
    public class EngineRecursive
    {
        public List<IPosition> lines = new List<IPosition>();

        private readonly IEvaluate _evaluationEngine;

        public EngineRecursive(IEvaluate evaluationEngine)
        {
            _evaluationEngine = evaluationEngine;
        }

        public void Analyze(IPosition position)
        {
            lines.AddRange(AnalyzeLines(position, 4));
        }

        private List<IPosition> AnalyzeLines(IPosition initialLine, int depth, float min = float.MaxValue, float max = float.MinValue)
        {
            if (depth == 0 || (Math.Abs(_evaluationEngine.Evaluate(initialLine)) == IEvaluate.MaxEvaluation))
            {
                return new[] { initialLine }.ToList();
            }
            bool whiteToMove = initialLine.WhiteToMove;
            List<IPosition> bestLines = new List<IPosition>();
            var bestEvaluation = whiteToMove ? -IEvaluate.MaxEvaluation : IEvaluate.MaxEvaluation;
            var linesToConsider = initialLine.GetValidMoves()
                .Select(m => initialLine.MakeMove(m));
           
            foreach (var line in linesToConsider)
            {
                var consideredLines = AnalyzeLines(line, depth - 1, min, max);
                foreach (var consideredLine in consideredLines)
                {
                    var eval = _evaluationEngine.Evaluate(consideredLine);
                    if (whiteToMove && bestEvaluation < eval ||
                        !whiteToMove && bestEvaluation > eval)
                    {
                        bestLines.Clear();
                        bestLines.Add(consideredLine);
                        bestEvaluation = eval;
                    }
                    else if (bestEvaluation == eval)
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
