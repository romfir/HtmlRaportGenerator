namespace HtmlRaportGenerator.Tools.Enums;

public enum DayInputState
{
    None = 0,
    DataLoading = 1,
    DataSaving = 2,
    ShiftNotStarted = 3,
    ShiftStarted = 4,
    ShiftEnded = 5,
    AwaitingInput = 6
}