using PersonalWealth.Domain.Entities;
using Xunit;

namespace PersonalWealth.UnitTests.Domain.Entities;

public sealed class EntityContractTests
{
    [Fact]
    public void Entity_exposes_a_stable_identity()
    {
        var id = Guid.NewGuid();
        var entity = new TestEntity(id);

        Assert.Equal(id, entity.Id);
        Assert.Equal(id, ((IEntity<Guid>)entity).Id);
    }

    [Fact]
    public void Entities_with_the_same_identity_and_type_are_equal()
    {
        var id = Guid.NewGuid();

        var first = new TestEntity(id);
        var second = new TestEntity(id);

        Assert.True(first.Equals(second));
        Assert.True(first.Equals((object)second));
        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void Entities_with_different_identities_are_not_equal()
    {
        var first = new TestEntity(Guid.NewGuid());
        var second = new TestEntity(Guid.NewGuid());

        Assert.NotEqual(first, second);
    }

    [Fact]
    public void Entities_of_different_types_with_the_same_identity_are_not_equal()
    {
        var id = Guid.NewGuid();

        Assert.NotEqual(new TestEntity(id), new OtherTestEntity(id));
    }

    [Fact]
    public void Entity_equality_handles_null_and_other_object_types()
    {
        var entity = new TestEntity(Guid.NewGuid());

        Assert.False(entity.Equals((Entity<Guid>?)null));
        Assert.False(entity.Equals((object?)null));
        Assert.False(entity.Equals("not an entity"));
    }

    private sealed class TestEntity(Guid id) : Entity<Guid>(id);

    private sealed class OtherTestEntity(Guid id) : Entity<Guid>(id);
}
