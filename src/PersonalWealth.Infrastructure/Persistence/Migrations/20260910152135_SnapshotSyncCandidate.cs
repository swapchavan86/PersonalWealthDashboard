using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalWealth.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SnapshotSyncCandidate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "OutboxMessages",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(byte[]),
                oldType: "rowversion",
                oldRowVersion: true,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    AcquisitionValue = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    AcquisitionDate = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BankAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Institution = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AccountType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExpenseCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ParentCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Expenses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExpenseDate = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    BankTransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImportDuplicateDecisions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Scope = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fingerprint = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDuplicate = table.Column<bool>(type: "bit", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportDuplicateDecisions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImportIdentities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportIdentities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvestmentAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Institution = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AccountType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestmentAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Liabilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    OriginalPrincipal = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    OutstandingPrincipal = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    AnnualInterestRate = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    MaturityDate = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Liabilities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecurringExpenses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Frequency = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurringExpenses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Securities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    SecurityType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Isin = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Securities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserIdentities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserIdentities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssetValuations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ValuationDate = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Source = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetValuations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetValuations_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BankTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Direction = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ImportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceFingerprint = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TransferId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankTransactions_BankAccounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "BankAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LiabilityRepayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LiabilityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    PrincipalAmount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    InterestAmount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiabilityRepayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LiabilityRepayments_Liabilities_LiabilityId",
                        column: x => x.LiabilityId,
                        principalTable: "Liabilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CorporateActions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SecurityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActionType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    RatioNumerator = table.Column<decimal>(type: "decimal(19,10)", precision: 19, scale: 10, nullable: false),
                    RatioDenominator = table.Column<decimal>(type: "decimal(19,10)", precision: 19, scale: 10, nullable: false),
                    NewSymbol = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NewName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorporateActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CorporateActions_Securities_SecurityId",
                        column: x => x.SecurityId,
                        principalTable: "Securities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InvestmentHoldings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvestmentAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SecurityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(28,10)", precision: 28, scale: 10, nullable: false),
                    CostBasis = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestmentHoldings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvestmentHoldings_InvestmentAccounts_InvestmentAccountId",
                        column: x => x.InvestmentAccountId,
                        principalTable: "InvestmentAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvestmentHoldings_Securities_SecurityId",
                        column: x => x.SecurityId,
                        principalTable: "Securities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InvestmentTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvestmentAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SecurityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(28,10)", precision: 28, scale: 10, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Fees = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestmentTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvestmentTransactions_InvestmentAccounts_InvestmentAccountId",
                        column: x => x.InvestmentAccountId,
                        principalTable: "InvestmentAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvestmentTransactions_Securities_SecurityId",
                        column: x => x.SecurityId,
                        principalTable: "Securities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assets_TenantId",
                table: "Assets",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_TenantId_Name",
                table: "Assets",
                columns: new[] { "TenantId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_AssetValuations_AssetId",
                table: "AssetValuations",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetValuations_TenantId",
                table: "AssetValuations",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetValuations_TenantId_AssetId_ValuationDate",
                table: "AssetValuations",
                columns: new[] { "TenantId", "AssetId", "ValuationDate" });

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_TenantId",
                table: "BankAccounts",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_TenantId_AccountNumber",
                table: "BankAccounts",
                columns: new[] { "TenantId", "AccountNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BankTransactions_AccountId",
                table: "BankTransactions",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BankTransactions_TenantId",
                table: "BankTransactions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_BankTransactions_TenantId_AccountId_SourceFingerprint",
                table: "BankTransactions",
                columns: new[] { "TenantId", "AccountId", "SourceFingerprint" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BankTransactions_TenantId_ImportId",
                table: "BankTransactions",
                columns: new[] { "TenantId", "ImportId" });

            migrationBuilder.CreateIndex(
                name: "IX_CorporateActions_SecurityId",
                table: "CorporateActions",
                column: "SecurityId");

            migrationBuilder.CreateIndex(
                name: "IX_CorporateActions_TenantId",
                table: "CorporateActions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CorporateActions_TenantId_SecurityId_EffectiveDate",
                table: "CorporateActions",
                columns: new[] { "TenantId", "SecurityId", "EffectiveDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseCategories_TenantId",
                table: "ExpenseCategories",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseCategories_TenantId_Name",
                table: "ExpenseCategories",
                columns: new[] { "TenantId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseCategories_TenantId_ParentCategoryId",
                table: "ExpenseCategories",
                columns: new[] { "TenantId", "ParentCategoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_TenantId",
                table: "Expenses",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_TenantId_BankTransactionId",
                table: "Expenses",
                columns: new[] { "TenantId", "BankTransactionId" },
                unique: true,
                filter: "[BankTransactionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_TenantId_CategoryId_ExpenseDate",
                table: "Expenses",
                columns: new[] { "TenantId", "CategoryId", "ExpenseDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_TenantId_ExpenseDate",
                table: "Expenses",
                columns: new[] { "TenantId", "ExpenseDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ImportDuplicateDecisions_TenantId",
                table: "ImportDuplicateDecisions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportIdentities_TenantId",
                table: "ImportIdentities",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentAccounts_TenantId",
                table: "InvestmentAccounts",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentAccounts_TenantId_AccountNumber",
                table: "InvestmentAccounts",
                columns: new[] { "TenantId", "AccountNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentHoldings_InvestmentAccountId",
                table: "InvestmentHoldings",
                column: "InvestmentAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentHoldings_SecurityId",
                table: "InvestmentHoldings",
                column: "SecurityId");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentHoldings_TenantId",
                table: "InvestmentHoldings",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentHoldings_TenantId_InvestmentAccountId_SecurityId",
                table: "InvestmentHoldings",
                columns: new[] { "TenantId", "InvestmentAccountId", "SecurityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentTransactions_InvestmentAccountId",
                table: "InvestmentTransactions",
                column: "InvestmentAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentTransactions_SecurityId",
                table: "InvestmentTransactions",
                column: "SecurityId");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentTransactions_TenantId",
                table: "InvestmentTransactions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentTransactions_TenantId_InvestmentAccountId_TransactionDate",
                table: "InvestmentTransactions",
                columns: new[] { "TenantId", "InvestmentAccountId", "TransactionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentTransactions_TenantId_SecurityId_TransactionDate",
                table: "InvestmentTransactions",
                columns: new[] { "TenantId", "SecurityId", "TransactionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Liabilities_TenantId",
                table: "Liabilities",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Liabilities_TenantId_Name",
                table: "Liabilities",
                columns: new[] { "TenantId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_LiabilityRepayments_LiabilityId",
                table: "LiabilityRepayments",
                column: "LiabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_LiabilityRepayments_TenantId",
                table: "LiabilityRepayments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_LiabilityRepayments_TenantId_LiabilityId_PaymentDate",
                table: "LiabilityRepayments",
                columns: new[] { "TenantId", "LiabilityId", "PaymentDate" });

            migrationBuilder.CreateIndex(
                name: "IX_RecurringExpenses_TenantId",
                table: "RecurringExpenses",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringExpenses_TenantId_IsActive",
                table: "RecurringExpenses",
                columns: new[] { "TenantId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_RecurringExpenses_TenantId_StartDate",
                table: "RecurringExpenses",
                columns: new[] { "TenantId", "StartDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Securities_TenantId",
                table: "Securities",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Securities_TenantId_Isin",
                table: "Securities",
                columns: new[] { "TenantId", "Isin" });

            migrationBuilder.CreateIndex(
                name: "IX_Securities_TenantId_Symbol",
                table: "Securities",
                columns: new[] { "TenantId", "Symbol" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Name",
                table: "Tenants",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserIdentities_TenantId",
                table: "UserIdentities",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_UserIdentities_TenantId_Email",
                table: "UserIdentities",
                columns: new[] { "TenantId", "Email" });

            migrationBuilder.CreateIndex(
                name: "IX_UserIdentities_TenantId_Subject",
                table: "UserIdentities",
                columns: new[] { "TenantId", "Subject" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetValuations");

            migrationBuilder.DropTable(
                name: "BankTransactions");

            migrationBuilder.DropTable(
                name: "CorporateActions");

            migrationBuilder.DropTable(
                name: "ExpenseCategories");

            migrationBuilder.DropTable(
                name: "Expenses");

            migrationBuilder.DropTable(
                name: "ImportDuplicateDecisions");

            migrationBuilder.DropTable(
                name: "ImportIdentities");

            migrationBuilder.DropTable(
                name: "InvestmentHoldings");

            migrationBuilder.DropTable(
                name: "InvestmentTransactions");

            migrationBuilder.DropTable(
                name: "LiabilityRepayments");

            migrationBuilder.DropTable(
                name: "RecurringExpenses");

            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropTable(
                name: "UserIdentities");

            migrationBuilder.DropTable(
                name: "Assets");

            migrationBuilder.DropTable(
                name: "BankAccounts");

            migrationBuilder.DropTable(
                name: "InvestmentAccounts");

            migrationBuilder.DropTable(
                name: "Securities");

            migrationBuilder.DropTable(
                name: "Liabilities");

            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "OutboxMessages",
                type: "rowversion",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "rowversion",
                oldRowVersion: true);
        }
    }
}
