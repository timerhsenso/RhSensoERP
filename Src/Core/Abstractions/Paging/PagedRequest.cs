namespace RhSensoERP.Core.Abstractions.Paging;

public sealed record PagedRequest(int Page = 1, int PageSize = 20);
