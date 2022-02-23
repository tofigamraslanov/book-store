namespace BulkyBook.Utility
{
    public static class StaticDetails
    {
        public const string ProcedureCoverTypeCreate = "usp_CreateCoverType";
        public const string ProcedureCoverTypeGet = "usp_GetCoverType";
        public const string ProcedureCoverTypeGetAll = "usp_GetCoverTypes";
        public const string ProcedureCoverTypeUpdate = "usp_UpdateCoverType";
        public const string ProcedureCoverTypeDelete = "usp_DeleteCoverType";

        public const string RoleUserIndividual = "Individual Customer";
        public const string RoleUserCompany = "Company Customer";
        public const string RoleAdmin = "Admin";
        public const string RoleEmployee = "Employee";

        public const string SessionShoppingCart = "Shopping Cart Session";

        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusInProcess = "InProcess";
        public const string StatusShipped = "Shipped";
        public const string StatusCancelled = "Cancelled";
        public const string StatusRefunded = "Refunded";

        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusDelayedPayment = "ApprovedForDelayedPayment";
        public const string PaymentStatusRejected = "Rejected";

        public static double GetPriceBasedOnQuantity(double quantity, double price, double price50, double price100)
        {
            return quantity switch
            {
                < 50 => price,
                < 100 => price50,
                _ => price100
            };
        }

        public static string ConvertToRawHtml(string source)
        {
            var array = new char[source.Length];
            var arrayIndex = 0;
            var inside = false;

            foreach (var let in source)
            {
                switch (let)
                {
                    case '<':
                        inside = true;
                        continue;
                    case '>':
                        inside = false;
                        continue;
                }

                if (inside) continue;

                array[arrayIndex] = let;
                arrayIndex++;
            }
            return new string(array, 0, arrayIndex);
        }
    }
}