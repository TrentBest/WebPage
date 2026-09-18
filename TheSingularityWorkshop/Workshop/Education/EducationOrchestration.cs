using System.Collections.ObjectModel;
using TheSingularityWorkshop.FSM_API;

namespace TheSingularityWorkshop.Workshop.Education;

/// <summary>
/// Shared educational content that can be reused by many teachers while
/// teacher-specific presentation remains an overlay rather than a fork.
/// </summary>
public sealed record SharedLesson(
    int Id,
    string Title,
    string Subject,
    IReadOnlyList<LessonBlock> Blocks);

public sealed record LessonBlock(
    int Id,
    string Heading,
    string Content,
    IReadOnlyList<string> Concepts);

public sealed record LessonTheme(
    string Name,
    string Accent,
    string Density);

/// <summary>Teacher-specific choices applied without duplicating the shared lesson.</summary>
public sealed class TeacherLessonView
{
    private readonly HashSet<int> _hiddenBlocks = [];
    private readonly Dictionary<int, string> _blockOverrides = [];

    public TeacherLessonView(SharedLesson lesson, LessonTheme theme)
    {
        Lesson = lesson;
        Theme = theme;
    }

    public SharedLesson Lesson { get; }
    public LessonTheme Theme { get; private set; }

    public void SetTheme(LessonTheme theme) => Theme = theme;

    public void HideBlock(int blockId) => _hiddenBlocks.Add(blockId);

    public void OverrideBlock(int blockId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Override content is required.", nameof(content));

        _blockOverrides[blockId] = content.Trim();
    }

    public IReadOnlyList<LessonBlock> VisibleBlocks()
        => Lesson.Blocks
            .Where(block => !_hiddenBlocks.Contains(block.Id))
            .Select(block => _blockOverrides.TryGetValue(block.Id, out var value)
                ? block with { Content = value }
                : block)
            .ToArray();
}

/// <summary>
/// A student's spatial and orchestration state. The FSM_API context is the
/// state-bearing object; presentation can move between Blazor, Unity, WPF, etc.
/// </summary>
public sealed class StudentWorkspace : IStateContext
{
    public StudentWorkspace(int id, string name, string zone)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
        Id = id;
        Name = string.IsNullOrWhiteSpace(name)
            ? throw new ArgumentException("Student name is required.", nameof(name))
            : name.Trim();
        Zone = string.IsNullOrWhiteSpace(zone)
            ? throw new ArgumentException("Student zone is required.", nameof(zone))
            : zone.Trim();
    }

    public int Id { get; }
    public string Name { get; set; }
    public string Zone { get; private set; }
    public bool IsValid { get; set; } = true;
    public StudentActivity Activity { get; private set; } = StudentActivity.Working;

    public void MoveTo(string zone)
    {
        if (string.IsNullOrWhiteSpace(zone))
            throw new ArgumentException("Zone is required.", nameof(zone));
        Zone = zone.Trim();
    }

    public void Assign(StudentActivity activity) => Activity = activity;
}

public enum StudentActivity
{
    Working,
    Observing,
    WaitingForTeacher,
    Reviewing,
    Complete
}

public sealed record StudentQuestion(
    int StudentId,
    string Text,
    DateTimeOffset AskedAt);

public sealed record WaitingQuestion(
    StudentQuestion Question,
    TimeSpan RecognitionWindow,
    StudentActivity AssignedActivity);

/// <summary>
/// Teacher-facing controls for attention management. A waiting student is
/// explicitly acknowledged and assigned useful work rather than disappearing.
/// </summary>
public sealed class EducationCoordinator
{
    private readonly List<StudentWorkspace> _students = [];
    private readonly Queue<WaitingQuestion> _waitingQuestions = new();
    private readonly Dictionary<string, string> _faq = new(StringComparer.OrdinalIgnoreCase);

    public ReadOnlyCollection<StudentWorkspace> Students => _students.AsReadOnly();
    public IReadOnlyCollection<WaitingQuestion> WaitingQuestions => _waitingQuestions.ToArray();
    public TimeSpan RecognitionWindow { get; private set; } = TimeSpan.FromMinutes(5);

    public void SetRecognitionWindow(TimeSpan window)
    {
        if (window <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(window));
        RecognitionWindow = window;
    }

    public StudentWorkspace AddStudent(StudentWorkspace student)
    {
        ArgumentNullException.ThrowIfNull(student);
        if (_students.Any(x => x.Id == student.Id))
            throw new InvalidOperationException($"Student {student.Id} already exists.");

        _students.Add(student);
        return student;
    }

    public void AddFaq(string question, string answer)
    {
        if (string.IsNullOrWhiteSpace(question)) throw new ArgumentException("Question is required.", nameof(question));
        if (string.IsNullOrWhiteSpace(answer)) throw new ArgumentException("Answer is required.", nameof(answer));
        _faq[Normalize(question)] = answer.Trim();
    }

    /// <summary>
    /// Returns the normalized FAQ answer when known; otherwise creates a
    /// visible waiting state and assigns the student an activity.
    /// </summary>
    public QuestionResponse Ask(StudentQuestion question, StudentActivity waitingActivity = StudentActivity.Observing)
    {
        ArgumentNullException.ThrowIfNull(question);

        if (_faq.TryGetValue(Normalize(question.Text), out var answer))
        {
            FindStudent(question.StudentId).Assign(StudentActivity.Working);
            return QuestionResponse.Answered(answer);
        }

        var waiting = new WaitingQuestion(question, RecognitionWindow, waitingActivity);
        _waitingQuestions.Enqueue(waiting);
        FindStudent(question.StudentId).Assign(StudentActivity.WaitingForTeacher);
        return QuestionResponse.Waiting(waiting);
    }

    public WaitingQuestion? RecognizeNext()
    {
        if (!_waitingQuestions.TryDequeue(out var waiting))
            return null;

        FindStudent(waiting.Question.StudentId).Assign(StudentActivity.Working);
        return waiting;
    }

    private StudentWorkspace FindStudent(int id)
        => _students.FirstOrDefault(x => x.Id == id)
           ?? throw new InvalidOperationException($"Student {id} is not registered.");

    private static string Normalize(string value)
        => string.Join(' ', value.Trim().Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries))
            .ToUpperInvariant();
}

public sealed record QuestionResponse(
    bool AnsweredImmediately,
    string? Answer,
    WaitingQuestion? Waiting)
{
    public static QuestionResponse Answered(string answer) => new(true, answer, null);
    public static QuestionResponse Waiting(WaitingQuestion waiting) => new(false, null, waiting);
}

/// <summary>
/// Normalizes shared content into a canonical form while leaving teacher
/// presentation choices outside the canonical content record.
/// </summary>
public static class EducationalContentNormalizer
{
    public static SharedLesson Normalize(SharedLesson lesson)
    {
        ArgumentNullException.ThrowIfNull(lesson);

        var blocks = lesson.Blocks
            .Select(block => block with
            {
                Heading = NormalizeText(block.Heading),
                Content = NormalizeText(block.Content),
                Concepts = block.Concepts.Select(NormalizeText).Distinct(StringComparer.OrdinalIgnoreCase).ToArray()
            })
            .ToArray();

        return lesson with
        {
            Title = NormalizeText(lesson.Title),
            Subject = NormalizeText(lesson.Subject),
            Blocks = blocks
        };
    }

    private static string NormalizeText(string value)
        => string.Join(' ', value.Trim().Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
}
