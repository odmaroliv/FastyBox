namespace FastyBox.Domain.Enums
{
    public enum ShipmentStatus
    {
        Draft = 0,
        Submitted = 1,
        AwaitingDocuments = 2,
        DocumentsReviewed = 3,
        AwaitingPayment = 4,
        AwaitingArrival = 5,
        ReceivedInWarehouse = 6,
        Processing = 7,
        InTransit = 8,
        InDelivery = 9,
        Delivered = 10,
        Exception = 11,
        Cancelled = 12
    }
}
