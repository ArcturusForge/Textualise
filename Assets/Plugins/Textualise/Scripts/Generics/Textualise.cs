using Arcturus.Textualise.Internal;
using System.Collections.Generic;
using UnityEngine;

namespace Arcturus.Textualise
{
    public static class Textualise
    {
        private static TextualiseSettings _settings;
        public static TextualiseSettings Settings { get { if (_settings == null) { _settings = new TextualiseSettings(); } return _settings; } }

        #region Extension Helpers
        /// <summary>
        /// Converts a color to the text tag equivalent.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string GenerateColorTag(this Color color)
        {
            return $"{Settings.OpenSymbol}color=#{ColorUtility.ToHtmlStringRGBA(color)}{Settings.CloseSymbol}";
        }

        /// <summary>
        /// Wraps a segment of text within a paragraph with a tag.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="segmentToWrap"></param>
        /// <returns></returns>
        public static string InsertAroundSegment(this string text, string segmentToWrap)
        {
            return text.Replace(segmentToWrap, $"{Settings.OpenSymbol}{segmentToWrap}{Settings.CloseSymbol}");
        }
        #endregion

        /// <summary>
        /// Purges the parser settings and restores the defaults.
        /// </summary>
        public static void PurgeSettings()
        {
            _settings = null;
        }

        /// <summary>
        /// Parses through the input text looking for any tags and replaces them with the corresponding info if there are.<br/>
        /// To ignore replacement return '*' when replacement functions are called, which will print the tag instead of replace it.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ParseText(this string input)
        {
            if (!input.Contains(Settings.OpenSymbol.ToString()) || !input.Contains(Settings.CloseSymbol.ToString())) return input;

            var openTags = new Stack<OpenTag>();
            var splitLines = input.Split(Settings.OpenSymbol, Settings.CloseSymbol);
            var isTag = false;
            string finalText = "";

            foreach (var line in splitLines)
            {
                // Regular Text or a simple pin tag.
                if (!isTag || (isTag && Settings.ContainsPin(line)))
                {
                    // Checks if text is currently wrapped and not empty otherwise just adds line directly to output.
                    if (openTags.Count > 0 && !string.IsNullOrEmpty(line))
                    {
                        var tags = new Stack<OpenTag>(openTags);
                        string finalLine = line;

                        if (isTag) // If pin
                            finalLine = string.Concat(finalText, Settings.Pins[$"{Settings.OpenSymbol}{line}{Settings.CloseSymbol}"].TagReplacement);

                        while (tags.Count > 0) // Loop through all of the open tags.
                            finalLine = Settings.Wraps[openTags.Pop().Opening].WhileWrapping(finalLine);

                        string.Concat(finalText, finalLine);
                    }
                    else
                    {
                        if (!isTag)
                            finalText = string.Concat(finalText, line);
                        else
                            finalText = string.Concat(finalText, Settings.Pins[$"{Settings.OpenSymbol}{line}{Settings.CloseSymbol}"].TagReplacement);
                    }

                    isTag = !isTag;
                    continue;
                }

                // Cycles the tag detection.
                isTag = false;

                if (Settings.ContainsWrap(line))
                {
                    var customWrap = Settings.Wraps[$"{Settings.OpenSymbol}{line}{Settings.CloseSymbol}"];
                    openTags.Push(new OpenTag(customWrap.OpeningTag, customWrap.ClosingTag));
                    continue;
                }
            }

            return finalText;
        }
    }
}
