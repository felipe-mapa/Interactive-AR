public static class Events
{
    public delegate void Method();

    public static event Method OnTrackingFound;
    public static void TrackingFound() {
        OnTrackingFound?.Invoke();
    }

    public static event Method OnTrackingLost;
    public static void TrackingLost() {
        OnTrackingLost?.Invoke();
    }
}
