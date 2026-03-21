using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalFinanceTracker.Domain.Common
{
    public static class CategoryTypes
    {
        public const string Income = "Income";
        public const string Expense = "Expense";

        // Danh sách hỗ trợ cho việc Validation hoặc Seeding
        public static readonly List<string> All = new() { Income, Expense };
    }
}