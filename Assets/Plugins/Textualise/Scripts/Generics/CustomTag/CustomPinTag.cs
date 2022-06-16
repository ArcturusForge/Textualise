using Arcturus.Textualise.Internal;

namespace Arcturus.Textualise
{
    public class CustomPinTag : CustomTag
    {
        public string Tag { get; private set; }
        public string TagReplacement { get; set; }

        public CustomPinTag(string tag, string replacement)
        {
            Tag = tag;
            TagReplacement = replacement;
        }
    }
}
