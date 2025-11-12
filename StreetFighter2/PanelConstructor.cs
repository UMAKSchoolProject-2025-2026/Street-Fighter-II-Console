using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace StreetFighter2
{
    // OOP-focused notes:
    // - Encapsulation: PanelConstructor exposes a small public surface (SetLeftContent, Render, RenderWithColors, etc.)
    //   while keeping layout helpers and internal buffers private.
    // - Single Responsibility: This class is responsible only for building and rendering a 3-column console panel.
    // - Composition: It composes content lists (left/center/right) and alignment enums rather than inheriting behavior.
    // - Testability: Widths and alignment are deterministically calculated from WindowWidth and provided sizes.
    //
    // Niche / interesting methods:
    // - InitializeWidths: enforces minimum CenterWidth and redistributes space when needed (defensive layout).
    // - WrapText: a simple word-wrapping implementation that prefers breaking at spaces; useful for console art/descriptions.
    // - RenderInteractiveMenu: builds a selectable center menu and highlights the current choice with direct console color writes.
    // - RenderWithLeftPanelInverted: shows how to invert the left panel's background for emphasis (useful for "active" panels).
    // - BuildHighlightedList: reusable helper to build selection lists (returns lines with highlight markers).
    public enum HorizontalAlign { Left, Center, Right }
    public enum VerticalAlign { Top, Middle, Bottom }
    class PanelConstructor
    {
        // Public properties provide read-only access to computed panel sizes (encapsulation).
        public int LeftWidth { get; private set; }
        public int CenterWidth { get; private set; }
        public int RightWidth { get; private set; }
        public int Height { get; private set; }

        // Internal buffers for content — kept private so callers cannot mutate state directly.
        private List<string> leftContent = new List<string>();
        private List<string> rightContent = new List<string>();
        private List<string> centerContent = new List<string>();

        // Public alignment knobs (composition of behavior via simple enums).
        public HorizontalAlign LeftAlignH { get; set; } = HorizontalAlign.Center;
        public HorizontalAlign CenterAlignH { get; set; } = HorizontalAlign.Center;
        public HorizontalAlign RightAlignH { get; set; } = HorizontalAlign.Center;

        public VerticalAlign LeftAlignV { get; set; } = VerticalAlign.Middle;
        public VerticalAlign CenterAlignV { get; set; } = VerticalAlign.Middle;
        public VerticalAlign RightAlignV { get; set; } = VerticalAlign.Middle;

        // Presentation options
        public bool ShowBorders { get; set; } = true;

        // Auto-wrap flags — toggles behavior without changing API shape (open for extension).
        public bool AutoWrapLeft { get; set; } = false;
        public bool AutoWrapCenter { get; set; } = false;
        public bool AutoWrapRight { get; set; } = false;

        // Constructor overloads: height optional (Height==0 means auto-size to content).
        public PanelConstructor(int leftWidth, int rightWidth, int height, bool showBorders = true)
        {
            Height = height;
            ShowBorders = showBorders;
            InitializeWidths(leftWidth, rightWidth);
        }

        public PanelConstructor(int leftWidth, int rightWidth, bool showBorders = true)
        {
            Height = 0; // auto-height
            ShowBorders = showBorders;
            InitializeWidths(leftWidth, rightWidth);
        }

        // Niche method: defensive width computation and redistribution.
        // Ensures CenterWidth remains usable (minimum of 1, tries to maintain at least 10 if possible).
        private void InitializeWidths(int leftWidth, int rightWidth)
        {
            int totalWidth = WindowWidth;
            int seperatorCount = ShowBorders ? 2 : 0; 

            LeftWidth = leftWidth;
            RightWidth = rightWidth;

            CenterWidth = totalWidth - LeftWidth - RightWidth - seperatorCount;

            if (CenterWidth < 10)
            {
                int deficit = 10 - CenterWidth;
                int takeFromLeft = Math.Min((deficit + 1) / 2, Math.Max(0, LeftWidth - 1));
                int takeFromRight = Math.Min(deficit / 2, Math.Max(0, RightWidth - 1));

                LeftWidth -= takeFromLeft;
                RightWidth -= takeFromRight;

                CenterWidth = totalWidth - LeftWidth - RightWidth - seperatorCount;
                if (CenterWidth < 1)
                {
                    CenterWidth = 1;
                }
            }
        }

        // Content setters apply optional wrapping (single responsibility: mutate content + recalc height).
        public void SetLeftContent(params string[] lines)
        {
            if (AutoWrapLeft)
            {
                leftContent = WrapText(lines, LeftWidth);
            }
            else
            {
                leftContent = new List<string>(lines);
            }
            RecalculateHeight();
        }

        public void SetCenterContent(params string[] lines)
        {
            if (AutoWrapCenter)
            {
                centerContent = WrapText(lines, CenterWidth);
            }
            else
            {
                centerContent = new List<string>(lines);
            }
            RecalculateHeight();
        }

        public void SetRightContent(params string[] lines)
        {
            if (AutoWrapRight)
            {
                rightContent = WrapText(lines, RightWidth);
            }
            else
            {
                rightContent = new List<string>(lines);
            }
            RecalculateHeight();
        }

        // Render draws the full panel. It composes vertical alignment, horizontal alignment and optionally borders.
        public void Render()
        {
            if (ShowBorders)
            {
                WriteLine(BuildRuler());
                WriteLine(BuildRow("", "", ""));
            }

            var leftLines = ApplyVerticalAlign(leftContent, Height, LeftAlignV);
            var centerLines = ApplyVerticalAlign(centerContent, Height, CenterAlignV);
            var rightLines = ApplyVerticalAlign(rightContent, Height, RightAlignV);

            for (int i = 0; i < Height; i++)
            {
                string leftText = i < leftLines.Count ? leftLines[i] : "";
                string centerText = i < centerLines.Count ? centerLines[i] : "";
                string rightText = i < rightLines.Count ? rightLines[i] : "";

                string leftAligned = ApplyHorizontalAlign(leftText, LeftWidth, LeftAlignH);
                string centerAligned = ApplyHorizontalAlign(centerText, CenterWidth, CenterAlignH);
                string rightAligned = ApplyHorizontalAlign(rightText, RightWidth, RightAlignH);

                // Simple composition of the three columns + optional borders
                if (ShowBorders)
                {
                    WriteLine(leftAligned + '|' + centerAligned + '|' + rightAligned);
                }
                else
                {
                    WriteLine(leftAligned + centerAligned + rightAligned);
                }
            }
        }

        // Niche: interactive menu rendered in the center column; highlights selection and returns index.
        // This method writes directly to the console and reads keys — blending presentation and input handling.
        public int RenderInteractiveMenu(string[] options, int startingIndex = 0)
        {
            int selectedIndex = startingIndex;
            ConsoleKey keyPressed;
            int startRow = CursorTop;

            do
            {
                List<string> menuLines = new List<string>();

                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selectedIndex)
                    {
                        menuLines.Add($"> << {options[i]} >>");
                    }
                    else
                    {
                        menuLines.Add($"  << {options[i]} >>");
                    }
                }

                SetCenterContent(menuLines.ToArray());
                SetCursorPosition(0, startRow);

                if (ShowBorders)
                {
                    WriteLine(BuildRuler());
                    WriteLine(BuildRow("", "", ""));
                }

                var leftLines = ApplyVerticalAlign(leftContent, Height, LeftAlignV);
                var centerLines = ApplyVerticalAlign(centerContent, Height, CenterAlignV);
                var rightLines = ApplyVerticalAlign(rightContent, Height, RightAlignV);

                for (int i = 0; i < Height; i++)
                {
                    string leftText = i < leftLines.Count ? leftLines[i] : "";
                    string centerText = i < centerLines.Count ? centerLines[i] : "";
                    string rightText = i < rightLines.Count ? rightLines[i] : "";

                    string leftAligned = ApplyHorizontalAlign(leftText, LeftWidth, LeftAlignH);
                    string rightAligned = ApplyHorizontalAlign(rightText, RightWidth, RightAlignH);

                    string centerAligned;
                    bool isSelectedLine = centerText.StartsWith(">");

                    if (isSelectedLine)
                    {
                        centerAligned = ApplyHorizontalAlign(centerText, CenterWidth, CenterAlignH);

                        if (ShowBorders)
                        {
                            Write(leftAligned + '|');
                        }
                        else
                        {
                            Write(leftAligned);
                        }

                        // Temporary UI highlight: background/foreground swap
                        ForegroundColor = ConsoleColor.Black;
                        BackgroundColor = ConsoleColor.White;
                        Write(centerAligned);
                        ResetColor();

                        if (ShowBorders)
                        {
                            WriteLine('|' + rightAligned);
                        }
                        else
                        {
                            WriteLine(rightAligned);
                        }
                    }
                    else
                    {
                        centerAligned = ApplyHorizontalAlign(centerText, CenterWidth, CenterAlignH);

                        if (ShowBorders)
                        {
                            WriteLine(leftAligned + '|' + centerAligned + '|' + rightAligned);
                        }
                        else
                        {
                            WriteLine(leftAligned + centerAligned + rightAligned);
                        }
                    }
                }

                ConsoleKeyInfo keyInfo = ReadKey(true);
                keyPressed = keyInfo.Key;

                if (keyPressed == ConsoleKey.UpArrow)
                {
                    selectedIndex--;
                    if (selectedIndex < 0)
                    {
                        selectedIndex = options.Length - 1;
                    }
                }
                else if (keyPressed == ConsoleKey.DownArrow)
                {
                    selectedIndex++;
                    if (selectedIndex >= options.Length)
                    {
                        selectedIndex = 0;
                    }
                }

            } while (keyPressed != ConsoleKey.Enter);

            return selectedIndex;
        }

        // Render with color support; recognizes a highlight marker to invert background for individual cells.
        public void RenderWithColors()
        {
            if (ShowBorders)
            {
                WriteLine(BuildRuler());
                WriteLine(BuildRow("", "", ""));
            }

            var leftLines = ApplyVerticalAlign(leftContent, Height, LeftAlignV);
            var centerLines = ApplyVerticalAlign(centerContent, Height, CenterAlignV);
            var rightLines = ApplyVerticalAlign(rightContent, Height, RightAlignV);

            for (int i = 0; i < Height; i++)
            {
                string leftText = i < leftLines.Count ? leftLines[i] : "";
                string centerText = i < centerLines.Count ? centerLines[i] : "";
                string rightText = i < rightLines.Count ? rightLines[i] : "";

                // Marker-based highlighting (niche pattern for console UI)
                bool leftHighlight = leftText.StartsWith(ConsoleMenu.HIGHLIGHT_MARKER);
                bool centerHighlight = centerText.StartsWith(ConsoleMenu.HIGHLIGHT_MARKER);
                bool rightHighlight = rightText.StartsWith(ConsoleMenu.HIGHLIGHT_MARKER);

                if (leftHighlight) leftText = leftText.Substring(ConsoleMenu.HIGHLIGHT_MARKER.Length);
                if (centerHighlight) centerText = centerText.Substring(ConsoleMenu.HIGHLIGHT_MARKER.Length);
                if (rightHighlight) rightText = rightText.Substring(ConsoleMenu.HIGHLIGHT_MARKER.Length);

                string leftAligned = ApplyHorizontalAlign(leftText, LeftWidth, LeftAlignH);
                string centerAligned = ApplyHorizontalAlign(centerText, CenterWidth, CenterAlignH);
                string rightAligned = ApplyHorizontalAlign(rightText, RightWidth, RightAlignH);

                if (ShowBorders)
                {
                    if (leftHighlight)
                    {
                        ForegroundColor = ConsoleColor.Black;
                        BackgroundColor = ConsoleColor.White;
                        Write(leftAligned);
                        ResetColor();
                    }
                    else
                    {
                        Write(leftAligned);
                    }

                    Write('|');

                    if (centerHighlight)
                    {
                        ForegroundColor = ConsoleColor.Black;
                        BackgroundColor = ConsoleColor.White;
                        Write(centerAligned);
                        ResetColor();
                    }
                    else
                    {
                        Write(centerAligned);
                    }

                    Write('|');

                    if (rightHighlight)
                    {
                        ForegroundColor = ConsoleColor.Black;
                        BackgroundColor = ConsoleColor.White;
                        WriteLine(rightAligned);
                        ResetColor();
                    }
                    else
                    {
                        WriteLine(rightAligned);
                    }
                }
                else
                {
                    if (leftHighlight)
                    {
                        ForegroundColor = ConsoleColor.Black;
                        BackgroundColor = ConsoleColor.White;
                        Write(leftAligned);
                        ResetColor();
                    }
                    else
                    {
                        Write(leftAligned);
                    }

                    if (centerHighlight)
                    {
                        ForegroundColor = ConsoleColor.Black;
                        BackgroundColor = ConsoleColor.White;
                        Write(centerAligned);
                        ResetColor();
                    }
                    else
                    {
                        Write(centerAligned);
                    }

                    if (rightHighlight)
                    {
                        ForegroundColor = ConsoleColor.Black;
                        BackgroundColor = ConsoleColor.White;
                        WriteLine(rightAligned);
                        ResetColor();
                    }
                    else
                    {
                        WriteLine(rightAligned);
                    }
                }
            }
        }

        // Special-case rendering where the left panel is inverted (niche visual emphasis).
        public void RenderWithLeftPanelInverted()
        {
            if (ShowBorders)
            {
                WriteLine(BuildRuler());
                WriteLine(BuildRow("", "", ""));
            }

            var leftLines = ApplyVerticalAlign(leftContent, Height, LeftAlignV);
            var centerLines = ApplyVerticalAlign(centerContent, Height, CenterAlignV);
            var rightLines = ApplyVerticalAlign(rightContent, Height, RightAlignV);

            for (int i = 0; i < Height; i++)
            {
                string leftText = i < leftLines.Count ? leftLines[i] : "";
                string centerText = i < centerLines.Count ? centerLines[i] : "";
                string rightText = i < rightLines.Count ? rightLines[i] : "";

                string leftAligned = ApplyHorizontalAlign(leftText, LeftWidth, LeftAlignH);
                string centerAligned = ApplyHorizontalAlign(centerText, CenterWidth, CenterAlignH);
                string rightAligned = ApplyHorizontalAlign(rightText, RightWidth, RightAlignH);

                if (ShowBorders)
                {
                    // Left panel inverted to indicate activity / selection
                    ForegroundColor = ConsoleColor.Black;
                    BackgroundColor = ConsoleColor.White;
                    Write(leftAligned);
                    ResetColor();

                    Write('|');

                    Write(centerAligned);
                    Write('|');

                    WriteLine(rightAligned);
                }
                else
                {
                    ForegroundColor = ConsoleColor.Black;
                    BackgroundColor = ConsoleColor.White;
                    Write(leftAligned);
                    ResetColor();

                    Write(centerAligned);
                    WriteLine(rightAligned);
                }
            }
        }

        // Apply vertical alignment by padding above or below content.
        private List<string> ApplyVerticalAlign(List<string> content, int totalHeight, VerticalAlign align)
        {
            var result = new List<string>();
            int contentLines = content.Count;
            int emptyLines = Math.Max(0, totalHeight - contentLines);

            switch (align)
            {
                case VerticalAlign.Top:
                    result.AddRange(content);
                    for (int i = 0; i < emptyLines; i++)
                    {
                        result.Add("");
                    }
                break;

                case VerticalAlign.Middle:
                    int topPadding = emptyLines / 2;
                    int bottomPadding = emptyLines - topPadding;

                    for (int i = 0; i < topPadding; i++)
                    {
                        result.Add("");
                    }
                    result.AddRange(content);

                    for (int i = 0; i < bottomPadding; i++)
                    {
                        result.Add("");
                    }

                break;

                case VerticalAlign.Bottom:
                    for (int i = 0; i < emptyLines; i++)
                    {
                        result.Add("");
                    }
                    result.AddRange(content);
                break;
            }

            return result;
        }

        // Horizontal alignment helper — truncates long text and pads short text.
        private string ApplyHorizontalAlign(string text, int width, HorizontalAlign align)
        {
            if (text == null)
            {
                text = string.Empty;
            }
            if (width <= 0)
            {
                return string.Empty;
            }

            if (text.Length > width)
            {
                return text.Substring(0, width);
            }

            int spaces = width - text.Length;

            switch (align)
            {
                case HorizontalAlign.Left:
                    return text + new string(' ', spaces);

                case HorizontalAlign.Center:
                    int padLeft = spaces / 2;
                    int padRight = spaces - padLeft;
                    return new string(' ', padLeft) + text + new string(' ', padRight);

                case HorizontalAlign.Right:
                    return new string(' ', spaces) + text;

                default:
                    return text;
            }
        }

        // Recalculate Height when auto-sizing (keeps rendering deterministic).
        private void RecalculateHeight()
        {
            if (Height == 0)
            {
                int maxLines = Math.Max(leftContent.Count, Math.Max(centerContent.Count, rightContent.Count));
                Height = Math.Max(1, maxLines); 
            }
        }

        // Build the top/bottom ruler string used as a border.
        private string BuildRuler()
        {
            string leftPart = new string('=', LeftWidth);
            string centerPart = new string('=', CenterWidth);
            string rightPart = new string('=', RightWidth);

            return leftPart + '|' + centerPart + '|' + rightPart;
        }

        // Small helper used by BuildRow to center a short label inside a column.
        private string CenterText(string text, int width)
        {
            if (text == null)
            {
                text = string.Empty;
            }
            if (width <= 0)
            {
                return string.Empty;
            }

            if (text.Length > width)
            {
                return text.Substring(0, width);
            }

            int space = width - text.Length;
            int padLeft = space / 2;
            int padRight = space - padLeft;

            return new string(' ', padLeft) + text + new string(' ', padRight);
        }

        // Public convenience: build a single row from three labels (used for headers / separators).
        public string BuildRow(string leftText, string centerText, string rightText)
        {
            string left = CenterText(leftText, LeftWidth);
            string center = CenterText(centerText, CenterWidth);
            string right = CenterText(rightText, RightWidth);

            return left + '|' + center + '|' + right;
        }

        // Utility: counts occurrences of a char within a substring (used for debugging ruler statistics).
        private int CountChar(string text, char character, int startIndex, int length)
        {
            if (text == null || startIndex < 0 || length < 0 || startIndex >= text.Length)
            {
                return 0;
            }

            int end = Math.Min(text.Length, startIndex + length);

            int count = 0;
            for (int i = startIndex; i < end; i++)
            {
                if (text[i] == character)
                {
                    count++;
                }
            }

            return count;
        }

        // Demo method: prints repeated rows and optional statistics about column sizes.
        public void ShowDemo(string leftLabel, string centerLabel, string rightLabel, int rows)
        {
            if (ShowBorders)
            {
                string ruler = BuildRuler();
                WriteLine(ruler);

                string boundaryLine = new string(' ', LeftWidth) + '|' +
                                      new string(' ', CenterWidth) + '|' +
                                      new string(' ', RightWidth);

                WriteLine(boundaryLine);
            }

            for (int i = 0; i < rows; i++)
            {
                string row = BuildRow(leftLabel, centerLabel, rightLabel);
                WriteLine(row);
            }

            if (ShowBorders)
            {
                string ruler = BuildRuler();
                int leftSize = CountChar(ruler, '=', 0, LeftWidth);
                int centerStart = LeftWidth + 1;
                int centerSize = CountChar(ruler, '=', centerStart, CenterWidth);
                int rightStart = centerStart + CenterWidth + 1;
                int rightSize = CountChar(ruler, '=', rightStart, RightWidth);

                WriteLine();
                WriteLine("stats:");
                WriteLine($"left size: {leftSize}");
                WriteLine($"center size: {centerSize}");
                WriteLine($"right size: {rightSize}");
            }
        }

        // Word-wrap helper: splits long lines into multiple lines respecting word boundaries where possible.
        // Niche: prefers breaking at spaces when available; avoids mid-word splits unless necessary.
        private List<string> WrapText(string[] lines, int maxWidth)
        {
            List<string> wrappedLines = new List<string>();

            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    wrappedLines.Add("");
                    continue;
                }

                if (line.Length <= maxWidth)
                {
                    wrappedLines.Add(line);
                }
                else
                {
                    string remainingText = line;
                    while (remainingText.Length > maxWidth)
                    {
                        int wrapIndex = maxWidth;

                        // Prefer breaking at the last space if it's not too far back.
                        int lastSpace = remainingText.LastIndexOf(' ', maxWidth);
                        if (lastSpace > maxWidth / 2)
                        {
                            wrapIndex = lastSpace;
                        }

                        wrappedLines.Add(remainingText.Substring(0, wrapIndex).TrimEnd());
                        remainingText = remainingText.Substring(wrapIndex).TrimStart();
                    }

                    if (remainingText.Length > 0)
                    {
                        wrappedLines.Add(remainingText);
                    }
                }
            }

            return wrappedLines;
        }

        // Static helper to build a highlighted list (reusable for menus). Returns lines with a marker prefix if selected.
        public static List<string> BuildHighlightedList(string[] items, int selectedIndex, string highlightMarker, bool isActive = true)
        {
            List<string> content = new List<string>();

            for (int i = 0; i < items.Length; i++)
            {
                if (isActive && i == selectedIndex)
                {
                    // Marker-based approach keeps rendering logic simple and decoupled from selection rendering.
                    content.Add($"{highlightMarker}>>> {items[i]} <<<");
                }
                else
                {
                    content.Add($"    {items[i]}");
                }
            }

            return content;
        }
    }
}
