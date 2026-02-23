namespace WorkflowApi.Models;

public sealed record OrderPayload(string Name, double TotalCost, int Quantity = 1);
public sealed record InventoryRequest(string RequestId, string ItemName, int Quantity);
public sealed record InventoryResult(bool Success, OrderPayload? OrderPayload);
public sealed record ApprovalRequest(string RequestId, string ItemBeingPurchased, int Quantity, double Amount);
public sealed record ApprovalResponse(string RequestId, bool IsApproved);
public sealed record PaymentRequest(string RequestId, string ItemBeingPurchased, int Amount, double Currency);
public sealed record OrderResult(bool Processed);
