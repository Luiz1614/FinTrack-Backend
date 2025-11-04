namespace Fintrack.Contracts.Pagination;

public class TransactionParameters
{
    const int MaxPageSize = 50;
    public int PageNumber { get; set; } = 1;
    private int _pageSize;

    public int PageSize
    {
        get
        {
            return _pageSize;
        }
        set
        {
            _pageSize = (value > MaxPageSize)? MaxPageSize : value;
        }
    }
}
