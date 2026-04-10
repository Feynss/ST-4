using Stateless;
using Stateless.Graph;


public enum BugState
{
    NewDefect,
    Triage,
    Fix,
    Verification,
    Closed,
    Returned,
    Reopened
}

public enum BugTrigger
{
    Submit,
    NeedMoreInfo,
    NotABug,
    SendToFix,
    FixDone,
    VerifyOk,
    VerifyFail,
    ProblemSolved,
    ProblemNotSolved,
    Reopen,
    ReturnToTriage
}

public class Bug
{
    private readonly StateMachine<BugState, BugTrigger> _stateMachine;
    public string Title { get; }
    public List<string> History { get; } = new();
    public BugState State => _stateMachine.State;

    public Bug(string title, BugState initialState = BugState.NewDefect)
    {
        Title = title;
        _stateMachine = new StateMachine<BugState, BugTrigger>(initialState);
        ConfigureStateMachine();
    }

    private void ConfigureStateMachine()
    {
        _stateMachine.Configure(BugState.NewDefect)
            .Permit(BugTrigger.Submit, BugState.Triage)
            .OnEntry(() => Log("Появление баг: новый дефект"));

        _stateMachine.Configure(BugState.Triage)
            .Permit(BugTrigger.SendToFix, BugState.Fix)
            .Permit(BugTrigger.NotABug, BugState.Returned)
            .Permit(BugTrigger.NeedMoreInfo, BugState.Returned)
            .OnEntry(() => Log("Баг под разбором"));

        _stateMachine.Configure(BugState.Fix)
            .Permit(BugTrigger.FixDone, BugState.Verification)
            .OnEntry(() => Log("Баг исправляется"));

        _stateMachine.Configure(BugState.Verification)
            .Permit(BugTrigger.VerifyOk, BugState.Closed)
            .Permit(BugTrigger.VerifyFail, BugState.Reopened)
            .Permit(BugTrigger.ProblemSolved, BugState.Closed)
            .Permit(BugTrigger.ProblemNotSolved, BugState.Reopened)
            .OnEntry(() => Log("Исправление бага тестируется"));

        _stateMachine.Configure(BugState.Closed)
            .Permit(BugTrigger.Reopen, BugState.Reopened)
            .OnEntry(() => Log("Баг закрыт"));

        _stateMachine.Configure(BugState.Returned)
            .Permit(BugTrigger.ReturnToTriage, BugState.Triage)
            .OnEntry(() => Log("Баг возвращен (не баг / нужно больше информации / дублирование)"));

        _stateMachine.Configure(BugState.Reopened)
            .Permit(BugTrigger.ReturnToTriage, BugState.Triage)
            .OnEntry(() => Log("Баг переоткрыт"));
    }

    private void Log(string message)
    {
        var entry = $"[{DateTime.Now:HH:mm:ss}] [{State}] {message}";
        History.Add(entry);
        Console.WriteLine(entry);
    }

    public void Submit()         => _stateMachine.Fire(BugTrigger.Submit);
    public void NeedMoreInfo()   => _stateMachine.Fire(BugTrigger.NeedMoreInfo);
    public void MarkNotABug()    => _stateMachine.Fire(BugTrigger.NotABug);
    public void SendToFix()      => _stateMachine.Fire(BugTrigger.SendToFix);
    public void FixDone()        => _stateMachine.Fire(BugTrigger.FixDone);
    public void VerifyOk()       => _stateMachine.Fire(BugTrigger.VerifyOk);
    public void VerifyFail()     => _stateMachine.Fire(BugTrigger.VerifyFail);
    public void ProblemSolved()  => _stateMachine.Fire(BugTrigger.ProblemSolved);
    public void ProblemNotSolved() => _stateMachine.Fire(BugTrigger.ProblemNotSolved);
    public void Reopen()         => _stateMachine.Fire(BugTrigger.Reopen);
    public void ReturnToTriage() => _stateMachine.Fire(BugTrigger.ReturnToTriage);

    public bool CanFire(BugTrigger trigger) => _stateMachine.CanFire(trigger);
    public IEnumerable<BugTrigger> PermittedTriggers => _stateMachine.GetPermittedTriggers();
    public string ToDotGraph() => UmlDotGraph.Format(_stateMachine.GetInfo());

    public override string ToString() =>
        $"Bug: \"{Title}\" | State: {State} | History entries: {History.Count}";
}
