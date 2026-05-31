using TaskFlow.Modules.Projects.Domain;

namespace TaskFlow.Tests.Domain;

public class ProjectTests
{
    private static readonly Guid Owner = Guid.NewGuid();

    [Fact]
    public void Create_enrolls_the_owner_as_the_sole_owner_member()
    {
        var project = Project.Create("Analytical Engine", Owner).Value;

        Assert.Equal(Owner, project.OwnerId);
        Assert.Equal(ProjectStatus.Active, project.Status);
        var member = Assert.Single(project.Members);
        Assert.Equal(Owner, member.UserId);
        Assert.Equal(ProjectRole.Owner, member.Role);
    }

    [Fact]
    public void AddMember_cannot_grant_the_owner_role()
    {
        var project = Project.Create("P", Owner).Value;

        var result = project.AddMember(Guid.NewGuid(), ProjectRole.Owner);

        Assert.True(result.IsFailure);
        Assert.Equal("Error.Validation", result.Error.Code);
    }

    [Fact]
    public void AddMember_twice_for_the_same_user_is_a_conflict()
    {
        var project = Project.Create("P", Owner).Value;
        var user = Guid.NewGuid();

        Assert.True(project.AddMember(user, ProjectRole.Collaborator).IsSuccess);
        var second = project.AddMember(user, ProjectRole.Viewer);

        Assert.True(second.IsFailure);
        Assert.Equal("Error.Conflict", second.Error.Code);
    }

    [Fact]
    public void Archived_project_freezes_membership_changes()
    {
        var project = Project.Create("P", Owner).Value;
        Assert.True(project.Archive().IsSuccess);

        Assert.True(project.IsArchived);
        Assert.True(project.AddMember(Guid.NewGuid(), ProjectRole.Viewer).IsFailure);
        Assert.True(project.Archive().IsFailure);  // idempotency treated as conflict
    }

    [Fact]
    public void RemoveMember_cannot_remove_the_owner()
    {
        var project = Project.Create("P", Owner).Value;

        var result = project.RemoveMember(Owner);

        Assert.True(result.IsFailure);
        Assert.True(project.HasMember(Owner));
    }
}
