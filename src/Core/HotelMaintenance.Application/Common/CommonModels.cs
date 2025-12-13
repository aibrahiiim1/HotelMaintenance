namespace HotelMaintenance.Application.Common;

/// <summary>
/// Generic paged result for list queries
/// </summary>
public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}

/// <summary>
/// Generic result wrapper for operations
/// </summary>
public class Result
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();

    public static Result Success(string message = "")
    {
        return new Result { IsSuccess = true, Message = message };
    }

    public static Result Failure(string error)
    {
        return new Result { IsSuccess = false, Errors = new List<string> { error } };
    }

    public static Result Failure(List<string> errors)
    {
        return new Result { IsSuccess = false, Errors = errors };
    }
}

/// <summary>
/// Generic result wrapper with data
/// </summary>
public class Result<T> : Result
{
    public T? Data { get; set; }

    public static Result<T> Success(T data, string message = "")
    {
        return new Result<T> { IsSuccess = true, Data = data, Message = message };
    }

    public new static Result<T> Failure(string error)
    {
        return new Result<T> { IsSuccess = false, Errors = new List<string> { error } };
    }

    public new static Result<T> Failure(List<string> errors)
    {
        return new Result<T> { IsSuccess = false, Errors = errors };
    }
}

/// <summary>
/// Base filter/query parameters
/// </summary>
public class BaseQueryParameters
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;

    public int PageNumber { get; set; } = 1;
    
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    public string? SortBy { get; set; }
    public string SortOrder { get; set; } = "asc";
    public string? SearchTerm { get; set; }
}
