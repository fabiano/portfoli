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
