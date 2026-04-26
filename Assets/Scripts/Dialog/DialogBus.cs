using System;

public readonly struct DialogRequest
{
    public readonly string Speaker;
    public readonly string[] Lines;

    public DialogRequest(string speaker, string[] lines)
    {
        Speaker = speaker;
        Lines = lines;
    }
}

public static class DialogBus
{
    public static event Action<DialogRequest> OnRequest;

    public static void Request(DialogRequest req) => OnRequest?.Invoke(req);
}