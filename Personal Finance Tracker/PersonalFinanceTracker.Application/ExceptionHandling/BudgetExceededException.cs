namespace PersonalFinanceTracker.Application.Exceptions
{
    public class BudgetExceededException : Exception
    {
        public BudgetExceededException(string message) : base(message) { }
    }
}