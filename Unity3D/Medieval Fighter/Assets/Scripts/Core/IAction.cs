namespace RPG.Core
{
    public interface IAction
    {
        // any class that implements this interface has to have certain methods
        // in this case, the Mover and Fighter classes will need to implement Cancel() method
        void Cancel();
    }
}