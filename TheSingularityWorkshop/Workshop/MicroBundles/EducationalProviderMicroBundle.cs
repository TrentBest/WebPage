using System;
using System.Collections.Generic;
using System.Linq;
using TheSingularityWorkshop.SingularityHub;

namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// Free educational capability for teachers, lessons, questions, answers, and reusable course content.
/// </summary>
/// <remarks>
/// The provider intentionally stores semantic teaching records rather than owning a school-specific
/// user interface. A future ApiGuiBuilder or RestGuiBuilder can manifest the same capability for
/// different learning systems.
///
/// The capability is free by design. External grading or school systems are integrations, not
/// prerequisites for authoring foundational Workshop content.
/// </remarks>
public sealed class EducationalProviderMicroBundle : IDisposable
{
    public const int BundleId = 2301;

    private readonly MicroBundle _lifecycle;
    private readonly List<EducationalLesson> _lessons = [];
    private readonly List<EducationalQuestionAnswer> _questionAnswers = [];
    private bool _disposed;

    public EducationalProviderMicroBundle()
    {
        _lifecycle = new MicroBundle(BundleId, "EDUCATIONAL PROVIDER", new WebMicroBundleProvider());
        Presentation = new MicroBundlePresentation(
            "Educational Provider",
            "FREE EDUCATION",
            "Reusable teaching capability for lessons, questions, answers, and foundational course content.",
            ["LESSONS", "QUESTIONS", "ANSWERS", "REUSE"]);
    }

    public ulong Id => (ulong)_lifecycle.Id;
    public MicroBundlePresentation Presentation { get; }

    public IReadOnlyList<EducationalLesson> Lessons => _lessons;
    public IReadOnlyList<EducationalQuestionAnswer> QuestionAnswers => _questionAnswers;

    /// <summary>Adds a lesson to the shared educational catalog.</summary>
    public void AddLesson(EducationalLesson lesson)
    {
        if (string.IsNullOrWhiteSpace(lesson.Id))
            throw new ArgumentException("Lesson id is required.", nameof(lesson));

        if (_lessons.Any(x => string.Equals(x.Id, lesson.Id, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"Lesson '{lesson.Id}' already exists.");

        _lessons.Add(lesson);
    }

    /// <summary>Captures a teacher-authored question and answer for later reuse.</summary>
    public void CaptureQuestionAnswer(EducationalQuestionAnswer questionAnswer)
    {
        if (string.IsNullOrWhiteSpace(questionAnswer.Question))
            throw new ArgumentException("Question is required.", nameof(questionAnswer));

        if (string.IsNullOrWhiteSpace(questionAnswer.Answer))
            throw new ArgumentException("Answer is required.", nameof(questionAnswer));

        _questionAnswers.Add(questionAnswer);
    }

    /// <summary>
    /// Returns recorded answers whose normalized question text is an exact match.
    /// Semantic/LLM similarity belongs above this deterministic storage boundary.
    /// </summary>
    public IReadOnlyList<EducationalQuestionAnswer> FindExactQuestionMatches(string question)
    {
        var normalized = Normalize(question);
        return _questionAnswers
            .Where(x => Normalize(x.Question) == normalized)
            .ToArray();
    }

    /// <summary>Advances the capability lifecycle.</summary>
    public void Update() => _lifecycle.Update();

    public void Dispose()
    {
        if (_disposed)
            return;

        _lifecycle.Dispose();
        _disposed = true;
    }

    private static string Normalize(string value)
        => string.Join(' ', value.Trim().Split(
            (char[]?)null,
            StringSplitOptions.RemoveEmptyEntries));
}

/// <summary>A reusable lesson authored by an educator.</summary>
public sealed record EducationalLesson(
    string Id,
    string Title,
    string Subject,
    string AuthorId,
    IReadOnlyList<string> Concepts);

/// <summary>A teacher-authored answer that can become reusable course knowledge.</summary>
public sealed record EducationalQuestionAnswer(
    string Question,
    string Answer,
    string AuthorId,
    string CourseId,
    IReadOnlyList<string> Concepts);
