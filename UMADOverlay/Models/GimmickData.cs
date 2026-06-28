namespace UMADOverlay.Models
{
    public enum Lang { ZH, JP, EN }

    public record GimmickRow(
        string LabelKey,
        string ShinAnswerKey,
        string GiAnswerKey,
        bool IsToggle = false
    );

    public static class GimmickData
    {
        public static readonly (string SectionKey, GimmickRow[] Rows)[] Sections =
        [
            ("sec-early",
            [
                new("early-water", "ans-stack",    "ans-spread"),
                new("early-light", "ans-spread",   "ans-stack"),
                new("early-gaze",  "ans-lookaway", "ans-lookat"),
                new("elem-1",      "ans-out",      "ans-in"),
            ]),
            ("sec-late",
            [
                new("late-water",  "ans-stack",    "ans-spread"),
                new("late-light",  "ans-spread",   "ans-stack"),
                new("late-gaze",   "ans-lookaway", "ans-lookat"),
                new("elem-2",      "ans-in",       "ans-out"),
            ]),
            ("sec-safe",
            [
                new("g-thunder",   "ans-no-line",  "ans-stay-line", IsToggle: true),
                new("g-ice",       "ans-no-fan",   "ans-stay-fan",  IsToggle: true),
                new("g-accel",     "ans-stop",     "ans-move"),
            ]),
        ];
    }
}
