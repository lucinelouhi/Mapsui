using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;

namespace Mapsui.Fetcher
{
    internal class FeatureFetcher
    {
        private readonly FetchInfo _fetchInfo;
        private readonly DataArrivedDelegate _dataArrived;
        private readonly IProvider _provider;
        private readonly SemaphoreSlim _providerLock = new(1, 1);
        private readonly long _timeOfRequest;

        public delegate void DataArrivedDelegate(IEnumerable<IFeature> features, object? state = null);

        public FeatureFetcher(FetchInfo fetchInfo, IProvider provider, DataArrivedDelegate dataArrived, long timeOfRequest = default)
        {
            _dataArrived = dataArrived;
            var biggerBox = fetchInfo.Extent.Grow(
                SymbolStyle.DefaultWidth * 2 * fetchInfo.Resolution,
                SymbolStyle.DefaultHeight * 2 * fetchInfo.Resolution);
            _fetchInfo = new FetchInfo(biggerBox, fetchInfo.Resolution, fetchInfo.CRS, fetchInfo.ChangeType);

            _provider = provider;
            _timeOfRequest = timeOfRequest;
        }

        public async Task FetchOnThreadAsync()
        {
            await _providerLock.WaitAsync();
            try
            {
                var features = _provider.GetFeaturesAsync(_fetchInfo);
                _dataArrived.Invoke(await features.ToListAsync(), _timeOfRequest);
            }
            finally
            {
                _providerLock.Release();
            }
        }
    }
}
