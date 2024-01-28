using Amazon.DynamoDBv2.DataModel;

namespace BankKRT.Infrastructure.Persistence.Models
{
    [DynamoDBTable("LimitManager")]

    public class CustomerDynamoDBModel
    {
        [DynamoDBHashKey]
        public string Document { get; set; }

        [DynamoDBProperty]
        public int NumberAgency { get; set; }

        [DynamoDBProperty]
        public int NumberAccount { get; set; }

        [DynamoDBProperty]
        public decimal LimitPix { get; set; }
    }
}
