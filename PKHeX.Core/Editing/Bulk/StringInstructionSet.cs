using System;
using System.Collections.Generic;
using System.Text;

namespace PKHeX.Core;

/// <summary>
/// Processes input of strings into a list of valid Filters and Instructions.
/// </summary>
public sealed class StringInstructionSet
{
    public readonly IReadOnlyList<StringInstruction> Filters;
    public readonly IReadOnlyList<StringInstruction> Instructions;

    private const string SetSeparator = ";";
    private const char SetSeparatorChar = ';';

    public StringInstructionSet(IReadOnlyList<StringInstruction> filters, IReadOnlyList<StringInstruction> instructions)
    {
        Filters = filters;
        Instructions = instructions;
    }

    public StringInstructionSet(ReadOnlySpan<char> text)
    {
        var set = text.EnumerateLines();
        Filters = StringInstruction.GetFilters(set);
        Instructions = StringInstruction.GetInstructions(set);
    }

    public StringInstructionSet(SpanLineEnumerator set)
    {
        Filters = StringInstruction.GetFilters(set);
        Instructions = StringInstruction.GetInstructions(set);
    }

    public StringInstructionSet(ReadOnlySpan<string> set)
    {
        Filters = StringInstruction.GetFilters(set);
        Instructions = StringInstruction.GetInstructions(set);
    }

    public static bool HasEmptyLine(ReadOnlySpan<char> text)
    {
        var lines = text.EnumerateLines();
        foreach (var line in lines)
        {
            if (line.IsEmpty || line.IsWhiteSpace())
                return true;
        }
        return false;
    }

    public static StringInstructionSet[] GetBatchSets(ReadOnlySpan<string> lines)
    {
        int ctr = 0;
        int start = 0;
        while (start < lines.Length)
        {
            var slice = lines[start..];
            var count = GetInstructionSetLength(slice);
            ctr++;
            start += count + 1;
        }

        var result = new StringInstructionSet[ctr];
        ctr = 0;
        start = 0;
        while (start < lines.Length)
        {
            var slice = lines[start..];
            var count = GetInstructionSetLength(slice);
            var set = slice[..count].ToArray();
            result[ctr++] = new StringInstructionSet(set);
            start += count + 1;
        }
        return result;
    }

    public static StringInstructionSet[] GetBatchSets(ReadOnlySpan<char> text)
    {
        int ctr = 0;
        int start = 0;
        while (start < text.Length)
        {
            var slice = text[start..];
            var count = GetInstructionSetLength(slice);
            ctr++;
            start += count + 1;
        }

        var result = new StringInstructionSet[ctr];
        ctr = 0;
        start = 0;
        while (start < text.Length)
        {
            var slice = text[start..];
            var count = GetInstructionSetLength(slice);
            var set = slice[..count];
            result[ctr++] = new StringInstructionSet(set);
            start += count + 1;
        }
        return result;
    }

    public static int GetInstructionSetLength(ReadOnlySpan<char> text)
    {
        int start = 0;
        while (start < text.Length)
        {
            var line = text[start..];
            if (line.Length != 0 && line[0] == SetSeparatorChar)
                return start;
            start++;
        }
        return start;
    }

    public static int GetInstructionSetLength(ReadOnlySpan<string> lines)
    {
        int start = 0;
        while (start < lines.Length)
        {
            if (lines[start++].StartsWith(SetSeparator, StringComparison.Ordinal))
                return start;
        }
        return start;
    }
}
