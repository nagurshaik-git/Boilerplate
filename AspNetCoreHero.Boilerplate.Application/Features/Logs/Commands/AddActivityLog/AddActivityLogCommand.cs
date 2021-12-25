using AspNetCoreHero.Boilerplate.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCoreHero.Boilerplate.Application.Features.Logs.Commands.AddActivityLog;

public partial class AddActivityLogCommand : IRequest<Result<int>>
{
    public string Action { get; set; }
    public string UserId { get; set; }
}

public class AddActivityLogCommandHandler : IRequestHandler<AddActivityLogCommand, Result<int>>
{
    private readonly ILogRepository _repo;

    private IUnitOfWork UnitOfWork { get; set; }

    public AddActivityLogCommandHandler(ILogRepository repo, IUnitOfWork unitOfWork)
    {
        _repo = repo;
        UnitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(AddActivityLogCommand request, CancellationToken cancellationToken)
    {
        await _repo.AddLogAsync(request.Action, request.UserId);
        await UnitOfWork.Commit(cancellationToken);
        return Result<int>.Success(1);
    }
}
