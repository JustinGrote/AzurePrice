using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Client.Shared.OData;
using System.Net.Http.Json;
using System;
using System.Text.RegularExpressions;

namespace Client.Shared
{
    public class PriceClient
    {
        //Reused Blazor WASM client
        protected readonly HttpClient http;

        //Quirk: Retail Price API will only return a maximum of this number of records
        protected readonly int defaultPageSize = 100;

        //Path the the api
        public readonly string path = "/api/prices";
        public PriceClient(HttpClient httpclient)
        {
            http = httpclient;
        }

        public async Task<List<Price>> GetPrices(ODataQuery<Price> query, bool firstPageOnly)
        {
            List<Price> priceList = new();
            string requestUri = path + '?' + query.ToString();

            int requestItemCount;
            Uri nextPageLink = null;

            //ToDo: Add concurrent readahead support since the paging count doesn't display properly
            do
            {
                if (nextPageLink != null)
                {
                    requestUri = ConvertNextPageLinkToRelativeURI(nextPageLink);
                }

                var response = await http.GetFromJsonAsync<PriceResponse>(requestUri);
                priceList.AddRange(response.Prices);
                requestItemCount = response.Count;
                nextPageLink = response.NextPageLink;
            } while (
                //TODO: Structure this better for concurrent readahead than this firstpageonly option
                !firstPageOnly &&
                requestItemCount == 100 && 
                Regex.IsMatch(nextPageLink.ToString(), "skip=\\d+$")
            );

            return priceList;
        }

        //Prefetch to operate more quickly
        public async Task<List<Price>> GetPrices(ODataQuery<Price> query, int prefetchCount = 5)
        {
            List<Price> priceList = new();
            List<Task<List<Price>>> prefetchTasks = new();
            
            int currentPage;
            for(currentPage = 0; currentPage < prefetchCount; currentPage++) {
                query.Skip = currentPage * defaultPageSize;
                prefetchTasks.Add(
                    //true here makes sure that we don't follow the paging and end up with duplicate results
                    GetPrices(query, true)
                );
            }

            await Task.WhenAll(prefetchTasks);
            foreach (Task<List<Price>> task in prefetchTasks) {
                priceList.AddRange(task.Result);
            }

            //If there were 100 items in the last query, continue to fetch more items normally until all items retrieved
            if (prefetchTasks[^1].Result.Count == 100) {
                query.Skip = currentPage * defaultPageSize;
                priceList.AddRange(await GetPrices(query, false));
            }

            return priceList;
        }

        string ConvertNextPageLinkToRelativeURI(Uri nextPageLink) {
            return path + nextPageLink.Query;
        }

        public async Task<List<Price>> GetSpotPrices(ODataQuery<Price> query, int prefetchCount)
        {
            string spotVMFilter = " and endswith(skuName,'Spot') eq true and endswith(productName,'Windows') eq false";
            query.Filter += spotVMFilter;
            query.OrderBy = "unitPrice";
            return await GetPrices(query, prefetchCount);
        }
    }
}