namespace Nextens.Application.Indicators
{
    public sealed class IndicatorReport<TData>
    {
        public Guid CustomerId { get; init; }

        public IEnumerable<TData> Data { get; init; } = new List<TData>();
    }
}
