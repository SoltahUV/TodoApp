namespace Shared.Constraints
{
    public static class FieldLimits
    {
        public const int UserNameMin = 3;
        public const int UserNameMax = 255;

        public const int PasswordMin = 12;
        public const int PasswordMaxChars = 72;
        public const int PasswordMaxBytes = 72;

        public const int ItemNameMax = 60;
        public const int NoteTextMax = 140;
        public const int TaskDescriptionMax = 140;

        public const int DefaultPageSize = 50;
        public const int MaxPageSize = 100;
    }
}
