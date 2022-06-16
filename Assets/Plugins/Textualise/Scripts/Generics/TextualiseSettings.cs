using System.Collections.Generic;

namespace Arcturus.Textualise.Internal
{
    public class TextualiseSettings
    {
        public char OpenSymbol { get; set; }
        public char CloseSymbol { get; set; }

        public Dictionary<string, CustomPinTag> Pins { get; private set; }
        public Dictionary<string, CustomWrapTag> Wraps { get; private set; }

        public TextualiseSettings()
        {
            OpenSymbol = '<';
            CloseSymbol = '>';
            Pins = new Dictionary<string, CustomPinTag>();
            Wraps = new Dictionary<string, CustomWrapTag>();
        }

        #region Pin Tag Functions
        public bool ContainsPin(string input)
        {
            var tag = input;

            if (!input.Contains(OpenSymbol.ToString()))
                tag = string.Concat(OpenSymbol, input);

            if (!input.Contains(CloseSymbol.ToString()))
                tag = string.Concat(tag, CloseSymbol);

            return Pins.ContainsKey(tag);
        }

        public CustomPinTag CreatePin(string tag, string replacement)
        {
            return Pins[tag] = new CustomPinTag(tag, replacement);
        }

        public void AssignPin(CustomPinTag pinTag)
        {
            Pins[pinTag.Tag] = pinTag;
        }
        #endregion

        // TODO:
        #region Wrapper Tag Functions
        public bool ContainsWrap(string input) // TODO: Check for tag closing symbol as well.
        {
            var tag = input;

            if (!input.Contains(OpenSymbol.ToString()))
                tag = string.Concat(OpenSymbol, input);

            if (!input.Contains(CloseSymbol.ToString()))
                tag = string.Concat(tag, CloseSymbol);

            return Wraps.ContainsKey(tag);
        }

        //public CustomWrapTag CreateWrap(string)
        //{
        //     public string OpeningTag { get; private set; }
        //    public string ClosingTag { get; private set; }
        //    public Func<string, string> WhileWrappingFunction { get; private set; }
        //}
        #endregion
    }
}
