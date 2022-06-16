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

        private CustomPinTag genderTag = new CustomPinTag("<gender>", "Male");

        private void Start()
        {
            // We assign as a pin because there is no closing tag, this tag does not wrap a body of text it simply replaces the tag in the text.
            Textualise.PurgeSettings();
            Textualise.Settings.AssignPin(genderTag);
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
    }
}
