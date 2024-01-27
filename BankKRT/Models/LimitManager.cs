using Amazon.DynamoDBv2.DataModel;

namespace BankKRT.Models
{
    [DynamoDBTable("LimitManager")]

    public class LimitManager
    {
        [DynamoDBHashKey("PK")]
        public string PK { get; set; }

        [DynamoDBRangeKey("SK")]
        public string SK { get; set; }

        [DynamoDBProperty]
        public string CustomerDocument { get; set; }

        [DynamoDBProperty]
        public int NumberAgency { get; set; }

        [DynamoDBProperty]
        public int NumberAccount { get; set; }

        [DynamoDBProperty]
        public decimal LimitValue { get; set; }

        [DynamoDBProperty]
        public decimal TransactionValue { get; set; }

        [DynamoDBProperty]
        public DateTime CreatedAt { get; set; }

        [DynamoDBProperty]
        public DateTime UpdatedAt { get; set; }
    }
}
