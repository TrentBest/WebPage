using TheSingularityWorkshop.SingularityHub;

namespace TheSingularityWorkshop.Workshop.Education;

/// <summary>
/// Educational Experience composition boundary. The shared lesson is the
/// reusable data product; orchestration and teacher personalization are
/// separate capabilities.
/// </summary>
public sealed class EducationExperience : IExperience
{
    public const ulong ExperienceId = 2400;

    public EducationExperience(SharedLesson lesson)
    {
        Lesson = EducationalContentNormalizer.Normalize(lesson);
        Coordinator = new EducationCoordinator();
        TeacherView = new TeacherLessonView(
            Lesson,
            new LessonTheme("Workshop", "cyan", "focused"));

        MicroBundleIds =
        [
            2401, // shared lesson
            2402, // teacher controls
            2403, // student workspaces
            2404, // question recognition
            2405  // content normalization
        ];

        Capabilities =
        [
            2402,
            2403,
            2404,
            2405
        ];

        SensorySystems = [1, 2, 3];
        ProcessingGroups = ["Education", "Education.Questions", "Education.Students"];
    }

    public SharedLesson Lesson { get; }
    public EducationCoordinator Coordinator { get; }
    public TeacherLessonView TeacherView { get; }

    public ulong Id => ExperienceId;
    public string Name => "Education";
    public BundleVersion Version => new(1, 0, 0);
    public OntologySignature Ontology => new(4, 1, 1, 1, 1, 1, 1, 1, 2400);
    public IReadOnlyList<ulong> MicroBundleIds { get; }
    public IReadOnlyList<ulong> Capabilities { get; }
    public IReadOnlyList<ulong> SensorySystems { get; }
    public IReadOnlyList<string> ProcessingGroups { get; }
}
