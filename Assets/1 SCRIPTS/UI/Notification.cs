public class Notification {
    public string message;
    public int id;
    
    private static int _idCounter = 0;
    
    public Notification(string message) {
        this.message = message;
        id = _idCounter++;
    }
}