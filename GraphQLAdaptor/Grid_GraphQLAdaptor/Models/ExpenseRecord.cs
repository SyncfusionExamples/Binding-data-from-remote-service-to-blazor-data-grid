using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Grid_GraphQLAdaptor.Models
{
    public class ExpenseRecord
    {
        /// <summary>
        /// Static in-memory store for expense records.
        /// This persists data across mutations and queries during application lifetime.
        /// </summary>
        private static List<ExpenseRecord> _expenseStore = null;

        /// <summary>
        /// Initializes the expense store with seed data if not already initialized.
        /// </summary>
        private static void InitializeStore()
        {
            if (_expenseStore != null) return;

            // Load embedded avatars_base64.json and construct data URLs
            var avatars = LoadEmbeddedAvatars();

            // Seed 70 realistic ExpenseRecord entries
            _expenseStore = GenerateExpenses(500, avatars);
        }

        /// <summary>
        /// Retrieves all expense records from the in-memory store.
        /// This list persists across mutations and queries.
        /// </summary>
        public static List<ExpenseRecord> GetAllRecords()
        {
            InitializeStore();
            return _expenseStore;
        }

        /// <summary>
        /// Clears the expense store and reinitializes with seed data.
        /// Useful for testing or resetting the application state.
        /// </summary>
        public static void ResetStore()
        {
            _expenseStore = null;
            InitializeStore();
        }

        // -------------------- Seed generation helpers --------------------
        private static readonly string[] FIRST_NAMES = new[] { "Jane", "Mark", "Olivia", "Ethan", "Sophia", "Liam", "Ava", "Noah", "Mia", "Lucas" };
        private static readonly string[] LAST_NAMES = new[] { "Smith", "Johnson", "Davis", "Brown", "Garcia", "Miller", "Wilson", "Martinez", "Anderson", "Clark" };

        private static readonly string[] DEPARTMENTS = new[] { "Finance", "HR & People", "Engineering", "Marketing", "Sales", "Operations" };
        private static readonly string[] CATEGORIES = new[] { "Travel & Mileage", "Meals & Entertainment", "Office Supplies", "Training & Education", "Software & SaaS", "Lodging" };

        private static readonly Dictionary<string, string[]> CATEGORY_DESCRIPTIONS = new()
        {
            ["Travel & Mileage"] = new[]
            {
                "Mileage reimbursement for regional client visits",
                "Cab fare for airport transfer during client onsite",
                "Fuel expense submitted after sales road trip",
                "Ride-share to partner meeting downtown"
            },
            ["Meals & Entertainment"] = new[]
            {
                "Team lunch with client account executives",
                "Customer dinner during product demo tour",
                "Event catering invoice for investor briefing",
                "Coffee meetup with channel partner"
            },
            ["Office Supplies"] = new[]
            {
                "Bulk stationery order for HQ workspace",
                "Printer ink cartridges for finance pod",
                "Whiteboard markers and notebooks restock",
                "Desk accessories purchase for new hires"
            },
            ["Training & Education"] = new[]
            {
                "Conference registration fee for leadership summit",
                "Online course subscription for certifications",
                "Workshop materials for internal enablement",
                "Tuition reimbursement for professional development"
            },
            ["Software & SaaS"] = new[]
            {
                "Monthly license renewal for analytics suite",
                "Productivity app subscription for marketing",
                "Security software upgrade and support",
                "Design tool seat assignment for creative team"
            },
            ["Lodging"] = new[]
            {
                "Hotel stay for cross-country sales visit",
                "Accommodation invoice for training week",
                "Business travel lodging near client HQ",
                "Extended stay for project deployment"
            }
        };

        private static readonly string[] PAYMENT_METHODS = new[] { "Corporate Card", "Personal Card", "Bank Transfer", "Cash Advance" };
        private static readonly string[] CURRENCIES = new[] { "USD - US Dollar", "EUR - Euro", "GBP - Pound", "JPY - Yen" };
        private static readonly string[] STATUSES = new[] { "Submitted", "Under Review", "Approved", "Paid", "Rejected" };
        private static readonly string[] TAG_OPTIONS = new[] { "Urgent", "Client-Billable", "Non-Billable", "Conference", "Recurring", "Capital Expense" };

        private static Random _rand = new Random();

        private static List<ExpenseRecord> GenerateExpenses(int count, List<string> avatars)
        {
            var list = new List<ExpenseRecord>(count);
            var (startUtc, endUtc) = GetTwoMonthWindowUtc();

            for (int i = 0; i < count; i++)
            {
                var first = Pick(FIRST_NAMES);
                var last = Pick(LAST_NAMES);
                var amount = RandomAmount();
                var taxPct = RandomTaxPct();
                var category = Pick(CATEGORIES);
                var descriptions = CATEGORY_DESCRIPTIONS.ContainsKey(category) ? CATEGORY_DESCRIPTIONS[category] : new[] { "Expense submitted" };
                var dateIso = RandomDateInRangeUtc(startUtc, endUtc);

                var record = new ExpenseRecord
                {
                    ExpenseId = $"EXP{1001 + i}",
                    EmployeeName = $"{first} {last}",
                    EmployeeEmail = $"{first}.{last}@example.com".ToLowerInvariant(),
                    EmployeeAvatarUrl = avatars.Count > 0 ? Pick(avatars) : "https://via.placeholder.com/36",
                    Department = Pick(DEPARTMENTS),
                    Category = category,
                    Description = Pick(descriptions),
                    Amount = amount,
                    TaxPct = taxPct,
                    TotalAmount = Math.Round(amount * (1 + taxPct), 2),
                    ExpenseDate = DateTime.Parse(dateIso, null, System.Globalization.DateTimeStyles.AdjustToUniversal),
                    PaymentMethod = Pick(PAYMENT_METHODS),
                    Currency = Pick(CURRENCIES),
                    ReimbursementStatus = Pick(STATUSES),
                    IsPolicyCompliant = _rand.NextDouble() > 0.2,
                    Tags = RandomTags()
                };

                list.Add(record);
            }

            return list;
        }

        private static (DateTime startUtc, DateTime endUtc) GetTwoMonthWindowUtc()
        {
            // Two months prior to today (UTC) through today (UTC)
            var today = DateTime.UtcNow.Date;
            var start = today.AddMonths(-2);
            var end = today; // inclusive
            return (start, end);
        }

        private static string RandomDateInRangeUtc(DateTime startUtc, DateTime endUtc)
        {
            var dayMs = TimeSpan.FromDays(1).TotalMilliseconds;
            var totalDays = (int)Math.Floor((endUtc - startUtc).TotalDays);
            var offsetDays = _rand.Next(0, totalDays + 1);
            var result = startUtc.AddDays(offsetDays);
            // Return ISO 8601
            return result.ToString("o");
        }

        private static double RandomAmount()
        {
            // 40 - 2000
            var val = _rand.NextDouble() * 1960 + 40;
            return Math.Round(val, 2);
        }

        private static double RandomTaxPct()
        {
            // 2% - 12%
            var pct = _rand.NextDouble() * 0.10 + 0.02;
            return Math.Round(pct, 4);
        }

        private static T Pick<T>(IReadOnlyList<T> list) => list[_rand.Next(0, list.Count)];

        private static List<string> RandomTags()
        {
            var arr = new List<string>(TAG_OPTIONS);
            // Fisher-Yates shuffle
            for (int i = arr.Count - 1; i > 0; i--)
            {
                int j = _rand.Next(i + 1);
                (arr[i], arr[j]) = (arr[j], arr[i]);
            }
            var take = _rand.Next(0, 3);
            arr = arr.GetRange(0, take);
            arr.Sort(StringComparer.Ordinal);
            return arr;
        }

        private static List<string> LoadEmbeddedAvatars()
        {
            var result = new List<string>();
            var asm = typeof(ExpenseRecord).Assembly;
            // Find resource name (Assets/avatars_base64.json embedded)
            string resourceName = null;
            foreach (var name in asm.GetManifestResourceNames())
            {
                if (name.EndsWith("Assets.avatars_base64.json", StringComparison.OrdinalIgnoreCase))
                {
                    resourceName = name;
                    break;
                }
            }
            if (resourceName == null) return result;

            using var stream = asm.GetManifestResourceStream(resourceName);
            using var reader = new System.IO.StreamReader(stream);
            var json = reader.ReadToEnd();

            try
            {
                var doc = System.Text.Json.JsonDocument.Parse(json);
                foreach (var item in doc.RootElement.EnumerateArray())
                {
                    if (item.TryGetProperty("Base64Data", out var b64))
                    {
                        var dataUrl = $"data:image/jpeg;base64,{b64.GetString()}";
                        result.Add(dataUrl);
                    }
                }
            }
            catch
            {
                // ignore and return what we have
            }
            return result;
        }

        [JsonPropertyName("expenseId")]
        public string? ExpenseId { get; set; }

        [JsonPropertyName("employeeName")]
        public string? EmployeeName { get; set; }

        [JsonPropertyName("employeeEmail")]
        public string? EmployeeEmail { get; set; }

        [JsonPropertyName("employeeAvatarUrl")]
        public string? EmployeeAvatarUrl { get; set; }

        [JsonPropertyName("department")]
        public string? Department { get; set; }

        [JsonPropertyName("category")]
        public string? Category { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("amount")]
        public double Amount { get; set; }

        [JsonPropertyName("taxPct")]
        public double TaxPct { get; set; }

        [JsonPropertyName("totalAmount")]
        public double TotalAmount { get; set; }

        [JsonPropertyName("expenseDate")]
        public DateTime? ExpenseDate { get; set; }

        [JsonPropertyName("paymentMethod")]
        public string? PaymentMethod { get; set; }

        [JsonPropertyName("currency")]
        public string? Currency { get; set; }

        [JsonPropertyName("reimbursementStatus")]
        public string? ReimbursementStatus { get; set; } // Values: "Submitted", "Under Review", "Approved", "Paid", "Rejected"

        [JsonPropertyName("isPolicyCompliant")]
        public bool IsPolicyCompliant { get; set; }

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; } = new List<string>();
    }
}
