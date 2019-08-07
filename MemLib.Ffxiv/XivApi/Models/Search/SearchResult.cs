using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MemLib.Ffxiv.XivApi.Models.Search {
    public class SearchResult<T> : BaseRequest, IEnumerable<T> {
        public PaginationResult Pagination;
        public List<T> Results = new List<T>();

        public SearchResult<T> GetPage(int page) => GetPageAsync(page).Result;

        public async Task<SearchResult<T>> GetPageAsync(int page) {
            if (Pagination.PageTotal > page || string.IsNullOrEmpty(RequestUri))
                return null;
            var uri = RequestUri.Contains("?") ? $"{RequestUri}&page={page}" : $"{RequestUri}?page={page}";
            var result = await XivApiClient.RequestAsync<SearchResult<T>>(uri);
            result.RequestUri = RequestUri;
            return result;
        }

        public SearchResult<T> GetNextPage() => GetNextPageAsync().Result;

        public async Task<SearchResult<T>> GetNextPageAsync() {
            if ((int)Pagination.PageNext <= 0 || string.IsNullOrEmpty(RequestUri))
                return null;
            var uri = RequestUri.Contains("?") ? $"{RequestUri}&page={Pagination.PageNext}" : $"{RequestUri}?page={Pagination.PageNext}";
            var result = await XivApiClient.RequestAsync<SearchResult<T>>(uri);
            result.RequestUri = RequestUri;
            return result;
        }

        public SearchResult<T> GetPreviousPage() => GetPreviousPageAsync().Result;

        public async Task<SearchResult<T>> GetPreviousPageAsync() {
            if ((int)Pagination.PagePrev <= 0 || string.IsNullOrEmpty(RequestUri))
                return null;
            var uri = RequestUri.Contains("?") ? $"{RequestUri}&page={Pagination.PagePrev}" : $"{RequestUri}?page={Pagination.PagePrev}";
            var result = await XivApiClient.RequestAsync<SearchResult<T>>(uri);
            result.RequestUri = RequestUri;
            return result;
        }

        #region Implementation of IEnumerable

        public IEnumerator<T> GetEnumerator() {
            return Results.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion
    }
}