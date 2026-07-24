namespace MarketLens.Application.Features.Identity.Register;

using MarketLens.Application.Abstractions.Messaging;
using MarketLens.Domain.Common;

public sealed record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string ConfirmPassword) : ICommand;
