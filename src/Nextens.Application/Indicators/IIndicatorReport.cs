namespace Nextens.Application.Indicators
{
    public interface IIndicatorReport<TData>
    {
        Guid CustomerId { get; }

        IEnumerable<TData> Data { get; }
    }
}
