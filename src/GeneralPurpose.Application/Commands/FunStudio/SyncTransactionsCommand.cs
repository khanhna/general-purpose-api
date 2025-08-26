using System.Net;
using GeneralPurpose.Domain.Entities;
using GeneralPurpose.Domain.Models;
using GeneralPurpose.Domain.Models.Requests.FunStudio;
using GeneralPurpose.Domain.Models.Responses.FunStudio;
using GeneralPurpose.Infrastructure.Helpers;
using GeneralPurpose.Infrastructure.Persistence.KeyTypedRepositories;
using Mediator;
using Microsoft.Extensions.Logging;
using OneOf;

namespace GeneralPurpose.Application.Commands.FunStudio;

public class SyncTransactionsCommand : IRequest<OneOf<SyncTransactionsResponse, HttpResponse>>
{
    public string WorkingUnitIdentifier { get; set; }
    public SyncTransactionsRequest Request { get; set; }

    public SyncTransactionsCommand(string workingUnitIdentifier, SyncTransactionsRequest request)
    {
        WorkingUnitIdentifier = workingUnitIdentifier;
        Request = request;
    }
}

public class SyncTransactionsCommandHandler : IRequestHandler<SyncTransactionsCommand,
    OneOf<SyncTransactionsResponse, HttpResponse>>
{
    private readonly IRepository<WorkingUnit, int> _workingUnitRepository;
    private readonly IRepository<Transaction, int> _transactionRepository;
    private readonly ILogger<SyncTransactionsCommandHandler> _logger;

    public SyncTransactionsCommandHandler(IRepository<WorkingUnit, int> workingUnitRepository,
        IRepository<Transaction, int> transactionRepository, ILogger<SyncTransactionsCommandHandler> logger)
    {
        _workingUnitRepository = workingUnitRepository;
        _transactionRepository = transactionRepository;
        _logger = logger;
    }

    public async ValueTask<OneOf<SyncTransactionsResponse, HttpResponse>> Handle(
        SyncTransactionsCommand request, CancellationToken cancellationToken)
    {
        var workingUnit = await _workingUnitRepository.FirstOrDefaultAsync(
            x => x.Identifier == request.WorkingUnitIdentifier, isNoTracking: true,
            cancellationToken: cancellationToken);

        if (workingUnit?.Id == null)
            return new HttpResponse(HttpStatusCode.BadRequest, "Working unit is not found!");

        if (!SecurityHelper.IsValidSignature(request.Request.Sig, workingUnit.Identifier ?? string.Empty,
                request.Request.CurrentTime))
            return new HttpResponse(HttpStatusCode.BadRequest, "Invalid Signature!!");

        var milestonesToSync =
            request.Request.Transactions.ToDictionary(x => ExtractTransactionKey(x.RecordAt, x.Type));
        var farestTransaction = request.Request.Transactions.MinBy(x => x.RecordAt);
        var syncMilestone = farestTransaction?.RecordAt.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc) ??
                            DateTime.UtcNow.Date.AddDays(-30);

        var transactions = await _transactionRepository.ListAsync(
            x => x.WorkingUnitId == workingUnit.Id && x.RecordAt >= syncMilestone,
            cancellationToken: cancellationToken);

        var transactionDict = transactions.ToDictionary(x => ExtractTransactionKey(x.RecordAt, x.Type));

        var transactionsToUpdate =
            new Dictionary<string, Transaction>(transactionDict.Where(x => milestonesToSync.ContainsKey(x.Key)));
        var transactionsToAdd = milestonesToSync.Where(x => !transactionsToUpdate.ContainsKey(x.Key));

        foreach (var item in transactionsToUpdate)
        {
            if (!milestonesToSync.TryGetValue(item.Key, out var milestone))
            {
                _logger.LogWarning("Sync transactions might have some issues for record at {RecordAt} - type {Type}",
                    item.Value.RecordAt, item.Value.Type);
                continue;
            }

            item.Value.Quantity = milestone.Quantity;
            item.Value.Price = milestone.Price;
        }

        foreach (var item in transactionsToAdd)
        {
            _transactionRepository.Add(new Transaction
            {
                WorkingUnitId = workingUnit.Id,
                RecordAt = item.Value.RecordAt.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
                Type = item.Value.Type,
                Quantity = item.Value.Quantity,
                Price = item.Value.Price
            });
        }

        await _transactionRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        var syncedTransactions = await _transactionRepository.ListAsync(
            x => x.WorkingUnitId == workingUnit.Id && x.RecordAt >= syncMilestone, isNoTracking: true,
            cancellationToken: cancellationToken);

        return new SyncTransactionsResponse
        {
            Transactions = syncedTransactions.Select(x => new SyncTransactionsResponseDetail
            {
                RecordAt = DateOnly.FromDateTime(x.RecordAt),
                Type = x.Type,
                LastSyncAt = x.LastUpdatedTime
            }).ToArray()
        };
    }
    
    private string ExtractTransactionKey(DateOnly date, TransactionProcessType type) => $"{date:yyyyMMdd}-{(int)type}";
    private string ExtractTransactionKey(DateTime date, TransactionProcessType type) => $"{date:yyyyMMdd}-{(int)type}";
}