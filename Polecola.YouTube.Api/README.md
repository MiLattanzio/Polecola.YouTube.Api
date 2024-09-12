#Polecola.YouTube.Api

This library can be used to query videos on YouTube and search between pages

##Usage

###Init the client

To init the client you can call the constructor with no parameters or pass an HttpClient like so.

```
new YouTubeClient()
```

```
new YouTubeClient(new HttpClient())
```

###Query for videos

```
        /// <summary>
        /// Searches for videos on YouTube based on the provided query string.
        /// </summary>
        /// <param name="query">The search query string.</param>
        /// <param name="page">The page number to retrieve. Defaults to 0.</param>
        /// <param name="retry">The number of retries if the search fails. Defaults to 3.</param>
        /// <returns>A <see cref="VideoSearchResult"/> object containing the search results, or <c>null</c> if the search fails after the specified number of retries.</returns>
        public async Task<VideoSearchResult?> SearchVideoAsync(string query, int page = 0, int retry = 3)
```

###Next page search

```
        /// <summary>
        /// Retrieves the next page of YouTube videos based on the provided search information.
        /// </summary>
        /// <param name="searchInfo">The current search information containing context and continuation tokens.</param>
        /// <param name="retry">The number of retries if the request for the next set of videos fails. Defaults to 3.</param>
        /// <returns>A <see cref="VideoSearchResult"/> object containing the next set of search results, or <c>null</c> if the request fails after the specified number of retries.</returns>
        public async Task<VideoSearchResult?> NextVideosAsync(Models.SearchInfo searchInfo, int retry = 3)
```
