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

        private CustomPinTag genderTag;
        private CustomWrapTag hideTag;
        private CustomWrapTag ifTag;

        private void Start()
        {
            genderTag = new CustomPinTag("<gender>", "Male");
            hideTag = new CustomWrapTag("<hide>", "</hide>", HideEffect);
            ifTag = new CustomWrapTag("<if>", "</if>", IfEffect);

            // We assign as a pin because there is no closing tag, this tag does not wrap a body of text it simply replaces the tag in the text.
            Textualise.PurgeSettings();
            Textualise.Settings.AssignPin(genderTag);
            Textualise.Settings.AssignWrap(hideTag);
            Textualise.Settings.AssignWrap(ifTag);
        }

        #region Background
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
        #endregion

        public void ParseText()
        {
            ToggleGender();

            parsedOutput.text = Textualise.ParseText(input);
            defaultOutput.text = input;
        }

        private string HideEffect(string input, WrapReplacementStage stage, string data)
        {
            switch (stage)
            {
                case WrapReplacementStage.Opening:
                    return "";

                case WrapReplacementStage.Inside:
                    return "";

                case WrapReplacementStage.Closing:
                    return "";

                default: return "";
            }
        }

        private string IfEffect(string input, WrapReplacementStage stage, string data)
        {
            switch (stage)
            {
                case WrapReplacementStage.Opening:
                    return "";

                case WrapReplacementStage.Inside:
                    if ((genderReplacement == TestGender.Male && data == "male") || (genderReplacement == TestGender.Female && data == "female"))
                        return input;
                    return "";

                case WrapReplacementStage.Closing:
                    return "";

                default: return "";
            }
        }
    }
}
