using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stateless;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void NewBug_InitialState_IsNewDefect()
    {
        var bug = new Bug("Test bug");
        Assert.AreEqual(BugState.NewDefect, bug.State);
    }

    [TestMethod]
    public void Submit_FromNewDefect_TransitionsToTriage()
    {
        var bug = new Bug("Test bug");
        bug.Submit();
        Assert.AreEqual(BugState.Triage, bug.State);
    }

    [TestMethod]
    public void SendToFix_FromTriage_TransitionsToFix()
    {
        var bug = new Bug("Test bug");
        bug.Submit();
        bug.SendToFix();
        Assert.AreEqual(BugState.Fix, bug.State);
    }

    [TestMethod]
    public void FixDone_FromFix_TransitionsToVerification()
    {
        var bug = new Bug("Test bug");
        bug.Submit();
        bug.SendToFix();
        bug.FixDone();
        Assert.AreEqual(BugState.Verification, bug.State);
    }

    [TestMethod]
    public void VerifyOk_FromVerification_TransitionsToClosed()
    {
        var bug = new Bug("Test bug");
        bug.Submit();
        bug.SendToFix();
        bug.FixDone();
        bug.VerifyOk();
        Assert.AreEqual(BugState.Closed, bug.State);
    }

    [TestMethod]
    public void VerifyFail_FromVerification_TransitionsToReopened()
    {
        var bug = new Bug("Test bug");
        bug.Submit();
        bug.SendToFix();
        bug.FixDone();
        bug.VerifyFail();
        Assert.AreEqual(BugState.Reopened, bug.State);
    }

    [TestMethod]
    public void ProblemSolved_FromVerification_TransitionsToClosed()
    {
        var bug = new Bug("Test bug");
        bug.Submit();
        bug.SendToFix();
        bug.FixDone();
        bug.ProblemSolved();
        Assert.AreEqual(BugState.Closed, bug.State);
    }

    [TestMethod]
    public void ProblemNotSolved_FromVerification_TransitionsToReopened()
    {
        var bug = new Bug("Test bug");
        bug.Submit();
        bug.SendToFix();
        bug.FixDone();
        bug.ProblemNotSolved();
        Assert.AreEqual(BugState.Reopened, bug.State);
    }

    [TestMethod]
    public void MarkNotABug_FromTriage_TransitionsToReturned()
    {
        var bug = new Bug("Test bug");
        bug.Submit();
        bug.MarkNotABug();
        Assert.AreEqual(BugState.Returned, bug.State);
    }

    [TestMethod]
    public void NeedMoreInfo_FromTriage_TransitionsToReturned()
    {
        var bug = new Bug("Test bug");
        bug.Submit();
        bug.NeedMoreInfo();
        Assert.AreEqual(BugState.Returned, bug.State);
    }

    [TestMethod]
    public void ReturnToTriage_FromReturned_TransitionsToTriage()
    {
        var bug = new Bug("Test bug");
        bug.Submit();
        bug.MarkNotABug();
        bug.ReturnToTriage();
        Assert.AreEqual(BugState.Triage, bug.State);
    }

    [TestMethod]
    public void ReturnToTriage_FromReopened_TransitionsToTriage()
    {
        var bug = new Bug("Test bug");
        bug.Submit();
        bug.SendToFix();
        bug.FixDone();
        bug.VerifyFail();
        bug.ReturnToTriage();
        Assert.AreEqual(BugState.Triage, bug.State);
    }

    [TestMethod]
    public void Reopen_FromClosed_TransitionsToReopened()
    {
        var bug = new Bug("Test bug");
        bug.Submit();
        bug.SendToFix();
        bug.FixDone();
        bug.VerifyOk();
        bug.Reopen();
        Assert.AreEqual(BugState.Reopened, bug.State);
    }

    [TestMethod]
    public void FullHappyPath_NewToClosed()
    {
        var bug = new Bug("Full path bug");
        bug.Submit();
        bug.SendToFix();
        bug.FixDone();
        bug.VerifyOk();
        Assert.AreEqual(BugState.Closed, bug.State);
    }

    [TestMethod]
    public void History_IsPopulated_AfterTransitions()
    {
        var bug = new Bug("History bug");
        bug.Submit();
        bug.SendToFix();
        Assert.IsGreaterThanOrEqualTo(bug.History.Count, 2);
    }

    [TestMethod]
    public void CanFire_Submit_FromNewDefect_ReturnsTrue()
    {
        var bug = new Bug("Test bug");
        Assert.IsTrue(bug.CanFire(BugTrigger.Submit));
    }

    [TestMethod]
    public void CanFire_SendToFix_FromNewDefect_ReturnsFalse()
    {
        var bug = new Bug("Test bug");
        Assert.IsFalse(bug.CanFire(BugTrigger.SendToFix));
    }

    [TestMethod]
    public void PermittedTriggers_FromNewDefect_ContainsSubmit()
    {
        var bug = new Bug("Test bug");
        CollectionAssert.Contains(bug.PermittedTriggers.ToList(), BugTrigger.Submit);
    }

    [TestMethod]
    public void Title_IsStoredCorrectly()
    {
        var title = "My unique bug title";
        var bug = new Bug(title);
        Assert.AreEqual(title, bug.Title);
    }

    [TestMethod]
    public void InvalidTrigger_FromNewDefect_ThrowsInvalidOperationException()
    {
        var bug = new Bug("Test bug");
        try
        {
            bug.SendToFix();
            Assert.Fail("Expected InvalidOperationException was not thrown");
        }
        catch (InvalidOperationException) { }
    }

    [TestMethod]
    public void FixDone_FromTriage_ThrowsInvalidOperationException()
    {
        var bug = new Bug("Test bug");
        bug.Submit();
        try
        {
            bug.FixDone();
            Assert.Fail("Expected InvalidOperationException was not thrown");
        }
        catch (InvalidOperationException) { }
    }

    [TestMethod]
    public void VerifyOk_FromFix_ThrowsInvalidOperationException()
    {
        var bug = new Bug("Test bug");
        bug.Submit();
        bug.SendToFix();
        try
        {
            bug.VerifyOk();
            Assert.Fail("Expected InvalidOperationException was not thrown");
        }
        catch (InvalidOperationException) { }
    }

    [TestMethod]
    public void Reopen_FromNewDefect_ThrowsInvalidOperationException()
    {
        var bug = new Bug("Test bug");
        try
        {
            bug.Reopen();
            Assert.Fail("Expected InvalidOperationException was not thrown");
        }
        catch (InvalidOperationException) { }
    }

    [TestMethod]
    public void Submit_Twice_ThrowsInvalidOperationException()
    {
        var bug = new Bug("Test bug");
        bug.Submit();
        try
        {
            bug.Submit();
            Assert.Fail("Expected InvalidOperationException was not thrown");
        }
        catch (InvalidOperationException) { }
    }

    [TestMethod]
    public void FullReCycle_ClosedThenReopenedAndFixedAgain()
    {
        var bug = new Bug("Recurring bug");
        bug.Submit();
        bug.SendToFix();
        bug.FixDone();
        bug.VerifyOk();
        bug.Reopen();
        bug.ReturnToTriage();
        bug.SendToFix();
        bug.FixDone();
        bug.ProblemSolved();
        Assert.AreEqual(BugState.Closed, bug.State);
    }
}
