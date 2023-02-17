using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationSystem {
    private readonly Action<string> _showNotification;
    private readonly Action _hideNotification;
    private readonly float _notificationTime;

    private readonly Queue<string> _notifications = new ();
    private bool _isShowingNotification;
    
    #region public methods
    
    public NotificationSystem(Action<string> showNotification, Action hideNotification, float notificationTime) {
        _showNotification = showNotification;
        _hideNotification = hideNotification;
        _notificationTime = notificationTime;
    }
    
    public void AddNotification(string message) {
        _notifications.Enqueue(message);
        if (!_isShowingNotification)
            ShowNext();
    }
    
    #endregion
    
    #region private methods

    private void ShowNext() {
        if (_notifications.Count == 0) return;
        _isShowingNotification = true;
        
        // send message to UI manager to pop up a window with the message
        var message = _notifications.Dequeue();
        _showNotification(message);
        
        // use the coroutine runner to wait for the specified amount of time
        CoroutineRunner.Instance.DelayedCall(FinishCurrent, _notificationTime);
    }

    private void FinishCurrent() {
        _hideNotification();
        _isShowingNotification = false;
        ShowNext();
    }

    #endregion
}