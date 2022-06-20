using TMPro;
using UnityEngine;

namespace Arcturus.Textualise.Testing
{
    public enum TestGender { Male, Female }

    public class TestingScr : MonoBehaviour
    {
        [SerializeField] private TMP_Text parsedOutput;
        [SerializeField] private TMP_Text defaultOutput;
        [SerializeField] private TestGender genderReplacement;

        [TextArea(4, 10)] public string input;

        // Pins
        private CustomPinTag genderTag;

        // Wraps
        private CustomWrapTag hideTag;
        private CustomWrapTag ifTag;
        private CustomWrapTag colorTag;

        private void Start()
        {
            // Ensures all Textualise settings are reset. This restores the default configuration of Textualise Settings.
            Textualise.PurgeSettings();

            // You can assign custom Open and Close symbols for tags.
            Textualise.Settings.OpenSymbol = '<'; // => This is the default.
            Textualise.Settings.CloseSymbol = '>'; // => This is the default.

            /// For wrap tags you can send through data as part of the tag.
            // Example: <if=Health+5> filler text </if>
            // The tag breakdown: Tag -> <if>, Data -> Health+5
            /// The system will detect that the tag contains data and will send it through in the replacement handling function.
            Textualise.Settings.TagDataSplitSymbol = '='; // => This is the default.

            // We create a pin because there is no closing tag, this tag does not wrap a body of text it simply replaces the tag in the text.
            genderTag = Textualise.Settings.CreatePin("<gender>", "Male");

            // We create these tags as wraps since they wrap around a body of text.
            /// Note: Some of these require data but the declaration does not include that since it is not required. (Likely doesn't work if you do. Untested)
            hideTag = Textualise.Settings.CreateWrap("<hide>", "</hide>", HideEffect);
            ifTag = Textualise.Settings.CreateWrap("<if>", "</if>", IfEffect);
            colorTag = Textualise.Settings.CreateWrap("<col>", "</col>", ColorEffect);
        }

        /// <summary>
        /// Changes the replacement text for the gender tag.<br/>
        /// Pin tag replacement values can be dynamically modified.
        /// </summary>
        private void ToggleGender()
        {
            switch (genderReplacement)
            {
                case TestGender.Male:
                    genderTag.TagReplacement = "Male";
                    break;

                case TestGender.Female:
                    genderTag.TagReplacement = "Female";
                    break;
            }
        }

        /// <summary>
        /// Handles the replacement of text that sits within the <hide> tags.<br/>
        /// Opening: Replaces the opening <hide> tag.<br/>
        /// Inside: Replaces the text inside the tag wrap.<br/>
        /// Closing: Replaces the closing </hide> tag.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="stage"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private string HideEffect(string input, WrapReplacementStage stage, string data)
        {
            return stage switch
            {
                WrapReplacementStage.Opening => "",
                WrapReplacementStage.Inside => "",
                WrapReplacementStage.Closing => "",
                _ => "",
            };
        }

        /// <summary>
        /// Handles the replacement of text that sits within the <if> tags.<br/>
        /// Opening: Replaces the opening <if> tag.<br/>
        /// Inside: Replaces the text inside the tag wrap.<br/>
        /// Closing: Replaces the closing </if> tag.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="stage"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private string IfEffect(string input, WrapReplacementStage stage, string data)
        {
            switch (stage)
            {
                case WrapReplacementStage.Inside:
                    if ((genderReplacement == TestGender.Male && data == "male") || (genderReplacement == TestGender.Female && data == "female"))
                        return input;
                    return "";

                case WrapReplacementStage.Opening:
                case WrapReplacementStage.Closing:
                default: return "";
            }
        }

        /// <summary>
        /// Yes I know TMPro has a built in color tag. This is part of an example for nested tagging.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="stage"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private string ColorEffect(string input, WrapReplacementStage stage, string data)
        {
            switch (stage)
            {
                case WrapReplacementStage.Opening:
                    if (data == "red")
                        return Textualise.GenerateColorTag(Color.red);
                    else if (data == "green")
                        return Textualise.GenerateColorTag(Color.green);
                    else
                    {
                        Debug.LogWarning("Detected data for the color tag that this example has not been coded for! Double-Click me to have a look.");
                        return Textualise.GenerateColorTag(Color.blue);
                    }

                case WrapReplacementStage.Closing:
                    return "</color>";

                case WrapReplacementStage.Inside:
                default: return input;
            }
        }

        /// <summary>
        /// Called by ui to trigger the parsing of the demo text.
        /// </summary>
        public void ParseText()
        {
            ToggleGender();

            parsedOutput.text = Textualise.ParseText(input);
            defaultOutput.text = input;
        }
    }
}
