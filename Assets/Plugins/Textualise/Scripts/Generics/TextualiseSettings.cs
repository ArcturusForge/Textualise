using System;
using System.Collections.Generic;

namespace Arcturus.Textualise.Internal
{
    public enum WrapTagType { Opening, Closing }

    public class TextualiseSettings
    {
        public char OpenSymbol { get; set; }
        public char CloseSymbol { get; set; }
        public char TagDataSplitSymbol { get; set; }

        public Dictionary<string, CustomPinTag> Pins { get; private set; }
        public Dictionary<string, CustomWrapTag> Wraps { get; private set; }

        public TextualiseSettings()
        {
            OpenSymbol = '<';
            CloseSymbol = '>';
            TagDataSplitSymbol = '=';
            Pins = new Dictionary<string, CustomPinTag>();
            Wraps = new Dictionary<string, CustomWrapTag>();
        }

        #region Pin Tag Functions
        /// <summary>
        /// Internal function that gets used to find a corresponding PinTag.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool ContainsPin(string tag)
        {
            if (!tag.Contains(OpenSymbol.ToString()))
                tag = string.Concat(OpenSymbol, tag);

            if (!tag.Contains(CloseSymbol.ToString()))
                tag = string.Concat(tag, CloseSymbol);

            return Pins.ContainsKey(tag);
        }

        /// <summary>
        /// Creates a CustomPinTag object and returns it while also assigning it internally.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public CustomPinTag CreatePin(string tag, string replacement)
        {
            return Pins[tag] = new CustomPinTag(tag, replacement);
        }

        /// <summary>
        /// Assigns the custom pin to the settings so Textualiser knows to look for it.
        /// </summary>
        /// <param name="pinTag"></param>
        public void AssignPin(CustomPinTag pinTag)
        {
            Pins[pinTag.Tag] = pinTag;
        }
        #endregion

        #region Wrapper Tag Functions
        /// <summary>
        /// Internal function that gets used to find a corresponding WrapTag.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool ContainsWrap(string tag, out WrapTagType type)
        {
            if (!tag.Contains("/"))
            {
                type = WrapTagType.Opening;

                // Seperate instance specific data from tag recognition.
                // Example -> <if=health+5> ---- </if> where the tag type is "if" and the data is "health+5"
                if (tag.Contains(TagDataSplitSymbol.ToString()))
                    tag = tag.Split(TagDataSplitSymbol)[0];

                // Reconstruct the proper tag form if it is not already repaired.
                if (!tag.Contains(OpenSymbol.ToString()))
                    tag = string.Concat(OpenSymbol, tag);

                if (!tag.Contains(CloseSymbol.ToString()))
                    tag = string.Concat(tag, CloseSymbol);

                return Wraps.ContainsKey(tag);
            }
            else
            {
                type = WrapTagType.Closing;

                if (!tag.Contains(OpenSymbol.ToString()))
                    tag = string.Concat(OpenSymbol, tag);

                if (!tag.Contains(CloseSymbol.ToString()))
                    tag = string.Concat(tag, CloseSymbol);

                var openForm = tag.Replace("/", "");
                if (Wraps.ContainsKey(openForm))
                    return Wraps[openForm].ClosingTag == tag;

                return false;
            }
        }

        /// <summary>
        /// Creates a CustomWrapTag object and returns it while also assigning it internally.<br/><br/>
        /// ReplacementFunc structure:<br/>
        /// (string) input -> The tag/text that is currently being parsed and replaced.<br/>
        /// (WrapReplacementStage) stage -> What segment of the tag that is being replaced.<br/>
        /// (string) data -> The additional data that was detected as part of the tag. Empty if there is no data.<br/>
        /// Returns string -> The replacement text.
        /// </summary>
        /// <param name="openingTag"></param>
        /// <param name="closingTag"></param>
        /// <param name="replacementFunc"></param>
        /// <returns></returns>
        public CustomWrapTag CreateWrap(string openingTag, string closingTag, Func<string, WrapReplacementStage, string, string> replacementFunc)
        {
            return Wraps[openingTag] = new CustomWrapTag(openingTag, closingTag, replacementFunc);
        }

        /// <summary>
        /// Assigns the custom wrap to the settings so Textualiser knows to look for it.
        /// </summary>
        /// <param name="wrapTag"></param>
        public void AssignWrap(CustomWrapTag wrapTag)
        {
            Wraps[wrapTag.OpeningTag] = wrapTag;
        }
        #endregion
    }
}
