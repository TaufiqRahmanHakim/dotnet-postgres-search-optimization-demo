using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerDashboard.Migrations
{
    /// <inheritdoc />
    public partial class AddOptimizedIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP INDEX IF EXISTS \"IX_Customers_FirstName\";");
            migrationBuilder.Sql("DROP INDEX IF EXISTS \"IX_Customers_LastName\";");
            migrationBuilder.Sql("DROP INDEX IF EXISTS \"IX_Customers_City\";");
            migrationBuilder.Sql(@"
            CREATE INDEX idx_customers_firstname_lower 
            ON ""Customers"" (lower(""FirstName"") text_pattern_ops);
        ");

            migrationBuilder.Sql(@"
            CREATE INDEX idx_customers_lastname_lower 
            ON ""Customers"" (lower(""LastName"") text_pattern_ops);
        ");

            migrationBuilder.Sql(@"
            CREATE INDEX idx_customers_city_lower 
            ON ""Customers"" (lower(""City"") text_pattern_ops);
        ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP INDEX IF EXISTS idx_customers_firstname_lower;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS idx_customers_lastname_lower;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS idx_customers_city_lower;");
        }
    }
}
