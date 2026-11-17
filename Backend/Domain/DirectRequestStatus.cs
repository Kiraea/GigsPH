using Microsoft.AspNetCore.Http.HttpResults;
using StackExchange.Redis;

namespace PrjA.Domain;

public enum DirectRequestStatus
{
    Accepted,
    Pending,
    Rejected,
}