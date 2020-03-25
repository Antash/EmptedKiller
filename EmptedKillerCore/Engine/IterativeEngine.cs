using EmptedKillerCore.Evaluation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace EmptedKillerCore.Engine
{
    public class IterativeEngine
    {
        public List<IPosition> lines = new List<IPosition>();

        private readonly IEvaluate _evaluationEngine;

        private readonly ConcurrentQueue<AnalysisNode> _queue;

        public IterativeEngine(IEvaluate evaluationEngine)
        {
            _evaluationEngine = evaluationEngine;
        }

        public void Analyze(IPosition position)
        {
            _queue.Enqueue(new AnalysisNode(position, _evaluationEngine.Evaluate(position), 0));
            AnalyzeLines();
        }

        private void AnalyzeLines(int min = int.MaxValue, int max = int.MinValue)
        {
            while (_queue.TryDequeue(out var node))
            {
                var nodesToConsider = node.Position.GetValidMoves()
                    .Select(m => {
                        var pos = node.Position.MakeMove(m);
                        var eval = _evaluationEngine.Evaluate(pos);
                        return new AnalysisNode(pos, eval, node.Depth + 1);
                        })
                    .OrderByDescending(n => Math.Abs(n.Evaluation));
                foreach (var n in nodesToConsider)
                {
                    _queue.Enqueue(n);
                }
            }

            //if (depth == 0 || (Math.Abs(_evaluationEngine.Evaluate(initialLine)) == IEvaluate.MaxEvaluation))
            //{
            //    return new[] { initialLine }.ToList();
            //}
            //bool whiteToMove = initialLine.WhiteToMove;
            //List<IPosition> bestLines = new List<IPosition>();
            //var bestEvaluation = whiteToMove ? -IEvaluate.MaxEvaluation : IEvaluate.MaxEvaluation;
            //var linesToConsider = initialLine.GetValidMoves()
            //    .Select(m => initialLine.MakeMove(m));

            //foreach (var line in linesToConsider)
            //{
            //    var consideredLines = AnalyzeLines(line, depth - 1, min, max);
            //    foreach (var consideredLine in consideredLines)
            //    {
            //        var eval = _evaluationEngine.Evaluate(consideredLine);
            //        if (whiteToMove && bestEvaluation < eval ||
            //            !whiteToMove && bestEvaluation > eval)
            //        {
            //            bestLines.Clear();
            //            bestLines.Add(consideredLine);
            //            bestEvaluation = eval;
            //        }
            //        else if (bestEvaluation == eval)
            //        {
            //            bestLines.Add(consideredLine);
            //        }

            //        if (!whiteToMove)
            //        {
            //            min = Math.Min(min, bestEvaluation);
            //        }
            //        else
            //        {
            //            max = Math.Max(max, bestEvaluation);
            //        }
            //        if (min < max)
            //        {
            //            break;
            //        }
            //    }
            //}
            //return bestLines;
        }
    }
}
