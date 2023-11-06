using System;
using System.Threading.Tasks;

namespace Yosu.Services.AlertDialog;

public interface IAlertService
{
    // ----- async calls (use with "await" - MUST BE ON DISPATCHER THREAD) -----
    //Task ShowAlertAsync(string title, string message, string cancel = "OK");
    Task ShowAlertAsync(string title, string message, string cancel = "DISMISS");

    Task<bool> ShowConfirmationAsync(
        string title,
        string message,
        string accept = "Yes",
        string cancel = "No"
    );

    // ----- "Fire and forget" calls -----
    //void ShowAlert(string title, string message, string cancel = "OK");
    void ShowAlert(string title, string message, string cancel = "DISMISS");

    /// <param name="callback">Action to perform afterwards.</param>
    void ShowConfirmation(
        string title,
        string message,
        Action<bool> callback,
        string accept = "Yes",
        string cancel = "No"
    );
}
