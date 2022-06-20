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
        /// Parses through the input text looking for any tags and replaces them with the corresponding info if there are.
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

                        if (isTag) // If pin tag, replace it with the actual text. 
                            finalLine = Settings.Pins[$"{Settings.OpenSymbol}{line}{Settings.CloseSymbol}"].TagReplacement;

                        while (tags.Count > 0) // Loop through all of the open wrap tags.
                        {
                            var tag = tags.Pop();
                            finalLine = Settings.Wraps[tag.Opening].HandleWrapReplacement(finalLine, WrapReplacementStage.Inside, tag.Data);
                        }

                        finalText = string.Concat(finalText, finalLine);
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

                if (Settings.ContainsWrap(line, out var type))
                {
                    switch (type)
                    {
                        case WrapTagType.Opening:
                            CustomWrapTag customWrap;
                            var data = "";
                            if (line.Contains(Settings.TagDataSplitSymbol.ToString()))
                            {
                                var split = line.Split(Settings.TagDataSplitSymbol);
                                data = split[1];
                                if (split.Length > 2)
                                {
                                    for (int i = 2; i < split.Length; i++)
                                    {
                                        // If a custom tag uses the '=' symbol to split more data it needs to be restored.
                                        data = string.Concat(data, Settings.TagDataSplitSymbol.ToString(), split[i]);
                                    }
                                }
                                customWrap = Settings.Wraps[$"{Settings.OpenSymbol}{split[0]}{Settings.CloseSymbol}"];
                            }
                            else
                                customWrap = Settings.Wraps[$"{Settings.OpenSymbol}{line}{Settings.CloseSymbol}"];

                            openTags.Push(new OpenTag(customWrap.OpeningTag, customWrap.ClosingTag, data));
                            finalText = string.Concat(finalText, customWrap.HandleWrapReplacement(line, WrapReplacementStage.Opening, openTags.Peek().Data));
                            continue;

                        case WrapTagType.Closing:
                            if (openTags.Peek().Closing.Contains(line))
                            {
                                var ot = openTags.Pop();
                                finalText = string.Concat(finalText, Settings.Wraps[ot.Opening].HandleWrapReplacement(line, WrapReplacementStage.Closing, ot.Data));
                            }
                            else
                            {
                                // Tag Nesting Rule:
                                // First tag opened must be the last tag closed.
                                // Vice versa the last tag opened must be the first tag closed.
                                // Example -> [Tag1 Open] [Tag2 Open] --- [Tag2 Closed] [Tag1 Closed]
                                Debug.LogError("Wrap tags are being opened and closed while breaking the Tag Nesting Rule!");
                            }
                            continue;
                    }
                }
            }

            return finalText;
        }
    }
}
