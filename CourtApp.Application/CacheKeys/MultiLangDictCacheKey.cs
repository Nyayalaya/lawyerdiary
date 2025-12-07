using System;

namespace CourtApp.Application.CacheKeys
{
    public static class MultiLangDictCacheKey
    {
        /// <summary>
        /// Cache prefix for all entries.
        /// </summary>
        private const string Prefix = "MultiLangDict";

        /// <summary>
        /// Cache key for getting one dictionary entry by ID.
        /// </summary>
        public static string GetById(Guid id)
            => $"{Prefix}:Id:{id}";

        /// <summary>
        /// Cache key for a complete list of all entries.
        /// </summary>
        public static string All
            => $"{Prefix}:All";

        /// <summary>
        /// Cache key for list filtered by language code.
        /// </summary>
        public static string ByLanguage(string langCode)
            => $"{Prefix}:Lang:{langCode?.Trim().ToLower()}";

        /// <summary>
        /// Cache key for list filtered by keyword.
        /// </summary>
        public static string ByKeyword(string keyword)
            => $"{Prefix}:Keyword:{keyword?.Trim().ToUpper()}";

        /// <summary>
        /// Cache key for keyword + lang combination.
        /// </summary>
        public static string ByKeywordLang(string keyword, string langCode)
            => $"{Prefix}:Keyword:{keyword?.Trim().ToUpper()}:Lang:{langCode?.Trim().ToLower()}";
    }
}
