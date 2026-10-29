using DataAccess.Data;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Constraints;

namespace XUnit_Tests.Data;

public class AppDbContextModelTests
{
    private const string ConnectionString = "Host=localhost;Database=todo_tests;Username=todo;Password=todo";

    [Fact]
    public void TaskColumns_MatchSharedLimitsAndNullability()
    {
        using var context = CreateContext();
        var entity = context.Model.FindEntityType(typeof(TaskModel));

        Assert.NotNull(entity);
        Assert.Equal(FieldLimits.ItemNameMax, MaxLength<TaskModel>(context, nameof(TaskModel.Name)));
        Assert.Equal(FieldLimits.TaskDescriptionMax, MaxLength<TaskModel>(context, nameof(TaskModel.Description)));
        Assert.False(IsNullable<TaskModel>(context, nameof(TaskModel.Name)));
        Assert.True(IsNullable<TaskModel>(context, nameof(TaskModel.Description)));
    }

    [Fact]
    public void NoteColumns_MatchSharedLimitsAndNullability()
    {
        using var context = CreateContext();

        Assert.Equal(FieldLimits.ItemNameMax, MaxLength<NoteModel>(context, nameof(NoteModel.Name)));
        Assert.Equal(FieldLimits.NoteTextMax, MaxLength<NoteModel>(context, nameof(NoteModel.Text)));
        Assert.False(IsNullable<NoteModel>(context, nameof(NoteModel.Name)));
        Assert.True(IsNullable<NoteModel>(context, nameof(NoteModel.Text)));
    }

    [Fact]
    public void UserName_HasSharedLimitAndUniqueIndex()
    {
        using var context = CreateContext();
        var entity = context.Model.FindEntityType(typeof(UserModel));

        Assert.NotNull(entity);
        Assert.Equal(FieldLimits.UserNameMax, MaxLength<UserModel>(context, nameof(UserModel.Name)));
        Assert.Contains(
            entity.GetIndexes(),
            index => index.IsUnique && index.Properties.Count == 1 &&
                     index.Properties[0].Name == nameof(UserModel.Name));
    }

    [Fact]
    public void ItemRelationships_CascadeDelete()
    {
        using var context = CreateContext();

        var taskForeignKey = context.Model.FindEntityType(typeof(TaskModel))!.GetForeignKeys().Single();
        var noteForeignKey = context.Model.FindEntityType(typeof(NoteModel))!.GetForeignKeys().Single();

        Assert.Equal(typeof(UserModel), taskForeignKey.PrincipalEntityType.ClrType);
        Assert.Equal(DeleteBehavior.Cascade, taskForeignKey.DeleteBehavior);
        Assert.Equal(typeof(UserModel), noteForeignKey.PrincipalEntityType.ClrType);
        Assert.Equal(DeleteBehavior.Cascade, noteForeignKey.DeleteBehavior);
    }

    private static AppDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(ConnectionString)
            .Options);

    private static int? MaxLength<TEntity>(DbContext context, string propertyName) =>
        context.Model.FindEntityType(typeof(TEntity))!
            .FindProperty(propertyName)!
            .GetMaxLength();

    private static bool IsNullable<TEntity>(DbContext context, string propertyName) =>
        context.Model.FindEntityType(typeof(TEntity))!
            .FindProperty(propertyName)!
            .IsNullable;
}
