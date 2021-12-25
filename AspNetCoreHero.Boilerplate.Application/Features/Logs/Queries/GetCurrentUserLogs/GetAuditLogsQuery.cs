using AspNetCoreHero.Boilerplate.Application.DTOs.Logs;
using AspNetCoreHero.Boilerplate.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCoreHero.Boilerplate.Application.Features.Logs.Queries.GetCurrentUserLogs;

public class GetAuditLogsQuery : IRequest<Result<List<AuditLogResponse>>>
{
    public string UserId { get; set; }

    public GetAuditLogsQuery()
    {
    }
}

public class GetAuditLogsQueryHandler : IRequestHandler<GetAuditLogsQuery, Result<List<AuditLogResponse>>>
{
    private readonly ILogRepository _repo;

    public GetAuditLogsQueryHandler(ILogRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<List<AuditLogResponse>>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
    {
        var logs = await _repo.GetAuditLogsAsync(request.UserId);
        return Result<List<AuditLogResponse>>.Success(logs);
    }
}
