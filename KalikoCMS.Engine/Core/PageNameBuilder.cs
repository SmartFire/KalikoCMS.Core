﻿#region License and copyright notice
/* 
 * Kaliko Content Management System
 * 
 * Copyright (c) Fredrik Schultz and Contributors
 * 
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
 */
#endregion

namespace KalikoCMS.Core {
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Web;
    using KalikoCMS.Core.Collections;

    public class PageNameBuilder {
        private static List<KeyValuePair<string, string>> _letterTranslation;

        private static IEnumerable<KeyValuePair<string, string>> LetterTranslation {
            get {
                return _letterTranslation ?? (_letterTranslation = SetupLetterTranslation());
            }
        }

        public static string PageNameToUrl(string pageName, Guid parentId) {
            string baseUrl = pageName.ToLower();

            baseUrl = HandleWhiteSpaces(baseUrl);
            baseUrl = HandleNonAsciiLetters(baseUrl);
            baseUrl = RemoveNonValidCharacters(baseUrl);

            var finalUrl = GetUniqueUrl(parentId, baseUrl);

            return finalUrl;
        }

        private static string GetUniqueUrl(Guid parentId, string baseUrl) {
            PageCollection sieblings = PageFactory.GetChildrenForPage(parentId, PublishState.All);
            var sieblingNames = new List<string>();

            foreach (CmsPage child in sieblings) {
                sieblingNames.Add(child.UrlSegment);
            }

            string suggestedUrl = baseUrl;
            int counter = 1;

            while (sieblingNames.Contains(suggestedUrl)) {
                counter++;
                suggestedUrl = string.Format("{0}~{1}", baseUrl, counter);
            }

            return suggestedUrl;
        }

        private static string RemoveNonValidCharacters(string urlSuggestion) {
            return HttpUtility.UrlEncode(Regex.Replace(urlSuggestion, "[^a-zA-Z0-9._-]", ""));
        }

        private static string HandleNonAsciiLetters(string urlSuggestion) {
            foreach (var translation in LetterTranslation) {
                urlSuggestion = urlSuggestion.Replace(translation.Key, translation.Value);
            }

            return urlSuggestion;
        }

        private static string HandleWhiteSpaces(string urlSuggestion) {
            return urlSuggestion.Replace(" ", "-");
        }

        private static List<KeyValuePair<string, string>> SetupLetterTranslation() {
            var letterTranslation = new List<KeyValuePair<string, string>>();
            letterTranslation.Add(new KeyValuePair<string, string>("á", "a"));
            letterTranslation.Add(new KeyValuePair<string, string>("à", "a"));
            letterTranslation.Add(new KeyValuePair<string, string>("å", "a"));
            letterTranslation.Add(new KeyValuePair<string, string>("ã", "a"));
            letterTranslation.Add(new KeyValuePair<string, string>("ä", "a"));
            letterTranslation.Add(new KeyValuePair<string, string>("ç", "c"));
            letterTranslation.Add(new KeyValuePair<string, string>("è", "e"));
            letterTranslation.Add(new KeyValuePair<string, string>("é", "e"));
            letterTranslation.Add(new KeyValuePair<string, string>("ü", "u"));
            letterTranslation.Add(new KeyValuePair<string, string>("ö", "o"));
            letterTranslation.Add(new KeyValuePair<string, string>("ß", "ss"));

            return letterTranslation;
        }
    }
}