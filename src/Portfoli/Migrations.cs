using FluentMigrator;

namespace Portfoli;

[Migration(202508121930)]
public class CreatePortfolioTable : Migration
{
    public override void Up()
    {
        Create
            .Table("Portfolio")

            .WithColumn("Id")
                .AsGuid()
                .PrimaryKey()

            .WithColumn("Name")
                .AsString()
                .NotNullable();
    }

    public override void Down()
    {
        Delete.Table("Portfolio");
    }
}

[Migration(202508122030)]
public class CreateHoldingTable : Migration
{
    public override void Up()
    {
        Create
            .Table("Holding")

            .WithColumn("Id")
                .AsGuid()
                .PrimaryKey()

            .WithColumn("PortfolioId")
                .AsGuid()
                .NotNullable()

            .WithColumn("Asset_Exchange")
                .AsString(128)
                .NotNullable()

            .WithColumn("Asset_Ticker")
                .AsString(128)
                .NotNullable()

            .WithColumn("Asset_Name")
                .AsString(512)
                .NotNullable()

            .WithColumn("Asset_Type")
                .AsString(32)
                .NotNullable()

            .WithColumn("Quantity")
                .AsDecimal()
                .NotNullable();
    }

    public override void Down()
    {
        Delete.Table("Holding");
    }
}

[Migration(202508122035)]
public class CreateTransactionTable : Migration
{
    public override void Up()
    {
        Create
            .Table("Transaction")

            .WithColumn("Id")
                .AsGuid()
                .PrimaryKey()

            .WithColumn("PortfolioId")
                .AsGuid()
                .NotNullable()

            .WithColumn("HoldingId")
                .AsGuid()
                .NotNullable()

            .WithColumn("Type")
                .AsString(8)
                .NotNullable()

            .WithColumn("Date")
                .AsDateTime()
                .NotNullable()

            .WithColumn("Quantity")
                .AsDecimal()
                .NotNullable()

            .WithColumn("Price")
                .AsDecimal()
                .NotNullable()

            .WithColumn("Commission")
                .AsDecimal()
                .NotNullable();
    }

    public override void Down()
    {
        Delete.Table("Transaction");
    }
}
