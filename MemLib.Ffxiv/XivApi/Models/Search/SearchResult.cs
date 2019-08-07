using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MemLib.Ffxiv.XivApi.Models.Search {
    public class SearchResult<T> : BaseRequest, IEnumerable<T> {
        public PaginationResult Pagination;
        public List<T> Results = new List<T>();
        
        public async Task<SearchResult<T>> GetPage(int page) {
            if (Pagination.PageTotal > page || string.IsNullOrEmpty(RequestUri))
                return null;
            var result = await XivApiClient.Request<SearchResult<T>>($"{RequestUri}&page={page}");
            result.RequestUri = RequestUri;
            return result;
        }

        public async Task<SearchResult<T>> GetNextPage() {
            if ((int)Pagination.PageNext <= 0 || string.IsNullOrEmpty(RequestUri))
                return null;
            var result = await XivApiClient.Request<SearchResult<T>>($"{RequestUri}&page={Pagination.PageNext}");
            result.RequestUri = RequestUri;
            return result;
        }

        public async Task<SearchResult<T>> GetPreviousPage() {
            if ((int)Pagination.PagePrev <= 0 || string.IsNullOrEmpty(RequestUri))
                return null;
            var result = await XivApiClient.Request<SearchResult<T>>($"{RequestUri}&page={Pagination.PagePrev}");
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