using Arcturus.Textualise.Internal;
using System;

namespace Arcturus.Textualise
{
    public class CustomWrapTag : CustomTag
    {
        public string OpeningTag { get; private set; }
        public string ClosingTag { get; private set; }
        public Func<string, string> WhileWrappingFunction { get; private set; }

        public CustomWrapTag(string openingTag, string closingTag, Func<string, string> whileWrappingFunc)
        {
            OpeningTag = openingTag;
            ClosingTag = closingTag;
            WhileWrappingFunction = whileWrappingFunc;
        }

        public string WhileWrapping(string feed)
        {
            return WhileWrappingFunction.Invoke(feed);
        }
    }
}
