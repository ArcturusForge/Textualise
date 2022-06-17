using Arcturus.Textualise.Internal;
using System;

namespace Arcturus.Textualise
{
    public enum WrapReplacementStage { Opening, Inside, Closing }

    public class CustomWrapTag : CustomTag
    {
        public string OpeningTag { get; private set; }
        public string ClosingTag { get; private set; }
        public Func<string, WrapReplacementStage, string, string> WrapFunction;

        public CustomWrapTag(string openingTag, string closingTag, Func<string, WrapReplacementStage, string, string> wrapReplacementHandler)
        {
            OpeningTag = openingTag;
            ClosingTag = closingTag;
            WrapFunction = wrapReplacementHandler;
        }

        public string HandleWrapReplacement(string input, WrapReplacementStage stage, string data)
        {
            return WrapFunction.Invoke(input, stage, data);
        }
    }
}
