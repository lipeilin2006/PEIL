using System;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace PEILCore
{
    public class PEILCoreError : BaseErrorListener
    {
        public override void SyntaxError([NotNull] IRecognizer recognizer, [Nullable] IToken offendingSymbol, int line, int charPositionInLine, [NotNull] string msg, [Nullable] RecognitionException e)
        {
            throw new Exception(msg + " on (" + line + "," + charPositionInLine + ")");
        }
    }
}
