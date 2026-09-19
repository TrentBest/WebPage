using TheSingularityWorkshop.Workshop.Advertising;
using TheSingularityWorkshop.Workshop.Education;
using Xunit;

namespace SingularityHub.Tests;

public sealed class MadmenEducationTests
{
    [Fact(DisplayName = "Madmen agency can create inventory and procure it for a virtual client")]
    public void MadmenCanCreateAndProcure()
    {
        var experience = new MadmenExperience();
        var client = experience.Agency.AddClient(1, "Virtual Tractor Company");
        var space = experience.Agency.CreateSpace(
            10,
            "Explore / Construction District / Tractor Depot",
            AdvertisingFormat.EnvironmentObject,
            "Visitors exploring construction technology",
            attentionCost: 1);

        var campaign = experience.Agency.Procure(client, space, "See how this machine is built.");

        Assert.Equal(client, campaign.Client);
        Assert.Equal(space, campaign.Space);
        Assert.False(space.IsAvailable);
        Assert.Single(experience.Agency.Campaigns);
    }

    [Fact(DisplayName = "Madmen can create low-attention inventory without forcing interruption")]
    public void MadmenCanRepresentNonInterruptiveSpace()
    {
        var experience = new MadmenExperience();
        var space = experience.Agency.CreateSpace(
            11,
            "Education / Lesson / Reference Rail",
            AdvertisingFormat.LessonSponsor,
            "Students already studying the lesson",
            attentionCost: 0,
            isInterruptive: false);

        Assert.False(space.IsInterruptive);
        Assert.Equal(0, space.AttentionCost);
        Assert.True(space.IsAvailable);
    }

    [Fact(DisplayName = "Education shares normalized lesson content while allowing teacher-specific presentation")]
    public void EducationReusesAndPersonalizes()
    {
        var lesson = new SharedLesson(
            1,
            "  Forces   and Motion ",
            " physics ",
            [
                new LessonBlock(1, " Newton's First Law ", " Objects   resist change. ", [" inertia ", "inertia"])
            ]);

        var experience = new EducationExperience(lesson);
        experience.TeacherView.SetTheme(new LessonTheme("Blueprint", "orange", "dense"));
        experience.TeacherView.OverrideBlock(1, "Show the same concept using the tractor we just observed.");

        Assert.Equal("Forces and Motion", experience.Lesson.Title);
        Assert.Equal("Physics", experience.Lesson.Subject);
        Assert.Equal("Show the same concept using the tractor we just observed.", experience.TeacherView.VisibleBlocks()[0].Content);
        Assert.Equal("Blueprint", experience.TeacherView.Theme.Name);
        Assert.Equal(" Objects   resist change. ", lesson.Blocks[0].Content);
    }

    [Fact(DisplayName = "Unknown student question becomes acknowledged waiting work")]
    public void UnknownQuestionBecomesWaitingWork()
    {
        var coordinator = new EducationCoordinator();
        coordinator.SetRecognitionWindow(TimeSpan.FromMinutes(7));
        var student = coordinator.AddStudent(new StudentWorkspace(1, "Student 1", "North Table"));
        coordinator.AddFaq("What is inertia?", "Resistance to a change in motion.");

        var response = coordinator.Ask(
            new StudentQuestion(1, "What happens when a question isn't in the FAQ?", DateTimeOffset.UtcNow),
            StudentActivity.Observing);

        Assert.False(response.AnsweredImmediately);
        Assert.NotNull(response.Waiting);
        Assert.Equal(TimeSpan.FromMinutes(7), response.Waiting.RecognitionWindow);
        Assert.Equal(StudentActivity.WaitingForTeacher, student.Activity);
        Assert.Equal("Observing", response.Waiting.AssignedActivity.ToString());
    }

    [Fact(DisplayName = "Known FAQ question returns immediately and restores student work")]
    public void KnownQuestionReturnsImmediately()
    {
        var coordinator = new EducationCoordinator();
        var student = coordinator.AddStudent(new StudentWorkspace(1, "Student 1", "South Table"));
        coordinator.AddFaq("What is inertia?", "Resistance to a change in motion.");

        var response = coordinator.Ask(
            new StudentQuestion(1, "  WHAT IS INERTIA? ", DateTimeOffset.UtcNow));

        Assert.True(response.AnsweredImmediately);
        Assert.Equal("Resistance to a change in motion.", response.Answer);
        Assert.Equal(StudentActivity.Working, student.Activity);
        Assert.Empty(coordinator.WaitingQuestions);
    }

    [Fact(DisplayName = "Student spatial zones remain independent")]
    public void StudentZonesRemainIndependent()
    {
        var coordinator = new EducationCoordinator();
        var first = coordinator.AddStudent(new StudentWorkspace(1, "A", "North"));
        var second = coordinator.AddStudent(new StudentWorkspace(2, "B", "South"));

        first.MoveTo("Observation");
        second.MoveTo("West");

        Assert.Equal("Observation", first.Zone);
        Assert.Equal("West", second.Zone);
        Assert.NotEqual(first.Zone, second.Zone);
    }
}
