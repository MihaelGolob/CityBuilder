using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NotificationSystem {
    private readonly Action<string> _showNotification;
    private readonly Action _hideNotification;
    private readonly float _notificationTime;

    private readonly LinkedList<Notification> _notifications = new ();
    private int _currentNotificationId = -1;
    
    #region public methods
    
    public NotificationSystem(Action<string> showNotification, Action hideNotification, float notificationTime) {
        _showNotification = showNotification;
        _hideNotification = hideNotification;
        _notificationTime = notificationTime;
    }
    
    public int AddNotification(string message) {
        var notification = new Notification(message);
        _notifications.AddFirst(notification);
        if (_currentNotificationId == -1)
            ShowNext();

        return notification.id;
    }
    
    public void RemoveNotification(int id) {
        if (_currentNotificationId == id) {
            FinishCurrent();
            return;
        }
        
        var notification = _notifications.FirstOrDefault(m => m.id == id);
        if (notification == null) return;
        
        _notifications.Remove(notification);
    }
    
    #endregion
    
    #region private methods

    private void ShowNext() {
        if (_notifications.Count == 0) return;
        
        // send message to UI manager to pop up a window with the message
        var notification = _notifications.First();
        _notifications.RemoveFirst();
        _currentNotificationId = notification.id;
        
        _showNotification(notification.message);
        
        // use the coroutine runner to wait for the specified amount of time
        CoroutineRunner.Instance.DelayedCall(FinishCurrent, _notificationTime);
    }

    private void FinishCurrent() {
        _hideNotification();
        _currentNotificationId = -1;
        
        ShowNext();
    }

    #endregion
}